using Mig.Model.Utils;
using UnityEngine;

namespace Mig.Model.ModelLoader
{
    public class ModelPostProcess
    {
        public static void ProcessModel(GameObject root)
        {
            var allComponents = root.GetComponentsInChildren<Renderer>();

            foreach (var component in allComponents)
            {
                Debug.Log($"[Mig] ProcessModel add mesh collider at {component.gameObject.name}");

                if (component is SkinnedMeshRenderer smr)
                {
                    if (smr.sharedMesh.subMeshCount != 1)
                    {
                        Debug.LogWarning($"[Mig] {component.gameObject.name} submesh count is not equals to 1");
                        continue;
                    }
                    var mc = component.gameObject.GetOrAddComponent<MeshCollider>();

                    mc.sharedMesh = smr.sharedMesh;
                }
                else if (component is MeshRenderer)
                {
                    var mesh = component.GetComponent<MeshFilter>();

                    if (mesh.sharedMesh.subMeshCount > 1)
                    {
                        var childMeshes = ExtractSubmeshes.Extract(mesh);

                        if (childMeshes.Count > 0)
                        {
                            //mesh.GetComponent<MeshRenderer>().enabled = false;
                            Object.DestroyImmediate(mesh.GetComponent<MeshRenderer>());
                            Object.DestroyImmediate(mesh);
                        }

                        foreach (MeshFilter child in childMeshes)
                        {
                            var mc = child.gameObject.GetOrAddComponent<MeshCollider>();

                            mc.sharedMesh = child.sharedMesh;
                        }
                    }
                    else
                    {
                        if (mesh.sharedMesh.subMeshCount != 1)
                        {
                            Debug.LogWarning($"[Mig] {component.gameObject.name} submesh count is not equals to 1");
                            continue;
                        }
                        var mc = component.gameObject.GetOrAddComponent<MeshCollider>();

                        mc.sharedMesh = mesh.sharedMesh;
                    }

                }
            }
        }
    }
}
