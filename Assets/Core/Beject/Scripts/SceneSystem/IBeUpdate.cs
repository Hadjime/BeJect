namespace MBSCore.Beject.SceneSystem
{
    public interface IBeUpdate
    {
        bool IsActive { get; }
        
        void Awake();
        void Update();
        void FixUpdate();
        void LateUpdate();
    }
}