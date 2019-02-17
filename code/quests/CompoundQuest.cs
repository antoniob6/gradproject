/*
 *a compound quest that inhertes from the quest super class
 * that combines multiple quest into one big quest
 * */
using System.Collections.Generic;
using UnityEngine;

public class CompoundQuest:Quest{

    private List<Quest> QL;
    private Quest qAll;//encapsulates all quests
    int[] connections;



    public CompoundQuest(List<Quest> questList)
    {
        reward = 0;
        QL = questList;
        foreach(Quest q in QL){
            q.linkedQuest = true;
            reward += q.reward;

        }
        GM = QL[0].GM;
        players = QL[0].getPlayers();



        connections = new int[questList.Count];
       for(int i=0;i<questList.Count;i++) {
            connections[i] = Random.Range(0, 2);
        }

        init();
    }
    private void init()
    {
        questMessage = QL[1].getMessage();
        qAll = QL[0];
        for (int i = 1; i < QL.Count; i++) {


                if (connections[i] == 0) {//and quest
                    questMessage += " AND ";
                    qAll = new ANDQuest(qAll, QL[i]);
                qAll.linkedQuest = true;
                } else { 
                    questMessage += " OR ";
                    qAll = new ORQuest(qAll, QL[i]);
                qAll.linkedQuest = true;
                }
            // questMessage += QL[i].getMessage();
            questMessage = qAll.getMessage();
        }
        updateQuestMessage();
        
    }
    public override void tick() {
        if (isComplete)
            return;
        qAll.tick();
        


        if (qAll.isComplete) {
            Debug.Log("compound quests completed");

            winners = qAll.winners;
            losers = qAll.losers;
            questCompleted();

        }
    }

    public override void DestroyQuest() {
        qAll.DestroyQuest();
       // Debug.Log("destroying the object");

    }


}
