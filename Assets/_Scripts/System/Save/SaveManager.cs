using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    #region Singleton
    public static SaveManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    string jsonPathProject;
    string jsonPathPersistent;

    private void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        jsonPathPersistent = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";

        if (File.Exists(jsonPathProject))
            isFirstTime = false;
    }

    #region <-- General -->

    public bool isFirstTime = true;

    #region <-- Saving -->

    public void SaveGame()
    {
        AllGameData data = new AllGameData();
        data.playerData = GetPlayerData();

        SaveGameDataToJsonFile(data);
        isFirstTime = false;
    }

    private PlayerData GetPlayerData()
    {
        int[] playerStats = new int[5];
        playerStats[0] = PlayerInfo.Instance.playerLevel;
        playerStats[1] = PlayerInfo.Instance.playerExperience;
        playerStats[2] = PlayerInfo.Instance.playerExperiencePerLevel;
        playerStats[3] = PlayerInfo.Instance.totalCoin;
        playerStats[4] = PlayerInfo.Instance.dayInGame;

        float[] playerCoords = new float[6];
        playerCoords[0] = GameObject.FindWithTag("Player").transform.position.x;
        playerCoords[1] = GameObject.FindWithTag("Player").transform.position.y;
        playerCoords[2] = GameObject.FindWithTag("Player").transform.position.z;

        playerCoords[3] = GameObject.FindWithTag("Player").transform.rotation.x;
        playerCoords[4] = GameObject.FindWithTag("Player").transform.rotation.y;
        playerCoords[5] = GameObject.FindWithTag("Player").transform.rotation.z;

        return new PlayerData(playerStats, playerCoords);
    }

    #endregion

    #region <-- Loading -->

    public void LoadGame()
    {
        //PlayerData
        SetPlayerData(LoadGameDatafromJsonFile().playerData);

        //EnvironmentData
    }

    private void SetPlayerData(PlayerData playerData)
    {
        // PlayerStats
        PlayerInfo.Instance.playerLevel = playerData.playerStats[0];
        PlayerInfo.Instance.playerExperience = playerData.playerStats[1];
        PlayerInfo.Instance.playerExperiencePerLevel = playerData.playerStats[2];
        PlayerInfo.Instance.totalCoin = playerData.playerStats[3];
        PlayerInfo.Instance.dayInGame = playerData.playerStats[4];

        // PlayerCoords
        Vector3 loadedPosition = new Vector3(
            playerData.playerCoords[0],
            playerData.playerCoords[1],
            playerData.playerCoords[2]
        );

        PlayerInfo.Instance.playerPosition = loadedPosition;

        Vector3 loadedRotation = new Vector3(
            playerData.playerCoords[3],
            playerData.playerCoords[4],
            playerData.playerCoords[5]
        );

        PlayerInfo.Instance.playerRotation = loadedRotation;
    }

    public void StartLoadedGame()
    {
        StartCoroutine(DelayedLoad());

        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator DelayedLoad()
    {
        LoadGame();

        yield return new WaitForSeconds(1f);
    }

    #endregion

    #endregion

    #region <-- JSON Section -->

    private void SaveGameDataToJsonFile(AllGameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        using (StreamWriter writer = new StreamWriter(jsonPathProject))
        {
            writer.Write(json);
            print("Saved Game to Json file at " + jsonPathProject);
        }
    }

    private AllGameData LoadGameDatafromJsonFile()
    {
        using (StreamReader reader = new StreamReader(jsonPathProject))
        {
            string json = reader.ReadToEnd();
            print("Loaded Game from Json file at " + jsonPathProject);

            AllGameData gameData = JsonUtility.FromJson<AllGameData>(json);
            return gameData;
        }
    }

    #endregion

    #region <-- Settings Section -->

    #region <-- Volume Settings -->
    public class VolumeSettings
    {
        public float master;
        public float music;
        public float effects;
    }

    public void SaveVolumeSettings(float _master, float _music, float _effects)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            master = _master,
            music = _music,
            effects = _effects
        };

        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }
    #endregion

    #endregion

    public void DeleteSaveAndCreateNewSave()
    {
        if (File.Exists(jsonPathProject))
        {
            File.Delete(jsonPathProject);
            Debug.Log("Save file deleted from project path: " + jsonPathProject);
        }

        if (File.Exists(jsonPathPersistent))
        {
            File.Delete(jsonPathPersistent);
            Debug.Log("Save file deleted from persistent path: " + jsonPathPersistent);
        }

        isFirstTime = true;
        SceneManager.LoadScene("GameScene");
        Debug.Log("New save file created.");
    }
}