using System;
using Dialogue_System.Types;

namespace Dialogue_System.Nodes
{
    [NodeIgnore]
    public class EntryNode : DialogueNode
    {
        public int interactionCount;
        public override Output[] GetDefaultOutputs() =>
            new []
            {
                new Output("Entry"),
                new Output("Interaction Counter", typeof(int))
            };

        public override Input[] GetDefaultInputs() => Array.Empty<Input>();

        public override bool OnUpdate(IDialogueController controller) => true;

        public override DialogueNode GetNextNode()
        {
            return DefaultNextNode();
        }

        protected override object ReadOutput(int index)
        {
            return index == 1 ? interactionCount : null;
        }
    }
}