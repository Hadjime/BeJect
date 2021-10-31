namespace MBSCore.BeTweenSystem
{
    public interface IBeTweenManager
    {
        int CreateScaledTween();
        int CreateUnscaledTween();
        void PlayTween(int tweenId);
        bool IsPlayed(int tweenId);
        void StopTween(int tweenId);
        bool NeedPlay(int tweenId);
        bool IsScaledTween(int tweenId);
    }
}