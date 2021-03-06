﻿/*
 * corrects the scale and rotation on the objects on the client
 * without it scaled and rotated objects will have problems on the client
 */


using UnityEngine;
using UnityEngine.Networking;

public class NetCorrector : NetworkBehaviour
{

    [HideInInspector] [SyncVar] public Vector3 scale;
    [HideInInspector] [SyncVar] public Quaternion rotation;

    void Start()
    {
        transform.localScale = scale;
        transform.rotation = rotation;
    }
}

