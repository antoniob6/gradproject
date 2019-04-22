/*
 *a survive quest that inhertes from the quest super class
 * that monitors the players, and makes sure that when they die,
 * they don't win the quest
 * */



using System.Collections.Generic;
using UnityEngine;

public class SurviveQuest : Quest {

    private GameObject center;
    private float timeLimit;

    private Vector3 spawnPosition;
    private float spawnrange;
    private List<GameObject> enemies;

    private string originalQM;
    private int oldTime;

    public SurviveQuest(List<GameObject> _players, GameObject _center,
            int _reward, string _questMessage, GameManager _GM, float _threshold = 10, float _spawnrange = 5) {
        players = _players;
        center = _center;
        reward = _reward;
        questMessage = _questMessage+" for " + (int)_threshold+ " seconds";
        originalQM = _questMessage;
        GM = _GM;
        timeLimit = _threshold;
        oldTime = (int)timeLimit;
        spawnrange = _spawnrange;
        enemies = new List<GameObject>();
        init();

    }
    private void init() {
        updateQuestMessage();
        Random.InitState(System.DateTime.Now.Millisecond);

        //GameObject enemy = GM.networkSpawn("enemyPrefab", spawnPosition);
        // enemies.Add(enemy);


        foreach (GameObject p in players) {
            winners.Add(p);
            p.GetComponent<PlayerData>().RpchasDied(false);
        }

        GM.startSpawingEnemies();
    }

    public override void tick() {
        if (timeLimit > 0f) 
            timeLimit -= Time.deltaTime;
        if(oldTime!=(int)timeLimit) {
            oldTime = (int)timeLimit;
            questMessage=originalQM+ " for " + oldTime + " seconds";
            updateQuestMessage();
        }
     
        

        foreach (GameObject p in players) {
            if (!p)
                continue;
            
            PlayerData pdata = p.GetComponent<PlayerData>();
            if (pdata.hasDied) {
              //  Debug.Log("winner count " + winners.Count);
                winners.Remove(p);

            }


        }
        if (winners.Count == 0) {
            if (!isComplete)
                questCompleted();
            isComplete = true;


        }


    }
    public override void DestroyQuest() {
        GM.stopSpawingEnemies();


    }





}
