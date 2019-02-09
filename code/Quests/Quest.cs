/*
 * this is the quest super class, quests inherets from this class.
 * 
 * */ 
using System.Collections.Generic;
using UnityEngine;


public class Quest{
    public bool isComplete=false;
    public int reward;
    public string questMessage;
    public List<GameObject> winners = new List<GameObject>();
    public List<GameObject> losers = new List<GameObject>();
    protected List<GameObject> players;

    protected GameManager GM;

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


}
