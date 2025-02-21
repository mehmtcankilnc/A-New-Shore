using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SaveManager;

public class EscMenuManager : MonoBehaviour
{
    #region Singleton
    public static EscMenuManager Instance { get; set; }
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

    public Canvas mainCanvas;
    public Canvas escMenuCanvas;
    public AudioSource buttonClick;
    public AudioSource bgMusic;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public bool isActive = false;

    private void Start()
    {
        StartCoroutine(LoadAndApplySettings());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscMenu();
        }
    }

    private void HandleEscMenu()
    {
        if (!isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            mainCanvas.gameObject.SetActive(false);
            escMenuCanvas.gameObject.SetActive(true);
            isActive = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            escMenuCanvas.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
            isActive = false;
        }
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        escMenuCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
        isActive = false;
    }

    public void TempSaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void PlayClick()
    {
        buttonClick.Play();
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

        Debug.Log(masterSlider.value + "/" + musicSlider.value + "/" + effectsSlider.value);

        buttonClick.volume = masterSlider.value != 0 ? Mathf.Clamp01(effectsSlider.value) : 0;
        bgMusic.volume = masterSlider.value != 0 ? Mathf.Clamp01(musicSlider.value) : 0;
    }

    public void SaveMusicVolume()
    {
        SaveManager.Instance.SaveVolumeSettings(masterSlider.value, musicSlider.value, effectsSlider.value);
        StartCoroutine(LoadAndApplySettings());
    }
}
