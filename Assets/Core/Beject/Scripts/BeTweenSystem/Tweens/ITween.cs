using System;
using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public interface ITween
    {
        TweenState CurrentState { get; }
        
        event Action OnComplete;
        
        void SetDuration(float value);
        void SetCurve(AnimationCurve value);

        void Play(IBeTweenManager beTweenManager, int tweenId);
        TweenState TweenProcessing();
    }
}