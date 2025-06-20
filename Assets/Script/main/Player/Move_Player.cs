using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// �v���C���[����F������ + ���s�v�n���h���Ή���
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Move_Player : MonoBehaviour
{
    /*===== ��{�ړ� =====*/
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    /*===== F�L�[ =====*/
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;

    /*===== �ڒn���� =====*/
    public Transform groundCheck;
    public float groundCheckRadius = .08f;
    public LayerMask groundMask;

    /*===== ���Z�b�g =====*/
    public HashSet<string> keyRing = new HashSet<string>();

    /*===== UI (�C��) =====*/
    public Image hookIcon;
    public Sprite iconGot, iconNot;

    /*===== ������� =====*/
    Rigidbody2D rb;
    bool isGrounded, isSliding;
    HandleController currentHandle;
    bool isPressingF; float fTime; bool slideTrig;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!groundCheck) groundCheck = transform.Find("groundCheck");
        if (hookIcon && iconNot) hookIcon.sprite = iconNot;
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

    /*---------------- Trigger ���� ----------------*/
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
                if (hookIcon && iconGot) hookIcon.sprite = iconGot;
                Debug.Log($"�� [{it.keyId}] ���擾");
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Handle") &&
            other.GetComponent<HandleController>() == currentHandle)
            currentHandle = null;
    }
}
