using System.Collections.Generic;
using UnityEngine;

public class ChronoModeManager : MonoBehaviour
{
    public Transform player;                     // Référence au joueur
    public Transform ghostPrefab;                // Préfabriqué pour le fantôme
    public float chronoTime = 10f;               // Durée de la course en secondes
    private List<Vector3> ghostPositions;        // Liste des positions du fantôme
    private float elapsedTime = 0f;              // Temps écoulé pour la course
    private bool isChronoActive = false;         // Indique si le chrono est actif

    private List<Vector3> bestGhostPositions;    // Meilleur trajet enregistré
    private Transform ghostInstance;             // Instance du fantôme en cours
    private int ghostPlaybackIndex = 0;          // Index de lecture du fantôme

    private bool isFirstRun = true;              // Indique si c’est la première partie

    private void Start()
    {
        ghostPositions = new List<Vector3>();
        bestGhostPositions = new List<Vector3>();

        // Charger les positions du meilleur score précédent (si disponibles)
        LoadBestGhost();

        if (bestGhostPositions.Count == 0)
        {
            isFirstRun = true;
        }
        else
        {
            isFirstRun = false;
        }
    }

    private void EndChrono()
    {
        isChronoActive = false;

        // Comparer la distance parcourue au meilleur score
        float totalDistance = CalculateTotalDistance(ghostPositions);
        float bestDistance = CalculateTotalDistance(bestGhostPositions);

        if (totalDistance > bestDistance)
        {
            bestGhostPositions = new List<Vector3>(ghostPositions); // Sauvegarder le nouveau meilleur trajet
            SaveBestGhost();                                       // Sauvegarder les données
            isFirstRun = false;                                    // Désactiver le premier run
        }

        // Instancier le fantôme uniquement s'il ne s'agit pas de la première partie
        if (!isFirstRun)
        {
            ghostInstance = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        if (isChronoActive)
        {
            // Enregistrer les positions à chaque FixedUpdate
            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime <= chronoTime)
            {
                ghostPositions.Add(player.position);
            }
            else
            {
                EndChrono();
            }
        }

        if (ghostInstance != null && bestGhostPositions.Count > 0)
        {
            // Rejoue les positions du fantôme
            PlayGhost();
        }
    }

    public void StartChrono()
    {
        isChronoActive = true;
        elapsedTime = 0f;
        ghostPositions.Clear();
        if (ghostInstance != null)
        {
            Destroy(ghostInstance.gameObject);
        }
    }

    private void PlayGhost()
    {
        if (ghostPlaybackIndex < bestGhostPositions.Count)
        {
            ghostInstance.position = bestGhostPositions[ghostPlaybackIndex];
            ghostPlaybackIndex++;
        }
        else
        {
            ghostPlaybackIndex = 0; // Réinitialiser le fantôme
        }
    }

    private float CalculateTotalDistance(List<Vector3> positions)
    {
        float distance = 0f;

        for (int i = 1; i < positions.Count; i++)
        {
            distance += Vector3.Distance(positions[i - 1], positions[i]);
        }

        return distance;
    }

    private void SaveBestGhost()
    {
        PlayerPrefs.SetInt("GhostCount", bestGhostPositions.Count);

        for (int i = 0; i < bestGhostPositions.Count; i++)
        {
            PlayerPrefs.SetFloat($"GhostX_{i}", bestGhostPositions[i].x);
            PlayerPrefs.SetFloat($"GhostY_{i}", bestGhostPositions[i].y);
            PlayerPrefs.SetFloat($"GhostZ_{i}", bestGhostPositions[i].z);
        }

        PlayerPrefs.Save();
    }

    private void LoadBestGhost()
    {
        int count = PlayerPrefs.GetInt("GhostCount", 0);

        for (int i = 0; i < count; i++)
        {
            float x = PlayerPrefs.GetFloat($"GhostX_{i}", 0f);
            float y = PlayerPrefs.GetFloat($"GhostY_{i}", 0f);
            float z = PlayerPrefs.GetFloat($"GhostZ_{i}", 0f);

            bestGhostPositions.Add(new Vector3(x, y, z));
        }
    }
}
