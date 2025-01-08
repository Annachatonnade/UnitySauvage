using TMPro;
using UnityEngine;

public class Ghost_DistanceManager : MonoBehaviour
{
    public static Ghost_DistanceManager Instance; // Instance pour un accès global

    private float totalDistance = 0;             // Distance totale parcourue
    private float[] bestRecords = new float[3];  // Tableau pour les 3 meilleurs scores
    public TextMeshProUGUI distanceText;         // Texte UI pour afficher la distance
    public TextMeshProUGUI bestRecordsText;      // Texte UI pour afficher les 3 meilleurs scores
    public Transform player;                     // Référence au joueur

    private Vector3 spawnPoint;                  // Point de spawn initial du joueur

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
        // Enregistre la position initiale du joueur comme point de spawn
        spawnPoint = player.position;

        // Charge les 3 meilleurs scores depuis PlayerPrefs
        LoadBestRecords();

        // Initialise l'affichage de la distance et des meilleurs scores
        UpdateUI();
    }

    private void Update()
    {
        // Calcule la distance entre le joueur et son point de spawn
        totalDistance = Vector3.Distance(spawnPoint, player.position);

        // Compare la distance parcourue avec les 3 meilleurs scores et met à jour si nécessaire
        if (totalDistance > bestRecords[2]) // Vérifie si le score dépasse le plus bas
        {
            UpdateBestRecords(totalDistance);
        }

        // Met à jour l'affichage de la distance et des meilleurs scores
        UpdateUI();
    }

    private void UpdateBestRecords(float newDistance)
    {
        // Insère le nouveau score dans le tableau des meilleurs scores
        for (int i = 0; i < bestRecords.Length; i++)
        {
            if (newDistance > bestRecords[i])
            {
                // Décale les scores inférieurs d'un rang pour faire de la place
                for (int j = bestRecords.Length - 1; j > i; j--)
                {
                    bestRecords[j] = bestRecords[j - 1];
                }

                // Insère le nouveau score à la bonne position
                bestRecords[i] = newDistance;

                // Sauvegarde les nouveaux scores
                SaveBestRecords();
                break;
            }
        }
    }

    private void SaveBestRecords()
    {
        for (int i = 0; i < bestRecords.Length; i++)
        {
            PlayerPrefs.SetFloat($"BestRecord_{i}", bestRecords[i]);
        }
        PlayerPrefs.Save(); // Sauvegarde immédiatement les changements
    }

    private void LoadBestRecords()
    {
        for (int i = 0; i < bestRecords.Length; i++)
        {
            bestRecords[i] = PlayerPrefs.GetFloat($"BestRecord_{i}", 0f);
        }
    }

    private void UpdateUI()
    {
        if (distanceText != null)
        {
            // Affiche la distance parcourue avec une précision de 1 décimale
            distanceText.text = "Distance parcourue : " + totalDistance.ToString("F1") + " m";
        }

        if (bestRecordsText != null)
        {
            // Construit le texte des meilleurs scores
            bestRecordsText.text = "Meilleurs records :\n";
            for (int i = 0; i < bestRecords.Length; i++)
            {
                bestRecordsText.text += $"{i + 1}. {bestRecords[i].ToString("F1")} m\n";
            }
        }
    }
}
