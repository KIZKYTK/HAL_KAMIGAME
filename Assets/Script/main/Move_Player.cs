using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Move_Player : MonoBehaviour
{
    /* ===== ��{�ړ��p�����[�^ ===== */
    [Header("�ړ����x")] public float moveSpeed = 5f;
    [Header("�W�����v�����x")] public float jumpForce = 12f;

    /* ===== F�L�[�ݒ� ===== */
    [Header("F�L�[�ݒ�")]
    public KeyCode interactKey = KeyCode.F;   // ����葀��L�[
    public float holdThreshold = 0.25f;       // ���������莞��

    /* ===== �ڒn���� ===== */
    [Header("�ڒn����")]
    public Transform groundCheck;                 // ���� Empty
    public float groundCheckRadius = 0.08f;
    public LayerMask groundMask;

    /* ===== ��ID�Ǘ� ===== */
    [Header("���ݏ������Ă��� keyId")]
    public string currentItemKey = "";          // �󕶎���������

    /* ===== UI�i�C�Ӂj ===== */
    public Image hookIcon;                       // ���A�C�R�� Image
    public Sprite iconGot;                        // ���擾��ɕ\������X�v���C�g
    public Sprite iconNot;                        // ���擾���̃X�v���C�g

    /* ===== ������� ===== */
    Rigidbody2D rb;
    bool isGrounded;
    bool isSliding;

    HandleController currentHandle;               // ���ܐG��Ă���n���h��

    bool isPressingF;
    float fTime;
    bool slideTrig;

    /* ---------------- ������ ---------------- */
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null) groundCheck = transform.Find("groundCheck");

        // UI �����\���i���擾�A�C�R���j
        if (hookIcon && iconNot) hookIcon.sprite = iconNot;
    }

    /* ---------------- ���t���[������ ---------------- */
    void Update()
    {
        /* --- �ڒn���� --- */
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        /* --- �ړ� & �W�����v --- */
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            rb.velocity = Vector2.zero;   // �X���C�h���͎����샍�b�N
        }

        /* --- F�L�[�����J�n --- */
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true;
            fTime = 0f;
            slideTrig = false;
        }

        /* --- F�L�[�������i����������j --- */
        if (isPressingF)
        {
            fTime += Time.deltaTime;

            if (!slideTrig &&
                fTime >= holdThreshold &&          // ���������ԓ��B
                currentHandle && !currentHandle.isMoving &&
                currentItemKey == currentHandle.requiredKeyId) // ����v
            {
                slideTrig = true;
                isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject); // �t���X���C�h�J�n
            }
        }

        /* --- F�L�[�����i�Z��������j --- */
        if (Input.GetKeyUp(interactKey))
        {
            if (!slideTrig &&                     // �����������ɂȂ��Ă��Ȃ�
                fTime < holdThreshold &&          // �Z��������
                currentHandle && !currentHandle.isMoving &&
                currentItemKey == currentHandle.requiredKeyId)   // ����v
            {
                currentHandle.StartMoveAlone();   // �����݈̂ړ�
            }

            isPressingF = false;
            fTime = 0f;
        }

        /* --- �X���C�h�I�����m --- */
        if (isSliding && currentHandle && !currentHandle.isMoving)
            isSliding = false;
    }

    /* ---------------- �g���K�[���� ---------------- */
    void OnTriggerEnter2D(Collider2D other)
    {
        /* �n���h���ɓ����� */
        if (other.CompareTag("Handle"))
        {
            currentHandle = other.GetComponent<HandleController>();
            return;
        }

        /* �A�C�e���ɓ������i���擾�j */
        if (other.CompareTag("Item"))
        {
            ItemEntity it = other.GetComponent<ItemEntity>();
            if (it)
            {
                currentItemKey = it.keyId;                    // ��ID �ۑ�
                if (hookIcon && iconGot) hookIcon.sprite = iconGot;
                Destroy(other.gameObject);                    // �A�C�e������
                Debug.Log($"�� [{currentItemKey}] ���擾");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Handle") &&
            other.GetComponent<HandleController>() == currentHandle)
        {
            currentHandle = null;
        }
    }
}
