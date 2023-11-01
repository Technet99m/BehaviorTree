using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace BTEditor
{
    public class BTEdgeConnectorListener : IEdgeConnectorListener
    {
        private BTGraphView _graphView;

        public BTEdgeConnectorListener(BTGraphView graphView)
        {
            _graphView = graphView;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            _graphView.ShowSearchWindowAndConnect(position, edge.output);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            
        }
    }
}
