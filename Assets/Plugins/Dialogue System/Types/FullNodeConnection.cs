using System;

namespace Dialogue_System.Types
{
    [Serializable]
    public struct FullNodeConnection
    {
        public DialogueNode source;
        public DialogueNode destination;
        public int outputIndex;
        public int inputIndex;

        public FullNodeConnection(DialogueNode source, DialogueNode destination, int outputIndex, int inputIndex)
        {
            this.source = source;
            this.destination = destination;
            this.outputIndex = outputIndex;
            this.inputIndex = inputIndex;
        }
    }
}