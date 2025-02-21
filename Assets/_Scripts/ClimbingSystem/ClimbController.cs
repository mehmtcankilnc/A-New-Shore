using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    private ClimbPoint currentPoint;
    private PlayerController playerController;
    private EnvironmentScanner environmentScanner;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    void Update()
    {
        if (!playerController.IsHanging)
        {
            if (Input.GetButton("Jump") && !playerController.InAction)
            {
                if (environmentScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("IdleToBracedHang", currentPoint.transform, 0.41f, 0.54f));
                }
            }

            if (Input.GetButton("Drop") && !playerController.InAction)
            {
                if (environmentScanner.DropLedgeCheck(out RaycastHit ledgeHit))
                {
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("DropToHang", currentPoint.transform, 0.20f, 0.45f, handOffset: new Vector3(0.3f, 0.15f, -0.15f)));
                }
            }
        }
        else
        {
            //Jump From Hang
            if (Input.GetButton("Drop") && !playerController.InAction)
            {
                StartCoroutine(JumpFromHang());
                return;
            }
            float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
            var inputDir = new Vector2(h, v);

            if (playerController.InAction || inputDir == Vector2.zero) return;
            //Climb From Hang
            if (currentPoint.MountPoint && inputDir.y == 1 && Input.GetButton("Jump"))
            {
                StartCoroutine(ClimbFromHang());
                return;
            }
            //Ledge To Ledge Jump
            var neighbour = currentPoint.GetNeighbour(inputDir);
            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge("HangHopUp", currentPoint.transform, 0.35f, 0.65f));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge("HangDropHang", currentPoint.transform, 0.31f, 0.65f));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f));
            }
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neighbour.climbPoint;

                if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("ShimmyRight", currentPoint.transform, 0f, 0.38f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("ShimmyLeft", currentPoint.transform, 0f, 0.38f, AvatarTarget.LeftHand));
            }
        }
    }

    private IEnumerator JumpToLedge(string animName, Transform ledge, float matchStartTime, float matchTargetTime, AvatarTarget hand=AvatarTarget.RightHand, Vector3? handOffset=null)
    {
        var matchParams = new MatchTargetParams()
        {
            pos = GetHandPos(ledge, hand, handOffset),
            bodyPart = hand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one
        };

        var targetRotation = Quaternion.LookRotation(-ledge.forward);

        yield return playerController.DoAction(animName, matchParams, targetRotation, true);

        playerController.IsHanging = true;
    }

    private Vector3 GetHandPos(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        var offsetValue = handOffset != null ? handOffset.Value : new Vector3(0.1f, 0.1f, -0.2f);

        var hDir = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
        return ledge.position - Vector3.forward * offsetValue.z - Vector3.up * offsetValue.y - hDir * offsetValue.x;
    }

    private IEnumerator JumpFromHang()
    {
        playerController.IsHanging = false;
        yield return playerController.DoAction("JumpFromHang");

        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    private IEnumerator ClimbFromHang()
    {
        playerController.IsHanging = false;
        yield return playerController.DoAction("ClimbFromHang");

        playerController.EnableCharacterController(true);

        yield return new WaitForSeconds(0.5f);

        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    private ClimbPoint GetNearestClimbPoint(Transform ledge, Vector3 hitPoint)
    {
        var points = ledge.GetComponentsInChildren<ClimbPoint>();

        ClimbPoint nearestPoint = null;
        float nearestPointDistance = Mathf.Infinity;

        foreach (var point in points)
        {
            float distance = Vector3.Distance(point.transform.position, hitPoint);

            if (distance < nearestPointDistance)
            {
                nearestPoint = point;
                nearestPointDistance = distance;
            }
        }

        return nearestPoint;
    }
}
