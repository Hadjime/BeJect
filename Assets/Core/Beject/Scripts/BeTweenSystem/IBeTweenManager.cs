namespace MBSCore.BeTweenSystem
{
    public interface IBeTweenManager
    {
        int AddScaledTween(ITween tween);
        int AddUnscaledTween(ITween tween);
        void PlayTween(int tweenId);
        bool IsPlayed(int tweenId);
        void StopTween(int tweenId);
        bool NeedPlay(int tweenId);
        bool NeedStop(int tweenId);
        bool IsScaledTween(int tweenId);
    }
}