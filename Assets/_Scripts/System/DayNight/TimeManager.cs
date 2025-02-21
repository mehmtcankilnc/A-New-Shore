using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Singleton
    public static TimeManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    public TextMeshProUGUI dayUI;

    private void Start()
    {
        dayUI.text = $"Day: {PlayerInfo.Instance.dayInGame}";
    }

    public void TriggerNextDay()
    {
        PlayerInfo.Instance.dayInGame++;
        dayUI.text = $"Day: {PlayerInfo.Instance.dayInGame}";
    }
}