using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData Instance;
    public bool[] playersLoaded;
    public bool[] playerNotAI;
    public int[] charactersID;

    public static SaveData getInstance(){
        if(Instance == null){
            Instance = new SaveData();
            Instance.leaderboard = new LeaderBoard();
            Instance.lastEnteredSurname = "";
            Instance.playersLoaded = new bool[4];
            Instance.playerNotAI = new bool[4];
            Instance.charactersID = new int[4];
            for (int i = 0; i < 4; i++){
                Instance.playersLoaded[i] = false;
                Instance.playerNotAI[i] = false;
                Instance.charactersID[i] = -1;
            }
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
