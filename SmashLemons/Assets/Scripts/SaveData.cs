using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData Instance;
    public bool[] playersLoaded = new bool[4];
    public bool[] playerNotAI = new bool[4];
    public int[] charactersID = new int[4];

    public static SaveData getInstance(){
        if(Instance == null){
            Instance = new SaveData();
            Instance.leaderboard = new LeaderBoard();
            Instance.lastEnteredSurname = "";
        }
        return Instance;
    }
    
    public LeaderBoard leaderboard;
    public string lastEnteredSurname;
    public long lastScore;

    public void OnLoad(){
        Debug.Log("LOADING...");
        SaveData.Instance = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/save.save");
    }

}
