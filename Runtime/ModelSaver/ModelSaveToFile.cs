using UnityEngine;
using System;
using Mig.Model.Utils;

namespace Mig.Model.ModelSaver
{
    /// <summary>
    ///  We only support one format : glb
    /// </summary>
    public class ModelSaveToFile : IModelSaver
    {
        private ModelOperateState m_state = ModelOperateState.SAVEING;

        public string ErrorMsg()
        {
            return "Something went wrong exporting a glTF";
        }

        public float GetPercentage()
        {
            return m_state == ModelOperateState.SAVEING ? 0 : 1;
        }

        public ModelOperateState GetState() => m_state;

        public void OnDispose()
        {

        }

        public async void Save(string pathORAddress, GameObject modelParent, Action<bool> onSaveComplete)
        {
            var success = await ModelSaveUtils.SaveModelAsGLBTo(modelParent, pathORAddress);

            if (!success)
            {
                m_state = ModelOperateState.ERROR;

                onSaveComplete?.Invoke(false);

                return;
            }

            m_state = ModelOperateState.SAVE_COMPLETE;
            onSaveComplete?.Invoke(true);
        }

        public void Save(string pathORAddress, ISerializer serializer, Action<bool> onSaveComplete)
        {
            throw new NotImplementedException();
        }
    }
}
