﻿
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
    public float runningSpeed = 10;
    public bool isReverseGravity = false;

    public Rules() {
        randomizeRules();
    }
    public void randomizeRules() {

        Radius = Random.Range(20f, 90f);
        isCircle = Random.Range(0, 2)==1?true:false;
        if(isCircle)
            length = Random.Range(400f,1500f);
        else
            length = Random.Range(100f, 1000f);
        seed = Random.Range(0, 3000);
        gravityForce = Random.Range(10f, 100f);
        jumpHeight = Random.Range(2f, 6f);
        isReverseGravity = Random.Range(0, 2) == 1 ? true :false;
        runningSpeed = Random.Range(15f, 50f);

    }

    public GravitySystem.GravityType resolveGravityType() {
        if (isCircle) {
            if (isReverseGravity)
                return GravitySystem.GravityType.ToOut;
            return GravitySystem.GravityType.ToCenter;
        } else {
            if (isReverseGravity)
                return GravitySystem.GravityType.Up;
            return GravitySystem.GravityType.Down;
        }
    }




}
