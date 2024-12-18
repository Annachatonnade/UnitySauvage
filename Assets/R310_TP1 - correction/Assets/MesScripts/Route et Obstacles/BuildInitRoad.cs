using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInitRoad
{
    /* c'est une classe qui nen derive pas de : MonoBehaviour
     * elle ne peut donc pas être associée à un GameObject
     * elle ne recoît pas non plus des appels à Update  (etc.) du moteur d'unity
     * 
     * Cette classe va créeer des GameObject dans la scène (les uns à côté des autres) et les associer à un gameObject parent 
     */

/* c'est le consctructeur de la classe qui va créer les objets 
 * pour l',appeler : BuildInitRoad ir = new BuildInitRoad(posInit, pattern, nbpatterns, out lastPos, trsfParentRoad);   
 */
    public BuildInitRoad(Transform posInit, GameObject pattern, int nbPatterns, out Vector3 lastPos, Transform trsfParentRoad)
{
    lastPos = posInit.position;

    for (int i = 0; i < nbPatterns; i++)
        AddPatternRoad(pattern, ref lastPos, "patternInitRoad" + i, trsfParentRoad);
}

    // appelé par le constrcuteur de la classe pour construire la route initiale (ci dessus) 
    // appelé également lors de l'ajout de pattern de route lors de la construction dynamique en temps réel de la route
    // au fur et a mesure de l'avancement du player : appelé par la methode update  de la classe CreateRealTimeRoad
    public GameObject AddPatternRoad(GameObject pattern, ref Vector3 pos, string name = "patternRd", Transform trsfParentRoad = null)
{
    GameObject obj = GameObject.Instantiate(pattern, pos, Quaternion.Euler(0, 90, 0), trsfParentRoad);
    obj.name = name;
    pos += Vector3.forward * pattern.GetComponent<Renderer>().bounds.size.x;
    return obj;
}

}
