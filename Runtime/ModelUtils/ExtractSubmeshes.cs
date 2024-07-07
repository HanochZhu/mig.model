using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Mig.Model.Utils
{
    static class ArrayUtilities
    {
        // create a subset from a range of indices
        public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        // create a subset from a specific list of indices
        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            T[] subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }
            return subset;
        }
    }

    [RequireComponent(typeof(Renderer), typeof(MeshFilter))]
    public class ExtractSubmeshes
    {
        private static Mesh ExtractSubmesh(Mesh mesh, int submesh)
        {
            Mesh newMesh = new Mesh();
            SubMeshDescriptor descriptor = mesh.GetSubMesh(submesh);
            newMesh.vertices = ArrayUtilities.RangeSubset(mesh.vertices, descriptor.firstVertex, descriptor.vertexCount);

            if (mesh.tangents != null && mesh.tangents.Length == mesh.vertices.Length)
            {
                newMesh.tangents = ArrayUtilities.RangeSubset(mesh.tangents, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.boneWeights != null && mesh.boneWeights.Length == mesh.vertices.Length)
            {
                newMesh.boneWeights = ArrayUtilities.RangeSubset(mesh.boneWeights, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv != null && mesh.uv.Length == mesh.vertices.Length)
            {
                newMesh.uv = ArrayUtilities.RangeSubset(mesh.uv, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv2 != null && mesh.uv2.Length == mesh.vertices.Length)
            {
                newMesh.uv2 = ArrayUtilities.RangeSubset(mesh.uv2, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv3 != null && mesh.uv3.Length == mesh.vertices.Length)
            {
                newMesh.uv3 = ArrayUtilities.RangeSubset(mesh.uv3, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv4 != null && mesh.uv4.Length == mesh.vertices.Length)
            {
                newMesh.uv4 = ArrayUtilities.RangeSubset(mesh.uv4, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv5 != null && mesh.uv5.Length == mesh.vertices.Length)
            {
                newMesh.uv5 = ArrayUtilities.RangeSubset(mesh.uv5, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv6 != null && mesh.uv6.Length == mesh.vertices.Length)
            {
                newMesh.uv6 = ArrayUtilities.RangeSubset(mesh.uv6, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv7 != null && mesh.uv7.Length == mesh.vertices.Length)
            {
                newMesh.uv7 = ArrayUtilities.RangeSubset(mesh.uv7, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.uv8 != null && mesh.uv8.Length == mesh.vertices.Length)
            {
                newMesh.uv8 = ArrayUtilities.RangeSubset(mesh.uv8, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.colors != null && mesh.colors.Length == mesh.vertices.Length)
            {
                newMesh.colors = ArrayUtilities.RangeSubset(mesh.colors, descriptor.firstVertex, descriptor.vertexCount);
            }

            if (mesh.colors32 != null && mesh.colors32.Length == mesh.vertices.Length)
            {
                newMesh.colors32 = ArrayUtilities.RangeSubset(mesh.colors32, descriptor.firstVertex, descriptor.vertexCount);
            }

            var triangles = ArrayUtilities.RangeSubset(mesh.triangles, descriptor.indexStart, descriptor.indexCount);
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] -= descriptor.firstVertex;
            }

            newMesh.triangles = triangles;

            if (mesh.normals != null && mesh.normals.Length == mesh.vertices.Length)
            {
                newMesh.normals = ArrayUtilities.RangeSubset(mesh.normals, descriptor.firstVertex, descriptor.vertexCount);
            }
            else
            {
                newMesh.RecalculateNormals();
            }

            newMesh.Optimize();
            newMesh.OptimizeIndexBuffers();
            newMesh.RecalculateBounds();
            newMesh.name = mesh.name + $" Submesh {submesh}";
            return newMesh;
        }

        public static List<MeshFilter> Extract(MeshFilter meshFilter)
        {
            List<MeshFilter> result = new();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogWarning("No mesh exists on this gameObject");
                return result;
            }

            if (meshFilter.sharedMesh.subMeshCount <= 1)
            {
                Debug.LogWarning("Mesh has <= 1 submesh components. No additional extraction required.");
                return result;
            }

            for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
            {
                //string filePath = EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "", LastFilePath);
                //if (filePath == "") continue;

                //LastFilePath = Directory.GetDirectoryRoot(filePath);
                //Debug.Log(LastFilePath);

                Mesh mesh = ExtractSubmesh(meshFilter.sharedMesh, i);

                var childi = new GameObject(meshFilter.gameObject.name + "sub_" + i);
                childi.transform.SetParent(meshFilter.transform, false);
                childi.AddComponent<MeshFilter>().sharedMesh = mesh;
                childi.AddComponent<MeshRenderer>().material = meshFilter.GetComponent<MeshRenderer>().sharedMaterials[i];

                result.Add(childi.GetComponent<MeshFilter>());
                //AssetDatabase.CreateAsset(mesh, filePath);
            }
            return result;
        }
    }
}
