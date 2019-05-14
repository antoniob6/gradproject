/*
 *a location quest that inhertes from the quest super class
 * that monitors the players, and when the first one get close to the goal, he/she wins 
 * */
using System.Collections.Generic;
using UnityEngine;

public class BossQuest:Quest{

    private GameObject center;

    private float threshold;

    private Vector3 spawnPosition;
    private float spawnrange;
    private GameObject bossGO;



    public BossQuest(List<GameObject> _players, GameObject _center, 
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
        bossGO=GM.networkSpawn("bossPrefab",spawnPosition);
    }
    public override void tick() {
        if (isComplete)
            return;

        if (!bossGO) {
            if(!isComplete)
               questCompleted();
            isComplete = true;     
        }


    }
    public override void DestroyQuest() {
       // Debug.Log("destroying the object");
        GM.networkDestroy(bossGO);

    }


}
