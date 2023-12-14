using System;

namespace Dialogue_System
{
    [Serializable]
    public struct NodeConnection
    {
        public DialogueNode node;
        public int inputIndex;

        public NodeConnection(DialogueNode node, int inputIndex)
        {
            this.node = node;
            this.inputIndex = inputIndex;
        }
    }
}