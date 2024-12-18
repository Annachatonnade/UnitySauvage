using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CreateStaticRoad : MonoBehaviour
{

    [Tooltip("le pattern de  route ")] 
    public GameObject pattern;
    [Tooltip("Nombre total de pattern de route ")]
    [Range(10, 30)]
    public int nbpatterns = 20;
    [Tooltip("début de la route ")]
    public Transform posInit;
    [Tooltip("le modèle d'obstacles à poser sur la route")]
    public GameObject obst;
    [Tooltip("nombre d'obstacles à poser sur la route")]
    public int nbObst = 0;
    [Tooltip("début de la route (nb pattern)  sans obstacles ")]
    public int nbPatternFreeObst;
    [Tooltip("le GO vide dans la hiérarchie auquel seront rattachés les pattern créés")]
    public Transform trsfParentRoad = null;

    private Vector3 lastPos;
    private BuildInitRoad ir;

    // Start is called before the first frame update
    void Awake()                    // Creer la route dans le AWAKE, première fonction appelée 
    {
        ir = new BuildInitRoad(posInit, pattern, nbpatterns, out lastPos, trsfParentRoad);
    }


    void Start()            // créer les obstacles dans le START, fonction appellée appelée APRES TOUS les AWAKE 
                            //... on sait donc que la route est bien déjà créee
    {
        float endLength, rand_l, startLength;
        float width, rand_w;

        endLength = pattern.GetComponent<Renderer>().bounds.size.x * nbpatterns;
        startLength = pattern.GetComponent<Renderer>().bounds.size.x * nbPatternFreeObst;
        width = pattern.GetComponent<Renderer>().bounds.size.z - obst.GetComponent<Renderer>().bounds.size.x / 2;
        for (int i = 0; i < nbObst; i++)
        {
            rand_l = Random.Range(startLength, endLength);            // répartition des obstacles sauf sur le début de route (nbPatternFreeObst)
            rand_w = Random.Range(-width / 2, width / 2);             // répartition des obstacles sur la largeur de la route
            GameObject obj = Instantiate(obst, posInit.TransformPoint(new Vector3(rand_w, 0, rand_l)), Quaternion.identity);
            obj.name = "obst" + i;
        }
    }

}
