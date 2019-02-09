/*
 *a kill quest that inhertes from the quest super class
 * that monitors the players, and when a player kills another first, 
 * he/she wins
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuest : Quest {





    public KillQuest(List<GameObject> _players,
            int _reward, string _questMessage, GameManager _GM) {
        players = _players;

        reward = _reward;
        questMessage = _questMessage;
        GM = _GM;
        init();
    }
    private void init() {
        updateQuestMessage();
        Random.InitState(System.DateTime.Now.Millisecond);
        foreach (GameObject p in players) {
           
            p.GetComponent<PlayerData>().RpchasDied(false);
        }

    }
    public override void tick() {
        if (isComplete)
            return;

        if (winners.Count==players.Count) {
            isComplete = true;
            GM.questCompleted(this);
        }



        foreach (GameObject p in players) {
            if (!p)
                continue;
            if (p.GetComponent<PlayerData>().hasDied) {
                GameObject LHB = p.GetComponent<ReceiveDamage>().lastHitby;
  
                if (LHB.tag == "Player") {
                    if (winners.IndexOf(LHB) == -1) {
                        winners.Add(LHB);
                        isComplete = true;
                        GM.questCompleted(this);
                        // GM.UpdatePlayerObjective(LHB, "good, now waiting for others");
                    }
                }



            }

        }
    }
    public override void DestroyQuest() {
  

    }
}
