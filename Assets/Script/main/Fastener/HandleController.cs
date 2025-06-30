using System.Collections;
using UnityEngine;

/// <summary>
/// 取っ手（Handle）制御
/// ------------------------------------------------------------
/// 1. requiredKeyId が空なら最初から可視／操作可  
/// 2. 鍵イベント(PlayerController.OnKeyCollected) を受信すると可視化＆Collider解放  
/// 3. Solid / Ghost 状態で取っ手 Sprite を自動切替  
/// 4. 長押し開始時、子ノード SpriteRenderer をプレイヤー方向へ一度だけ回転  
/// ------------------------------------------------------------
/// ※ 構造例:
///   Fastener
///     └─ HandleRoot   (BoxCollider2D + HandleController)   ← localScale = 1
///         └─ HandleVisual (SpriteRenderer)                 ← srHandle が参照
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    /*========== Inspector に表示する設定値 ==========*/
    [Header("スライド速度 (unit/秒)")]
    public float moveSpeed = 6f;

    [Header("必要 keyId (空文字なら鍵不要)")]
    public string requiredKeyId = "red";

    [Header("取っ手 Sprite")]
    public Sprite solidHandleSprite;   // ファスナーが Solid の時
    public Sprite ghostHandleSprite;   // ファスナーが Ghost の時

    [Header("長押し開始時にプレイヤー方向へ回転")]
    public bool rotateToPlayer = true;
    [Tooltip("取っ手画像の基準向きが+X以外なら角度補正 (例: 上向き=90)")]
    public float spriteForwardAngle = 0f;

    [Header("取っ手可視ノード (SpriteRenderer)")]
    [SerializeField] private SpriteRenderer srHandle;   // HandleVisual の SpriteRenderer

    /*========== ランタイム ==========*/
    public bool isMoving { get; private set; }

    System_Fastener rail;        // 親のファスナー
    Transform visualTF;          // srHandle の Transform

    /*------------------------------------------------------------*/
    void Start()
    {
        rail = GetComponentInParent<System_Fastener>();

        // SpriteRenderer 自動取得（Inspector 未設定時の保険）
        if (!srHandle) srHandle = GetComponentInChildren<SpriteRenderer>(true);
        if (srHandle) visualTF = srHandle.transform;

        // 鍵未取得なら非表示＆Collider 無効
        bool unlocked = requiredKeyId == "";
        if (srHandle) srHandle.enabled = unlocked;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = unlocked;

        UpdateHandleSprite();

        // 鍵イベント購読
        PlayerController.OnKeyCollected += HandleKeyCollected;
    }

    void OnDestroy()
    {
        PlayerController.OnKeyCollected -= HandleKeyCollected;
    }

    /*========== 鍵取得イベントを受信 ==========*/
    void HandleKeyCollected(string keyId)
    {
        if (keyId != requiredKeyId) return;

        // 可視化 + Collider 有効化
        if (srHandle) srHandle.enabled = true;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;

        UpdateHandleSprite();
        Debug.Log($"[Handle {name}] 解放 key={keyId}");
    }

    /*========== Fキー操作 ==========
     * PlayerController から以下 API が呼ばれる
     =================================*/

    /*--- 短押し：取っ手のみ移動 ---*/
    public void StartMoveAlone()
    {
        if (isMoving || !srHandle.enabled) return;

        var se = rail.GetComponentInChildren<SE>();
        if (se != null) se.PlaySE();

        rail.Toggle();            // ファスナー状態を即切替
        UpdateHandleSprite();
        StartCoroutine(MoveCoroutine(null));
    }

    /*--- 長押し：プレイヤー付着移動 ---*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving || !srHandle.enabled) return;

        // プレイヤー方向へ一度だけ回転
        if (rotateToPlayer && player && visualTF)
        {
            Vector2 dir = (player.transform.position - visualTF.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - spriteForwardAngle;
            visualTF.rotation = Quaternion.Euler(0, 0, ang);
        }

        StartCoroutine(MoveCoroutine(player)); // トグルは到達後
    }

    /*========== 移動コルーチン ==========*/
    IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;

        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();
        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        Rigidbody2D rbP = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rbP) { rbP.velocity = Vector2.zero; rbP.isKinematic = true; }

        float t = 0f, dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;
            if (rbP) player.transform.position = pos;
            yield return null;
        }

        if (rbP) rbP.isKinematic = false;

        // 長押し時：到達後にファスナー状態トグル
        if (player != null)
        {
            var se = rail.GetComponentInChildren<SE>(); 
            if (se != null) se.PlaySE();               

            rail.Toggle();
            UpdateHandleSprite();
        }
        isMoving = false;
    }

    /*========== Sprite 切替 ==========*/
    void UpdateHandleSprite()
    {
        if (!srHandle || !rail) return;

        srHandle.sprite = rail.CurrentState == System_Fastener.State.Solid
            ? solidHandleSprite
            : ghostHandleSprite;
    }
}
