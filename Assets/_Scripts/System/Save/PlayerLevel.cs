using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    #region Singleton
    public static PlayerLevel Instance {  get;set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void Start()
    {
        OrderUIManager.Instance.SetLevelText(PlayerInfo.Instance.playerLevel);
        OrderUIManager.Instance.SetLevelSlider(PlayerInfo.Instance.playerExperiencePerLevel);
    }

    public void CompleteOrder(int experienceGained)
    {
        PlayerInfo.Instance.playerExperience += experienceGained;
        OrderUIManager.Instance.IncreaseExperience(experienceGained);
        CheckLevelUp();
    }

    public void CheckLevelUp()
    {
        while (PlayerInfo.Instance.playerExperience >= PlayerInfo.Instance.playerExperiencePerLevel)
        {
            PlayerInfo.Instance.playerExperience -= PlayerInfo.Instance.playerExperiencePerLevel;
            PlayerInfo.Instance.playerLevel++;
            OrderUIManager.Instance.ResetExperience();
            OrderUIManager.Instance.SetLevelText(PlayerInfo.Instance.playerLevel);
            Debug.Log("Level atlad�n! Yeni level: " + PlayerInfo.Instance.playerLevel);

            // �ste�e ba�l�: Her levelda gerekli puan� artt�rma
            PlayerInfo.Instance.playerExperiencePerLevel += 10;
        }
    }
}