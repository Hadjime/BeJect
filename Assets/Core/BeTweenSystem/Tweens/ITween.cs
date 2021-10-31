using System;
using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public interface ITween
    {
        int TweenId { get; }
        TweenState CurrentState { get; }
        
        event Action<ITween> OnComplete;

        void SetTweenId(int value);
        void SetDuration(float value);
        void SetCurve(AnimationCurve value);

        void Play(IBeTweenManager beTweenManager, int tweenId);
        void Stop(IBeTweenManager beTweenManager, int tweenId);
        
        TweenState TweenProcessing();
    }
}