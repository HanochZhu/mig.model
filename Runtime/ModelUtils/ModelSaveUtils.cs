using UnityEngine;
using System.Threading.Tasks;
using UnityGLTF;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace Mig.Model.Utils
{ 
    public class ModelSaveUtils
    {
        public static async Task<bool> SaveModelAsGLBTo(GameObject modelParent, string saveDir)
        {

            List<Transform> children = new List<Transform>();

            foreach(Transform child in modelParent.transform)
            {
                children.Add(child);
            }

            var resultFile = System.IO.Path.Combine(saveDir, modelParent.name + ".glb");
            var sceneName = "model";

            var settings = GLTFSettings.GetOrCreateSettings();
            var exportOptions = new ExportContext(settings);
            var exporter = new GLTFSceneExporter(children.ToArray(), exportOptions);

            settings.SaveFolderPath = saveDir;
            exporter.SaveGLB(saveDir, sceneName);

            return true;
        }
    }

}
