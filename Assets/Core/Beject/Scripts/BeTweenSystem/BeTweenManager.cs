using System;
using System.Collections.Generic;
using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public class BeTweenManager : MonoBehaviour, IBeTweenManager
    {
        private const int SCALED_TWEEN_ID_PREFIX = 0;
        private const int UNSCALED_TWEEN_ID_PREFIX = 1000;

        private readonly List<ITween> scaledTweens = new List<ITween>();
        private readonly List<ITween> unscaledTweens = new List<ITween>();
        private readonly List<ITween> completedTweens = new List<ITween>();
        private readonly List<ITween> needPlay = new List<ITween>();
        private readonly List<ITween> needStop = new List<ITween>();
        
        public int AddScaledTween(ITween tween)
        {
            if (TryFindTween(tween, scaledTweens, out int tweenId))
            {
                return tweenId;
            }

            tweenId = CalculateTweenId(TweenType.Scaled);
            tween.SetTweenId(tweenId);
            tween.OnComplete += TweenCompleteHandler;
            scaledTweens.Add(tween);
            return tweenId;
        }

        public int AddUnscaledTween(ITween tween)
        {
            if (TryFindTween(tween, unscaledTweens, out int tweenId))
            {
                return tweenId;
            }

            tweenId = CalculateTweenId(TweenType.UnScaled);
            tween.SetTweenId(tweenId);
            tween.OnComplete += TweenCompleteHandler;
            unscaledTweens.Add(tween);
            return tweenId;
        }

        public void PlayTween(int tweenId)
        {
            if (IsPlayed(tweenId))
            {
                return;
            }
            
            GetRuntimeTweens(tweenId, out List <ITween> tweens);
            PlayTween(tweenId, tweens);
        }

        public bool IsPlayed(int tweenId)
        {
            GetRuntimeTweens(tweenId, out List <ITween> tweens);
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = tweens[i];
                if (runtimeTween.TweenId == tweenId)
                {
                    return runtimeTween.CurrentState == TweenState.Processing;
                }
            }

            return false;
        }

        public void StopTween(int tweenId)
        {
            GetRuntimeTweens(tweenId, out List <ITween> tweens);
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = tweens[i];
                if (runtimeTween.TweenId == tweenId)
                {
                    needStop.Add(runtimeTween);
                    runtimeTween.Stop(this, tweenId);
                    break;
                }
            }
        }

        public bool NeedPlay(int tweenId)
        {
            for (int i = needPlay.Count - 1; i >= 0; i--)
            {
                ITween needPlayTween = needPlay[i];
                if (needPlayTween.TweenId == tweenId)
                {
                    needPlay.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool NeedStop(int tweenId)
        {
            for (int i = needStop.Count - 1; i >= 0; i--)
            {
                ITween needStopTween = needStop[i];
                if (needStopTween.TweenId == tweenId)
                {
                    needStop.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool IsScaledTween(int tweenId)
        {
            return tweenId - UNSCALED_TWEEN_ID_PREFIX < 0;
        }

        private void PlayTween(int tweenId, List<ITween> tweens)
        {
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = tweens[i];
                if (runtimeTween.TweenId == tweenId)
                {
                    needPlay.Add(runtimeTween);
                    runtimeTween.Play(this, tweenId);
                    break;
                }
            }
        }

        private int CalculateTweenId(TweenType type)
        {
            int tweenId = -1;
            List<ITween> tweens = new List<ITween>();
            if (!TryCalculateTweenData(type, ref tweens, out int tweenPrefix))
            {
                return tweenId;
            }
            
            tweenId = tweenPrefix;
            int normalizedTweenId = 0;
            if (tweens.Count > 0)
            {
                int lastTweenIndex =  tweens.Count - 1;
                normalizedTweenId = tweens[lastTweenIndex].TweenId - tweenPrefix;
            }
            
            return tweenId + normalizedTweenId + 1;
        }

        private bool TryCalculateTweenData(TweenType type, ref List<ITween> tweens, out int tweenPrefix)
        {
            tweenPrefix = -1;
            switch (type)
            {
                case TweenType.Scaled:
                {
                    tweens.AddRange(scaledTweens);
                    tweenPrefix = SCALED_TWEEN_ID_PREFIX;
                } break;

                case TweenType.UnScaled:
                {
                    tweens.AddRange(unscaledTweens);
                    tweenPrefix = UNSCALED_TWEEN_ID_PREFIX;
                } break;

                case TweenType.None:
                default:
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryFindTween(ITween tween, List<ITween> tweens, out int tweenId)
        {
            tweenId = -1;
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = tweens[i];
                if (ReferenceEquals(runtimeTween, tween))
                {
                    tweenId = runtimeTween.TweenId;
                    return true;
                }
            }
            
            return false;
        }

        private void GetRuntimeTweens(int tweenId, out List<ITween> tweens)
        {
            tweens = IsScaledTween(tweenId) ? scaledTweens : unscaledTweens;
        }

        private void UpdateTweenProcessing(List<ITween> tweens)
        {
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                tweens[i].TweenProcessing();
            }
        }
        
        private void RemoveTweenProcessing()
        {
            for (int i = completedTweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = completedTweens[i];
                if (IsScaledTween(runtimeTween.TweenId))
                {
                    scaledTweens.Remove(runtimeTween);
                }
                else
                {
                    unscaledTweens.Remove(runtimeTween);
                }

                runtimeTween = null;
            }

            completedTweens.Clear();
        }

        private void TweenCompleteHandler(ITween tween)
        {
            GetRuntimeTweens(tween.TweenId, out List <ITween> tweens);
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                ITween runtimeTween = tweens[i];
                if (runtimeTween.TweenId == tween.TweenId)
                {
                    completedTweens.Add(runtimeTween);
                    break;
                }
            }
        }

        private void Update()
        {
            UpdateTweenProcessing(scaledTweens);
            UpdateTweenProcessing(unscaledTweens);
            RemoveTweenProcessing();
        }

        private enum TweenType
        {
            None = 0,
            Scaled = 1,
            UnScaled = 2,
        }
    }
}