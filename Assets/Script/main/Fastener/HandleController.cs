using System.Collections;
using UnityEngine;

/// <summary>
/// 取っ手制御  
/// ・鍵一致判定は従来どおり  
/// ・ファスナーの状態に合わせて取っ手 Sprite も自動切替
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class HandleController : MonoBehaviour
{
    [Header("スライド速度 (unit/秒)")]
    public float moveSpeed = 6f;

    [Header("必要 keyId (空文字なら鍵不要)")]
    public string requiredKeyId = "red";

    [Header("取っ手用 Sprite")]
    public Sprite solidHandleSprite;          // 実体時
    public Sprite ghostHandleSprite;          // 虚体時

    public bool isMoving { get; private set; } = false;

    System_Fastener rail;
    SpriteRenderer srHandle;

#if UNITY_EDITOR
    /* 編集モードで Sprite を即時反映 */
    void OnValidate()
    {
        if (!srHandle) srHandle = GetComponent<SpriteRenderer>();
        if (!rail) rail = GetComponentInParent<System_Fastener>();
        UpdateHandleSprite();
    }
#endif

    void Start()
    {
        rail = GetComponentInParent<System_Fastener>();
        srHandle = GetComponent<SpriteRenderer>();
        UpdateHandleSprite();                 // 初期反映
    }

    /*------------------ 公開 API ------------------*/
    /* 短押し：取っ手のみ移動（即トグル）*/
    public void StartMoveAlone()
    {
        if (isMoving) return;
        rail.Toggle();                        // ファスナー切替
        UpdateHandleSprite();                 // 取っ手画像も切替
        StartCoroutine(MoveCoroutine(null));
    }

    /* 長押し：プレイヤー付着（到達後トグル）*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        // ※ ここでは切替しない（到達後）
        StartCoroutine(MoveCoroutine(player));
    }

    /*------------------ コルーチン ------------------*/
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

        /* 長押し時は到達後にトグル */
        if (player != null)
        {
            rail.Toggle();
            UpdateHandleSprite();
        }

        isMoving = false;
    }

    /*------------------ 内部ヘルパー ------------------*/
    void UpdateHandleSprite()
    {
        if (!srHandle || !rail) return;

        if (rail.CurrentState == System_Fastener.State.Solid && solidHandleSprite)
            srHandle.sprite = solidHandleSprite;
        else if (rail.CurrentState == System_Fastener.State.Ghost && ghostHandleSprite)
            srHandle.sprite = ghostHandleSprite;
    }
}
