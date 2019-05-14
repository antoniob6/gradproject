using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rules  {

    public float length = 500f;
    public float jumpHeight = 1.5f;
     public float Radius = 4;
    public bool isCircle = false;
    public int seed = 0;
    public float gravityForce=20;

    public Rules() {
        randomizeRules();
    }
    public void randomizeRules() {

        Radius = Random.Range(20f, 90f);
        length = Random.Range(100f, 2000f);
        isCircle = Random.Range(0, 2)==1?false:false;
        seed = Random.Range(0, 3000);
        gravityForce = Random.Range(10f, 100f);
        jumpHeight = Random.Range(1f, 6f);
    }


}
