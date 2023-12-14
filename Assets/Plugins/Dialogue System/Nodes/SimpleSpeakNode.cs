#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using Dialogue_System.Types;
using Fields;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue_System.Nodes
{
    public class SimpleSpeakNode : DialogueNode
    {
        [TextArea(3, 15)]
        public string text;

        public bool flipAvatarPosition;
        [SerializeField] private Character character;
        [SerializeField] private CharacterData characterOverride;

        public CharacterData CharacterData
        {
            get
            {
                var data = character?.characterData ?? new CharacterData();
                if (characterOverride.characterName.valueSet)
                    data.characterName = characterOverride.characterName;
                if (characterOverride.avatar.valueSet)
                    data.avatar = characterOverride.avatar;
                if (characterOverride.sentenceSound.valueSet)
                    data.sentenceSound = characterOverride.sentenceSound;
                if (characterOverride.typingSound.valueSet)
                    data.typingSound = characterOverride.typingSound;
                return data;
            }
        }

        protected float _timer;
        private float _interactionCount;
        
        public override Input[] GetDefaultInputs() => new []{new Input("", orientation: Orientation.Vertical)};
        public override Output[] GetDefaultOutputs() => new []{new Output("", orientation: Orientation.Vertical)};
        
        public override bool OnUpdate(IDialogueController controller)
        {
            if (_timer > 0)
                _timer -= Time.unscaledDeltaTime;
            return _timer <= 0;
        }

        public override void ModifyVisualElement(VisualElement element)
        {
#if UNITY_EDITOR
            var preview = element.Q<Label>("Preview");
            var avatarPreview = element.Q<Image>("Avatar");
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
            });
#endif
        }

        public override void OnEnter(IDialogueController controller)
        {
            _timer = text.Length / parent.settings.charactersPerSecond;
            _interactionCount = 0;
            
            controller.InteractionCallback += OnInteraction;
            controller.UpdateText(text, flipAvatarPosition);
            controller.UpdateAvatar(CharacterData.avatar, flipAvatarPosition);
            controller.UpdateCharacterName(CharacterData.characterName, flipAvatarPosition);
            
            if(CharacterData.sentenceSound.valueSet)
                parent.PlaySound?.Invoke(CharacterData.sentenceSound.value);
        }

        public override void OnExit(IDialogueController controller)
        {
            controller.InteractionCallback -= OnInteraction;
        }

        protected void OnInteraction()
        {
            _interactionCount++;
            if (_interactionCount > 1 || _timer == 0f)
                _timer = 0;
        }
    }
}