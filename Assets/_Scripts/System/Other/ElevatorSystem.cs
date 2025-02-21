using System.Collections;
using UnityEngine;

public class ElevatorSystem : MonoBehaviour
{
    #region Singleton
    public static ElevatorSystem Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public GameObject wheel;
    public GameObject elevator;
    public GameObject[] chains;
    public float moveSpeed = 1f;
    public float rotateSpeed = 360f;
    public float waitTime = 3f;
    public Vector3 upperPosition;
    public Vector3 lowerPosition;

    private bool isMoving = false;

    private void Start()
    {
        elevator.transform.position = lowerPosition;
    }

    public void StartElevator()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveElevator(upperPosition, lowerPosition));
        }
    }

    private IEnumerator MoveElevator(Vector3 targetPosition, Vector3 returnPosition)
    {
        isMoving = true;

        yield return StartCoroutine(MoveToPosition(elevator, targetPosition, true));

        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(MoveToPosition(elevator, returnPosition, false));

        isMoving = false;
    }

    private IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition, bool deactivateChains)
    {
        float distance = Vector3.Distance(obj.transform.position, targetPosition);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        Vector3 startPosition = obj.transform.position;
        int chainIndex = deactivateChains ? 0 : chains.Length - 1;
        int chainDirection = deactivateChains ? 1 : -1;

        float chainInterval = duration / chains.Length;
        float nextChainTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            wheel.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

            if (elapsedTime >= nextChainTime && chainIndex >= 0 && chainIndex < chains.Length)
            {
                chains[chainIndex].SetActive(!deactivateChains);
                chainIndex += chainDirection;
                nextChainTime += chainInterval;
            }

            yield return null;
        }

        obj.transform.position = targetPosition;
    }
}
