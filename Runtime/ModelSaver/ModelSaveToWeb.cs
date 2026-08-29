using System;
using UnityEngine;
using System.IO;
using Mig.Core;
using UnityGLTF;


namespace Mig.Model.ModelSaver
{
    public class ModelSaveToWeb : IModelSaver
    {
        private ModelOperateState m_SaveState;
        private Action<bool> onSaveCompleteCallback;
        private string lastError = "";

        public string ErrorMsg()
        {
            return lastError;
        }

        public float GetPercentage()
        {
            return FTPClient.GetUpLoadPercentage();
        }

        public ModelOperateState GetState()
        {
            return m_SaveState;
        }

        public void OnDispose()
        {

        }

        public async void Save(string pathORAddress, GameObject modelParent, Action<bool> onSaveComplete)
        {
            onSaveCompleteCallback = onSaveComplete;
            lastError = "";
            m_SaveState = ModelOperateState.LOADING;

            if (modelParent == null)
            {
                lastError = "Model parent is null";
                m_SaveState = ModelOperateState.ERROR;
                onSaveCompleteCallback?.Invoke(false);
                return;
            }

            var settings = GLTFSettings.GetOrCreateSettings();
            var exportOptions = new ExportContext(settings);
            var exporter = new GLTFSceneExporter(modelParent.transform, exportOptions);

            using (Stream ftpStream = new MemoryStream())
            {
                exporter.SaveGLBToStream(ftpStream, "My new glTF scene");
                ftpStream.Position = 0;

                var getUploadPath = FTPClient.CombineUrl(
                    string.IsNullOrEmpty(pathORAddress) ? FTPClient.GetCurrentFTPDirRoot() : pathORAddress,
                    modelParent.name + ".glb");

                await FTPClient.UploadStream(getUploadPath, ftpStream, OnUploadCallback);
            }
        }

        public void Save(string pathORAddress, ISerializer serializer, Action<bool> onSaveComplete)
        {
            lastError = "Serializer save is not implemented for ModelSaveToWeb";
            onSaveComplete?.Invoke(false);
        }

        private void OnUploadCallback(bool result)
        {
            m_SaveState = result ? ModelOperateState.LOAD_COMPLETE : ModelOperateState.ERROR;
            if (!result)
            {
                lastError = "FTP upload failed";
            }
            Debug.Log(result ? "[Mig] Model uploaded" : "[Mig] Model upload failed");
            onSaveCompleteCallback?.Invoke(result);
        }
    }
}
