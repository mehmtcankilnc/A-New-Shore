using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    public float moveSpeed = 0.2f;
    public float walkCounter;
    public float waitCounter;
    public bool isWalking;

    [SerializeField] private float obstacleCheckDistance = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Animator animator;
    private Vector3 stopPosition;
    private float walkTime;
    private float waitTime;
    private int walkDirection;

    void Start()
    {
        animator = GetComponent<Animator>();

        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();
    }

    void Update()
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true);

            walkCounter -= Time.deltaTime;

            Vector3 moveDirection = Vector3.zero;
            switch (walkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    moveDirection = transform.forward;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    moveDirection = transform.forward;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    moveDirection = transform.forward;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    moveDirection = transform.forward;
                    break;
            }

            if (!IsObstacleInFront(moveDirection))
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            else
                ChooseDirection();

            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;

                transform.position = stopPosition;
                animator.SetBool("isRunning", false);

                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
                ChooseDirection();
        }
    }

    public void ChooseDirection()
    {
        walkDirection = Random.Range(0, 4);

        isWalking = true;
        walkCounter = walkTime;
    }

    private bool IsObstacleInFront(Vector3 direction)
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, obstacleCheckDistance, obstacleLayer);
    }
}
