using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue_System.Nodes;
using Dialogue_System.Nodes.Events;
using Dialogue_System.Types;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Dialogue_System
{
    [CreateAssetMenu]
    public class DialogueTree : ScriptableObject
    {
        public enum State
        {
            Stopped,
            Running
        }

        public State state;
        public DialogueSystemSettings settings;
        public List<DialogueNode> nodes = new ();
        public DialogueNode entryNode;

        public DialogueNode current;
        public Action<AudioClip> PlaySound;
        public IDialogueController CurrentController;

        public void BindEvents(List<DialogueEvent> events)
        {
            foreach (var node in nodes.OfType<EventNode>())
            {
                foreach (var @event in events)
                {
                    if (@event.key == node.eventName)
                    {
                        node.EventBinding = @event.action;
                    }
                }
            }
        }
        
        public void BindAudioSource(AudioSource source)
        {
            PlaySound = clip =>
            {
                source.clip = clip;
                source.Play();
            };
        }

        public bool Update()
        {
            if(CurrentController == null)
                Debug.Log("Current controller is null, expect errors");
            if (state == State.Stopped)
            {
                current = entryNode;
                if (entryNode is EntryNode node)
                {
                    node.interactionCount++;
                }
                current.OnEnter(CurrentController);
                state = State.Running;
            }

            if (current != null && current.OnUpdate(CurrentController))
            {
                current.OnExit(CurrentController);
                current = current.GetNextNode();
                if(current != null)
                    current.OnEnter(CurrentController);
            }

            if (current != null) return false;
            
            state = State.Stopped;
            return true;
        }
        
#if UNITY_EDITOR

        public DialogueNode CreateNode(Type type, Vector2 position = default)
        {
            var node = CreateInstance(type) as DialogueNode;

            Undo.RecordObject(this, "Add Dialogue Node");
            
            node.position = position;
            node.parent = this;
            node.name = type.Name;
            node.guid = Guid.NewGuid().ToString();
            nodes.Add(node);
            
            EditorUtility.SetDirty(this);
            
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Add Dialogue Node");

            AssetDatabase.SaveAssets();
            
            return node;
        }

        public DialogueNode CopyNode(DialogueNode source, Vector2 position = default)
        {
            var node = Instantiate(source);
            
            node.hideFlags = HideFlags.None;
            node.position = position;
            node.parent = this;
            node.name = source.name;
            node.guid = Guid.NewGuid().ToString();
            nodes.Add(node);
            
            EditorUtility.SetDirty(this);
            
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Paste in Dialogue Node");
            
            AssetDatabase.SaveAssets();
            
            return node;
        }

        public void RemoveNode(DialogueNode node)
        {
            if (node == null) return;
            Undo.RecordObject(this, "Remove Dialogue Node");
            
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            
            AssetDatabase.SaveAssets();
        }

        public void AddChild(DialogueNode parent, DialogueNode child, int inputIndex, int outputIndex)
        {
            Undo.RecordObject(parent, $"Connect {parent.EditorName} to {child.EditorName}");
            parent.Outputs[outputIndex].children.Add(new NodeConnection(child, inputIndex));
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(DialogueNode parent, DialogueNode child, int inputIndex, int outputIndex)
        {
            Undo.RecordObject(parent, $"Disconnect {child.EditorName} from {parent.EditorName}");
            if(parent.Outputs.Count > outputIndex)
                parent.Outputs[outputIndex].children.Remove(new NodeConnection(child, inputIndex));
            EditorUtility.SetDirty(parent);
        }
#endif
    }
}