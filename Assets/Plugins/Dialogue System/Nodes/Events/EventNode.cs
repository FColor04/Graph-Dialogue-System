using System;
using Dialogue_System.Types;
using UnityEngine.Events;

namespace Dialogue_System.Nodes.Events
{
    [NodeCategory("Events")]
    public class EventNode : DialogueNode
    {
        public string eventName;
        [NonSerialized]
        public UnityEvent EventBinding;
        public UnityEvent GameEventBinding;
        
        public override void OnEnter(IDialogueController controller)
        {
            EventBinding?.Invoke();
            GameEventBinding?.Invoke();
        }

        public override bool OnUpdate(IDialogueController controller)
        {
            return true;
        }
    }
}