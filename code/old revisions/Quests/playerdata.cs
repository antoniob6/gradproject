using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class playerdata : NetworkBehaviour
{
    public Canvas canvas;
    public Text scoreField;
    public Text objectiveField;
    public Text healthField;
    [SyncVar ]
    public Color spriteColor=Color.white;
    [SyncVar]
    public string Cname = "default";
    [SyncVar]
    public float scoref=0f;
    [SyncVar]
    public string objectiveText = "objectiveText";


    private SpriteRenderer sprite;


    void Start()
    {
        InvokeRepeating("checkHp", 1.0f, 1.0f);
        sprite = gameObject.GetComponent<SpriteRenderer>();


    }
    void LateUpdate()
    {
        if (sprite.color != spriteColor)
        {
         //   Debug.Log("color cahnged");
            sprite.color = spriteColor;
           
        }
        scoreField.text = "score: " + scoref.ToString();
        objectiveField.text  = objectiveText  ;
    }
    [ClientRpc]
    public void RpcUpdateScore(float scorefn)
    {
        scoref = scorefn;

    }
    [ClientRpc]
    public void RpcUpdateText(string text)
    {
        objectiveText=text;

    }

    public void checkHp()
    {
        ReceiveDamage RD = GetComponent<ReceiveDamage>();
        updateHealth(RD.getHealth());
    }


    public void updateHealth(int hp)
    {
        healthField.text = hp.ToString();

    }

}
