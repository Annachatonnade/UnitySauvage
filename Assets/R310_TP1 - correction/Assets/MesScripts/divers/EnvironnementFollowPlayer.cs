using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironnementFollowPlayer : MonoBehaviour
{
    public GameObject player;
        
    private Vector3 offset ; 
    private Vector3 newpos;

    void Start(){
        offset = this.transform.position - player.transform.position;
    }
    private void LateUpdate(){
        // sans latence
        newpos = transform.position;
        newpos.z = player.transform.position.z + offset.z;
        transform.position=newpos;
    }

}
