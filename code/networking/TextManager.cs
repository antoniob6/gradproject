using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextManager : NetworkBehaviour {
    [HideInInspector]
    public static TextManager instance;
    public XPTextMessage textPrefab;
    // Use this for initialization
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
        GameObject textObject = Instantiate(textPrefab.gameObject, position, Quaternion.identity);
        textObject.GetComponent<XPTextMessage>().updateTextOnLocalInstance(text);
    }

    public void createTextOnAll(Vector3 position, string text) {
        GameObject textObject = Instantiate(textPrefab.gameObject, position, Quaternion.identity);
        NetworkServer.Spawn(textObject);
        textObject.GetComponent<XPTextMessage>().updateTextOnAll(text);

    }


}
