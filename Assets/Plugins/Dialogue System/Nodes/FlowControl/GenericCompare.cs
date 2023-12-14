using System;
using Dialogue_System.Types;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Dialogue_System.Nodes.FlowControl
{
    [NodeIgnore]
    public class GenericCompare<T> : DialogueNode where T : IComparable<T>
    {
        public T compareTo;
        public Comparison comparison;
        private T _value;
        
        public override Output[] GetDefaultOutputs() =>
            new []
            {
                new Output("True"),
                new Output("False")
            };

        public override Input[] GetDefaultInputs() => new []
        {
            new Input(""),
            new Input("Value", typeof(T))
        };

        public override void OnEnter(IDialogueController controller)
        {
            
        }
        
        public override bool OnUpdate(IDialogueController controller) => true;

        public override DialogueNode GetNextNode()
        {
            var inputs = ReadInputs();
            _value = (T)inputs[1];

            var @true = Outputs[0]?.children[0].node;
            var @false = Outputs[1]?.children[0].node;
            var comparisonResult = _value.CompareTo(compareTo);
            return comparison switch
            {
                Comparison.Equal => comparisonResult == 0 ? @true : @false,
                Comparison.NotEqual => comparisonResult != 0 ? @true : @false,
                Comparison.Greater => comparisonResult > 0 ? @true : @false,
                Comparison.GreaterOrEqual => comparisonResult >= 0 ? @true : @false,
                Comparison.Less => comparisonResult < 0 ? @true : @false,
                Comparison.LessOrEqual => comparisonResult <= 0 ? @true : @false,
                _ => @true
            };
        }

        public override void ModifyVisualElement(VisualElement element)
        {
#if UNITY_EDITOR
                var preview = element.Q("input").Q(className:"type"+typeof(T).Name).Q<Label>("type");
            preview.text = $"{ToSymbol(comparison)} {compareTo}";

            preview.TrackSerializedObjectValue(new SerializedObject(this), so =>
            {
                preview.text = $"{ToSymbol(comparison)} {compareTo}";
            });
#endif
        }

        public static string ToSymbol(Comparison comparison)
        {
            return comparison switch
            {
                Comparison.Equal => "==",
                Comparison.NotEqual => "!=",
                Comparison.Greater => ">",
                Comparison.GreaterOrEqual => ">=",
                Comparison.Less => "<",
                Comparison.LessOrEqual => "<=",
                _ => ""
            };
        }
    }
}