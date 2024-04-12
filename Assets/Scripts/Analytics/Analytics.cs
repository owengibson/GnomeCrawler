using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on CodeMonkey's tutorial's 'JSON Simple Saving and Loading with JSON to a File' & 'What is JSON?'
/// https://www.youtube.com/watch?v=6uMFEM-napE&t=336s & https://www.youtube.com/watch?v=4oRVMCRCvN0
/// </summary>

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
        roomName = "Start Room"; 
    }

    public void TrackButtonPress(string buttonName)
    {
        if (buttonHits.ContainsKey(buttonName))
            buttonHits[buttonName]++;
        else
            buttonHits.Add(buttonName, 1);

        // ========= FOR USE OF THIS METHOD =========
        /// 
        /// Add the following code underneath any action taken in a script with a KeyBind.
        /// 
        /// <CodeToUse>
        /// string buttonName = context.action.name;
        /// analyticsScript.TrackButtonPress(buttonName);
        /// <CodeToUse>
        /// 
        /// e.g.PlayerStateMachine.cs
        /// 
        ///     private void OnAttack(InputAction.CallbackContext context)
        ///     {
        ///         _isAttackPressed = context.ReadValueAsButton();
        ///         string buttonName = context.action.name;
        ///         analyticsScript.TrackButtonPress(buttonName);
        ///     }
        /// <END>
       
        SaveDataToJson();
    }

    public void TrackTriggerEntry(string triggerName)
    {
        if (triggerEntries.ContainsKey(triggerName))
            triggerEntries[triggerName]++;
        else
            triggerEntries.Add(triggerName, 1);

        // ========= FOR USE OF THIS METHOD =========
        /// 
        /// Add the following code underneath any script that involves the player entering a trigger
        /// You can set up triggers in different parts of the map to test if the player goes there
        /// 
        /// <CodeToUse>
        /// string triggerName = gameObject.name;
        /// analyticsScript.TrackTriggerEntry(triggerName);
        /// <CodeToUse>
        /// 
        /// e.g.WaterReset.cs
        /// 
        ///     private void OnTriggerEnter(Collider other)
        ///     {
        ///         if (other.TryGetComponent(out IDamageable component))
        ///         {
        ///             component.TakeDamage(1);
        ///             other.enabled = false;
        ///             other.transform.position = _resetTransform.position;
        ///             other.enabled = true;
        ///         }
        ///
        ///         string triggerName = gameObject.name;
        ///         analyticsScript.TrackTriggerEntry(triggerName);
        ///     }
        /// <END>

        SaveDataToJson();
    }

    private void SaveDataToJson()
    {
        string relativePath = "Analytics/GameData.json"; 
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

    public DataObject(Dictionary<string, int> buttonHits, Dictionary<string, int> triggerEntries, Dictionary<string, int> buttonDamageTaken, string roomName, float timeSpent)
    {
        this.buttonHits = buttonHits;
        this.triggerEntries = triggerEntries;
    }
}