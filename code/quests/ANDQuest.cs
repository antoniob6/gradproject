﻿/*
 *a AND quest that inhertes from the quest super class
 * that creates an AND logical connection between two quests
 * */
using System.Collections.Generic;
using UnityEngine;

public class ANDQuest:Quest{

    private Quest quest1;

    private Quest quest2;


    public ANDQuest(Quest q1,Quest q2)
    {
        quest1 = q1;
        quest2 = q2;
        q1.linkedQuest = true;
        q2.linkedQuest = true;

        GM = q1.GM;
        reward = q1.reward + q2.reward;


        init();
    }
    private void init()
    {
        players = quest1.getPlayers();
        //players.AddRange(quest2.getPlayers());
        questMessage = "("+quest1.getMessage() + " AND " + quest2.getMessage()+")";
        updateQuestMessage();
        Random.InitState(System.DateTime.Now.Millisecond);
    }
    public override void tick() {
        if (isComplete)
            return;
        quest1.tick();
        quest2.tick();
        if (quest1.isComplete) { 
            if (questMessage != quest2.getMessage()) {
                questMessage = quest2.getMessage();
              //  updateQuestMessage();
             }
        } else if (quest2.isComplete) {
            if (questMessage != quest1.getMessage()) {
                questMessage = quest1.getMessage();
              //  updateQuestMessage();
            }
        }



        if (quest1.isComplete && quest2.isComplete) {
            Debug.Log("both quests completed");

            winners = quest1.winners;
            winners.AddRange(quest2.winners);
            questCompleted();

        }
    }

    public override void DestroyQuest() {
        quest1.DestroyQuest();
        quest2.DestroyQuest();
       // Debug.Log("destroying the object");

    }


}