using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace MyGame.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Physics)]
    [ActionDescription("Sends an event when another object enters the trigger, with optional tag and layer filtering.")]
    public sealed class OnTriggerEnterAdvanced : BaseTriggerEventAction<OnTriggerEnterEvent>
    {
        [Tooltip("Filter by Layer Mask. Only colliders on selected layers will trigger this action.")]
        [SerializeField, OptionalField]
        private LayerMaskVar _collideWithLayer;

        [Tooltip("Store the GameObject of the colliding object.")]
        [SerializeField, OptionalField, WriteOnly]
        private GameObjectRef _storeGameObject;

        [Tooltip("Store the layer index of the colliding object.")]
        [SerializeField, OptionalField, WriteOnly]
        private IntegerRef _storeLayer;

        public override bool OnEvent(BaseEvent baseEvent)
        {
            if (baseEvent is OnTriggerEnterEvent triggerEvent && triggerEvent.SentByGameObject == GameObject.Value)
            {
                Collider other = triggerEvent.Collider;
                if (other == null) return false;

                // 1. Tag Filtering (uses base class Tag field)
                if (!string.IsNullOrEmpty(Tag.Value) && !other.CompareTag(Tag.Value))
                {   
                    return false;
                }

                // 2. Layer Mask Filtering
                if (_collideWithLayer.IsAssigned)
                {
                    int otherLayerBit = 1 << other.gameObject.layer;
                    if ((_collideWithLayer.Value.value & otherLayerBit) == 0)
                    {
                        return false;
                    }
                }

                // Store basic collider info via base class field
                if (StoreColliderInfo.HasValue())
                {
                    StoreColliderInfo.Value = other;
                }

                // Store additional optional output variables
                if (_storeGameObject.IsAssigned)
                {
                    _storeGameObject.Value = other.gameObject;
                }

                if (_storeLayer.IsAssigned)
                {
                    _storeLayer.Value = other.gameObject.layer;
                }

                // Send event
                if (_sendEvent.IsSet)
                {
                    SendEvent(_sendEvent);
                }

                return true;
            }

            return false;
        }

        public override string GetSummary() =>
            "OnTriggerEnterAdvanced {GameObject} (Tag: {Tag}, Layer: {_collideWithLayer:output}) -> {_sendEvent} {StoreColliderInfo:output} {_storeGameObject:output}";
    }
}