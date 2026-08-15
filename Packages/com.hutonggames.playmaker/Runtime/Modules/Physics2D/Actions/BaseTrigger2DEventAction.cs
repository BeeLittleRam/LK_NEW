using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    public abstract class BaseTrigger2DEventAction<T> : GameObjectProxyEventAction<T> where T: BaseEvent, IHasCollider2DData, new()
    {
        
        [OptionalField]
        [Tooltip("Event to send.")]
        [SerializeField]
        protected EventRef _sendEvent;
        
        [DisplayOrder(1000)]
        [OptionalField, WriteOnly]
        [Tooltip("Store the Collider2D info.")]
        public Collider2DRef StoreCollider2DInfo;
        
        public override bool OnEvent(BaseEvent baseEvent)
        {
            if (baseEvent is T collisionEvent && collisionEvent.SentByGameObject == GameObject.Value)
            {
                if (StoreCollider2DInfo.HasValue())
                {
                    StoreCollider2DInfo.Value = collisionEvent.Collider2D;
                }

                if (_sendEvent.IsSet)
                {
                    SendEvent(_sendEvent);
                }

                return true;
            }

            return false;
        }
        
        public override string GetSummary()
        {
            var summary = "{GameObject} " + GetType().Name;
            
            if (_sendEvent.IsSet)
            {
                summary += " {_sendEvent}";
            }
            
            if (StoreCollider2DInfo.HasValue())
            {
                summary += " -> {StoreCollider2DInfo}";
            }

            return summary;
        }
    }
}