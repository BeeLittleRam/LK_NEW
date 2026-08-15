using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    public abstract class BaseCollision2DEventAction<T> : GameObjectProxyEventAction<T> where T: BaseEvent, IHasCollision2DData, new()
    {
        
        [OptionalField]
        [Tooltip("Event to send.")]
        [SerializeField]
        protected EventRef _sendEvent;
        
        [DisplayOrder(1000)]
        [OptionalField, WriteOnly]
        [Tooltip("Store the Collision2D info.")]
        public Collision2DRef StoreCollision2DInfo;
        
        public override bool OnEvent(BaseEvent baseEvent)
        {
            if (baseEvent is T collisionEvent && collisionEvent.SentByGameObject == GameObject.Value)
            {
                if (StoreCollision2DInfo.HasValue())
                {
                    StoreCollision2DInfo.Value = collisionEvent.Collision;
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
            
            if (StoreCollision2DInfo.HasValue())
            {
                summary += " -> {StoreCollision2DInfo}";
            }

            return summary;
        }
    }
}