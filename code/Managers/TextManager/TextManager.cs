using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextManager : NetworkBehaviour {


    public XPTextMessage XPTextPrefab;
    public XPTextMessage XPRedTextPrefab;
    public GameObject messageToAllPrefab;
    public GameObject GreenTopLeftMessagePrefab;
    [HideInInspector]
    public static TextManager instance;
    void Awake() {
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void createTextOnLocalInstance(Vector3 position,string text) {
        GameObject textObject = Instantiate(XPTextPrefab.gameObject, position, Quaternion.identity);
        if (GravitySystem.instance.isCircularMap())
            textObject.transform.LookAt(GravitySystem.instance.getUpDirection(textObject.transform.position));
        textObject.GetComponent<XPTextMessage>().updateTextOnLocalInstance(text);
    }
    public void createRedTextOnLocalInstance(Vector3 position, string text) {
        GameObject textObject = Instantiate(XPRedTextPrefab.gameObject, position, Quaternion.identity);
        if (GravitySystem.instance.isCircularMap())
            textObject.transform.LookAt(GravitySystem.instance.getUpDirection(textObject.transform.position));
        textObject.GetComponent<XPTextMessage>().updateTextOnLocalInstance(text);
    }
    /// <summary>sends debugs from server to clients, but doesn't show who called it. </summary>
    [ClientRpc]public void RpcDebugOnAll(string v) {
        Debug.Log(v);
    }


    public void createTextOnAll(Vector3 position, string text) {
        GameObject textObject = Instantiate(XPTextPrefab.gameObject, position, Quaternion.identity);
        NetworkServer.Spawn(textObject);
        textObject.GetComponent<XPTextMessage>().updateTextOnAll(text);

    }

    public void displayMessageToAll(string m, float time = 3) {
        if (!isServer)
            return;
        StartCoroutine(displayMessage(m, time));
    }
    private GameObject spawnedMessageObjOnServer;
    IEnumerator displayMessage(string m, float time) {
        if (spawnedMessageObjOnServer)
            NetworkServer.Destroy(spawnedMessageObjOnServer);
        spawnedMessageObjOnServer = Instantiate(messageToAllPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(spawnedMessageObjOnServer);
        spawnedMessageObjOnServer.GetComponent<GameMessage>().RpcUpdateText(m);
        yield return new WaitForSeconds(time);
        if (spawnedMessageObjOnServer)
            NetworkServer.Destroy(spawnedMessageObjOnServer);
    }


    [ClientRpc]
    public void RpcDisplayGreenMessageToPlayer(GameObject PCOGO, string m) {
        if (!PCOGO.GetComponent<NetworkIdentity>().isLocalPlayer) {
            return;
        }
        //found target Player object
        PlayerConnectionObject PCO = PCOGO.GetComponent<PlayerConnectionObject>();
        if (!PCO) {
            Debug.Log("couldn't find PCO");
            return;
        }

        StartCoroutine(displayGreenMessage(m, 6f));

    }

    private GameObject spawnedGreenMessage;
    IEnumerator displayGreenMessage(string m, float time) {
        if (spawnedGreenMessage)
            Destroy(spawnedGreenMessage);
        spawnedGreenMessage = Instantiate(GreenTopLeftMessagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        spawnedGreenMessage.GetComponent<GameMessage>().updateTextLocal(m);
        yield return new WaitForSeconds(time);
        if (spawnedGreenMessage)
            Destroy(spawnedGreenMessage);
    }

    public void clearMessage() {
        if (spawnedMessageObjOnServer)
            NetworkServer.Destroy(spawnedMessageObjOnServer);
    }

}
