using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SaveManager;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string mainSceneName;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadGameButton;
    [SerializeField] private GameObject alertBox;
    [SerializeField] private GameObject alertText;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    private CanvasGroup loadGameButtonCanvasGroup;

    private void Start()
    {
        loadGameButtonCanvasGroup = loadGameButton.GetComponent<CanvasGroup>();
        StartCoroutine(LoadAndApplySettings());
    }

    private void Update()
    {
        if (SaveManager.Instance.isFirstTime)
        {
            loadGameButtonCanvasGroup.alpha = 0.5f;
            loadGameButtonCanvasGroup.interactable = false;
            loadGameButtonCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            loadGameButtonCanvasGroup.alpha = 1f;
            loadGameButtonCanvasGroup.interactable = true;
            loadGameButtonCanvasGroup.blocksRaycasts = true;
        }
    }

    public void NewGameButton()
    {
        if (SaveManager.Instance.isFirstTime == false)
        {
            mainMenu.SetActive(false);
            alertBox.SetActive(true);
            alertText.GetComponent<TextMeshProUGUI>().text = "A saved game already exists. Starting a new game will overwrite your current progress. Are you sure you want to continue?";
        }
        else
        {
            SceneManager.LoadScene(mainSceneName);
        }
    }

    public void LoadGame()
    {
        SaveManager.Instance.StartLoadedGame();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ContinueButton()
    {
        PlayerInfo.Instance.ResetInfo();
        SaveManager.Instance.DeleteSaveAndCreateNewSave();
    }

    public void PlayClick()
    {
        buttonClick.Play();
    }

    public void SaveMusicVolume()
    {
        SaveManager.Instance.SaveVolumeSettings(masterSlider.value, musicSlider.value, effectsSlider.value);
        StartCoroutine(LoadAndApplySettings());
    }
    private IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();

        yield return new WaitForSeconds(0.1f);
    }

    private void LoadAndSetVolume()
    {
        VolumeSettings volumeSettings = SaveManager.Instance.LoadVolumeSettings();

        masterSlider.value = volumeSettings.master;
        musicSlider.value = volumeSettings.music;
        effectsSlider.value = volumeSettings.effects;

        buttonClick.volume = masterSlider.value != 0 ? Mathf.Clamp01(effectsSlider.value) : 0;
        bgMusic.volume = masterSlider.value != 0 ? Mathf.Clamp01(musicSlider.value) : 0;
    }
}
