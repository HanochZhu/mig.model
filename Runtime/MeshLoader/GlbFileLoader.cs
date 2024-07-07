using GLTFast;
using System;
using System.IO;
using UnityEngine;

namespace Mig.Model.ModelLoader
{
    public class GlbFileLoader : IModelLoader
    {
        private ModelOperateState operateState = ModelOperateState.LOADING;

        private Transform parent;

        public void SetParent(Transform root)
        {
            parent = root;
        }
        public string ErrorMsg()
        {
            throw new NotImplementedException();
        }

        public string GetLoaderName()
        {
            throw new NotImplementedException();
        }

        public float GetPercentage()
        {
            throw new NotImplementedException();
        }

        public ModelOperateState GetState()
        {
            return operateState;
        }

        public async void LoadAsync(string path, Action<GameObject> _callback)
        {
            operateState = ModelOperateState.LOADING;


            byte[] data = File.ReadAllBytes(path);
            var gltf = new GltfImport();
            bool success = await gltf.LoadGltfBinary(
                data,
                // The URI of the original data is important for resolving relative URIs within the glTF
                new Uri(path)
                );
            if (success)
            {
                success = await gltf.InstantiateMainSceneAsync(parent);

                // TODO
                if (success)
                {
                    _callback?.Invoke(parent.gameObject);
                }
                else
                {
                    _callback.Invoke(null);
                }
            }
            operateState = ModelOperateState.LOAD_COMPLETE;
        }

        public void OnDispose()
        {

        }
    }
}
