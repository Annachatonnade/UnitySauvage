using TMPro;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager Instance; // Instance pour un acc�s global

    private float totalDistance = 0;         // Distance totale parcourue
    private float bestRecord = 0;            // Meilleur record atteint
    public TextMeshProUGUI distanceText;     // Texte UI pour afficher la distance
    public TextMeshProUGUI bestRecordText;   // Texte UI pour afficher le meilleur record
    public Transform player;                 // R�f�rence au joueur

    private Vector3 spawnPoint;              // Point de spawn initial du joueur

    private void Awake()
    {
        // Assure qu'il n'y a qu'une seule instance de ce script
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Enregistrez la position initiale du joueur comme point de spawn
        spawnPoint = player.position;

        // Charge le meilleur record depuis PlayerPrefs
        bestRecord = PlayerPrefs.GetFloat("BestRecord", 0f);

        // Initialise l'affichage de la distance et du meilleur record
        UpdateUI();
    }

    private void Update()
    {
        // Calcule la distance entre le joueur et son point de spawn
        totalDistance = Vector3.Distance(spawnPoint, player.position);

        // Compare la distance parcourue avec le meilleur record et met � jour si n�cessaire
        if (totalDistance > bestRecord)
        {
            bestRecord = totalDistance; // Met � jour le meilleur record

            // Sauvegarde le nouveau meilleur record dans PlayerPrefs
            PlayerPrefs.SetFloat("BestRecord", bestRecord);
            PlayerPrefs.Save(); // Sauvegarde imm�diatement les changements
        }

        // Met � jour l'affichage de la distance et du meilleur record
        UpdateUI();
    }

    void UpdateUI()
    {
        if (distanceText != null)
        {
            // Affiche la distance parcourue avec une pr�cision de 1 d�cimale
            distanceText.text = "Distance parcourue : " + totalDistance.ToString("F1") + " m";
        }

        if (bestRecordText != null)
        {
            // Affiche le meilleur record avec une pr�cision de 1 d�cimale
            bestRecordText.text = "Meilleur record : " + bestRecord.ToString("F1") + " m";
        }
    }
}
