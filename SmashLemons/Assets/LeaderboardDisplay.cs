using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardDisplay : MonoBehaviour
{
    public GameObject playerEntryPrefab;

    private void Start() {
        if(SaveData.getInstance().lastEnteredSurname != ""){
            string surname = SaveData.getInstance().lastEnteredSurname;
            long points = SaveData.getInstance().lastScore;
            SaveData.getInstance().OnLoad();
            SaveData.getInstance().leaderboard.AddScore(points, surname);
            SaveData.getInstance().lastEnteredSurname = "";
            SaveData.getInstance().lastScore = 0;
            SerializationManager.Save("Save", SaveData.getInstance());
        }
        OnLoad();
    }
    public void SetUp(){
        Transform panel = GameObject.Find("Leaderboard").transform;
        foreach (Transform child in panel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 10; i++){
            GameObject entry = Instantiate(playerEntryPrefab, panel);
            entry.name = "Top" + (i+1);
            entry.GetComponent<TextMeshProUGUI>().text = SaveData.getInstance().leaderboard.getScore(i).name + " : " + SaveData.getInstance().leaderboard.getScore(i).score;
        }
           
    }

    public void addScoreTest(){
        SaveData.getInstance().leaderboard.AddScore(100, "CAM");
        SetUp();
    }

    public void OnLoad(){
        SaveData.getInstance().OnLoad();
        SetUp();
    }

    public void GoToMenu(){
        SceneManager.LoadScene(0);
    }
}
