using System;
using Fields;
using UnityEngine;

namespace Dialogue_System.Types
{
    [Serializable]
    public struct CharacterData
    {
        public Optional<string> characterName;
        public Optional<Sprite> avatar;
        public Optional<AudioClip> typingSound;
        public Optional<AudioClip> sentenceSound;
    }
}