using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices.ComTypes;
using Unity.IO.LowLevel.Unsafe;

public class Analytics : MonoBehaviour
{
    private Dictionary<string, int> buttonHits = new Dictionary<string, int>();
    private Dictionary<string, int> triggerEntries = new Dictionary<string, int>();
    private Dictionary<string, int> buttonDamageTaken = new Dictionary<string, int>();
    private float startTime;
    private string roomName;

    void Start()
    {
        startTime = Time.time;
        roomName = "StartingRoom"; 
    }

    public void TrackButtonPress(string buttonName)
    {

        // ========= FOR USE OF THIS METHOD =========
        /// 
        /// Add the following code underneath any action taken in a script with a KeyBind.
        /// 
        /// string buttonName = context.action.name;
        /// analyticsScript.TrackButtonPress(buttonName);
        /// 
        /// e.g.
        /// 
        ///     private void OnAttack(InputAction.CallbackContext context)
        ///     {
        ///         _isAttackPressed = context.ReadValueAsButton();
        ///         string buttonName = context.action.name;
        ///         analyticsScript.TrackButtonPress(buttonName);
        ///     }
        /// 

        if (buttonHits.ContainsKey(buttonName))
            buttonHits[buttonName]++;
        else
            buttonHits.Add(buttonName, 1);

        SaveDataToJson();
    }

    public void TrackTriggerEntry(string triggerName)
    {
        // ========= FOR USE OF THIS METHOD =========
        /// 
        /// Add the following code underneath any action taken in a script with a KeyBind.
        /// 
        /// string triggerName = gameObject.name;
        /// analyticsScript.TrackTriggerEntry(triggerName);
        /// 
        /// e.g.
        /// 
        ///

        if (triggerEntries.ContainsKey(triggerName))
            triggerEntries[triggerName]++;
        else
            triggerEntries.Add(triggerName, 1);

        SaveDataToJson();
    }

    public void TrackTimeInRoomAfterEnemiesKilled(string roomName)
    {
        this.roomName = roomName;
        SaveDataToJson();
    }

    private void SaveDataToJson()
    {
        string relativePath = "Analytics/GameData.json"; // chatGPT assisted with the file saving location - I could not for the fuckin life of me figure it out
        string filePath = Application.dataPath + "/" + relativePath;
        string jsonData = JsonUtility.ToJson(new DataObject(buttonHits, triggerEntries, buttonDamageTaken, roomName, Time.time - startTime));
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved to: " + filePath);
    }
}

public class DataObject
{
    public Dictionary<string, int> buttonHits;
    public Dictionary<string, int> triggerEntries;
    public Dictionary<string, int> buttonDamageTaken;
    public string roomName;
    public float timeSpent;

    public DataObject(Dictionary<string, int> buttonHits, Dictionary<string, int> triggerEntries, Dictionary<string, int> buttonDamageTaken, string roomName, float timeSpent)
    {
        this.buttonHits = buttonHits;
        this.triggerEntries = triggerEntries;
        this.buttonDamageTaken = buttonDamageTaken;
        this.roomName = roomName;
        this.timeSpent = timeSpent;
    }
}