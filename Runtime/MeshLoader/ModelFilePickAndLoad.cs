using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;

namespace Mig.Model.ModelLoader
{
    public class ModelFilePickAndLoad : IModelLoader
    {
        private ModelOperateState m_currentState;

        private float m_percentage = 0;

        private Exception m_errorException;

        private Transform m_loadParent;

        private Action<GameObject> loadCompleteCallback;

        /// <summary>
        /// Options used in this sample.
        /// </summary>
        protected AssetLoaderOptions AssetLoaderOptions;
        /// <summary>
        /// The AssetLoaderFilePicker instance created for this viewer.
        /// </summary>
        protected AssetLoaderFilePicker FilePickerAssetLoader;

        public void PickFileFromFolderAsync()
        {
            if (m_loadParent == null)
            {
                Debug.LogError("[Mig] parent need, call SetParent first");
                return;
            }
            if (AssetLoaderOptions == null)
            {
                AssetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            }
            AssetLoaderOptions.ImportCameras = true;
            AssetLoaderOptions.ImportLights = true;
            AssetLoaderOptions.LoadPointClouds = false;
            AssetLoaderOptions.AlphaMaterialMode = TriLibCore.General.AlphaMaterialMode.Transparent;
            AssetLoaderOptions.MergeSingleChild = true;
            AssetLoaderOptions.ImportVisibility = false;
            AssetLoaderOptions.ReadEnabled = true;
            FilePickerAssetLoader = AssetLoaderFilePicker.Create();

            FilePickerAssetLoader.LoadModelFromFilePickerAsync("Select a File", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoadModel, OnError, m_loadParent.gameObject, AssetLoaderOptions);
        }

        public void LoadAsync(string path, Action<GameObject> callback)
        {
            loadCompleteCallback = callback;

            PickFileFromFolderAsync();

            m_currentState = ModelOperateState.LOADING;

            if (m_currentState == ModelOperateState.ERROR)
            {
                callback?.Invoke(null);
            }
        }

        private void OnError(IContextualizedError error)
        {
            m_currentState = ModelOperateState.ERROR;
            m_errorException = error.GetInnerException();
        }

        private void OnBeginLoadModel(bool result)
        {
            m_currentState = ModelOperateState.LOADING;
        }

        private void OnMaterialsLoad(AssetLoaderContext context)
        {
            m_currentState = ModelOperateState.LOAD_COMPLETE;

            loadCompleteCallback?.Invoke(m_loadParent.gameObject);
        }

        private void OnProgress(AssetLoaderContext arg1, float arg2)
        {

            m_percentage = arg2;
        }

        private void OnLoad(AssetLoaderContext context)
        {

        }

        public void OnDispose()
        {
            // 
        }

        public void OnLoadComplete(Action<GameObject> go)
        {
            Debug.Log("OnLoadComplete");

        }

        public float GetPercentage()
        {
            return m_percentage;
        }

        public ModelOperateState GetState()
        {
            return m_currentState;
        }

        public string ErrorMsg()
        {
            return m_errorException.ToString();
        }

        public string GetLoaderName()
        {
            return "Model File Picker";
        }

        public void SetParent(Transform root)
        {
            m_loadParent = root;
        }
    }

}
