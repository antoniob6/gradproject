/*
 *a collect quest that inhertes from the quest super class
 * that monitors the players, and when the first one get close to the goal, he/she wins 
 * */
using System.Collections.Generic;
using UnityEngine;

public class CollectQuest : Quest
{

    private GameObject center;

    private float threshold;

    private Vector3 spawnPosition;
    private float spawnrange;
    private List<GameObject> candies;

    

    public CollectQuest(List<GameObject> _players, GameObject _center,
            int _reward, string _questMessage, GameManager _GM, float _threshold = 10, float _spawnrange = 5) {
        players = _players;
        center = _center;
        reward = _reward;
        questMessage = _questMessage;
        GM = _GM;
        threshold = _threshold;
        spawnrange = _spawnrange;
        init();
    }

    private int collectLimit=0;
    
    private void init() {
        candies = new List<GameObject>();

        questMessage = "collect " + collectLimit + " pieces of red candy";
        updateQuestMessage();
        Random.InitState(System.DateTime.Now.Millisecond);
        collectLimit = Random.Range(3, 10);
        
        for (int i = 0; i < collectLimit * 2; i++) {
            spawnPosition = new Vector2(players[0].transform.position.x+Random.Range(-spawnrange, spawnrange), players[0].transform.position.y + 10);
            candies.Add(GM.networkSpawn("candyPrefab", spawnPosition));

        }
    }
    public override void tick() {
        if (isComplete)
            return;

        foreach (GameObject p in players) {
            if (!p)
                continue;

            foreach (GameObject c in candies) {
                if (Vector3.Distance(c.transform.position, p.transform.position) < threshold) {
                    Debug.Log("player has found a piece of candy");
                    candies.Remove(c);
                    GM.networkDestroy(c);
                    collectLimit--;

                    if(collectLimit==0)
                        questCompleted();

                }
            }

        }
    }
    public override void DestroyQuest() {
        // Debug.Log("destroying the object");
        foreach (GameObject c in candies) {
            GM.networkDestroy(c);
        }
    }


}
