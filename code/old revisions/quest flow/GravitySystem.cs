using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem :MonoBehaviour {
    public enum GravityType
    {
        Down,Up,ToCenter,ToOut
    }
    public GravityType gravity= GravityType.Down;

}
