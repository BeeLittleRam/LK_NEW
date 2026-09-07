using System;
using UnityEngine;
using HutongGames.PlayMaker;
using Spine;
using Spine.Unity;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory("Spine")]
    [ActionDescription("Listens for a Spine 4.3 animation event and sends a PlayMaker event.")]
    public sealed class SpineUnityEventAction : BaseOnEventAction
    {
        [ActionTarget]
        [Tooltip("GameObject containing the Spine 4.3 SkeletonAnimation component.")]
        [SerializeField]
        private GameObject _gameObject;

        [Tooltip("Name of the Spine event to listen for.")]
        [SerializeField]
        private StringVar _spineEventName;

        [Tooltip("PlayMaker event to send when the Spine event occurs.")]
        [SerializeField]
        private EventRef _playMakerEvent;


        private SkeletonAnimation _skeletonAnimation;


        // ---------------------------------------------------------
        // Validation
        // ---------------------------------------------------------

        public override bool CanExecute()
        {
            return CheckParameters(
                _gameObject,
                _spineEventName,
                _playMakerEvent
            );
        }


        // ---------------------------------------------------------
        // Start
        // ---------------------------------------------------------

        public override void OnStart()
        {
            // -----------------------------------------------------
            // Validate GameObject
            // -----------------------------------------------------

            if (_gameObject == null)
            {
                LogWarning(
                    "SpineUnityEventAction: No GameObject assigned."
                );

                return;
            }


            // -----------------------------------------------------
            // Validate Event Name
            // -----------------------------------------------------

            if (_spineEventName == null ||
                string.IsNullOrEmpty(_spineEventName.Value))
            {
                LogWarning(
                    "SpineUnityEventAction: Spine Event Name is empty."
                );

                return;
            }


            // -----------------------------------------------------
            // Find SkeletonAnimation
            // -----------------------------------------------------

            _skeletonAnimation =
                _gameObject.GetComponent<SkeletonAnimation>();


            if (_skeletonAnimation == null)
            {
                LogWarning(
                    "SpineUnityEventAction: GameObject '" +
                    _gameObject.name +
                    "' does not have a Spine SkeletonAnimation component."
                );

                return;
            }


            // -----------------------------------------------------
            // Validate AnimationState
            // -----------------------------------------------------

            if (_skeletonAnimation.AnimationState == null)
            {
                LogWarning(
                    "SpineUnityEventAction: AnimationState is null on '" +
                    _gameObject.name +
                    "'."
                );

                return;
            }


            // -----------------------------------------------------
            // Subscribe to Spine Event
            // -----------------------------------------------------

            _skeletonAnimation.AnimationState.Event +=
                OnSpineEvent;
        }


        // ---------------------------------------------------------
        // Spine Event Callback
        // ---------------------------------------------------------

        private void OnSpineEvent(
            TrackEntry trackEntry,
            Spine.Event spineEvent)
        {
            if (spineEvent == null)
                return;

            if (spineEvent.Data == null)
                return;

            if (_spineEventName == null)
                return;


            // -----------------------------------------------------
            // Compare Event Names
            // -----------------------------------------------------

            if (spineEvent.Data.Name != _spineEventName.Value)
                return;


            // -----------------------------------------------------
            // Send PlayMaker 2 Event
            // -----------------------------------------------------

            SendEvent(_playMakerEvent);
        }


        // ---------------------------------------------------------
        // Stop
        // ---------------------------------------------------------

        public override void OnStop()
        {
            // -----------------------------------------------------
            // Unsubscribe from Spine Event
            // -----------------------------------------------------

            if (_skeletonAnimation == null)
                return;

            if (_skeletonAnimation.AnimationState == null)
                return;


            _skeletonAnimation.AnimationState.Event -=
                OnSpineEvent;


            _skeletonAnimation = null;
        }


        // ---------------------------------------------------------
        // Summary
        // ---------------------------------------------------------

        public override string GetSummary()
        {
            return "Listen for Spine event {_spineEventName} -> {_playMakerEvent}";
        }
    }
}