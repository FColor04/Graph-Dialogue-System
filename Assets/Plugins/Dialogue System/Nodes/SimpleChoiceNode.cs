using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue_System.Types;
using Fields;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine.UIElements;

namespace Dialogue_System.Nodes
{
    public class SimpleChoiceNode : SimpleSpeakNode
    {
        public List<string> playerChoices = new ();
        public Optional<float> time;
        public int selected = -1;
        
        public override Output[] GetDefaultOutputs() => Array.Empty<Output>();

        public override DialogueNode GetNextNode()
        {
            return Outputs[selected].children[0].node;
        }

        public override void OnEnter(IDialogueController controller)
        {
            selected = -1;
            controller.SelectionCallback += OnChoice;
            controller.DisplayChoices(playerChoices);
        }

        public override void OnExit(IDialogueController controller)
        {
            controller.InteractionCallback -= OnInteraction;
            controller.SelectionCallback -= OnChoice;
        }

        private void OnChoice(int choice)
        {
            selected = choice;
        }

        public override bool OnUpdate(IDialogueController controller)
        {
            return selected != -1;
        }

        public override void ModifyVisualElement(VisualElement element)
        {
#if UNITY_EDITOR
            element.AddToClassList("yellow");
            var preview = element.Q<Label>("Preview");
            var avatarPreview = element.Q<Image>("Avatar");
            var outputContainer = element.Q("output");
            preview.text = text;
            if(CharacterData.avatar.valueSet)
                avatarPreview.sprite = CharacterData.avatar.value;
            
            preview.TrackSerializedObjectValue(new SerializedObject(this), so =>
            {
                preview.text = text;
                if (CharacterData.avatar.valueSet)
                {
                    avatarPreview.sprite = CharacterData.avatar.value;
                }

                for (int i = 0; i < Outputs.Count; i++)
                {
                    if (playerChoices.Count > i)
                    {
                        Outputs[i].label = playerChoices[i];
                        outputContainer[i].Q<Label>().text = playerChoices[i];
                    }
                }

                var choicesCopy = playerChoices.ToList();
                var indexesToRemove = new List<int>();
                for (int i = 0; i < Outputs.Count; i++)
                {
                    if (!choicesCopy.Remove(Outputs[i].label))
                    {
                        indexesToRemove.Add(i);
                    }
                }

                indexesToRemove.Sort();
                foreach (var i in indexesToRemove)
                    Outputs.RemoveAt(i);

                foreach (var label in choicesCopy)
                {
                    Outputs.Add(new Output(label, orientation: Orientation.Vertical));
                }
                
                EditorRefresh?.Invoke();
            });
#endif
        }
    }
}