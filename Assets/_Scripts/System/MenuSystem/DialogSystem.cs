using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    #region Singleton
    public static DialogSystem Instance { get; set; }
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

    public Canvas dialogUI;
    public TextMeshProUGUI dialogText;
    public Button optionBtn;
    public AudioSource buttonClick;

    public bool dialogUIActive = false;

    public void OpenDialogUI()
    {
        dialogUI.gameObject.SetActive(true);
        dialogUIActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseDialogUI()
    {
        dialogUI.gameObject.SetActive(false);
        dialogUIActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void PlayClick()
    {
        buttonClick.Play();
    }
}