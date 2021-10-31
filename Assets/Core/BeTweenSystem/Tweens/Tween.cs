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

        public int TweenId { get; private set; }

        public TweenState CurrentState
        {
            get => currentState;
            protected set
            {
                currentState = value;
                if (currentState == TweenState.Complete)
                {
                    OnComplete?.Invoke(this);
                }
            }
        }

        protected float TweenDuration => tweenDuration;
        protected AnimationCurve TweenCurve => tweenCurve;
        protected bool IsScaledTween { get; private set; }

        public event Action<ITween> OnComplete;

        public void SetTweenId(int value)
        {
            TweenId = value;
        }

        public void SetDuration(float value)
        {
            if (CurrentState != TweenState.Waiting)
            {
                return;
            }
            
            tweenDuration = value;
        }

        public void SetCurve(AnimationCurve value)
        {
            if (CurrentState != TweenState.Waiting)
            {
                return;
            }
            
            tweenCurve = value;
        }

        public void Play(IBeTweenManager beTweenManager, int tweenId)
        {
            if (beTweenManager.NeedPlay(tweenId) == false)
            {
                return;
            }

            TweenId = tweenId;
            CurrentState = TweenState.Processing;
            IsScaledTween = beTweenManager.IsScaledTween(TweenId);
            Play();
        }

        public void Stop(IBeTweenManager beTweenManager, int tweenId)
        {
            if (TweenId != tweenId ||
                beTweenManager.NeedStop(tweenId) == false)
            {
                return;
            }

            Stop();
            CurrentState = TweenState.Complete;
        }

        public virtual TweenState TweenProcessing()
        {
            return CurrentState;
        }

        protected float GetInterpolateTime(float endTime, float currentTime)
        {
            float timeFactor = 1f - (endTime - currentTime) / TweenDuration;
            return TweenCurve.Evaluate(timeFactor);
        }

        protected abstract void Play();
        protected abstract void Stop();
    }
}