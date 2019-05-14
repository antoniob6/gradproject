/*
 * this is the quest super class, quests inherets from this class.
 * 
 * */ 
using System.Collections.Generic;
using UnityEngine;


public class Quest{
    public bool isComplete=false;
    public int reward=100;
    public string questMessage;
    public List<GameObject> winners = new List<GameObject>();
    public bool linkedQuest=false;
    public List<GameObject> players;

    public float timeLeft = 10f;

    

    public GameManager GM;

    public Quest() {
        setTimeLimit();
    }
    private int OldTime;
    public virtual void tick() {
        if (timeLeft <= 0f) {
            questCompleted();
        } else {
            timeLeft -= Time.deltaTime;
            if (OldTime != (int)timeLeft) {
                updateQuestMessage();
                OldTime = (int)timeLeft;
            }
        }

    }
    public virtual void DestroyQuest() { }

    public void updateQuestMessage() {
        string newQuestMessage = questMessage;
        newQuestMessage += " (timeleft: " + (int)timeLeft + ")";
        
        foreach( GameObject p in players) {
            PlayerData pd = p.GetComponent<PlayerData>();
            if(pd!=null)
                pd.RpcUpdateText(newQuestMessage);
        }
    }
    public virtual void RewardPlayers() {
        foreach (GameObject p in winners) {
            PlayerData pd = p.GetComponent<PlayerData>();
            if (pd != null) {
                pd.RpcAddScore(reward);
                
            }
        }

    }
    public virtual void questCompleted() {
        if (!isComplete&&!linkedQuest)
            GM.questCompleted(this);
        isComplete = true;

        foreach (GameObject p in players) {
            if (!p)
                return;
            bool playerWon = false;
            foreach (GameObject w in winners) {
                if (w == p)
                    playerWon = true;  
            }
            if(playerWon)
                p.GetComponent<PlayerData>().RpcUpdateText("you have won this round");
            else
                p.GetComponent<PlayerData>().RpcUpdateText("you have lost this round");
        }

        RewardPlayers();
        DestroyQuest();
    }
    public List<GameObject> getPlayers() {
        return players;
    }
    public string getMessage() {
        return questMessage;
    }

    public void setTimeLimit() {//set high propability for medium time
        float rand = Random.Range(0f, 1f);
        float selectedTime = 10f;
        if (rand < 0.2f) {
            selectedTime = Random.Range(10f, 20f);
        }else if (rand < 0.8f) {
            selectedTime = Random.Range(30f, 60f);
        } else {
            selectedTime = Random.Range(60f, 120f);
        }

        timeLeft = selectedTime;
    }


}
