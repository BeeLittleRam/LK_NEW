using UnityEngine;
using HutongGames.PlayMaker;
using Spine;
using Spine.Unity;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Spine")]
    [Tooltip("Sets a Spine animation with strict loop control and prevents frame-by-frame re-triggering.")]
    public class PlaymakerSpineSetAnimation : BaseAction
    {
        [RequiredField]
        [Tooltip("GameObject containing the Spine SkeletonAnimation component.")]
        public GameObject gameObject;

        [RequiredField]
        [Tooltip("Spine animation name.")]
        public StringVar animationName;

        [Tooltip("Spine animation track.")]
        public int trackIndex = 0;

        [Tooltip("Whether the animation should loop.")]
        public bool loop = false;

        [Tooltip("Restart the animation if already playing.")]
        public bool restart = false;

        [Tooltip("Clear the selected track before setting the animation.")]
        public bool clearTrack = false;

        public override void Reset()
        {
            gameObject = null;
            animationName = null;
            trackIndex = 0;
            loop = false;
            restart = false;
            clearTrack = false;
        }

        public override bool CanExecute()
        {
            return gameObject != null &&
                   animationName != null &&
                   !string.IsNullOrEmpty(animationName.Value);
        }

        public override void Execute()
        {
            if (gameObject == null || animationName == null || string.IsNullOrEmpty(animationName.Value))
            {
                Finish();
                return;
            }

            string requestedAnimation = animationName.Value;

            SkeletonAnimation skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
            if (skeletonAnimation == null)
            {
                Finish();
                return;
            }

            Spine.AnimationState animationState = skeletonAnimation.AnimationState;
            if (animationState == null)
            {
                Finish();
                return;
            }

            int track = Mathf.Max(0, trackIndex);
            TrackEntry currentEntry = animationState.GetTrack(track);

            // Prevents the FSM from restarting a non-looping animation every frame
            if (!restart &&
                currentEntry != null &&
                currentEntry.Animation != null &&
                currentEntry.Animation.Name == requestedAnimation)
            {
                Finish();
                return;
            }

            if (clearTrack)
            {
                animationState.ClearTrack(track);
            }

            TrackEntry newEntry = animationState.SetAnimation(
                track,
                requestedAnimation,
                loop
            );

            if (newEntry != null)
            {
                newEntry.Loop = loop;
            }

            Finish();
        }
    }
}