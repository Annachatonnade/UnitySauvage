
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using Unity.VisualScripting;
//using UnityEngine;
//using static CreateRealTimeRoad;

//public class CreateRealTimeRoad : MonoBehaviour
//{
//    [Header("Player")]
//    public Transform player;
//    private float lastPosPlayer;
//    private Vector3 dimPattern;
//    [Space(20)]

//    [Header("Road & Patterns")]
//    public GameObject pattern;
//    [Range(10, 30)]
//    public int nbPatternsInitRoad = 20;    // valeur par défaut


//    public Transform posInitPattern;
//    [Tooltip("Distance au player au dela de laquelle un pattern procédural est supprimé")]
//    public Transform trsfParentRoad = null;
//    private Vector3 lastPosPattern;


//    [Tooltip("LA probabilité d'avoir un obsctacle lorsqu'un pattern de route est créé")]
//    [Range(0.0f, 1.0f)]
//    public float probaAllObst;
//    [System.Serializable]

//    public class probaObst
//    {
//        public float proba;      // probabilité d'avoir cet objet quand il faut ajouter un obstacle
//        public GameObject obst;
//    }
//    [Space(20)]
//    [Header("Obstacles")]
//    public probaObst[] probaObsts;      // tous les obstacles possibles et leur probabilité (relative) 
//    private float totalProbaObst;
//    private Vector3[] dimObst;
//    public Transform trsfParentObst = null;

//    [Space(20)]
//    [Header("difficulty")]
//    [Tooltip("la difficulté (d'avoir un ostacle ) augmente toutes les ... secondes ")]
//    [Range(5, 30)]
//    public float increasePeriod = 10;
//    private const float COEFF_INCREASE_DIFF = 1.05f;    // une constante : augmentation de 5% de la difficulté à chaque période


//    private BuildInitRoad initialRoad;       // la route initiale

//    private float coeff;                    // variable utilisée dans update
//    private float distDestructionPattern;   // distance au player à partir de laquelle il faut destruire un objet non visible 

//    [Header("Bonus")]
//    public GameObject bonusObject;
//    [Range(0f, 1f)]
//    public float initialBonusProba = 0.3f;
//    private float currentBonusProba;
//    public float bonusSpeedIncrease = 5f;

//    [Header("Sapins")]
//    public GameObject treeObject;
//    public float treeSpacing = 20f;
//    private bool treesPlaced = false;
//    private const float TREE_SECTION_LENGTH = 400f;

//    private float distanceTraveled = 0f;
//    private const float DIFFICULTY_INCREASE_DISTANCE = 100f;
//    private const float BONUS_DECREASE_RATE = 0.98f;
//    private const float OBSTACLE_INCREASE_RATE = 1.02f;

//    Creer la route dans le AWAKE, première fonction appelée
//    void Awake()
//    { // création de la route intiale sans obstacle
//        initialRoad = new BuildInitRoad(posInitPattern, pattern, nbPatternsInitRoad, out lastPosPattern, trsfParentRoad);


//        préparation des données
//       dimObst = new Vector3[probaObsts.Length];
//        for (int i = 0; i < probaObsts.Length; i++)
//            dimObst[i] = probaObsts[i].obst.GetComponent<Renderer>().bounds.size;

//        dimPattern = pattern.GetComponent<Renderer>().bounds.size;
//        lastPosPlayer = player.position.z;

//        modification du tableau de proba
//         e.g.  10 , 10, 30, 150 devient 10 , 20, 50 , 200
//        totalProbaObst = 0.0f;
//        float previousProb = 0;
//        foreach (probaObst p in probaObsts)
//        {
//            totalProbaObst += p.proba;
//            p.proba += previousProb;
//            previousProb = p.proba;
//        }
//        puis  10 , 20, 50 , 200  devient   0.05 , 0.10, 0.25,1
//        foreach (probaObst p in probaObsts)
//        {
//            p.proba /= totalProbaObst;
//        }

//        modification de la difficulté dans le temps
//        InvokeRepeating("UpdateDifficulty", increasePeriod, increasePeriod);

//        coeff = 0.0f;

//        distDestructionPattern = player.GetComponent<Renderer>().bounds.size.z * 10;
//        currentBonusProba = initialBonusProba;
//    }

