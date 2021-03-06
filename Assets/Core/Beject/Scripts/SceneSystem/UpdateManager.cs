using System.Collections.Generic;
using UnityEngine;

namespace MBSCore.Beject.SceneSystem
{
    [AddComponentMenu("Beject/SceneSystem/Update Manager")]
    public class UpdateManager : MonoBehaviour, IUpdateManager
    {
        private readonly List<IBeUpdate> UpdaterList = new List<IBeUpdate>();
        private readonly List<IBeUpdate> RemoveUpdaterList = new List<IBeUpdate>();
        
        private bool sceneWasLoaded = false;

        [InjectProperty]
        private IBeUpdate[] BeUpdates
        {
            set => UpdaterList.AddRange(value);
        }
        
        public void AddUpdater(IBeUpdate updater)
        {
            if (!UpdaterList.Contains(updater))
            {
                UpdaterList.Add(updater);
            }
        }

        public void RemoveUpdater(IBeUpdate updater)
        {
            if (!RemoveUpdaterList.Contains(updater))
            {
                RemoveUpdaterList.Add(updater);
            }
        }

        private void UpdateHandler()
        {
            for (int i = UpdaterList.Count - 1; i >= 0; i--)
            {
                IBeUpdate updater = UpdaterList[i];
                if (updater.IsActive)
                {
                    updater.Update();
                }
            }
        }

        private void FixUpdateHandler()
        {
            for (int i = UpdaterList.Count - 1; i >= 0; i--)
            {
                IBeUpdate updater = UpdaterList[i];
                if (updater.IsActive)
                {
                    updater.FixUpdate();
                }
            }
        }

        private void LateUpdateHandler()
        {
            for (int i = UpdaterList.Count - 1; i >= 0; i--)
            {
                IBeUpdate updater = UpdaterList[i];
                if (updater.IsActive)
                {
                    updater.LateUpdate();
                }
            }
        }

        private void RemoveUpdateHandler()
        {
            for (int i = RemoveUpdaterList.Count - 1; i >= 0; i--)
            {
                IBeUpdate updater = RemoveUpdaterList[i];
                UpdaterList.Remove(updater);
            }
            
            RemoveUpdaterList.Clear();
        }

        private void Start()
        {
            for (int i = UpdaterList.Count - 1; i >= 0; i--)
            {
                UpdaterList[i].Awake();
            }

            sceneWasLoaded = true;
        }

        private void Update()
        {
            if (sceneWasLoaded == false)
            {
                return;
            }

            UpdateHandler();
        }
        
        private void FixedUpdate()
        {
            if (sceneWasLoaded == false)
            {
                return;
            }

            FixUpdateHandler();
        }
        
        private void LateUpdate()
        {
            if (sceneWasLoaded == false)
            {
                return;
            }

            LateUpdateHandler();
            RemoveUpdateHandler();
        }
    }
}