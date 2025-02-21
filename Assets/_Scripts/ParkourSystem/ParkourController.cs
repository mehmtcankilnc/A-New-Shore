using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    private EnvironmentScanner environmentScanner;
    private Animator animator;
    private PlayerController playerController;

    [SerializeField] private List<ParkourAction> parkourActions;
    [SerializeField] private ParkourAction jumpDownAction;
    [SerializeField] private float autoDropHeightLimit = 1.0f;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();
        if (Input.GetButton("Jump") && !playerController.InAction && !playerController.IsHanging && playerController.IsGrounded)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }

        if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound )
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
                shouldJump = false;

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }
    }

    private IEnumerator DoParkourAction(ParkourAction action)
    {
        playerController.SetControl(false);

        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams()
            {
                pos = action.MatchPos,
                posWeight = action.MatchPosWeight,
                bodyPart = action.MatchBodyPart,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime,
            };
        }

        yield return playerController.DoAction(action.AnimName, matchParams, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.Mirror);

        playerController.SetControl(true);
    }
}