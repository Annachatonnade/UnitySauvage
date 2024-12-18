using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // pour utiliser SceneManager.

public class PlayerController : MonoBehaviour
{
    public float speed = 20;
    public float turnspeed = 20;
    public Transform positionRoute;
    
    
    private  GameObject player;


    //[SerializeField]  pour voir des données privates dans l'inspector 
    [SerializeField] private float horizontalinput;
    [SerializeField] private float verticalinput;

    private void Start()
    {
        player = gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        horizontalinput = Input.GetAxis("Horizontal");  // avancer sur fleches Haut Bas
        verticalinput = Input.GetAxis("Vertical");      // tourner sur flèches Droite Gauche 

        transform.Translate(Vector3.forward * Time.deltaTime * speed     * verticalinput);
        transform.Rotate(   Vector3.up  ,     Time.deltaTime * turnspeed * horizontalinput);



        // Test sortie de route 
        if ((positionRoute.position.y - transform.position.y) > player.GetComponent<Renderer>().bounds.size.y)
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("ENDscene");

    }   
}
