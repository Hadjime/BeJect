﻿using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public class MoveTween : Tween, IMoveTween
    {
        private Transform currentTransform;
        private Vector3 targetPosition;
        private float endTweenTime = 0f;

        public void BeMove(Transform target, Vector3 moveTarget)
        {
            currentTransform = target;
            targetPosition = moveTarget;
        }

        public override TweenState TweenProcessing()
        {
            if (CurrentState == TweenState.Waiting ||
                CurrentState == TweenState.Complete)
            {
                return CurrentState;
            }

            float currentTime = IsScaledTween ? Time.time : Time.unscaledTime;
            float interpolateTime = GetInterpolateTime(endTweenTime, currentTime);
            Vector3 currentPosition = currentTransform.position;
            currentTransform.position = Vector3.Lerp(currentPosition, targetPosition, interpolateTime);
            return IsComplete();
        }

        protected override void Play()
        {
            float currentTime = IsScaledTween ? Time.time : Time.unscaledTime;
            endTweenTime = currentTime + TweenDuration;
        }

        private TweenState IsComplete()
        {
            Vector3 currentPosition = currentTransform.position;
            Vector3 heading = targetPosition - currentPosition;
            CurrentState = Mathf.Approximately(heading.sqrMagnitude, 0f) ?
                TweenState.Complete :
                TweenState.Processing;
            return CurrentState;
        }
    }
}