using System;

namespace Dialogue_System.Editor
{
    [Serializable]
    public struct ClipboardNodeConnection<T>
    {
        public T sourceNodeKey;
        public int destinationNodeIndex;
        public int outputIndex;
        public int inputIndex;

        public ClipboardNodeConnection(T sourceNodeKey, int destinationNodeIndex, int outputIndex, int inputIndex)
        {
            this.sourceNodeKey = sourceNodeKey;
            this.destinationNodeIndex = destinationNodeIndex;
            this.outputIndex = outputIndex;
            this.inputIndex = inputIndex;
        }
    }
}