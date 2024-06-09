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

        /*void Start()
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

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = spawnPosition;
                sphere.transform.localScale = Vector3.one * 0.1f;
            }

            pbMesh.Refresh();
        }*/

        /*Vector3 GetRandomSpawnPosition(ProBuilderMesh mesh, Face face)
        {
            Vector3[] verti = ;

            Vector3[] vertices = pbMesh.positions.ToArray();
            Vector3 v0 = vertices[randomFace.indexesInternal[0]];
            Vector3 v1 = vertices[randomFace.indexesInternal[1]];
            Vector3 v2 = vertices[randomFace.indexesInternal[2]];

            return Vector3.zero;
        }*/


        Vector3 GetRandomPointOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float r1 = Random.Range(0.0f, 1.0f);
            float r2 = Random.Range(0.0f, 1.0f);
            return (1 - Mathf.Sqrt(r1)) * v0 + (Mathf.Sqrt(r1) * (1 - r2)) * v1 + (Mathf.Sqrt(r1) * r2) * v2;
        }
    }
}
