using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dialogue_System.Editor
{
    public class SinglePanelView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SinglePanelView, UxmlTraits> {}

        public SinglePanelView()
        {
            
        }

        public void UpdateSelection(int panelIndex)
        {
            for (int i = 0; i < childCount; i++)
            {
                this[i].style.display = new StyleEnum<DisplayStyle>(i == panelIndex ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }
    }
}