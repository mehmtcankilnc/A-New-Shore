using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetInt("hasPlayedBefore", 0) == 0)
        {
            PlayerPrefs.SetInt("hasPlayedBefore", 1);
            PlayerPrefs.Save();
            LoadCinematicScene();
        }
        else
            SceneManager.LoadScene("MainMenu");
    }

    private void LoadCinematicScene()
    {
        SceneManager.LoadScene("MainStory");
    }
}