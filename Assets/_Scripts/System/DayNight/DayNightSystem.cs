using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    #region Singleton
    public static DayNightSystem Instance { get; set; }
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

    public TextMeshProUGUI timeUI;
    public Light directionalLight;
    public float dayDurationInSeconds = 24.0f;
    public int currentHour;
    public List<SkyboxTimeMapping> timeMappings;
    public bool dayCompleted = false;
    public float currentTimeOfDay = 0.35f;

    private float blendedValue = 0.0f;

    void Update()
    {
        if (!dayCompleted)
        {
            currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
            currentTimeOfDay %= 1;
        }

        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24);
        if (currentHour > 22)
        {
            currentHour = 22;
            dayCompleted = true;
        }

        timeUI.text = $"{currentHour}:00";

        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay * 360) - 90, 95, 175));

        UpdateSkybox();
    }

    private void UpdateSkybox()
    {
        Material currentSkybox = null;
        foreach (SkyboxTimeMapping mapping in timeMappings)
        {
            if (currentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader)
                {
                    if (currentSkybox.shader.name == "Custom/SkyboxTransition")
                    {
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);

                        currentSkybox.SetFloat("_TransitionFactor", blendedValue);
                    }
                    else
                    {
                        blendedValue = 0.0f;
                    }
                }

                break;
            }
        }

        if (currentSkybox)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }
}

[System.Serializable]
public class SkyboxTimeMapping
{
    public string phaseName;
    public int hour;
    public Material skyboxMaterial;
}