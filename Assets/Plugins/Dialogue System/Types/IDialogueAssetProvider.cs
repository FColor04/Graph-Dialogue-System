using System.Collections.Generic;

namespace Dialogue_System.Types
{
    public interface IDialogueAssetProvider
    {
        public IEnumerable<DialogueTree> DialogueAssets { get; }
    }
}