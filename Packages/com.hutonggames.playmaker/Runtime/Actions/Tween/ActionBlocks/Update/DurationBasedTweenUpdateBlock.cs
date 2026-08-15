using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base class for updating a tween using a duration.
    /// The duration can be specified or calculated
    /// (e.g., based on a desired speed).
    /// </summary>
    [Serializable]
    public abstract class DurationBasedTweenUpdateBlock : TweenUpdateBlock
    {
        [DisplayOrder(-1)] // first
        [Tooltip("Delay before starting the Tween (seconds).")]
        public FloatVar StartDelay;
        
        // Leave room for Duration, Speed, etc.

        [DisplayOrder(100)]
        [Tooltip("Use unscaled realtime.")]
        [FormerlySerializedAs("UnscaledTime")]
        public BoolVar UseRealtime;
        

        [DisplayOrder(101)]
        [OptionalField, WriteOnly]
        [Tooltip("Get the normalized time elapsed." +
                 "\n0 at the start of the tween, 1 at the end.")]
        [SerializeField]
        public FloatRef _getProgress;
        
        /// <summary>
        /// Derived classes must set the duration for the tween.
        /// </summary>
        protected abstract float TweenDuration { get; }
        
        public override void Awake()
        {
            Action.OnStarted -= Start;
            Action.OnStarted += Start;
        }

        private float _elapsedTime;
        private bool _started;
        private bool _reverse;

        protected virtual void Start(BaseAction _)
        {
            _elapsedTime = 0;
            _reverse = false;
            _started = false;
            Finished = false;
        }
        
        // ReSharper disable once CognitiveComplexity
        public override void Execute()
        {
            if (Finished) return;

            _elapsedTime += UseRealtime.Value 
                ? Time.unscaledDeltaTime 
                : Time.deltaTime;

            if (!_started)
            {
                if (_elapsedTime > StartDelay.Value)
                {
                    _started = true;
                    _elapsedTime -= StartDelay.Value;
                }
            }
            
            if (_started && _elapsedTime > TweenDuration)
            {
                switch (TweenAction.LoopMode)
                {
                    case LoopMode.None:
                        Finished = true;
                        _elapsedTime = TweenDuration;
                        break;
                    case LoopMode.Loop:
                        TweenAction.LoopCount++;
                        _elapsedTime -= TweenDuration;
                        break;
                    case LoopMode.PingPong:
                        TweenAction.LoopCount++;
                        _elapsedTime -= TweenDuration;
                        _reverse = !_reverse;
                        break;
                }
            }
            
            if (_getProgress.IsAssigned)
            {
                _getProgress.Value = GetProgress();
            }
        }
        
        public override float GetProgress()
        {
            if (!_started) return 0;
            if (Finished) return 1;
            
            var progress = Mathf.Clamp(_elapsedTime / TweenDuration, 0, 1);
            if (_reverse)
            {
                progress = 1 - progress;
            }

            return progress;
        }
    }
}
