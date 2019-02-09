using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMessage : NetworkBehaviour {


    public Text textUI;

    [SyncVar]
    public string sText = "MessageText";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    [ClientRpc]
    public void RpcUpdateText(string text) {
        sText = text;

        textUI.text= text;

    }




}
