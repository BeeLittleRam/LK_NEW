using UnityEngine;
using HutongGames.PlayMaker;
using Spine;
using Spine.Unity;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Spine")]
    [Tooltip("Sets a Spine 4.3 SkeletonAnimation animation.")]
    public class PlaymakerSpineSetAnimation : BaseAction
    {
        [RequiredField]
        [Tooltip("GameObject containing the Spine 4.3 SkeletonAnimation component.")]
        public GameObject gameObject;

        [RequiredField]
        [Tooltip("Spine animation name. Can be assigned from a PlayMaker 2 String Variable.")]
        public StringVar animationName;

        [Tooltip("Spine animation track. Usually 0 for the main animation.")]
        public int trackIndex = 0;

        [Tooltip("Whether the animation should loop.")]
        public bool loop = true;

        [Tooltip("Restart the animation if the requested animation is already playing.")]
        public bool restart = false;

        [Tooltip("Clear the selected track before setting the animation.")]
        public bool clearTrack = false;


        public override void Reset()
        {
            gameObject = null;
            animationName = null;

            trackIndex = 0;
            loop = true;
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
            // ---------------------------------------------------------
            // Validate GameObject
            // ---------------------------------------------------------

            if (gameObject == null)
            {
                Debug.LogWarning(
                    "PlaymakerSpineSetAnimation: No GameObject assigned."
                );

                Finish();
                return;
            }


            // ---------------------------------------------------------
            // Validate Animation Name
            // ---------------------------------------------------------

            if (animationName == null ||
                string.IsNullOrEmpty(animationName.Value))
            {
                Debug.LogWarning(
                    "PlaymakerSpineSetAnimation: Animation Name is empty."
                );

                Finish();
                return;
            }


            string requestedAnimation = animationName.Value;


            // ---------------------------------------------------------
            // Get SkeletonAnimation
            // ---------------------------------------------------------

            SkeletonAnimation skeletonAnimation =
                gameObject.GetComponent<SkeletonAnimation>();

            if (skeletonAnimation == null)
            {
                Debug.LogWarning(
                    "PlaymakerSpineSetAnimation: GameObject '" +
                    gameObject.name +
                    "' does not have a Spine SkeletonAnimation component."
                );

                Finish();
                return;
            }


            // ---------------------------------------------------------
            // Get AnimationState
            // ---------------------------------------------------------

            Spine.AnimationState animationState =
                skeletonAnimation.AnimationState;

            if (animationState == null)
            {
                Debug.LogWarning(
                    "PlaymakerSpineSetAnimation: AnimationState is null on '" +
                    gameObject.name +
                    "'."
                );

                Finish();
                return;
            }


            // ---------------------------------------------------------
            // Validate Animation Exists
            // ---------------------------------------------------------

            Spine.Animation animation =
                skeletonAnimation.Skeleton.Data.FindAnimation(
                    requestedAnimation
                );

            if (animation == null)
            {
                Debug.LogWarning(
                    "PlaymakerSpineSetAnimation: Animation '" +
                    requestedAnimation +
                    "' was not found on '" +
                    gameObject.name +
                    "'."
                );

                Finish();
                return;
            }


            // ---------------------------------------------------------
            // Validate Track
            // ---------------------------------------------------------

            int track = Mathf.Max(0, trackIndex);


            // ---------------------------------------------------------
            // Get Current TrackEntry
            //
            // Spine 4.3:
            // GetTrack() is used to retrieve the current TrackEntry.
            // ---------------------------------------------------------

            TrackEntry currentEntry =
                animationState.GetTrack(track);


            // ---------------------------------------------------------
            // Don't restart if already playing
            // ---------------------------------------------------------

            if (!restart &&
                currentEntry != null &&
                currentEntry.Animation != null &&
                currentEntry.Animation.Name == requestedAnimation)
            {
                Finish();
                return;
            }


            // ---------------------------------------------------------
            // Clear Track
            // ---------------------------------------------------------

            if (clearTrack)
            {
                animationState.ClearTrack(track);
            }


            // ---------------------------------------------------------
            // Set Animation
            // ---------------------------------------------------------

            animationState.SetAnimation(
                track,
                requestedAnimation,
                loop
            );


            // ---------------------------------------------------------
            // Finish
            // ---------------------------------------------------------

            Finish();
        }
    }
}