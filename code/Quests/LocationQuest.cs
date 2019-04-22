/*
 *a location quest that inhertes from the quest super class
 * that monitors the players, and when the first one get close to the goal, he/she wins 
 * */
using System.Collections.Generic;
using UnityEngine;

public class LocationQuest:Quest{

    private GameObject center;

    private float threshold;

    private Vector3 spawnPosition;
    private float spawnrange;
    private GameObject foundable;



    public LocationQuest(List<GameObject> _players, GameObject _center, 
            int _reward, string _questMessage, GameManager _GM, float _threshold=10,float _spawnrange = 5)
    {
        players = _players;
        center = _center;
        reward = _reward;
        questMessage = _questMessage;
        GM = _GM;
        threshold = _threshold;
        spawnrange = _spawnrange;
        init();
    }
    private void init()
    {
        updateQuestMessage();
        Random.InitState(System.DateTime.Now.Millisecond);
        spawnPosition = new Vector2(Random.Range(-spawnrange, spawnrange), GM.transform.position.y+10);
        foundable=GM.networkSpawn("locationPrefab",spawnPosition);
    }
    public override void tick() {
        if (isComplete)
            return;


        if (!foundable) {
            if(!isComplete)
               questCompleted();
            isComplete = true;
            
            
        }

        foreach (GameObject p in players)
        {
            if (!p||!foundable)
                continue;
            if (Vector3.Distance(foundable.transform.position, p.transform.position) < threshold)
            {
                Debug.Log("player has found the foundable goal");
                winners.Add(p);

                  questCompleted();

            }
            
        }
    }
    public override void DestroyQuest() {
       // Debug.Log("destroying the object");
        GM.networkDestroy(foundable);

    }


}
