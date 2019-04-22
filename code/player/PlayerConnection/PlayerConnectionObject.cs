using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject[] spawnableCharacters;
    [HideInInspector]   public bool active;
    public PlayerCamera playerCamera;
    [HideInInspector]public BoxCollider2D playerBoundingCollider;
 


    private int index = 0;
    void Start() {
        if (!isLocalPlayer) {
            active = false;
            return;
        }
        active = true;
        CmdSpawnMyUnit();
    }



    [SyncVar(hook = "OnPlayerNameChanged")]
    public string PlayerName = "Anonymous";

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C)) {

            if (index >= spawnableCharacters.Length-1) {
                index = 0;
            } else {
                index++;
            }
            Debug.Log("changing character "+ index);

            CmdSpawnMyUnit();
        }

    }

    void OnPlayerNameChanged(string newName) {
        PlayerName = newName;
        gameObject.name = "PlayerConnectionObject [" + newName + "]";
    }


    [Command]
    void CmdSpawnMyUnit() {
        Vector3 spawnPoint = transform.position;
        if (playerBoundingCollider != null) {
            spawnPoint = playerBoundingCollider.gameObject.transform.position;
            NetworkServer.Destroy(playerBoundingCollider.gameObject);
        }

        GameObject PlayerObject = Instantiate(spawnableCharacters[index],spawnPoint,Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(PlayerObject, connectionToClient);
        RpcUpdateTarget(PlayerObject);
    }
    [ClientRpc]
    void RpcUpdateTarget(GameObject newTarget) {
        if (isLocalPlayer) {
            playerCamera.TargetObject = newTarget;
            PlayableCharacter PC= newTarget.GetComponent<PlayableCharacter>();

            playerBoundingCollider =PC.getPlayerBoundingCollider();
            PC.RD = gameObject.GetComponent<PlayerReceiveDamage>();

        } else {//instances that are on the other clients

        }


        GetComponent<PlayerData>().playerNameText = newTarget.GetComponentInChildren<Text>();
        GetComponent<PlayerData>().spriteRenderer = newTarget.GetComponent<SpriteRenderer>();
    }
    

    [Command]
    void CmdChangePlayerName(string n) {
        Debug.Log("CmdChangePlayerName: " + n);
        PlayerName = n;
        // Tell all the client what this player's name now is.
        //RpcChangePlayerName(PlayerName);
    }
    public Camera getPlayerCamera() {
        return playerCamera.GetComponent<Camera>();
    }


    public void printString(string s) {
        CmdPrintString(s);
    }
    [Command] void CmdPrintString(string s) {
        print(s);
    }
}