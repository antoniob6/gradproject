using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayScores : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isActiveAndEnabled)
            return;
        string text = "game has ended, here are the scores \n";
        PlayerConnectionObject[] PCOs = FindObjectsOfType<PlayerConnectionObject>();
        float[] scores = new float[PCOs.Length];
        int namesIndex = 0;
        foreach (PlayerConnectionObject PCO in PCOs) {//find each player scores

            if (!PCO) {
                Debug.Log("null PCO");
                return;
            }
            PlayerData PD = PCO.GetComponent<PlayerData>();
            if (!PD) {
                Debug.Log("null PD");
                return;
            }

            float playerScore = PD.score;


            scores[namesIndex] = playerScore;
            namesIndex++;

        }
        doubleMergeSort(scores, PCOs);

        foreach (PlayerConnectionObject PCO in PCOs) {
            if (!PCO)
                continue;
            PlayerData pdata = PCO.GetComponent<PlayerData>();
            text += pdata.playerName + " score: " + pdata.score +
                " kills: "+ (pdata.killedEntityCount+pdata.killedPlayerCount) +
                " deaths: "+pdata.roundDeathCount+" jumps: "+pdata.roundJumpCount+"\n";
         
        }
        Text textRefernce = GetComponent<Text>();
        if (textRefernce.text!=text)
            textRefernce.text = text;
		
	}

    private void doubleMergeSort(float[] arr, PlayerConnectionObject[] names) {

        float tempf = 0;
        PlayerConnectionObject temps ;
        for (int write = 0; write < arr.Length; write++) {
            for (int sort = 0; sort < arr.Length - 1; sort++) {
                if (arr[sort] < arr[sort + 1]) {
                    tempf = arr[sort + 1];
                    arr[sort + 1] = arr[sort];
                    arr[sort] = tempf;

                    temps = names[sort + 1];
                    names[sort + 1] = names[sort];
                    names[sort] = temps;
                }
            }
        }
    }
}
