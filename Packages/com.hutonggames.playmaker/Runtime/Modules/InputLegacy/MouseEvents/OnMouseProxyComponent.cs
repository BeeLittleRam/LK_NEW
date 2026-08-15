namespace HutongGames.PlayMaker
{
    // NOTE: We include all one-off mouse events in one proxy component.
    // Not sure if this is a lot more expensive, but seems okay for now.
    // We can separate these into different components if needed.
    public class OnMouseProxyComponent : BaseProxyEventComponent
    {
        public void OnMouseDown() => SendEvent(OnMouseDownEvent.Instance);

        public void OnMouseUp() => SendEvent(OnMouseUpEvent.Instance);
        
        public void OnMouseUpAsButton() => SendEvent(OnMouseUpAsButtonEvent.Instance);

        public void OnMouseEnter() => SendEvent(OnMouseEnterEvent.Instance);
        
        public void OnMouseExit() => SendEvent(OnMouseExitEvent.Instance);
    }
}