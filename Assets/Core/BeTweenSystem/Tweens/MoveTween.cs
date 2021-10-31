using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public class MoveTween : Tween, IMoveTween
    {
        private Transform currentTransform;
        private Vector3 beginPosition;
        private Vector3 targetPosition;
        private float endTweenTime = 0f;

        public void BeMove(Transform target, Vector3 moveTarget)
        {
            if (CurrentState != TweenState.Waiting)
            {
                return;
            }
            
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
            currentTransform.position = Vector3.Lerp(beginPosition, targetPosition, interpolateTime);
            return IsComplete();
        }

        protected override void Play()
        {
            beginPosition = currentTransform.position;
            float currentTime = IsScaledTween ? Time.time : Time.unscaledTime;
            endTweenTime = currentTime + TweenDuration;
        }

        protected override void Stop()
        {
            currentTransform.position = targetPosition;
        }

        private TweenState IsComplete()
        {
            Vector3 currentPosition = currentTransform.position;
            Vector3 direction = targetPosition - currentPosition;
            CurrentState = Mathf.Approximately(direction.sqrMagnitude, 0f) ?
                TweenState.Complete :
                TweenState.Processing;
            return CurrentState;
        }
    }
}