using System;

namespace Dialogue_System
{
    [Serializable]
    public class Input
    {
        public string label;
        public string serializedType;
        public Orientation orientation;
        public Capacity capacity;

        public Input(string label, Type type = null, Orientation orientation = Orientation.Horizontal, Capacity capacity = Capacity.Multi)
        {
            this.label = label;
            var t = type ?? typeof(FlowType);
            serializedType = t.AssemblyQualifiedName;
            this.orientation = orientation;
            this.capacity = capacity;
        }
    }
}