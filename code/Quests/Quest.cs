/*
 * this is the quest super class, quests inherets from this class.
 * 
 * */ 
using System.Collections.Generic;
using UnityEngine;


public class Quest{
    public bool isComplete=false;
    public int reward=0;
    public string questMessage;
    public List<GameObject> winners = new List<GameObject>();
    public List<GameObject> losers = new List<GameObject>();
    public bool linkedQuest=false;
    protected List<GameObject> players;

    public GameManager GM;

    public virtual void tick() { }
    public virtual void DestroyQuest() { }
    public void updateQuestMessage() {
        foreach( GameObject p in players) {
            PlayerData pd = p.GetComponent<PlayerData>();
            if(pd!=null)
                pd.RpcUpdateText(questMessage);
        }
    }
    public virtual void EndQuestNow() {
        isComplete = true;
        if(!linkedQuest)
            GM.questCompleted(this);

    }
    public virtual void RewardPlayers() {
        foreach (GameObject p in winners) {
            if (!p)
                return;
            PlayerData pd = p.GetComponent<PlayerData>();
            if (pd != null)
                pd.RpcAddScore(reward);
        }

    }
    public virtual void questCompleted() {
        if (!isComplete&&!linkedQuest)
            GM.questCompleted(this);
        isComplete = true;

    }
    public List<GameObject> getPlayers() {
        return players;
    }
    public string getMessage() {
        return questMessage;
    }


}
