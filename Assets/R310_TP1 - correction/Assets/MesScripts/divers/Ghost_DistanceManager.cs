using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

//using System.Linq;

public class Ghost_DistanceManager : MonoBehaviour
{
    public static Ghost_DistanceManager Instance;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestRecordsText;
    [SerializeField] private Transform player;
    [SerializeField] private Transform ghost;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration;
    [SerializeField] private float recordingRate;
    [SerializeField] private float ghostStartingY = -10f; 
     
    private float remainingTime;
    private Vector3 spawnPoint;
    private float currentDistance = 0f;
    private float[] bestRecords = new float[3];
    private bool isGameActive = false;
    private bool hasPlayedBefore = false;
    private bool FirstGame = true;
    private Vector3 recordStartPosition;

    // Ghost recording system
    private List<PositionData> currentRunData;
    private List<PositionData> bestRunData;
    private float ghostReplayTimer = 0f;

    [System.Serializable]
    private class PositionData
    {
        public Vector3 position;
        public float timeStamp;

        public PositionData(Vector3 pos, float time)
        {
            position = pos;
            timeStamp = time;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentRunData = new List<PositionData>();
            bestRunData = new List<PositionData>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //if (FirstGame)
        //{
        //    FirstGame = !FirstGame;
        //    InitializeRecords();
        //    ghostStartingY = -10f;
        //}
        //ghostStartingY = 0f;
        LoadBestRecords();
        LoadBestRun();
        InitializeGame();
    }

    private void RecordPosition()
    {
        // Si c'est la première position enregistrée, on la sauvegarde comme position de départ
        if (currentRunData.Count == 0)
        {
            recordStartPosition = player.position;
        }
        currentRunData.Add(new PositionData(player.position, gameDuration - remainingTime));
    }

    private void InitializeRecords()
    {
        // Vérifie si c'est la première fois que le jeu est lancé
        hasPlayedBefore = PlayerPrefs.HasKey("HasPlayedBefore");

        if (!hasPlayedBefore)
        {
            // Première initialisation
            for (int i = 0; i < bestRecords.Length; i++)
            {
                bestRecords[i] = 0f;
                PlayerPrefs.SetFloat($"BestRecord_{i}", 0f);
            }
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();

            // Place le ghost sous la route
            if (ghost != null)
            {
                Vector3 ghostPos = ghost.position;
                ghostPos.y = ghostStartingY;
                ghost.position = ghostPos;
            }
        }
        else
        {
            // Charge les records existants
            ghostStartingY = 0f;
            LoadBestRecords();
            LoadBestRun();
        }
    }

    private void Update()
    {
        if (!isGameActive) return;

        // Update timer
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            EndGame();
            return;
        }

        // Update distance
        currentDistance = Vector3.Distance(spawnPoint, player.position);

        // Record player position
        if (Time.time % recordingRate < Time.deltaTime)
        {
            RecordPosition();
        }

        // Update ghost seulement s'il y a un record précédent
        if (bestRecords[0] > 0)
        {
            UpdateGhost();
        }

        UpdateUI();
    }

    private void EndGame()
    {
        isGameActive = false;
        ghost.position = new Vector3( ghost.position.x, -10f, ghost.position.z);
        player.rotation = Quaternion.Euler(player.rotation.eulerAngles.x, 0f, player.rotation.eulerAngles.z);
        
        // Vérifie si on a battuplayer.transform.rotation.y un record
        if (currentDistance > bestRecords[0])
        {
            // Nouveau meilleur record
            float oldBest = bestRecords[0];
            float oldSecond = bestRecords[1];

            bestRecords[2] = oldSecond;  // L'ancien 2ème devient 3ème
            bestRecords[1] = oldBest;    // L'ancien 1er devient 2ème
            bestRecords[0] = currentDistance; // Nouveau meilleur record

            // Sauvegarde le parcours pour le ghost
            bestRunData = new List<PositionData>(currentRunData);
            SaveBestRun();
        }
        else if (currentDistance > bestRecords[1])
        {
            // Nouveau 2ème meilleur record
            bestRecords[2] = bestRecords[1];  // L'ancien 2ème devient 3ème
            bestRecords[1] = currentDistance; // Nouveau 2ème record
        }
        else if (currentDistance > bestRecords[2])
        {
            // Nouveau 3ème meilleur record
            bestRecords[2] = currentDistance;
        }

        SaveBestRecords();

        // Restart après un délai
        Invoke(nameof(InitializeGame), 3f);
    }

