using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradePlayerController : MonoBehaviour
{
 public Transform positionRoute;



    [Header("SANS courbe d'animation")]
    [Range(3, 9)]
    public float minSpeed=6f;
    [Range(10, 100)]
    public float maxSpeed=20f;
    [Range(1,5)]
    public float accel= 3f;
    [Range(1, 5)]
    public float decel = 1f;



    private float speed;
    private float amplitudeSpeed;

    private float horizontalInput;
    private float forwardInput;


    [Range(1, 40)] 
    public float turnspeed = 20f; 

    // Start is called before the first frame update
    void Start()
    {
        //StatsGame.instance.InitStatsGame(player.transform);
        speed = minSpeed;
        amplitudeSpeed = maxSpeed - minSpeed;
    }






    // Version Accelerer Freiner (tourner)     	Décélération automatique 
    //	Ne pas pouvoir dépasser une VitesseMax ni descendre en dessous d’une VitesseMin 
    // (Le véhicule avance donc en permanence même sans accélérer)
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        
        speed += Time.deltaTime * accel * forwardInput;     // acceleration ou freingage
        if (Mathf.Abs(forwardInput) < 0.01f) {              //déceleration
            speed -= Time.deltaTime * decel ;
        }
        if (speed < minSpeed) speed = minSpeed;
        if (speed > maxSpeed) speed = maxSpeed;


        // ==> accelerer
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        //==> tourner
        //   >> rotation
        transform.Rotate(Vector3.up, Time.deltaTime * turnspeed * horizontalInput);


        // Test sortie de route 
        if ((positionRoute.position.y - transform.position.y) > this.GetComponent<Renderer>().bounds.size.y)
            //StatsGame.instance.StopGame(player.transform);
            SceneManager.LoadScene("ENDscene");
    }





}
