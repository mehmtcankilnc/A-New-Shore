using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
        }
    }
    #endregion

    private Quaternion targetRotation;
    private Animator animator;
    private CharacterController characterController;
    private bool isGrounded;
    private bool hasControl = true;
    private float ySpeed;
    private float speed;
    private Vector3 velocity;
    //private float gravity;
    //private float walkingGravity = Physics.gravity.y * 2;
    //private float swimmingGravity = -0.5f;

    public bool IsOnLedge {  get; set; }
    public LedgeData LedgeData { get; set; }
    public bool InAction { get; set; }
    public bool IsHanging { get; set; }

    //public bool isSwimming = false;
    //public bool isUnderWater = false;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float pushStrength = 5.0f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    private float stepInterval = 0.5f; // Adým sesinin tekrar süresi (saniye cinsinden)
    private float nextStepTime = 0f;

    private void Start()
    {
        transform.SetPositionAndRotation(PlayerInfo.Instance.playerPosition, Quaternion.Euler(PlayerInfo.Instance.playerRotation));
        Debug.Log(PlayerInfo.Instance.playerRotation);
        Debug.Log(Quaternion.Euler(PlayerInfo.Instance.playerRotation));
    }

    private void Update()
    {
        if (!hasControl || IsHanging) return;
        
        if (DialogSystem.Instance.dialogUIActive == false && EscMenuManager.Instance.isActive == false)
        {
            Movement();
        }

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        var moveInput = transform.right * horizontal + transform.forward * vertical;

        //if (isSwimming)
        //{
        //    if (isUnderWater)
        //    {
        //        gravity = swimmingGravity;
        //    }
        //    else
        //    {
        //        velocity.y = 0;
        //    }
        //}
        //else
        //{
        //    gravity = walkingGravity;
        //}

        GroundCheck();
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            velocity = moveInput * speed;

            float moveAmount = velocity.magnitude / speed;

            if (Input.GetKey(KeyCode.LeftShift))
                moveAmount = Mathf.Clamp(moveAmount, 0.2f, 1f);
            else
                moveAmount = Mathf.Clamp(moveAmount, 0f, 0.2f);

            animator.SetFloat("moveAmount", moveAmount, 0.1f, Time.deltaTime);

            if ((horizontal != 0 || vertical != 0) && Time.time >= nextStepTime)
            {
                nextStepTime = Time.time + stepInterval / (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    SoundEffectsManager.Instance.PlaySoundEffect("running", true);
                }
                else
                {
                    SoundEffectsManager.Instance.PlaySoundEffect("walking", true);
                }
            }
        }

        characterController.Move(moveInput * speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            SoundEffectsManager.Instance.PlaySoundEffect("jumping");
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y * 2);
        }

        velocity.y += Physics.gravity.y * 2 * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    //private void LedgeMovement()
    //{
    //    float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, moveDir, Vector3.up);
    //    float angle = Mathf.Abs(signedAngle);

    //    if (Vector3.Angle(transform.forward, desiredMoveDir) >= 80)
    //    {
    //        velocity = Vector3.zero;
    //        return;
    //    }

    //    if (angle < 60)
    //    {
    //        velocity = Vector3.zero;
    //        moveDir = Vector3.zero;
    //    }
    //    else if (angle < 90)
    //    {
    //        var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
    //        var dir = left * Mathf.Sign(signedAngle);

    //        velocity = velocity.magnitude * dir;
    //        moveDir = dir;
    //    }
    //}

    public IEnumerator PushObstacle(RaycastHit pushHit)
    {
        SetIsPushing(true);

        yield return null;

        if (pushHit.transform.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 pushDirection = new(transform.forward.x, 0, transform.forward.z);
            Vector3 targetPosition = rb.position + pushStrength * Time.deltaTime * pushDirection;

            rb.MovePosition(targetPosition);
        }

        yield return new WaitForSeconds(0.4f);
    }

    public void SetIsPushing(bool value)
    {
        animator.SetBool("isPushing", value);
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams=null, Quaternion targetRotation=new Quaternion(), bool rotate=false, float postDelay = 0f, bool mirror=false)
    {
        InAction = true;

        animator.SetBool("mirrorAction", mirror);
        animator.CrossFadeInFixedTime(animName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(animName))
            Debug.LogError("The parkour animation is wrong");

        float rotateStartTime = matchParams != null ? matchParams.startTime : 0f;

        float timer = 0f;
        while (timer < animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTimer = timer / animState.length;

            if (rotate && normalizedTimer > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (matchParams != null)
                MatchTarget(matchParams);

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(postDelay);

        InAction = false;
    }

    private void MatchTarget(MatchTargetParams mp)
    {
        if (animator.IsInTransition(0) || animator.isMatchingTarget)
            return;

        animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart, new MatchTargetWeightMask(mp.posWeight, 0), mp.startTime, mp.targetTime);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    public void EnableCharacterController(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void ResetTargetRotation()
    {
        targetRotation = transform.rotation;
    }

    public bool HasControl {
        get => hasControl;
        set => hasControl = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
    public bool IsGrounded => isGrounded;
}

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}