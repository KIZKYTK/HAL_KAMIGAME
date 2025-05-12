using System.Collections;
using UnityEngine;

/// <summary>
/// 取っ手（Handle）制御  
/// ・軽押し：取っ手のみ移動（開始時にトグル）  
/// ・長押し：プレイヤー付着移動（開始時にトグル）  
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    [Header("スライド速度 (unit/秒)")]
    public float moveSpeed = 6f;

    public bool isMoving { get; private set; } = false;   // 移動中
    public bool playerAttached { get; private set; } = false;   // 付着中

    private FastenerController rail;   // 親ファスナー
    private Coroutine moveCo;

    void Start()
    {
        rail = GetComponentInParent<FastenerController>();
        if (rail == null)
            Debug.LogError("[Handle] 親に FastenerController がありません");
    }

    /*―― 軽押し：取っ手のみ移動 ――*/
    public void StartMoveAlone()
    {
        if (isMoving || rail == null) return;

        rail.Toggle();                                // ★開始時にトグル
        moveCo = StartCoroutine(MoveCoroutine(null)); // プレイヤー無し
    }

    /*―― 長押し：プレイヤー付着移動 ――*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving || rail == null) return;

        rail.Toggle();                                // ★開始時にトグル
        playerAttached = true;
        moveCo = StartCoroutine(MoveCoroutine(player));
    }

    /*―― メイン協程 ――*/
    private IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;

        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();
        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        Rigidbody2D rbPlayer = null;
        if (player != null)
        {
            rbPlayer = player.GetComponent<Rigidbody2D>();
            rbPlayer.velocity = Vector2.zero;
            rbPlayer.isKinematic = true;
        }

        float t = 0f;
        float dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;
            if (rbPlayer != null) player.transform.position = pos;
            yield return null;
        }

        if (rbPlayer != null) rbPlayer.isKinematic = false;

        isMoving = false;
        playerAttached = false;
    }

#if UNITY_EDITOR
    /* シーンビューでレールを可視化 */
    void OnDrawGizmos()
    {
        if (rail == null) rail = GetComponentInParent<FastenerController>();
        if (rail == null) return;
        var (p0, p1) = rail.GetEndPoints();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p0, p1);
    }
#endif
}
