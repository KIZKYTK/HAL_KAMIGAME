using UnityEngine;

/// <summary>
/// �v���C���[����F�ړ��E�W�����v�E����葀��
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("�ړ����x")]
    public float moveSpeed = 5f;

    [Header("�W�����v��")]
    public float jumpForce = 12f;

    [Header("F�L�[�ݒ�")]
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;      // ���������莞��

    [Header("�ڒn����")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.08f;
    public LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isSliding = false;

    private HandleController currentHandle;

    private bool isPressingF = false;
    private float fPressedTime = 0f;
    private bool slideTriggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (groundCheck == null)
            groundCheck = transform.Find("groundCheck");
    }

    void Update()
    {
        /*�\�\ �ڒn���� �\�\*/
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundMask);

        /*�\�\ �����ړ� / �W�����v �\�\*/
        if (!isSliding)
        {
            float h = 0;
            if (Input.GetKey(KeyCode.A)) h = -1;
            else if (Input.GetKey(KeyCode.D)) h = 1;

            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            rb.velocity = Vector2.zero;   // �X���C�h���͌Œ�
        }

        /*�\�\ F�L�[���� �\�\*/
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true;
            fPressedTime = 0f;
            slideTriggered = false;
        }

        if (isPressingF)
        {
            fPressedTime += Time.deltaTime;

            // �������Ńv���C���[�t���X���C�h
            if (!slideTriggered &&
                fPressedTime >= holdThreshold &&
                currentHandle != null &&
                !currentHandle.isMoving)
            {
                slideTriggered = true;
                isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject);
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            // �Z�����Ŏ����̂݃X���C�h
            if (!slideTriggered &&
                fPressedTime < holdThreshold &&
                currentHandle != null &&
                !currentHandle.isMoving)
            {
                currentHandle.StartMoveAlone();
            }

            isPressingF = false;
            fPressedTime = 0f;
        }

        /*�\�\ �X���C�h�I�����m �\�\*/
        if (isSliding && currentHandle != null && !currentHandle.isMoving)
            isSliding = false;
    }

    /*�\�\ �����̃g���K�[���� �\�\*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Handle"))
        {
            HandleController hc = other.GetComponent<HandleController>();
            if (hc != null) currentHandle = hc;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Handle"))
        {
            HandleController hc = other.GetComponent<HandleController>();
            if (hc != null && hc == currentHandle)
                currentHandle = null;
        }
    }
}
