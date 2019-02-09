/*
 *this script hold player data(as the name implies),
 * from on side it stores the data about the specific player,
 * and from the other side, it connects the user interface to the game mechanics, 
 * and displays the required info, such as the player health, score and current objective. 
 */
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerData : NetworkBehaviour
{
    public Canvas canvas;
    public Text scoreField;
    public Text objectiveField;
    public Text healthField;

    [SyncVar]
    public Color spriteColor = Color.white;
    [SyncVar]
    public string Cname = "default";
    [SyncVar]
    public float scoref = 0f;
    [SyncVar]
    public string objectiveText = "objectiveText";
    [SyncVar]
    public bool hasDied = false;

    private SpriteRenderer sprite;



    void Start() {
        InvokeRepeating("checkHp", 1.0f, 1.0f);
        sprite = gameObject.GetComponent<SpriteRenderer>();


    }
    void LateUpdate() {
        if (sprite.color != spriteColor) {
            //   Debug.Log("color cahnged");
            sprite.color = spriteColor;

        }
        scoreField.text = "score: " + scoref.ToString();
        objectiveField.text = objectiveText;
    }
    [ClientRpc]
    public void RpcUpdateScore(float scorefn) {
        scoref = scorefn;

    }
    [ClientRpc]
    public void RpcAddScore(float scorefn) {
        scoref += scorefn;

    }
    [ClientRpc]
    public void RpcUpdateText(string text) {
        objectiveText = text;

    }
    [ClientRpc]
    public void RpcUpdateColor(Color c) {
        spriteColor = c;

    }
    [Command]
    public void CmdUpdateColor(Color c) {
        spriteColor = c;
    }


    [ClientRpc]
    public void RpchasDied(bool text) {
        hasDied = text;

    }

    public void checkHp() {
        ReceiveDamage RD = GetComponent<ReceiveDamage>();
        updateHealth(RD.getHealth());
    }


    public void updateHealth(int hp) {
        healthField.text = hp.ToString();

    }

}
