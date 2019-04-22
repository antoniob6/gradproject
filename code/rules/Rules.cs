using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rules :MonoBehaviour {
  public float length = 2f;
  public float width = 2f;

    // public float stepHeight = 2f;
    public float jumpHeight = 1.5f;

     public float Radius = 4;
      public bool isCircle = true;
    public int resX = 10; // 2 minimum
    public int stepWidth = 4;
    public int seed = 0;

    public void randomizeRules() {
        jumpHeight = Random.Range(1f, 6f);
        Radius = Random.Range(20f, 90f);
        length =  Random.Range(10f, 50f);
        resX =(int) length;
    }


}
