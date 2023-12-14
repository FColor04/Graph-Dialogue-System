#define Debug

using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue_System;
using Dialogue_System.Types;
using UnityEngine;
using UnityEngine.UIElements;
using Input = Dialogue_System.Input;


public abstract class DialogueNode : ScriptableObject
{
    public Action EditorRefresh;
    public Action EditorRefreshWindow;
    public Func<Type, Vector2, DialogueNode> EditorCreateNode;
    public Action<DialogueNode> EditorRemoveNode;
#if UNITY_EDITOR
    public string EditorName => UnityEditor.ObjectNames.NicifyVariableName(GetType().Name).Replace(" Node", "");
#endif
        
    //Editor
    #if !Debug
    [HideInInspector]
    #endif
    public Vector2 position;
    public DialogueTree parent;
    
#if !Debug
    [HideInInspector]
#endif
    public string guid;
    
    public virtual Output[] GetDefaultOutputs() => new []{new Output("")};
    public virtual Input[] GetDefaultInputs() => new []{new Input("")};
    
#if !Debug
    [HideInInspector]
#endif
    [SerializeField] 
    private List<Output> outputs;
    public List<Output> Outputs
    {
        get
        {
            if (outputs != null)
            {
                outputs.RemoveAll(output => string.IsNullOrWhiteSpace(output.serializedType) || Type.GetType(output.serializedType) == null);
                outputs.RemoveAll(output => Type.GetType(output.serializedType).AssemblyQualifiedName != output.serializedType);
                outputs.AddRange(GetDefaultOutputs().Except(outputs, new NodeIgnoreChildrenComparer()));
                return outputs;
            }
            outputs = GetDefaultOutputs().ToList();
            return outputs;
        }
        set => outputs = value;
    }

#if !Debug
    [HideInInspector]
#endif
    [SerializeField] 
    private List<Input> inputs;
    public List<Input> Inputs
    {
        get
        {
            if (inputs != null)
            {
                inputs.RemoveAll(input => string.IsNullOrWhiteSpace(input.serializedType) || Type.GetType(input.serializedType) == null);
                inputs.RemoveAll(input => Type.GetType(input.serializedType).AssemblyQualifiedName != input.serializedType);
                inputs.AddRange(GetDefaultInputs().Except(inputs, new NodeIgnoreChildrenComparer()));
                return inputs;
            }
            inputs = GetDefaultInputs().ToList();
            return inputs;
        }
    }


    public virtual void ModifyVisualElement(VisualElement element) {}

    public abstract bool OnUpdate(IDialogueController controller);
    public virtual void OnEnter(IDialogueController controller) {}
    public virtual void OnExit(IDialogueController controller) {}
    public virtual DialogueNode GetNextNode()
    {
        return Outputs[0].children.Count > 0 ? Outputs[0].children[0].node : null;
    }

    protected DialogueNode DefaultNextNode() => Outputs[0].children.Count > 0 ? Outputs[0].children[0].node : null;

    protected List<object> ReadInputs()
    {
        var parameters = new List<object>(inputs.Count);
        parameters.AddRange(Enumerable.Repeat<object>(null, inputs.Count));
        foreach (var input in GetInputNodeConnections())
        {
            parameters[input.inputIndex] = input.source.ReadOutput(input.outputIndex);
        }

        return parameters;
    }

    public IEnumerable<FullNodeConnection> GetInputNodeConnections()
    {
        foreach (var node in parent.nodes)
        {
            for (int j = 0; j < node.Outputs.Count; j++)
            {
                var connectionIndex = node.Outputs[j].children.FindIndex(a => a.node == this);
                if (connectionIndex < 0) continue;
                var connection = node.Outputs[j].children[connectionIndex];
                yield return new FullNodeConnection(node, this, j, connection.inputIndex);
            }
        }
    }

    protected virtual object ReadOutput(int index) => null;
    public virtual void OnNodeCreated() {}
}