using UnityEngine;

/// <summary>
/// プレイヤー操作：移動・ジャンプ・取っ手操作
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 5f;

    [Header("ジャンプ力")]
    public float jumpForce = 12f;

    [Header("Fキー設定")]
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;      // 長押し判定時間

    [Header("接地判定")]
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
        /*―― 接地判定 ――*/
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundMask);

        /*―― 水平移動 / ジャンプ ――*/
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
            rb.velocity = Vector2.zero;   // スライド中は固定
        }

        /*―― Fキー処理 ――*/
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true;
            fPressedTime = 0f;
            slideTriggered = false;
        }

        if (isPressingF)
        {
            fPressedTime += Time.deltaTime;

            // 長押しでプレイヤー付着スライド
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
            // 短押しで取っ手のみスライド
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

        /*―― スライド終了検知 ――*/
        if (isSliding && currentHandle != null && !currentHandle.isMoving)
            isSliding = false;
    }

    /*―― 取っ手のトリガー判定 ――*/
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
