using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    #region Singleton
    public static PlayerInfo Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    //PlayerStats
    public int playerLevel;
    public int playerExperience;
    public int playerExperiencePerLevel;
    public int totalCoin;
    public int dayInGame;

    //PlayerCoords
    public Vector3 playerPosition;
    public Vector3 playerRotation;

    public void ResetInfo()
    {
        playerLevel = 1;
        playerExperience = 0;
        playerExperiencePerLevel = 100;
        totalCoin = 0;
        dayInGame = 1;

        playerPosition = new Vector3(-40f, -2.5f, -35f);
        playerRotation = new Vector3(0, -11f, 0);
    }
}
