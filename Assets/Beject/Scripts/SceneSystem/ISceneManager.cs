namespace MBSCore.Beject.SceneSystem
{
    public interface ISceneManager
    {
        void AddUpdater(IBeUpdate updater);
        void RemoveUpdater(IBeUpdate updater);
    }
}