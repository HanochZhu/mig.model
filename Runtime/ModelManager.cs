using JigSpace;
using Mig.Core;
using Mig.Model.ModelLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using UnityEngine;

namespace Mig.Model
{
    public class ModelManager: JigSingleton<ModelManager>
    {
        public Action OnModelLoadCompleteEvent;
        public Action OnModelLoadFailedEvent;
        // Todo
        [HideInInspector]
        public GameObject Center;

        // TODO
        [JsonIgnore]
        public MigMaterial CurrentMaterial { get; private set; }

        private List<GameObject> m_loadedModel = new();

        private IModelLoader m_currentLoader;

        private IModelSaver m_currentSaver;

        private GameObject currentGameObjectRoot;

        public GameObject CurrentSelectGameObject { get; private set; }

        public GameObject CurrentGameObjectRoot
        {
            get
            {
                if (currentGameObjectRoot == null)
                {
                    currentGameObjectRoot = new GameObject("root");
                    MigElementWrapper.WrapperRoot = currentGameObjectRoot;
                }
                return currentGameObjectRoot;
            }
            private set
            {
                currentGameObjectRoot = value;
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(MigEventCommon.OnClickModel, OnClickModel);
            EventManager.StartListening(Events.OnDeleteModel, OnOnDeleteModel);
            EventManager.StartListening(MigEventCommon.OnChangeSelectModelTexture, OnChangeSelectModelTexture);
        }

        private void OnDisable()
        {
            EventManager.StopListening(MigEventCommon.OnClickModel, OnClickModel);
            EventManager.StopListening(Events.OnDeleteModel, OnOnDeleteModel);
            EventManager.StopListening(MigEventCommon.OnChangeSelectModelTexture, OnChangeSelectModelTexture);

        }

        private void OnChangeSelectModelTexture(object arg0, object arg1)
        {
            if (this.CurrentMaterial == null)
            {
                return;
            }

            var tex = (Texture)arg0;

            if (tex == null)
            {
                this.CurrentMaterial = null;
                return;
            }

            this.CurrentMaterial.mainTexture = tex;
        }

        private void OnClickModel(object obj, object arg1)
        {
            if (obj == null)
            {
                if (CurrentSelectGameObject != null)
                {
                    this.CurrentSelectGameObject = null;
                    this.CurrentMaterial = null;
                    EventManager.TriggerEvent(MigEventCommon.OnSelectedChanged, null);
                }
                return;
            }

            GameObject selected = (GameObject)obj;

            if (CurrentSelectGameObject == selected)
            {
                return;
            }

            var selectedRenderer = selected.GetComponent<Renderer>();
            if (selectedRenderer == null)
            {
                return;
            }

            CurrentMaterial = new MigMaterial(selectedRenderer.material, selected);
            CurrentSelectGameObject = selected;
            EventManager.TriggerEvent(MigEventCommon.OnSelectedChanged, CurrentSelectGameObject);
        }

        public void LoadFormFilePickerAsync(IModelLoader ModelFilePickAndLoader)
        {
            if (m_currentLoader != null)
            {
                Debug.LogError("Failed to load model from file. Waiting for last loader complete");
                OnModelLoadFailedEvent?.Invoke();
                return;
            }

            m_currentLoader = ModelFilePickAndLoader;

            m_currentLoader.SetParent(new GameObject("LoadingGameObject").transform);

            m_currentLoader.LoadAsync("", OnLoadComplete);
        }


        public void LoadGLBFromFileAsync(string glbPath, IModelLoader GlbFileLoader)
        {
            if (m_currentLoader != null)
            {
                Debug.LogError("[ModelManager] Failed to load model from web. Waiting for last loader complete");
                OnModelLoadFailedEvent?.Invoke();
                return;
            }

            m_currentLoader = GlbFileLoader;
            m_currentLoader.SetParent(CurrentGameObjectRoot.transform);

            m_currentLoader.LoadAsync(glbPath, OnLoadComplete);
        }

        public void SaveProjectToWeb(ISerializer serializer, IModelSaver server, string CurrentProjectName)
        {
            if (m_currentSaver != null)
            {
                Debug.LogWarning("[ModelManager] Failed to save model to web. Waiting for last model saver");
                return;
            }

            m_currentSaver = server;
            m_currentSaver.Save(CurrentProjectName, serializer, OnSaveComplete);
        }

        public void SaveToWeb(IModelSaver ModelSaveToWebSaver)
        {
            if (m_currentSaver != null)
            {
                Debug.LogWarning("[ModelManager] Failed to save model to web. Waiting for last model saver");
                return;
            }

            m_currentSaver = ModelSaveToWebSaver;
            m_currentSaver.Save("", CurrentGameObjectRoot, OnSaveComplete);
        }

        // 
        public void SaveToFile(IModelSaver saver, string filePath)
        {
            if (m_currentSaver != null)
            {
                Debug.LogWarning("[ModelManager] Failed to save model to file. Waiting for last model saver");
                return;
            }

            //string filePath = FileBrowser.Instance.SaveFile("out", "glb");

            m_currentSaver = saver;

            m_currentSaver.Save(filePath, CurrentGameObjectRoot, OnSaveComplete);
        }

        private void OnSaveComplete(bool isSuccess)
        {
            m_currentSaver?.OnDispose();

            m_currentSaver = null;
        }

        public void DestroyCurrentModel()
        {
            // TODO
        }

        private void OnOnDeleteModel(object arg0, object arg1)
        {
            Debug.Log("Destroy Model");
            if (CurrentGameObjectRoot != null)
            {
                Destroy(CurrentGameObjectRoot);
                CurrentGameObjectRoot = null;
            }
            CurrentSelectGameObject = null;
            CurrentMaterial = null;
        }
        private void NotifyLoadFailed()
        {
            if (m_currentLoader != null)
            {
                m_currentLoader.OnDispose();
                m_currentLoader = null;
            }

            OnModelLoadFailedEvent?.Invoke();
        }

        private void OnLoadComplete(GameObject loadedModelRoot)
        {
            if (m_currentLoader == null || m_currentLoader.GetState() == ModelOperateState.ERROR || loadedModelRoot == null)
            {
                if (m_currentLoader != null)
                {
                    Debug.LogError($"Failed to load module from {m_currentLoader.GetLoaderName()}, Detail {m_currentLoader.ErrorMsg()}");
                }
                NotifyLoadFailed();
                return;
            }
            if (loadedModelRoot != CurrentGameObjectRoot)
            {

                var bound = GameObjectExtensions.GetTotalBounds(loadedModelRoot);
                var shouldCenterPos = new Vector3(0, (bound.max.y - bound.min.y) / 2f, 0);

                if (bound.center != shouldCenterPos)
                {
                    var offset = shouldCenterPos - bound.center;
                    loadedModelRoot.transform.position += offset;
                }

                foreach(Transform trans in loadedModelRoot.transform)
                {
                    trans.SetParent(CurrentGameObjectRoot.transform, true);
                }
                GameObject.Destroy(loadedModelRoot.gameObject);
                loadedModelRoot = CurrentGameObjectRoot;
            }
            CurrentSelectGameObject = loadedModelRoot;
            m_loadedModel.Add(loadedModelRoot);
            m_currentLoader.OnDispose();
            m_currentLoader = null;

            ModelPostProcess.ProcessModel(loadedModelRoot);

            OnModelLoadCompleteEvent?.Invoke();
        }
    }

}

