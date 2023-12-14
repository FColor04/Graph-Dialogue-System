using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue_System.Types
{
    public interface IDialogueController
    {
        public void ClearAll() {}
        public void UpdateText(string text, bool isRight) {Debug.Log($"Display text {text}");}
        public void UpdateCharacterName(string text, bool isRight) {Debug.Log($"Display character name {text}");}
        public void UpdateAvatar(Sprite avatar, bool isRight) {Debug.Log($"Display avatar: {avatar?.name}");}
        public void UpdateTimer(float timeRemaining, float timeMax) {}
        public void DisplayChoices(List<string> choices) {Debug.Log($"Display choices: {string.Join(',', choices)}, Call selection callback when choice is made");}
        public event Action<int> SelectionCallback;
        public event Action InteractionCallback;
    }
}