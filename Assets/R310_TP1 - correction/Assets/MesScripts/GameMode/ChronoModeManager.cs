using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class ChronoModeManager : MonoBehaviour
{
    public Transform player;                     // Référence au joueur
    public Transform ghostPrefab;               // Préfabriqué pour le fantôme
    public TextMeshProUGUI TimeText;
    public float chronoTime = 90f;              // Durée de la course en secondes

    private List<Vector3> ghostPositions;       // Liste des positions du fantôme
    private float elapsedTime = 0f;             // Temps écoulé pour la course
    private bool isChronoActive = false;        // Indique si le chrono est actif

    private List<Vector3> bestGhostPositions;   // Meilleur trajet enregistré
    private Transform ghostInstance;            // Instance du fantôme en cours
    private int ghostPlaybackIndex = 0;         // Index de lecture du fantôme

    private string saveFilePath;                // Chemin du fichier JSON pour sauvegarder les positions

    private void Start()
    {
        ghostPositions = new List<Vector3>();
        bestGhostPositions = new List<Vector3>();
        saveFilePath = Application.persistentDataPath + "/bestGhost.json";

        TimeText.text = "Temps restant : " + chronoTime.ToString("F1") + " s";

        // Charger les positions du meilleur score précédent (si disponibles)
        LoadBestGhost();
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
        }

        // Instancier le fantôme
        if (bestGhostPositions.Count > 0)
        {
            ghostInstance = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        if (isChronoActive)
        {
            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime <= chronoTime)
            {
                ghostPositions.Add(player.position);
                TimeText.text = "Temps restant : " + (chronoTime - elapsedTime).ToString("F1") + " s";
            }
            else
            {
                EndChrono();
            }
        }

        if (ghostInstance != null && bestGhostPositions.Count > 0)
        {
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
        if (ghostPlaybackIndex < bestGhostPositions.Count - 1)
        {
            // Interpolation des positions
            Vector3 start = bestGhostPositions[ghostPlaybackIndex];
            Vector3 end = bestGhostPositions[ghostPlaybackIndex + 1];

            float lerpFactor = (elapsedTime % Time.fixedDeltaTime) / Time.fixedDeltaTime;
            ghostInstance.position = Vector3.Lerp(start, end, lerpFactor);

            if (Vector3.Distance(ghostInstance.position, end) < 0.1f)
            {
                ghostPlaybackIndex++;
            }
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
        GhostData ghostData = new GhostData
        {
            positions = bestGhostPositions
        };

        string json = JsonUtility.ToJson(ghostData);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadBestGhost()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GhostData ghostData = JsonUtility.FromJson<GhostData>(json);

            bestGhostPositions = ghostData.positions;
        }
    }

    [System.Serializable]
    public class GhostData
    {
        public List<Vector3> positions;
    }
}
