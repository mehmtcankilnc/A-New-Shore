using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int[] playerStats;

    public float[] playerCoords;

    //public string[] inventoryContent;

    public PlayerData(int[] _playerStats, float[] _playerCoords)
    {
        playerStats = _playerStats;
        playerCoords = _playerCoords;
    }
}