    private void InitializeGame()
    {
        player.position = new Vector3(0f, 0f, player.position.z);
        spawnPoint = player.position;
        remainingTime = gameDuration;
        currentDistance = 0f;
        ghostReplayTimer = 0f;
        currentRunData.Clear();
        isGameActive = true;

        // Recharge les positions du ghost avec la nouvelle position de départ
        LoadBestRun();

        // Place le ghost à la position de départ s'il y a un record
        if (ghost != null)
        {
            if (bestRecords[0] > 0)
            {
                ghost.position = spawnPoint;
            }
            else
            {
                // Cache le ghost sous la route s'il n'y a pas de record
                Vector3 ghostPos = spawnPoint;
                ghostPos.y = ghostStartingY;
                ghost.position = ghostPos;
            }
        }

        RecordPosition();
    }

    private void SaveBestRecords()
    {
        for (int i = 0; i < bestRecords.Length; i++)
        {
            PlayerPrefs.SetFloat($"BestRecord_{i}", bestRecords[i]);
        }
        PlayerPrefs.Save();
    }

    private void LoadBestRecords()
    {
        for (int i = 0; i < bestRecords.Length; i++)
        {
            bestRecords[i] = PlayerPrefs.GetFloat($"BestRecord_{i}", 0f);
        }
    }

    private void SaveBestRun()
    {
        PlayerPrefs.SetInt("BestRunCount", bestRunData.Count);

        for (int i = 0; i < bestRunData.Count; i++)
        {
            var data = currentRunData[i]; // Utilise currentRunData au lieu de bestRunData
            PlayerPrefs.SetFloat($"BestRun_{i}_X", data.position.x);
            PlayerPrefs.SetFloat($"BestRun_{i}_Y", data.position.y);
            PlayerPrefs.SetFloat($"BestRun_{i}_Z", data.position.z);
            PlayerPrefs.SetFloat($"BestRun_{i}_Time", data.timeStamp);
        }

        PlayerPrefs.Save();
    }

    private void LoadBestRun()
    {
        int count = PlayerPrefs.GetInt("BestRunCount", 0);
        bestRunData.Clear();

        if (count > 0)
        {
            // Sauvegarde la position de départ du record
            recordStartPosition = new Vector3(
                PlayerPrefs.GetFloat("BestRun_0_X"),
                PlayerPrefs.GetFloat("BestRun_0_Y"),
                PlayerPrefs.GetFloat("BestRun_0_Z")
            );

            for (int i = 0; i < count; i++)
            {
                // Charge la position originale
                Vector3 originalPosition = new Vector3(
                    PlayerPrefs.GetFloat($"BestRun_{i}_X"),
                    PlayerPrefs.GetFloat($"BestRun_{i}_Y"),
                    PlayerPrefs.GetFloat($"BestRun_{i}_Z")
                );

                // Calcule la position relative par rapport au début du record
                Vector3 relativePosition = originalPosition - recordStartPosition;

                // Ajoute la position relative à la position actuelle du joueur
                Vector3 adjustedPosition = player.position + relativePosition;

                float timeStamp = PlayerPrefs.GetFloat($"BestRun_{i}_Time");
                bestRunData.Add(new PositionData(adjustedPosition, timeStamp));
            }
        }
    }


    private void UpdateGhost()
    {
        if (ghost == null || bestRunData.Count < 2) return;

        ghostReplayTimer += Time.deltaTime;

        // Find the appropriate positions in the recorded data
        int nextIndex = bestRunData.FindIndex(p => p.timeStamp > ghostReplayTimer);

        if (nextIndex > 0 && nextIndex < bestRunData.Count)
        {
            var prevPos = bestRunData[nextIndex - 1];
            var nextPos = bestRunData[nextIndex];

            // Interpolate between positions
            float t = Mathf.InverseLerp(prevPos.timeStamp, nextPos.timeStamp, ghostReplayTimer);
            ghost.position = Vector3.Lerp(prevPos.position, nextPos.position, t);
        }
        else if (ghostReplayTimer > gameDuration)
        {
            // Reset ghost replay
            ghostReplayTimer = 0;
            ghost.position = spawnPoint;
        }
    }

    private void UpdateUI()
    {
        if (distanceText != null)
        {
            distanceText.text = $"Distance : {currentDistance:F1} m";
        }

        if (timerText != null)
        {
            timerText.text = $"Temps : {remainingTime:F1} s";
        }

        if (bestRecordsText != null)
        {
            bestRecordsText.text = "Records :\n";
            for (int i = 0; i < bestRecords.Length; i++)
            {
                bestRecordsText.text += $"{i + 1}. {bestRecords[i]:F1} m\n";
            }
        }
    }
}