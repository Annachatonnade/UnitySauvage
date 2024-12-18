using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // pour utiliser SceneManager.


public class RetourSceneJeu : MonoBehaviour
{
    public float delaiRetourSceneJeu=3;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("fctRetourScene", delaiRetourSceneJeu);
        
    }

    // Update is called once per frame
    private void fctRetourScene()
    {
        SceneManager.LoadScene("GameScene");

    }
}
