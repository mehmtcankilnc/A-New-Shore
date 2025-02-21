using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushController : MonoBehaviour
{
    private EnvironmentScanner environmentScanner;
    private PlayerController playerController;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        var moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        if (moveAmount != 0 && environmentScanner.PushObstacleCheck(out RaycastHit pushHit) && !playerController.InAction)
        {
            StartCoroutine(playerController.PushObstacle(pushHit));
        }
        else
            playerController.SetIsPushing(false);
    }
}