//    vérifier à chaque frame, en fonction de l'avancement du véhicule player  …
//     …. s'il est nécessaire une ou plusieurs portions de route 
//     pour chaque portion de route créée, selon tirage aléatoire et probabilité …
//     … créer ou non un obstacle
//     pour chaque obstacle àcréé , tirage aléatoire pour savoir lequel choisir en fonction …
//     … des proba fournies pour chacun d'eux 

//    void Update()
//    {
//        Calculer la distance parcourue
//        distanceTraveled = player.position.z - posInitPattern.position.z;

//        Placer les sapins si ce n'est pas déjà fait
//        if (!treesPlaced)
//        {
//            PlaceTreesAlongRoad();
//        }

//        Calculer combien de patterns sont nécessaires
//        coeff += (player.position.z - lastPosPlayer) / dimPattern.x;

//        while (coeff > 1.0f)
//        {
//        IMPORTANT: Créer le nouveau pattern de route d'abord
//            GameObject newPattern = initialRoad.AddPatternRoad(pattern, ref lastPosPattern, "patternRd", trsfParentRoad);
//            newPattern.AddComponent<AutoDestructWhenUseless>().player = player;
//            newPattern.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;

//            APRÈS avoir créé la route, gérer les obstacles et bonus
//            if (distanceTraveled > TREE_SECTION_LENGTH) // Ne pas mettre d'obstacles dans la section avec les sapins
//            {
//                Gestion des obstacles avec difficulté progressive
//                float obstacleProb = Random.Range(0.0f, 1.0f);
//                if (obstacleProb < probaAllObst)
//                {
//                    PlaceObstacle();
//                }

//                Gestion des bonus avec probabilité décroissante
//                float bonusProb = Random.Range(0.0f, 1.0f);
//                if (bonusProb < currentBonusProba)
//                {
//                    PlaceBonus();
//                }
//            }

//            Mettre à jour la position du dernier pattern
//            lastPosPlayer = player.position.z;
//            coeff -= 1.0f;
//        }

//        Ajuster la difficulté en fonction de la distance
//        if (distanceTraveled > TREE_SECTION_LENGTH && distanceTraveled % DIFFICULTY_INCREASE_DISTANCE == 0)
//        {
//            AdjustDifficulty();
//        }
//    }

//    private void UpdateDifficulty()
//    {
//        probaAllObst *= COEFF_INCREASE_DIFF;

//        player.GetComponent<RealistPlayerController>().minSpeed *= COEFF_INCREASE_DIFF;        // PAS SATISFAISANT : il faudrait un manager !!! on fera ca plus tard 
//        player.GetComponent<RealistPlayerController>().maxSpeed *= COEFF_INCREASE_DIFF;        // PAS SATISFAISANT : il faudrait un manager !!! on fera ca plus tard 

//    }

//    private void PlaceTreesAlongRoad()
//    {
//        float currentDistance = 0f;
//        while (currentDistance < TREE_SECTION_LENGTH) // TREE_SECTION_LENGTH = 400f
//        {
//            Placer des sapins des deux côtés de la route
//            float roadWidth = dimPattern.z;
//            Vector3 leftPos = posInitPattern.position + new Vector3(-roadWidth / 2 - 2f, 0, currentDistance);
//            Vector3 rightPos = posInitPattern.position + new Vector3(roadWidth / 2 + 2f, 0, currentDistance);

//            Créer les sapins
//           GameObject leftTree = Instantiate(treeObject, leftPos, Quaternion.identity, trsfParentRoad);
//            GameObject rightTree = Instantiate(treeObject, rightPos, Quaternion.identity, trsfParentRoad);

//            Ajouter l'auto-destruction quand les sapins sont trop loin
//            leftTree.AddComponent<AutoDestructWhenUseless>().player = player;
//            rightTree.AddComponent<AutoDestructWhenUseless>().player = player;
//            leftTree.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;
//            rightTree.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;

//            Passer à la position suivante
//           currentDistance += treeSpacing;
//        }
//        treesPlaced = true;
//    }

//    private void PlaceObstacle()
//    {
//        Tirage aléatoire pour choisir quel obstacle placer
//        float prob = Random.Range(0.0f, 1f);
//        int num_obst = 0;
//        while (prob >= probaObsts[num_obst].proba) num_obst++;
//        GameObject obst_a_placer = probaObsts[num_obst].obst;

//        Calculer la position de l'obstacle
//        Vector3 posObst = lastPosPattern;
//        posObst.y += dimObst[num_obst].y / 2;

