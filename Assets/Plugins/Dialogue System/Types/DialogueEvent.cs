using System;
using UnityEngine.Events;

[Serializable]
public class DialogueEvent
{
    public string key;
    public UnityEvent action;
}