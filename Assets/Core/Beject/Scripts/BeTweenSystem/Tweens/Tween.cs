using System;
using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public abstract class Tween : ITween
    {
        private TweenState currentState = TweenState.Waiting;
        private float tweenDuration = 0f;

        private AnimationCurve tweenCurve = new AnimationCurve
        {
            keys = new []
            {
                new Keyframe(0f, 0f),
                new Keyframe(1f, 1f),
            }
        };

        public TweenState CurrentState
        {
            get => currentState;
            protected set
            {
                currentState = value;
                if (currentState == TweenState.Complete)
                {
                    OnComplete?.Invoke();
                }
            }
        }

        protected float TweenDuration => tweenDuration;
        protected AnimationCurve TweenCurve => tweenCurve;
        protected bool IsScaledTween { get; private set; }

        public event Action OnComplete;

        public void SetDuration(float value)
        {
            tweenDuration = value;
        }

        public void SetCurve(AnimationCurve value)
        {
            tweenCurve = value;
        }

        public void Play(IBeTweenManager beTweenManager, int tweenId)
        {
            if (beTweenManager.NeedPlay(tweenId))
            {
                CurrentState = TweenState.Processing;
                IsScaledTween = beTweenManager.IsScaledTween(tweenId);
                Play();
            }
        }

        public virtual TweenState TweenProcessing()
        {
            return CurrentState;
        }

        protected float GetInterpolateTime(float endTime, float currentTime)
        {
            float timeFactor = Mathf.Clamp01(1f - endTime - currentTime) / TweenDuration;
            return TweenCurve.Evaluate(timeFactor);
        }

        protected abstract void Play();
    }
}