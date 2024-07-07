using GLTFast.Export;
using GLTFast.Logging;
using GLTFast;
using System;
using UnityEngine;
using System.IO;
using Mig.Core;


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
            // CollectingLogger lets you programmatically go through
            // errors and warnings the export raised
            var logger = new CollectingLogger();

            // ExportSettings and GameObjectExportSettings allow you to configure the export
            // Check their respective source for details

            // ExportSettings provides generic export settings
            var exportSettings = new ExportSettings
            {
                Format = GltfFormat.Binary,
                FileConflictResolution = FileConflictResolution.Overwrite,
                // Export everything except cameras or animation
                ComponentMask = ~(ComponentType.Camera | ComponentType.Animation),
                // Boost light intensities
                LightIntensityFactor = 100f,
            };

            // GameObjectExportSettings provides settings specific to a GameObject/Component based hierarchy
            var gameObjectExportSettings = new GameObjectExportSettings
            {
                // Include inactive GameObjects in export
                OnlyActiveInHierarchy = false,
                // Also export disabled components
                DisabledComponents = true,
                // Only export GameObjects on certain layers
                LayerMask = LayerMask.GetMask("Default", "MyCustomLayer"),
            };

            // GameObjectExport lets you create glTFs from GameObject hierarchies
            var export = new GameObjectExport(exportSettings, gameObjectExportSettings, logger: logger);

            // Add a scene
            export.AddScene(new GameObject[] { modelParent }, "My new glTF scene");

            using (Stream ftpStream = new MemoryStream())
            {
                // Async glTF export
                var success = await export.SaveToStreamAndDispose(ftpStream);

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
