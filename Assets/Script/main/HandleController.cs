using System.Collections;
using UnityEngine;

/// <summary>
/// 取っ手制御  
/// requiredKeyId が “” (空文字) なら鍵不要タイプ、  
/// それ以外は同じ keyId を持つプレイヤーだけが操作可能。
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    [Header("スライド速度 (unit/秒)")]
    public float moveSpeed = 6f;

    [Header("必要 keyId (空文字なら鍵不要)")]
    public string requiredKeyId = "red";

    public bool isMoving { get; private set; } = false;

    System_Fastener rail;

    void Start() => rail = GetComponentInParent<System_Fastener>();

    /*--- 短押し：取っ手だけ移動 ---*/
    public void StartMoveAlone()
    {
        if (isMoving) return;
        rail.Toggle();
        StartCoroutine(MoveCoroutine(null));
    }

    /*--- 長押し：プレイヤー付着移動 ---*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        rail.Toggle();
        StartCoroutine(MoveCoroutine(player));
    }

    /*--- コルーチン本体 ---*/
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
        isMoving = false;
    }
}
