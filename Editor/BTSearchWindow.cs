using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BTEditor
{
    public class BTSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private BTGraphView _graphView;

        private Texture2D _indentationIcon;
        
        public void Configure(EditorWindow window, BTGraphView graphView)
        {
            _window = window;
            _graphView = graphView;
            
            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            //add all node types to the search window
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeEntry(new GUIContent("Sequential"))
                {
                    level = 1,
                    userData = new SequentialNode(_graphView)
                },
                new SearchTreeEntry(new GUIContent("Action"))
                {
                    level = 1,
                    userData = new ActionNode(_graphView)
                },
                new SearchTreeEntry(new GUIContent("Negation"))
                {
                    level = 1,
                    userData = new NegationNode(_graphView)
                },
                new SearchTreeEntry(new GUIContent("Condition"))
                {
                    level = 1,
                    userData = new ConditionNode(_graphView)
                },
                new SearchTreeEntry(new GUIContent("Convert Running"))
                {
                    level = 1,
                    userData = new ConvertRunning(_graphView)
                }
            };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            _graphView.CreateNode(SearchTreeEntry.userData as BTNode, graphMousePosition);
            return true;
        }
    }
}
