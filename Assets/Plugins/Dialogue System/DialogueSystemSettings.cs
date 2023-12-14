using Fields;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue_System
{
    [CreateAssetMenu]
    public class DialogueSystemSettings : ScriptableObject
    {
        [FormerlySerializedAs("defaultTypingSpeed")] public float charactersPerSecond = 14;
        public Optional<Sprite> defaultAvatar;
        public Optional<AudioClip> defaultSound;
    }
}