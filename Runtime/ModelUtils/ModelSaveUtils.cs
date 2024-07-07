using GLTFast.Export;
using GLTFast.Logging;
using GLTFast;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

namespace Mig.Model.Utils
{ 
    public class ModelSaveUtils
    {
        public static async Task<bool> SaveModelAsGLBTo(GameObject modelParent, string saveDir)
        {
            var logger = new CollectingLogger();

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

            List<GameObject> scenes = new();
            foreach (Transform child in modelParent.transform)
            {
                scenes.Add(child.gameObject);
            }
            // Add a scene
            export.AddScene(scenes.ToArray(), "My new glTF scene");

            // Async glTF export
            var success = await export.SaveToFileAndDispose(Path.Combine(saveDir, modelParent.name + ".glb"));

            if (!success)
            {
                logger.LogAll();
            }

            return success;
        }
    }

}
