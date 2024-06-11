using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GnomeCrawler.Rooms
{
    public class EnemySpawning : MonoBehaviour
    {
        [SerializeField] private ProBuilderMesh pbMesh; 
        [SerializeField] private float numberOfEnemiesToSpawn;
        [SerializeField] private GameObject enemyToSpawn;

        void Start()
        {
            if (pbMesh == null)
            {
                Debug.LogError("No mesh on the spawner");
                return;
            }

            Face face = pbMesh.faces[0];
            pbMesh.SetFaceColor(face, Color.green);

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition(pbMesh, face);

                /*GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = pbMesh.transform.position + spawnPosition;
                sphere.transform.localScale = Vector3.one * 3f;*/

                GameObject enemy = Instantiate(enemyToSpawn, pbMesh.transform.position + spawnPosition, Quaternion.identity);
            }

            pbMesh.Refresh();
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
            float cumulativeArea = 0f;
            int selectedTriangleIndex = 0;

            for (int i = 0; i < triangleCount; i++)
            {
                cumulativeArea += triangleAreas[i];
                if (randomValue <= cumulativeArea)
                {
                    selectedTriangleIndex = i;
                    break;
                }
            }

            Vector3 pos0 = vertices[indices[selectedTriangleIndex * 3]];
            Vector3 pos1 = vertices[indices[selectedTriangleIndex * 3 + 1]];
            Vector3 pos2 = vertices[indices[selectedTriangleIndex * 3 + 2]];

            return GetRandomPointOnTriangle(pos0, pos1, pos2);
        }


        Vector3 GetRandomPointOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float r1 = Random.Range(0.0f, 1.0f);
            float r2 = Random.Range(0.0f, 1.0f);
            return (1 - Mathf.Sqrt(r1)) * v0 + (Mathf.Sqrt(r1) * (1 - r2)) * v1 + (Mathf.Sqrt(r1) * r2) * v2;
        }
    }
}
