using System.Collections.Generic;
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

    private bool isPressingF; float fTime; bool slideTrig;
    private float fPressedTime = 0f;
    private bool slideTriggered = false;


    /*===== ���Z�b�g =====*/
    public HashSet<string> keyRing = new HashSet<string>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (groundCheck == null)
            groundCheck = transform.Find("groundCheck");
    }

    void Update()
    {
        /*-- �ڒn���� --*/
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        /*-- �ړ� & �W�����v --*/
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else rb.velocity = Vector2.zero;

        /*-- F�L�[�����J�n --*/
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true; fTime = 0; slideTrig = false;
        }

        /*-- �������i����������j --*/
        if (isPressingF)
        {
            fTime += Time.deltaTime;
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||          // ���s�v
                          keyRing.Contains(currentHandle.requiredKeyId)); // ����v

            if (!slideTrig &&
                fTime >= holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                slideTrig = true; isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject);
            }
        }

        /*-- F�L�[�����i�Z��������j --*/
        if (Input.GetKeyUp(interactKey))
        {
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||
                          keyRing.Contains(currentHandle.requiredKeyId));

            if (!slideTrig &&
                fTime < holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                currentHandle.StartMoveAlone();
            }

            isPressingF = false; fTime = 0;
        }

        /*-- �X���C�h�I�� --*/
        if (isSliding && currentHandle && !currentHandle.isMoving)
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