//        Position aléatoire sur la largeur de la route
//        posObst.x += Random.Range(
//            -dimPattern.z / 2 + dimObst[num_obst].x / 2,
//            dimPattern.z / 2 - dimObst[num_obst].x / 2
//        );

//        Créer l'obstacle
//        GameObject newObst = GameObject.Instantiate(obst_a_placer, posObst, Quaternion.identity, trsfParentObst);

//        Ajouter l'auto-destruction
//        newObst.AddComponent<AutoDestructWhenUseless>().player = player;
//        newObst.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;
//    }

//    private void PlaceBonus()
//    {
//        Vector3 posBonus = lastPosPattern;
//        posBonus.y += 1f; // Hauteur du bonus
//        posBonus.x += Random.Range(-dimPattern.z / 3, dimPattern.z / 3);

//        GameObject bonus = Instantiate(bonusObject, posBonus, Quaternion.identity, trsfParentObst);
//        bonus.AddComponent<BonusEffect>().speedIncrease = bonusSpeedIncrease;
//        bonus.AddComponent<AutoDestructWhenUseless>().player = player;
//        bonus.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;
//    }

//    private void AdjustDifficulty()
//    {
//        Augmenter la probabilité d'obstacles
//        probaAllObst = Mathf.Min(probaAllObst * OBSTACLE_INCREASE_RATE, 0.9f);

//        Diminuer la probabilité de bonus
//       currentBonusProba = Mathf.Max(currentBonusProba * BONUS_DECREASE_RATE, 0.05f);
//    }
//}

//Script pour gérer l'effet des bonus
//public class BonusEffect : MonoBehaviour
//{
//    public float speedIncrease = 5f;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            RealistPlayerController playerController = other.GetComponent<RealistPlayerController>();
//            if (playerController != null)
//            {
//                playerController.maxSpeed += speedIncrease;
//                playerController.minSpeed += speedIncrease;
//                Destroy(gameObject);
//            }
//        }
//    }
//}

















using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using static CreateRealTimeRoad;

