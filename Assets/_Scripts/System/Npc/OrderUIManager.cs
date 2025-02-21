using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderUIManager : MonoBehaviour
{
    #region Singleton
    public static OrderUIManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public GameObject orderUI;
    public List<TextMeshProUGUI> orderTexts = new List<TextMeshProUGUI>();
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI priceQuantityText;
    public Slider levelSlider;
    public TextMeshProUGUI levelText;

    private bool isActivated = false;

    private void Start()
    {
        levelSlider.value = PlayerInfo.Instance.playerExperience;
        levelSlider.maxValue = PlayerInfo.Instance.playerExperiencePerLevel;
        coinText.text = PlayerInfo.Instance.totalCoin.ToString();
    }

    public void ActivateOrderUI(List<string> orderNameList, List<int> orderQuantityList, int totalPrice)
    {
        if (!isActivated)
        {
            isActivated = true;
            orderUI.SetActive(true);

            for (int i = 0; i < orderTexts.Count; i++)
            {
                if (i < orderNameList.Count)
                {
                    orderTexts[i].text = orderNameList[i];
                    orderTexts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = orderQuantityList[i].ToString();
                    orderTexts[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "pieces";
                    priceQuantityText.text = totalPrice.ToString() + " $";
                }
                else
                {
                    orderTexts[i].text = "";
                    orderTexts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    orderTexts[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }
    }

    public void ResetOrder()
    {
        for (int i = 0; i < orderTexts.Count; i++) 
        {
            orderTexts[i].text = "";
        }

        orderUI.SetActive(false);
        isActivated = false;
    }

    public void IncreaseCoin(int num)
    {
        PlayerInfo.Instance.totalCoin += num;
        coinText.text = PlayerInfo.Instance.totalCoin.ToString() + " $";
    }

    public void IncreaseExperience(int exp)
    {
        levelSlider.value += exp;

        if (levelSlider.value >= levelSlider.maxValue) 
        {
            PlayerInfo.Instance.playerLevel++;
            levelSlider.value = 0;
            SetLevelSlider(PlayerInfo.Instance.playerExperiencePerLevel);
            SetLevelText(PlayerInfo.Instance.playerLevel);
        }
    }
    
    public void ResetExperience()
    {
        levelSlider.value = 0;
    }

    public void SetLevelText(int level)
    {
        levelText.text = $"Level: {level}";
    }

    public void SetLevelSlider(int expLevel)
    {
        levelSlider.maxValue = expLevel;
    }
}