using System;
using System.IO;
using UnityEngine;
using UnityGLTF;

namespace Mig.Model.ModelLoader
{
    public class GlbFileLoader : IModelLoader
    {
        private ModelOperateState operateState = ModelOperateState.LOADING;
        private string lastError = "";
        private string lastPath = "";

        private Transform parent;
        public void SetParent(Transform root)
        {
            parent = root;
        }
        public string ErrorMsg()
        {
            return lastError;
        }

        public string GetLoaderName()
        {
            return nameof(GlbFileLoader);
        }

        public float GetPercentage()
        {
            return operateState == ModelOperateState.LOAD_COMPLETE ? 1f : 0f;
        }

        public ModelOperateState GetState()
        {
            return operateState;
        }

        public async void LoadAsync(string path, Action<GameObject> _callback)
        {
            operateState = ModelOperateState.LOADING;
            lastPath = path;
            lastError = "";

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                lastError = $"GLB file not found: {path}";
                operateState = ModelOperateState.ERROR;
                Debug.LogError($"[Mig] {lastError}");
                _callback?.Invoke(null);
                return;
            }

            if (parent == null)
            {
                lastError = "GLB loader parent is not set";
                operateState = ModelOperateState.ERROR;
                _callback?.Invoke(null);
                return;
            }

            ImportOptions glftLoaderOptions = new ImportOptions();
            var gltfImporter = new GLTFSceneImporter(path, glftLoaderOptions);

            await gltfImporter.LoadSceneAsync(onLoadComplete: (result, info) =>
            {
                if (info == null && result != null)
                {
                    foreach (Transform child in result.transform)
                    {
                        child.SetParent(parent);
                    }
                    operateState = ModelOperateState.LOAD_COMPLETE;
                    _callback?.Invoke(parent.gameObject);
                    return;
                }

                lastError = info != null ? info.SourceException?.Message ?? "GLTF import failed" : "GLTF import returned no scene";
                operateState = ModelOperateState.ERROR;
                Debug.LogError($"[Mig] Failed to load GLB {lastPath}: {lastError}");
                _callback?.Invoke(null);
            });
        }

        public void OnDispose()
        {

        }
    }
}
