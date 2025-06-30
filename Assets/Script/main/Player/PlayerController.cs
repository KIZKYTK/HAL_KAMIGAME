using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー操作：移動・ジャンプ・取っ手操作
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /*====== 1. 移動・ジャンプ ======*/
    [Header("移動速度")] public float moveSpeed = 5f;
    [Header("ジャンプ速度")] public float jumpForce = 12f;

    /*====== 2. Fキー ======*/
    [Header("Fキー設定")]
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;            // 長押し判定秒

    /*====== 3. 接地判定 ======*/
    [Header("接地判定")]
    public Transform groundCheck;
    public float groundCheckRadius = .08f;
    public LayerMask groundMask;

    /*====== 4. 鍵システム ======*/
    [Header("所持鍵セット")]
    public HashSet<string> keyRing = new();
    public static event Action<string> OnKeyCollected;      // 取っ手側が購読

    /*====== 5. SE =========*/
    PlayerSE playerSE;

    /// <summary>外部から鍵取得を通知するラッパー</summary>
    public static void BroadcastKeyCollected(string keyId)
    {
        OnKeyCollected?.Invoke(keyId);
    }

    /*====== 5. リスポーン ======*/
    [Header("初期リスポーン位置(空=現在)")]
    public Vector2 initialRespawnPos;
    [HideInInspector] public Vector2 currentRespawnPos;
    [Header("死亡→復活遅延秒")] public float respawnDelay = 1f;
    [Header("フェード時間(0で無効)")] public float fadeTime = 0.3f;

    /*====== 6. 一発リセット ======*/
    [Header("リセットキー")]
    public KeyCode resetKey = KeyCode.R;
    public bool resetWithFade = false;

    /*====== 7. 内部状態 ======*/
    Rigidbody2D rb;
    bool isGrounded, isSliding;

    HandleController currentHandle;
    bool isPressingF; float fTime; bool slideTrig;

    /*--------------------------------------------------*/
    /* 8. 初期化 */
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
    /* 9. 毎フレーム */
    void Update()
    {
        /*--- Rキー：リセット ---*/
        if (Input.GetKeyDown(resetKey))
        {
            //if (resetWithFade) Die();
            // else InstantRespawn();
        }

        /*--- 接地判定 ---*/
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundMask);

        /*--- 移動・ジャンプ ---*/
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else rb.velocity = Vector2.zero;

        /*--- Fキー押下 ---*/
        if (Input.GetKeyDown(interactKey))
        { isPressingF = true; fTime = 0; slideTrig = false; }

        if (isPressingF)
        {
            fTime += Time.deltaTime;

            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||
                          keyRing.Contains(currentHandle.requiredKeyId));

            /* 長押し：プレイヤー付着 */
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

            /* 短押し：取っ手のみ移動 */
            if (!slideTrig &&
                fTime < holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
                currentHandle.StartMoveAlone();

            isPressingF = false; fTime = 0;
        }

        /*--- スライド終了検知 ---*/
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
    /* 10. Trigger 判定 */
    void OnTriggerEnter2D(Collider2D other)
    {
        /* 取っ手 */
        if (other.CompareTag("Handle"))
        {
            currentHandle = other.GetComponent<HandleController>();
            return;
        }

        /* 鍵アイテム */
        if (other.CompareTag("Item"))
        {
            ItemEntity it = other.GetComponent<ItemEntity>();
            if (it && keyRing.Add(it.keyId))
            {
                BroadcastKeyCollected(it.keyId);      // イベント発火
                Debug.Log($"[Player] 鍵 {it.keyId} 取得");
            }
            Destroy(other.gameObject);
        }

        /* 死亡エリア */
        // if (other.CompareTag("Death")) Die();

        /* チェックポイント */
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
