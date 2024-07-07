using Mig.Core;
using Mig.Model.ModelLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mig.Model
{
    public class ModelManager: JigSingleton<ModelManager>
    {
        public Action OnModelLoadCompleteEvent;
        // Todo
        [HideInInspector]
        public GameObject Center;

        // TODO
        [JsonIgnore]
        public Material CurrentMaterial { get; set; }
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
                }
                return currentGameObjectRoot;
            }
            private set
            {
                currentGameObjectRoot = value;
            }
        }

        public Renderer CurrentSelectMeshRender
        {
            get
            {
                return CurrentSelectGameObject.GetComponent<Renderer>();
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(Events.OnSelectedChanged, OnSelectedChanged);
            EventManager.StartListening(Events.OnDeleteModel, OnOnDeleteModel);
        }

        private void OnDisable()
        {
            EventManager.StopListening(Events.OnSelectedChanged, OnSelectedChanged);
            EventManager.StopListening(Events.OnDeleteModel, OnOnDeleteModel);
        }

        private void OnSelectedChanged(object selected, object arg1)
        {
            GameObject obj = (GameObject)selected;
            CurrentSelectGameObject = obj;
            if (obj.GetComponent<MeshRenderer>() != null)
                CurrentMaterial = obj.GetComponent<MeshRenderer>()?.materials[0];
            else
                CurrentMaterial = null;
        }

        public void LoadFormFilePickerAsync(IModelLoader ModelFilePickAndLoader)
        {
            if (m_currentLoader != null)
            {
                Debug.LogError("Failed to load model from file. Waiting for last loader complete");
                return;
            }

            m_currentLoader = ModelFilePickAndLoader;

            m_currentLoader.SetParent(CurrentGameObjectRoot.transform);

            m_currentLoader.LoadAsync("", OnLoadComplete);
        }


        public void LoadGLBFromFileAsync(string glbPath, IModelLoader GlbFileLoader)
        {
            if (m_currentLoader != null)
            {
                Debug.LogError("[ModelManager] Failed to load model from web. Waiting for last loader complete");
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

            var saveWebDir = Path.Combine(FTPClient.GetCurrentFTPDirRoot(), CurrentProjectName);

            var isSuccess = FTPClient.MakeDir(saveWebDir);

            if (!isSuccess)
            {
                Debug.LogError($"[Mig] Failed to make dir at  {saveWebDir}");
                return;
            }

            m_currentSaver.Save(saveWebDir, serializer, OnSaveComplete);
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
            Destroy(CurrentGameObjectRoot);
        }
        private void OnLoadComplete(GameObject loadedModelRoot)
        {
            if (m_currentLoader.GetState() == ModelOperateState.ERROR)
            {
                Debug.LogError($"Failed to load module from {m_currentLoader.GetLoaderName()}, Detail {m_currentLoader.ErrorMsg()}");
                return;
            }
            if (loadedModelRoot != CurrentGameObjectRoot)
            {
                loadedModelRoot.transform.SetParent(CurrentGameObjectRoot.transform);
            }
            CurrentSelectGameObject = loadedModelRoot;
            m_loadedModel.Add(loadedModelRoot);
            m_currentLoader.OnDispose();
            m_currentLoader = null;

            ModelPostProcess.ProcessModel(loadedModelRoot);

            OnModelLoadCompleteEvent?.Invoke();

            EventManager.TriggerEvent(Events.OnModelRefresh, loadedModelRoot);

        }
    }

}

