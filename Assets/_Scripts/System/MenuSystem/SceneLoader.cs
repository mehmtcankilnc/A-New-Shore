using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStory : MonoBehaviour
{
    [SerializeField] private string sceneNameToLoad;

    private void OnEnable()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}