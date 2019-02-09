using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationQuest:Quest{

    [SerializeField]
    private GameObject collectiblePrefab;
    [SerializeField]
    private float spawnrange = 40f;
    [SerializeField]
    private float spawnInterval = 1.0f;


    public LocationQuest(string questNamee, GameObject[] _questOwners, string _reward)
    {
        
        questName = questNamee;
        questOwners = _questOwners;
        reward = _reward;
    }

}
