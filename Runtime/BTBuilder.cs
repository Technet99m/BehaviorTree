using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace BTRuntime
{
    public class BTBuilder
    {
        public static BehaviorTree BuildTree(string templateJson, Dictionary<string, IAction> actionLibrary)
        {
            var context = new Dictionary<string, object>();
            EntryNode root = null;
            var template = JsonMapper.ToObject(templateJson);
            var nodeData = new Dictionary<string, JsonData>();

            foreach (string guid in template.Keys)
            {
                nodeData.Add(guid, template[guid]);
            }

            var createdNodes = new Dictionary<string, Node>();

            var justCreated = new List<string>();
            do
            {
                justCreated.Clear();
                foreach(var node in nodeData.Keys)
                {
                    if(CanCreateNode(nodeData[node], createdNodes))
                    {
                        var newNode = CreateNode(nodeData[node], createdNodes, actionLibrary, context);
                        if(newNode is EntryNode)
                            root = newNode as EntryNode;
                        
                        createdNodes.Add(node, newNode);
                        justCreated.Add(node);
                    }
                }
                foreach(var node in justCreated)
                {
                    nodeData.Remove(node);
                }
            }
            while(justCreated.Count > 0);

            return new BehaviorTree(root, context);
        }

        private static bool CanCreateNode(JsonData nodeData, Dictionary<string, Node> createdNodes)
        {
            var type = Enum.Parse(typeof(NodeType), nodeData["type"].ToString());
            switch (type)
            {
                case NodeType.Action:
                    return true;
                case NodeType.Entry:
                case NodeType.Negation:
                    return createdNodes.ContainsKey(nodeData["child"].ToString());
                case NodeType.Sequential:
                    foreach (var child in nodeData["children"])
                    {
                        if (!createdNodes.ContainsKey(child.ToString()))
                        {
                            return false;
                        }
                    }
                    return true;
            }
            
            return true;
        }

        private static Node CreateNode(JsonData nodeData, Dictionary<string, Node> createdNodes, Dictionary<string, IAction> actionLibrary, Dictionary<string, object> context)
        {
            var type = Enum.Parse(typeof(NodeType), nodeData["type"].ToString());
            switch (type)
            {
                case NodeType.Action:
                    var action = actionLibrary[nodeData["actionName"].ToString()];
                    return new ActionNode(context, action);
                case NodeType.Entry:
                    var entry = new EntryNode(context, createdNodes[nodeData["child"].ToString()]);
                    return entry;
                case NodeType.Negation:
                    var negation = new NegationNode(context, createdNodes[nodeData["child"].ToString()]);
                    return negation;
                case NodeType.Sequential:
                    var children = nodeData["children"].DeserializeList(guid=> createdNodes[guid.ToString()]);
                    return new SequentialNode(context, children, nodeData["mode"].ToString() == "Any" ? SequentialNode.SequenceMode.Any: SequentialNode.SequenceMode.All, nodeData["runningIsSuccess"].AsBool);
                case NodeType.Condition:
                    var success = nodeData.ContainsKey("success") ? createdNodes[nodeData["success"].ToString()] : null;
                    return new ConditionNode(context, actionLibrary[nodeData["condition"].ToString()], success);
            }
            return null;
        }
    }
}
