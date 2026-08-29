using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityGLTF;
using System.Collections.Generic;

namespace Mig.Model.Utils
{
    public class ModelSaveUtils
    {
        public static async Task<bool> SaveModelAsGLBTo(GameObject modelParent, string saveDir)
        {
            if (modelParent == null || string.IsNullOrEmpty(saveDir))
            {
                return false;
            }

            List<Transform> children = new List<Transform>();

            foreach (Transform child in modelParent.transform)
            {
                children.Add(child);
            }

            if (children.Count == 0)
            {
                Debug.LogError("[Mig] No children to export as GLB");
                return false;
            }

            Directory.CreateDirectory(saveDir);
            var sceneName = "model";

            try
            {
                var settings = GLTFSettings.GetOrCreateSettings();
                var exportOptions = new ExportContext(settings);
                var exporter = new GLTFSceneExporter(children.ToArray(), exportOptions);
                exporter.SaveGLB(saveDir, sceneName);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Mig] Failed to export GLB: {ex.Message}");
                return false;
            }

            await Task.Yield();
            return File.Exists(Path.Combine(saveDir, sceneName + ".glb"));
        }
    }
}
