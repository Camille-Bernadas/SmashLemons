using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderBoard
{
    Score[] scores;


    public LeaderBoard(){
        scores = new Score[10];
        for (int i = 0; i < 10; i++){
            scores[i] = new Score();
        }
    }

    public LeaderBoard(Score[] lb) {
        scores = lb;
    }

    public void AddScore(long score, string name){
        int replace = 10;
        for (int i = 9; i >= 0; i--){
            if(score > scores[i].score){
                replace = i;
            }
        }
        for (int i = 9; i > replace; i--){
            scores[i] = scores[i - 1];
        }
        if(replace != 10){
            scores[replace] = new Score(score, name);
        }
        
    }
    public bool isHigh(long score){
        if (score > scores[9].score) {
            return true;
        }
        return false;
    }

    public Score getScore(int i){
        return scores[i];
    }

}

[System.Serializable]
public class Score{
    public long score;
    public string name;

    public Score(){
        score = 0;
        name = "AAA";
    }
    public Score (long points, string user){
        score = points;
        name = user;
    }
}
