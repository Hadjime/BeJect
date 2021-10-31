using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public class BeTweenManager : MonoBehaviour, IBeTweenManager
    {
        public int CreateScaledTween()
        {
            throw new System.NotImplementedException();
        }

        public int CreateUnscaledTween()
        {
            throw new System.NotImplementedException();
        }

        public void PlayTween(int tweenId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsPlayed(int tweenId)
        {
            throw new System.NotImplementedException();
        }

        public void StopTween(int tweenId)
        {
            throw new System.NotImplementedException();
        }

        public bool NeedPlay(int tweenId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsScaledTween(int tweenId)
        {
            throw new System.NotImplementedException();
        }

        private int CalculateTweenId()
        {
            return -1;
        }
        
        private enum TweenType
        {
            None = 0,
            Scaled = 1,
            UnScaled = 2,
        }
    }
}