using UnityEngine;

namespace HutongGames.PlayMaker
{
    [System.Serializable]
    [SystemEvent("Mouse")]
    [Tooltip("Sent when the user has clicked on a Collider and is still holding down the mouse.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseDrag.html")]
    public class OnMouseDragEvent : BaseSystemProxyEvent<OnMouseDragEvent, OnMouseDragProxyComponent>
    {
        public override bool HasData => false;
    }
    
    public class OnMouseDragProxyComponent : BaseProxyEventComponent
    {
        public void OnMouseDrag() => SendEvent(OnMouseDragEvent.Instance);
    }
}