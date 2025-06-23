using System.Collections.Generic;
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

    private bool isPressingF; float fTime; bool slideTrig;
    private float fPressedTime = 0f;
    private bool slideTriggered = false;


    /*===== 鍵セット =====*/
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
        /*-- 接地判定 --*/
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        /*-- 移動 & ジャンプ --*/
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else rb.velocity = Vector2.zero;

        /*-- Fキー押下開始 --*/
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true; fTime = 0; slideTrig = false;
        }

        /*-- 押下中（長押し判定） --*/
        if (isPressingF)
        {
            fTime += Time.deltaTime;
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||          // 鍵不要
                          keyRing.Contains(currentHandle.requiredKeyId)); // 鍵一致

            if (!slideTrig &&
                fTime >= holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                slideTrig = true; isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject);
            }
        }

        /*-- Fキー離す（短押し判定） --*/
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

        /*-- スライド終了 --*/
        if (isSliding && currentHandle && !currentHandle.isMoving)
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
