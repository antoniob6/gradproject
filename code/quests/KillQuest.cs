/*
 *a kill quest that inhertes from the quest super class
 * that monitors the players, and when a player kills another first, 
 * he/she wins
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuest : Quest {

    private int KillLimit = 0;
    public KillQuest(List<GameObject> _players, GameManager _GM) : base() {
        players = _players;
        GM = _GM;
        reward = Random.Range(50, 500);
        KillLimit = Random.Range(1, 20);
        questMessage = "kill " + KillLimit + " players, and get "+ reward+ " points";
        updateQuestMessage();





    }
    private void init() {

        foreach (GameObject p in players) {
            p.GetComponent<PlayerData>().resetRoundStats();
        }

    }

    bool initd = false;
    public override void tick() {
        base.tick();
        if (!initd) {
            init();
            initd = true;
        }
        if (isComplete)
            return;

        if (winners.Count==players.Count) {
            questCompleted();
        }



        foreach (GameObject p in players) {
            if (!p)
                continue;
            if (p.GetComponent<PlayerData>().killedPlayerCount>=KillLimit) {
                GameObject LHB = p.GetComponent<PlayerReceiveDamage>().lastHitby;
  
                if (LHB &&LHB.tag == "Player") {
                    if (winners.IndexOf(LHB) == -1) {
                        winners.Add(LHB);
                        questCompleted();
                        // GM.UpdatePlayerObjective(LHB, "good, now waiting for others");
                    }
                }



            }

        }
    }
    public override void DestroyQuest() {
  

    }
}
