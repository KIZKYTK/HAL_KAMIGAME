using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Move_Player : MonoBehaviour
{
    /* ===== 基本移動パラメータ ===== */
    [Header("移動速度")] public float moveSpeed = 5f;
    [Header("ジャンプ初速度")] public float jumpForce = 12f;

    /* ===== Fキー設定 ===== */
    [Header("Fキー設定")]
    public KeyCode interactKey = KeyCode.F;   // 取っ手操作キー
    public float holdThreshold = 0.25f;       // 長押し判定時間

    /* ===== 接地判定 ===== */
    [Header("接地判定")]
    public Transform groundCheck;                 // 足元 Empty
    public float groundCheckRadius = 0.08f;
    public LayerMask groundMask;

    /* ===== 鍵ID管理 ===== */
    [Header("現在所持している keyId")]
    public string currentItemKey = "";          // 空文字＝未所持

    /* ===== UI（任意） ===== */
    public Image hookIcon;                       // 鍵アイコン Image
    public Sprite iconGot;                        // 鍵取得後に表示するスプライト
    public Sprite iconNot;                        // 未取得時のスプライト

    /* ===== 内部状態 ===== */
    Rigidbody2D rb;
    bool isGrounded;
    bool isSliding;

    HandleController currentHandle;               // いま触れているハンドル

    bool isPressingF;
    float fTime;
    bool slideTrig;

    /* ---------------- 初期化 ---------------- */
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null) groundCheck = transform.Find("groundCheck");

        // UI 初期表示（未取得アイコン）
        if (hookIcon && iconNot) hookIcon.sprite = iconNot;
    }

    /* ---------------- 毎フレーム処理 ---------------- */
    void Update()
    {
        /* --- 接地判定 --- */
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        /* --- 移動 & ジャンプ --- */
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
            rb.velocity = Vector2.zero;   // スライド中は自操作ロック
        }

        /* --- Fキー押下開始 --- */
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true;
            fTime = 0f;
            slideTrig = false;
        }

        /* --- Fキー押下中（長押し判定） --- */
        if (isPressingF)
        {
            fTime += Time.deltaTime;

            if (!slideTrig &&
                fTime >= holdThreshold &&          // 長押し時間到達
                currentHandle && !currentHandle.isMoving &&
                currentItemKey == currentHandle.requiredKeyId) // 鍵一致
            {
                slideTrig = true;
                isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject); // 付着スライド開始
            }
        }

        /* --- Fキー離す（短押し判定） --- */
        if (Input.GetKeyUp(interactKey))
        {
            if (!slideTrig &&                     // 長押し扱いになっていない
                fTime < holdThreshold &&          // 短押し時間
                currentHandle && !currentHandle.isMoving &&
                currentItemKey == currentHandle.requiredKeyId)   // 鍵一致
            {
                currentHandle.StartMoveAlone();   // 取っ手のみ移動
            }

            isPressingF = false;
            fTime = 0f;
        }

        /* --- スライド終了検知 --- */
        if (isSliding && currentHandle && !currentHandle.isMoving)
            isSliding = false;
    }

    /* ---------------- トリガー判定 ---------------- */
    void OnTriggerEnter2D(Collider2D other)
    {
        /* ハンドルに入った */
        if (other.CompareTag("Handle"))
        {
            currentHandle = other.GetComponent<HandleController>();
            return;
        }

        /* アイテムに入った（鍵取得） */
        if (other.CompareTag("Item"))
        {
            ItemEntity it = other.GetComponent<ItemEntity>();
            if (it)
            {
                currentItemKey = it.keyId;                    // 鍵ID 保存
                if (hookIcon && iconGot) hookIcon.sprite = iconGot;
                Destroy(other.gameObject);                    // アイテム消去
                Debug.Log($"鍵 [{currentItemKey}] を取得");
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
