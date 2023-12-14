using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue_System
{
    [Serializable]
    public class Output
    {
        public string label;
        public List<NodeConnection> children = new ();
        public string serializedType;
        public Orientation orientation;
        public Capacity capacity;
        
        public Output(string label, Type type = null, Orientation orientation = Orientation.Horizontal, Capacity capacity = Capacity.Single)
        {
            this.label = label;
            var t = type ?? typeof(FlowType);
            serializedType = t.AssemblyQualifiedName;
            this.orientation = orientation;
            this.capacity = capacity;
        }
    }
}