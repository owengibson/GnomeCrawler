using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

namespace GnomeCrawler.Rooms
{
    public class EnemySpawning : MonoBehaviour
    {
        [SerializeField] private List<ProBuilderMesh> pbMeshes; 
        [SerializeField] private float numberOfEnemiesToSpawn;
        [SerializeField] private List<GameObject> enemiesToSpawn;

        private List<Vector3[]> debugTriangles = new List<Vector3[]>();
        private List<Vector3> debugSpawnPoints = new List<Vector3>();

        private RoomManager _room;

        void Start()
        { 
            //Invoke("SpawnEnemies", 10);
        }

        public void SpawnEnemies(int hashCode)
        {
            if (hashCode != transform.parent.GetHashCode()) return;

            if (pbMeshes == null || pbMeshes.Count == 0)
            {
                Debug.LogError("No mesh on the spawner");
                return;
            }

            List<(ProBuilderMesh mesh, Face face, float cumulativeArea)> meshAreas = CalculateMeshAreas();

            if (meshAreas.Count == 0)
            {
                Debug.LogError("No valid faces in meshes");
                return;
            }

            float totalArea = meshAreas[meshAreas.Count - 1].cumulativeArea;

            if (transform.parent.TryGetComponent<RoomManager>(out _room))
            {
                for (int i = 0; i < numberOfEnemiesToSpawn; i++)
                {
                    float randomValue = Random.Range(0f, totalArea);
                    (ProBuilderMesh selectedMesh, Face selectedFace) = SelectMeshAndFace(meshAreas, randomValue);

                    int randomEnemyChoice = Random.Range(0, enemiesToSpawn.Count);
                    Vector3 spawnPosition = GetRandomSpawnPosition(selectedMesh, selectedFace);

                    GameObject newEnemy = Instantiate(enemiesToSpawn[randomEnemyChoice]);
                    NavMeshAgent newAgent = newEnemy.GetComponent<NavMeshAgent>();

                    Vector3 worldSpawnPosition = selectedMesh.transform.TransformPoint(spawnPosition);

                    NavMeshHit closestHit;
                    if (NavMesh.SamplePosition(worldSpawnPosition, out closestHit, 1, NavMesh.AllAreas))
                    {
                        newEnemy.transform.position = closestHit.position;
                        newEnemy.transform.parent = transform;
                        newAgent.enabled = true;
                        debugSpawnPoints.Add(closestHit.position);

                        _room.AddEnemyToList(newEnemy);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to sample NavMesh position near {worldSpawnPosition}");
                    }
                }
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

                        debugTriangles.Add(new Vector3[] { pbMesh.transform.position + p0, pbMesh.transform.position + p1, pbMesh.transform.position + p2 });
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
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (Vector3[] triangle in debugTriangles)
            {
                Gizmos.DrawLine(triangle[0], triangle[1]);
                Gizmos.DrawLine(triangle[1], triangle[2]);
                Gizmos.DrawLine(triangle[2], triangle[0]);
            }

            Gizmos.color = Color.red;

            /*foreach (Vector3 point in debugSpawnPoints)
            {
                Gizmos.DrawSphere(point, 5f);
            }*/
        }

        private void OnEnable()
        {
            EventManager.OnRoomStarted += SpawnEnemies;
        }
        private void OnDisable()
        {
            EventManager.OnRoomStarted -= SpawnEnemies;
        }
    }

}