public class CreateRealTimeRoad : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    private float lastPosPlayer;
    private Vector3 dimPattern;
    [Space(20)]

    [Header("Road & Patterns")]
    public GameObject pattern;
    [Range(10, 30)]
    public int nbPatternsInitRoad = 20;    // valeur par défaut


    public Transform posInitPattern;
    [Tooltip("Distance au player au dela de laquelle un pattern procédural est supprimé")]
    public Transform trsfParentRoad = null;
    private Vector3 lastPosPattern;


    [Tooltip("LA probabilité d'avoir un obsctacle lorsqu'un pattern de route est créé")]
    [Range(0.0f, 1.0f)]
    public float probaAllObst;
    [System.Serializable]


    public class probaObst
    {
        public float proba;      // probabilité d'avoir cet objet quand il faut ajouter un obstacle
        public GameObject obst;
    }
    [Space(20)]
    [Header("Obstacles")]
    public probaObst[] probaObsts;      // tous les obstacles possibles et leur probabilité (relative) 
    private float totalProbaObst;
    private Vector3[] dimObst;
    public Transform trsfParentObst = null;

    [Space(20)]
    [Header("difficulty")]
    [Tooltip("la difficulté (d'avoir un ostacle ) augmente toutes les ... secondes ")]
    [Range(5, 30)]
    public float increasePeriod = 10;
    private const float COEFF_INCREASE_DIFF = 1.05f;    // une cnstante : augmentation de 5% de la difficulté à chaque période


    private BuildInitRoad initialRoad;       // la route initiale

    private float coeff;                    // variable utilisée dans update
    private float distDestructionPattern;   // distance au player à partir de laquelle il faut destruire un objet non visible 

    // Creer la route dans le AWAKE, première fonction appelée
    void Awake()
    { // création de la route intiale sans obstacle
        initialRoad = new BuildInitRoad(posInitPattern, pattern, nbPatternsInitRoad, out lastPosPattern, trsfParentRoad);


        // préparation des données
        dimObst = new Vector3[probaObsts.Length];
        for (int i = 0; i < probaObsts.Length; i++)
            dimObst[i] = probaObsts[i].obst.GetComponent<Renderer>().bounds.size;

        dimPattern = pattern.GetComponent<Renderer>().bounds.size;
        lastPosPlayer = player.position.z;

        // modification du tableau de proba
        // e.g.  10 , 10, 30, 150 devient 10 , 20, 50 , 200
        totalProbaObst = 0.0f;
        float previousProb = 0;
        foreach (probaObst p in probaObsts)
        {
            totalProbaObst += p.proba;
            p.proba += previousProb;
            previousProb = p.proba;
        }
        // puis  10 , 20, 50 , 200  devient   0.05 , 0.10, 0.25,1
        foreach (probaObst p in probaObsts)
        {
            p.proba /= totalProbaObst;
        }

        // modification de la difficulté dans le temps 
        InvokeRepeating("UpdateDifficulty", increasePeriod, increasePeriod);

        coeff = 0.0f;

        distDestructionPattern = player.GetComponent<Renderer>().bounds.size.z * 10;
    }

    // vérifier à chaque frame , en fonction de l'avancement du véhicule player  …
    // …. s'il est nécessaire une ou plusieurs portions de route 
    // pour chaque portion de route créée , selon tirage aléatoire et probabilité …
    // … créer ou non un obstacle
    // pour chaque obstacle àcréé , tirage aléatoire pour savoir lequel choisir en fonction …
    // … des proba fournies pour chacun d'eux 

    void Update()
    {
        coeff += (player.position.z - lastPosPlayer) / dimPattern.x;
        // coeff  au cas ou la vitesse du véhicule lui fait franchir plus d'un pattern entre 2 frames !! 
        // += pour ne pas perdre la portion de route non comptabilisé à la frame précédente
        while (coeff > 1.0f)              // si au moins la distance d'un pattern parcouru
                                          // à finir de tester , en crée trop a tres haute vitesse  ????
        {
            // connaitre dans l'IDE le game object qui a déclenché le message debug 
            //Debug.Log("new pattern ?" + lastPosPlayer + "   /   " + player.position.z + "    -   " + lengthPattern, this.gameObject);
            // ***  mettre en pause l'éxécution **** 
            //Debug.Break();

            // les GameObject  pattern de route sont instantié dynamiquement (3D procédurale) 
            // on leur ajoute l'instance de la classe permettant de les détruire automaqiuement quand inutiles  (Question H )
            // on leur ajoute les paramètres attendus (qui ne peuvent être connus par un prefab mais doivent etre connus par l'instance créée du prefab)
            GameObject newPattern = initialRoad.AddPatternRoad(pattern, ref lastPosPattern, "patternRd", trsfParentRoad);
            newPattern.AddComponent<AutoDestructWhenUseless>();                                     // (Question H )
            newPattern.GetComponent<AutoDestructWhenUseless>().player = player;                     // (Question H )
            newPattern.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;    // (Question H )
            lastPosPlayer = player.position.z;

            float prob = Random.Range(0.0f, 1.0f);
            if (prob < probaAllObst)
            {

                // positionner un obstacle   QUESTION 
                // tirage aléatoire pour savoir lequel parmis la liste d'obstacle et leur proba 
                prob = Random.Range(0.0f, 1f);
                int num_obst = 0;
                while (prob >= probaObsts[num_obst].proba) num_obst++;
                GameObject obst_a_placer = probaObsts[num_obst].obst;

                GameObject newObst = null;
                // *****  Question G et H
                // POSITIONNER l'obstacle     
                Vector3 posObst = lastPosPattern;
                posObst.y += dimObst[num_obst].y / 2;
                posObst.x += Random.Range(-dimPattern.z / 2 + dimObst[num_obst].x / 2, dimPattern.z / 2 - dimObst[num_obst].x / 2);
                newObst = GameObject.Instantiate(obst_a_placer, posObst, Quaternion.identity, trsfParentObst);
                newObst.AddComponent<AutoDestructWhenUseless>();                                     // (Question H )
                newObst.GetComponent<AutoDestructWhenUseless>().player = player;                     // (Question H )
                newObst.GetComponent<AutoDestructWhenUseless>().maxDist = distDestructionPattern;



            }
            coeff -= 1.0f;          // si plus d'1 pattern parcouru, autre itération  du while (coeff> 1.0f  )    
        }

    }

    private void UpdateDifficulty()
    {
        probaAllObst *= COEFF_INCREASE_DIFF;

        player.GetComponent<RealistPlayerController>().minSpeed *= COEFF_INCREASE_DIFF;        // PAS SATISFAISANT : il faudrait un manager !!! on fera ca plus tard 
        player.GetComponent<RealistPlayerController>().maxSpeed *= COEFF_INCREASE_DIFF;        // PAS SATISFAISANT : il faudrait un manager !!! on fera ca plus tard 

    }
}