namespace MBSCore.Beject.SceneSystem
{
    public interface IUpdateManager
    {
        void AddUpdater(IBeUpdate updater);
        void RemoveUpdater(IBeUpdate updater);
    }
}