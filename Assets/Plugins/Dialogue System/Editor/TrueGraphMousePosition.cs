using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue_System.Editor
{
    public class TrueGraphMousePosition : MouseManipulator
    {
        public Vector2 LastMousePosition;
        public Vector2 LastMouseGraphPosition;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
        }
        
        protected void OnMouseMove(MouseMoveEvent e)
        {
            LastMousePosition = e.localMousePosition;
            
            if (!(this.target is UnityEditor.Experimental.GraphView.GraphView target))
                return;
            Vector2 a = target.ChangeCoordinatesTo(target.contentViewContainer, e.localMousePosition);
            LastMouseGraphPosition = a;
            // Debug.Log($"{LastMousePosition} {LastMouseGraphPosition} {target.viewTransform.position} {target.viewTransform.scale}\n{target.contentViewContainer.transform}");
        }
    }
}