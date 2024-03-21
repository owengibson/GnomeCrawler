using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    private void Start()
    {
        /*
        PlayerData playerData = new PlayerData();
        playerData.position = new Vector3(5, 0, 0);
        playerData.health = 80;

        string json = JsonUtility.ToJson(playerData);
        Debug.Log(json);

        File.WriteAllText(Application.dataPath + "saveFile.json", json);
        */

        string json = File.ReadAllText(Application.dataPath + "saveFile.json");

        PlayerData loadedPlayerData= JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("Position: " + loadedPlayerData.position);
        Debug.Log("Health: " + loadedPlayerData.health);
    }
}

public class PlayerData
{
    public Vector3 position;
    public int health;
}
