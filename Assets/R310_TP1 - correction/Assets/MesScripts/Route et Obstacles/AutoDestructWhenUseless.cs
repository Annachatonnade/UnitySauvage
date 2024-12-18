using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class AutoDestructWhenUseless : MonoBehaviour{
    public Transform    player;
    public float        maxDist;

    void Update(){
        if ((player.position.z - transform.position.z) > maxDist)
            Destroy(this.gameObject);
    }
}
