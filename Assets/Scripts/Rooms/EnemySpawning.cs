using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GnomeCrawler.Rooms
{
    public class EnemySpawning : MonoBehaviour
    {
        [SerializeField] private List<ProBuilderMesh> pbMeshes; 
        [SerializeField] private float numberOfEnemiesToSpawn;
        [SerializeField] private GameObject enemyToSpawn;

        void Start()
        {
            if (pbMeshes == null)
            {
                Debug.LogError("No mesh on the spawner");
                return;
            }

            List<(ProBuilderMesh mesh, Face face, float cumulativeArea)> meshAreas = CalculateMeshAreas();

            float totalArea = meshAreas[meshAreas.Count - 1].cumulativeArea;

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                float randomValue = Random.Range(0f, totalArea);
                (ProBuilderMesh selectedMesh, Face selectedFace) = SelectMeshAndFace(meshAreas, randomValue);

                Vector3 spawnPosition = GetRandomSpawnPosition(selectedMesh, selectedFace);
                Instantiate(enemyToSpawn, selectedMesh.transform.position + spawnPosition, Quaternion.identity);
            }
        }

        List<(ProBuilderMesh mesh, Face face, float cumulativeArea)> CalculateMeshAreas()
        {
            List<(ProBuilderMesh, Face, float)> meshAreas = new List<(ProBuilderMesh, Face, float)>();
            float cumulativeArea = 0f;

            foreach (ProBuilderMesh pbMesh in pbMeshes)
            {
                if (pbMesh == null)
                {
                    Debug.LogWarning("One of the meshes is null, skipping.");
                    continue;
                }

                foreach (Face face in pbMesh.faces)
                {
                    if (face.indexes.Count < 3)
                    {
                        Debug.LogWarning("Invalid face for spawning on mesh " + pbMesh.name + ". The face does not have enough vertices.");
                        continue;
                    }

                    Vector3[] vertices = pbMesh.positions.ToArray();
                    int[] indices = face.indexes.ToArray();

                    int triangleCount = indices.Length / 3;

                    for (int i = 0; i < triangleCount; i++)
                    {
                        Vector3 p0 = vertices[indices[i * 3]];
                        Vector3 p1 = vertices[indices[i * 3 + 1]];
                        Vector3 p2 = vertices[indices[i * 3 + 2]];

                        float area = Vector3.Cross(p1 - p0, p2 - p0).magnitude / 2f;
                        cumulativeArea += area;
                    }

                    meshAreas.Add((pbMesh, face, cumulativeArea));
                }
            }

            return meshAreas;
        }

        (ProBuilderMesh, Face) SelectMeshAndFace(List<(ProBuilderMesh mesh, Face face, float cumulativeArea)> meshAreas, float randomValue)
        {
            foreach (var meshArea in meshAreas)
            {
                if (randomValue <= meshArea.cumulativeArea)
                {
                    return (meshArea.mesh, meshArea.face);
                }
            }

            return (meshAreas[meshAreas.Count - 1].mesh, meshAreas[meshAreas.Count - 1].face);
        }

        Vector3 GetRandomSpawnPosition(ProBuilderMesh mesh, Face face)
        {
            Vector3[] vertices = mesh.positions.ToArray();
            int[] indices = face.indexes.ToArray();

            int triangleCount = indices.Length / 3;
            float[] triangleAreas = new float[triangleCount];
            float totalArea = 0f;

            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 p0 = vertices[indices[i * 3]];
                Vector3 p1 = vertices[indices[i * 3 + 1]];
                Vector3 p2 = vertices[indices[i * 3 + 2]];

                float area = Vector3.Cross(p1 - p0, p2 - p0).magnitude / 2f;
                triangleAreas[i] = area;
                totalArea += area;
            }

            float randomValue = Random.Range(0f, totalArea);
            int selectedTriangleIndex = GetTriangleIndexByArea(randomValue, triangleAreas);

            Vector3 pos0 = vertices[indices[selectedTriangleIndex * 3]];
            Vector3 pos1 = vertices[indices[selectedTriangleIndex * 3 + 1]];
            Vector3 pos2 = vertices[indices[selectedTriangleIndex * 3 + 2]];

            return GetRandomPointOnTriangle(pos0, pos1, pos2);
        }

        int GetTriangleIndexByArea(float randomValue, float[] triangleAreas)
        {
            float cumulativeArea = 0f;
            for (int i = 0; i < triangleAreas.Length; i++)
            {
                cumulativeArea += triangleAreas[i];
                if (randomValue <= cumulativeArea)
                {
                    return i;
                }
            }
            return triangleAreas.Length - 1;
        }

        Vector3 GetRandomPointOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float r1 = Random.Range(0.0f, 1.0f);
            float r2 = Random.Range(0.0f, 1.0f);
            return (1 - Mathf.Sqrt(r1)) * v0 + (Mathf.Sqrt(r1) * (1 - r2)) * v1 + (Mathf.Sqrt(r1) * r2) * v2;
        }
    }
}
