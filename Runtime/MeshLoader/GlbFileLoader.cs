using System;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityGLTF;

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

            ImportOptions glftLoaderOptions = new ImportOptions();


            byte[] data = File.ReadAllBytes(path);
            var gltfImporter = new GLTFSceneImporter(path, glftLoaderOptions);

            await gltfImporter.LoadSceneAsync(onLoadComplete: (result, info) =>
            {
                if (info == null)
                {
                    foreach (Transform child in result.transform)
                    {
                        child.SetParent(parent);
                    }
                    _callback?.Invoke(parent.gameObject);
                }
                else
                {
                    _callback.Invoke(null);
                }
                operateState = ModelOperateState.LOAD_COMPLETE;

            });
        }

        public void OnDispose()
        {

        }
    }
}
