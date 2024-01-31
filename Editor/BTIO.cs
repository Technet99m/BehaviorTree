using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using LitJson;
using UnityEditor;

namespace BTEditor
{
    public class BTIO
    {
        public static void Export(BTGraphView graphView, string filename)
        {
            var jsonData = new JsonData();
            var serializedNodes = graphView.nodes.Select(node => 
            {
                var BTnode = node as BTNode;
                return(BTnode.GUID, BTnode.Export());
            });

            foreach(var (guid, data) in serializedNodes)
            {
                jsonData[guid] = data;
            }

            var json = JsonMapper.ToJson(jsonData);
            File.WriteAllText(filename, json);
            AssetDatabase.Refresh();
        }

        public static EntryNode Import(BTGraphView graphView, string filename)
        {
            var json = File.ReadAllText(filename);
            var jsonData = JsonMapper.ToObject(json);
            var nodes = new Dictionary<string, BTNode>();
            EntryNode entry = null;
            //create all nodes
            foreach (var key in jsonData.Keys)
            {
                var data = jsonData[key];
                var type = Enum.Parse(typeof(BTNodeType), data["type"].ToString());
                BTNode node = null;
                switch (type)
                {
                    case BTNodeType.Entry:
                        node = new EntryNode(graphView);
                        entry = node as EntryNode;
                        break;
                    case BTNodeType.Action:
                        node = new ActionNode(graphView);
                        break;
                    case BTNodeType.Sequential:
                        node = new SequentialNode(graphView);
                        break;
                    case BTNodeType.Negation:
                        node = new NegationNode(graphView);
                        break;
                }
                node.GUID = key;
                graphView.CreateNode(node, data["position"].ToVector2());
                nodes.Add(key, node);
            }

            //connect all nodes
            foreach (var key in jsonData.Keys)
            {
                var data = jsonData[key];
                var node = nodes[key];
                switch (node.type)
                {
                    case BTNodeType.Entry:
                        var entryNode = node as EntryNode;
                        var destination = data.ContainsKey("child") ? nodes[data["child"].ToString()] : null;
                        entryNode.ConnectTo(destination);
                        break;
                    case BTNodeType.Action:
                        var actionNode = node as ActionNode;
                        actionNode.SetAction(data["actionName"].ToString());
                        break;
                    case BTNodeType.Negation:
                        var negationNode = node as NegationNode;
                        var next = data.ContainsKey("child") ? nodes[data["child"].ToString()] : null;
                        negationNode.SetNext(next);
                        break;
                    case BTNodeType.Sequential:
                        var sequentialNode = node as SequentialNode;
                        sequentialNode.SetMode(data["mode"].ToString(), data["runningIsSuccess"].ToBool());
                        var connections = data["children"].DeserializeList(guid => nodes[guid.ToString()]);
                        sequentialNode.FillConnections(connections);
                        break;
                }
            }

            return entry;

        }
    }
}
