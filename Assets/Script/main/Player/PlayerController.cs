using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[����F�ړ��E�W�����v�E����葀��
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /*====== 1. �ړ��E�W�����v ======*/
    [Header("�ړ����x")] public float moveSpeed = 5f;
    [Header("�W�����v���x")] public float jumpForce = 12f;

    /*====== 2. F�L�[ ======*/
    [Header("F�L�[�ݒ�")]
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;            // ����������b

    /*====== 3. �ڒn���� ======*/
    [Header("�ڒn����")]
    public Transform groundCheck;
    public float groundCheckRadius = .08f;
    public LayerMask groundMask;

    /*====== 4. ���V�X�e�� ======*/
    [Header("�������Z�b�g")]
    public HashSet<string> keyRing = new();
    public static event Action<string> OnKeyCollected;      // ����葤���w��

    /*====== 5. SE =========*/
    PlayerSE playerSE;

    /// <summary>�O�����献�擾��ʒm���郉�b�p�[</summary>
    public static void BroadcastKeyCollected(string keyId)
    {
        OnKeyCollected?.Invoke(keyId);
    }

    /*====== 5. ���X�|�[�� ======*/
    [Header("�������X�|�[���ʒu(��=����)")]
    public Vector2 initialRespawnPos;
    [HideInInspector] public Vector2 currentRespawnPos;
    [Header("���S�������x���b")] public float respawnDelay = 1f;
    [Header("�t�F�[�h����(0�Ŗ���)")] public float fadeTime = 0.3f;

    /*====== 6. �ꔭ���Z�b�g ======*/
    [Header("���Z�b�g�L�[")]
    public KeyCode resetKey = KeyCode.R;
    public bool resetWithFade = false;

    /*====== 7. ������� ======*/
    Rigidbody2D rb;
    bool isGrounded, isSliding;

    HandleController currentHandle;
    bool isPressingF; float fTime; bool slideTrig;

    /*--------------------------------------------------*/
    /* 8. ������ */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (!groundCheck) groundCheck = transform.Find("groundCheck");
        playerSE = GetComponent<PlayerSE>();
    }

    void Start()
    {
        currentRespawnPos = initialRespawnPos == Vector2.zero
            ? (Vector2)transform.position
            : initialRespawnPos;
    }

    /*--------------------------------------------------*/
    /* 9. ���t���[�� */
    void Update()
    {
        /*--- R�L�[�F���Z�b�g ---*/
        if (Input.GetKeyDown(resetKey))
        {
            //if (resetWithFade) Die();
            // else InstantRespawn();
        }

        /*--- �ڒn���� ---*/
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundMask);

        /*--- �ړ��E�W�����v ---*/
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else rb.velocity = Vector2.zero;

        /*--- F�L�[���� ---*/
        if (Input.GetKeyDown(interactKey))
        { isPressingF = true; fTime = 0; slideTrig = false; }

        if (isPressingF)
        {
            fTime += Time.deltaTime;

            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||
                          keyRing.Contains(currentHandle.requiredKeyId));

            /* �������F�v���C���[�t�� */
            if (!slideTrig &&
                fTime >= holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                slideTrig = true; isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject);
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||
                          keyRing.Contains(currentHandle.requiredKeyId));

            /* �Z�����F�����݈̂ړ� */
            if (!slideTrig &&
                fTime < holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
                currentHandle.StartMoveAlone();

            isPressingF = false; fTime = 0;
        }

        /*--- �X���C�h�I�����m ---*/
        if (isSliding && currentHandle && !currentHandle.isMoving)
            isSliding = false;

        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (h != 0)
                playerSE?.PlayMoveSE();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                playerSE?.PlayJumpSE();
            }
        }

    }

    /*--------------------------------------------------*/
    /* 10. Trigger ���� */
    void OnTriggerEnter2D(Collider2D other)
    {
        /* ����� */
        if (other.CompareTag("Handle"))
        {
            currentHandle = other.GetComponent<HandleController>();
            return;
        }

        /* ���A�C�e�� */
        if (other.CompareTag("Item"))
        {
            ItemEntity it = other.GetComponent<ItemEntity>();
            if (it && keyRing.Add(it.keyId))
            {
                BroadcastKeyCollected(it.keyId);      // �C�x���g����
                Debug.Log($"[Player] �� {it.keyId} �擾");
            }
            Destroy(other.gameObject);
        }

        /* ���S�G���A */
        // if (other.CompareTag("Death")) Die();

        /* �`�F�b�N�|�C���g */
        //  if (other.CompareTag("Respawn"))
        //    currentRespawnPos = other.transform.position;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Handle") &&
            other.GetComponent<HandleController>() == currentHandle)
            currentHandle = null;
    }

}
