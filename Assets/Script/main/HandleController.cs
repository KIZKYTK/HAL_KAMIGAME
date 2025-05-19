using System.Collections;
using UnityEngine;

/// <summary>
/// ハンドル（取っ手）制御  
/// ・短押し／長押しのどちらでも、開始時にファスナー（実体?虚体）をトグル  
/// ・requiredKeyId はプレイヤーが保持する鍵 ID と一致した時のみ操作可能判定に用いる
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    /* ===== インスペクタ設定 ===== */
    [Header("スライド速度 (unit/秒)")]
    public float moveSpeed = 6f;

    [Header("必要な keyId (鍵の色など)")]
    public string requiredKeyId = "red";         // 例: "red" / "blue" ...

    /* ===== 実行時ステータス ===== */
    public bool isMoving { get; private set; } = false;  // 移動中フラグ

    /* ===== 参照キャッシュ ===== */
    private FastenerController rail;                      // 親ファスナー

    /*--------------------------------------------------*/
    void Start()
    {
        // 親階層から FastenerController を取得
        rail = GetComponentInParent<FastenerController>();
    }

    /* ---------------- 短押し：取っ手のみ移動 ---------------- */
    public void StartMoveAlone()
    {
        if (isMoving) return;     // 既に移動中なら無視
        rail.Toggle();            // 開始時にファスナーをトグル
        StartCoroutine(MoveCoroutine(null));   // プレイヤー無し
    }

    /* ---------------- 長押し：プレイヤー付着移動 ---------------- */
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        rail.Toggle();            // 同じく開始時にトグル
        StartCoroutine(MoveCoroutine(player)); // プレイヤー付き
    }

    /* ---------------- コア移動協程 ---------------- */
    private IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;

        // レール両端ワールド座標
        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();

        // 取っ手の現在位置から近い方をスタート、遠い方をゴールに設定
        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        // プレイヤー剛体を一時固定
        Rigidbody2D rbP = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rbP)
        {
            rbP.velocity = Vector2.zero;
            rbP.isKinematic = true;
        }

        /* 線形補間で移動 */
        float t = 0f;
        float dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;  // 0→1 の進行率
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;                // 取っ手移動
            if (rbP) player.transform.position = pos; // 付着中のプレイヤーを追従
            yield return null;
        }

        // プレイヤーを物理復帰
        if (rbP) rbP.isKinematic = false;

        isMoving = false;   // 終了フラグ
    }
}

