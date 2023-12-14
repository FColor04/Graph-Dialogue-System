using System;
using UnityEngine;

namespace Dialogue_System
{
    public class DialogueTreeRunner : MonoBehaviour
    {
        public DialogueTree tree;
        public bool run = true;

        private void Start()
        {
            tree.state = DialogueTree.State.Stopped;
        }

        private void Update()
        {
            if (run)
            {
                run = run && !tree.Update();
            }
        }
    }
}