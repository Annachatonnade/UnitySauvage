using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject player;
    [Tooltip("temps de latence : décalage de suivi du joueur ")]
    public float smoothTime ;

        private Vector3 offset ; 
        private Vector3 velocity = Vector3.zero;

    void Start()
    {
        //offset= new Vector3(0,5,-7);          // décalalge calculé au lancement de la scène
        offset = this.transform.position - player.transform.position;
    }

    void Update()    {
        // sans latence
        //transform.position=player.transform.position + offset ; 
    
        // avec latence    c'est la question K du TP1 
        Vector3 targetPosition = player.transform.TransformPoint(offset);
        transform.position=Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}
