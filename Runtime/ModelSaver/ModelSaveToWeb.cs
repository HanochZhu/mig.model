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
        public string ErrorMsg()
        {
            throw new NotImplementedException();
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
            var settings = GLTFSettings.GetOrCreateSettings();
            var exportOptions = new ExportContext(settings);
            var exporter = new GLTFSceneExporter(modelParent.transform, exportOptions);

            var invokedByShortcut = Event.current?.type == EventType.KeyDown;

            using (Stream ftpStream = new MemoryStream())
            {
                // Async glTF export
                exporter.SaveGLBToStream(ftpStream, "My new glTF scene");

                var getUploadPath = Path.Combine(FTPClient.GetCurrentFTPDirRoot(), modelParent.name + ".glb");

                await FTPClient.UploadStream(getUploadPath, ftpStream, OnUploadCallback);
            }
        }

        public void Save(string pathORAddress, ISerializer serializer, Action<bool> onSaveComplete)
        {
            throw new NotImplementedException();
        }

        private void OnUploadCallback(bool result)
        {
            Debug.Log("Save Sucsess");
        }
    }
}
