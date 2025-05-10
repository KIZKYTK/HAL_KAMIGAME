using System.Collections;
using UnityEngine;

public class HorizontalFastener : MonoBehaviour
{
    [Header("端の判定")]
    [Range(0f, 0.5f)]
    public float edgePercent = 0.1f;

    private float longPress_time = 0.2f;
    private float press_time = 0.0f;
    private bool isPressing = false;
    private bool canInput = true; // 初期状態は入力を受け付ける

    private Collider2D triggerCollider;
    private GameObject playerObj;

    [Header("有効軸")]
    public bool useHorizontal = true;  // 横方向を有効にするフラグ
    public bool useVertical = false;  // 縦方向を無効に設定

    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (!triggerCollider || !triggerCollider.isTrigger)
        {
            Debug.LogWarning("このオブジェクトには Trigger に設定された Collider2D が必要です！");
        }
    }

    void Update()
    {
        if (!canInput) return; // 入力受付が無効なら処理しない

        if (Input.GetKeyDown(KeyCode.F))
        {
            press_time = Time.time;
            isPressing = true;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            float heldDuration = Time.time - press_time;
            isPressing = false;

            if (heldDuration < longPress_time)
            {
                Debug.Log("short press");
                ShortPressAction();
            }
            else
            {
                Debug.Log("long press");
                LongPressAction();
            }

            // 長押し後は再度入力を許可する
            canInput = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerObj = other.gameObject;
            Vector2 playerPos = other.transform.position;
            Bounds bounds = triggerCollider.bounds;

            bool isOnHorizontalEdge = false;

            if (useHorizontal)
            {
                float width = bounds.size.x;
                float leftEdge = bounds.min.x + width * edgePercent;
                float rightEdge = bounds.max.x - width * edgePercent;
                isOnHorizontalEdge = (playerPos.x <= leftEdge || playerPos.x >= rightEdge);
            }

            canInput = isOnHorizontalEdge; // 横端にいるときだけ入力を受け付ける
        }
    }

    void ShortPressAction()
    {
        // 短い入力の場合、トリガーを切り替える
        if (triggerCollider)
        {
            triggerCollider.isTrigger = !triggerCollider.isTrigger;
            Debug.Log("ShortPress: Collider trigger = " + triggerCollider.isTrigger);
        }
    }

    void LongPressAction()
    {
        if (!playerObj) return;

        // 横方向の処理のみ
        if (useHorizontal)
        {
            Transform playerTrans = playerObj.transform;
            Collider2D playerCol = playerObj.GetComponent<Collider2D>();
            if (!playerCol) return;

            Bounds objBounds = triggerCollider.bounds;
            Bounds playerBounds = playerCol.bounds;

            Vector3 axis = transform.right.normalized; // 横方向
            float halfExtent = objBounds.extents.x;

            // プレイヤーの投影範囲を計算
            float maxProj = GetMaxProjection(playerBounds, axis);

            Vector3 center = objBounds.center;
            Vector3 toPlayer = playerTrans.position - center;
            float dot = Vector3.Dot(toPlayer, axis);

            // 横方向のターゲット位置
            Vector3 targetPos = (dot >= 0) ?
                center - axis * (halfExtent - maxProj) :
                center + axis * (halfExtent - maxProj);

            // 横方向に動かす
            if (Mathf.Abs(playerTrans.position.y - targetPos.y) > Mathf.Epsilon)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothMove(playerTrans, targetPos, 0.3f, () =>
                {
                    triggerCollider.isTrigger = !triggerCollider.isTrigger;
                    canInput = true; // 長押し後に入力を再度有効にする
                }));
            }
        }
    }

    // プレイヤーの投影範囲計算
    float GetMaxProjection(Bounds playerBounds, Vector3 axis)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = playerBounds.min;
        Vector3 max = playerBounds.max;

        int i = 0;
        for (int x = 0; x <= 1; x++)
            for (int y = 0; y <= 1; y++)
                for (int z = 0; z <= 1; z++)
                    corners[i++] = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z
                    );

        float maxProjection = 0f;
        foreach (var corner in corners)
        {
            float proj = Mathf.Abs(Vector3.Dot(corner - playerBounds.center, axis));
            if (proj > maxProjection)
                maxProjection = proj;
        }

        return maxProjection;
    }

    // SmoothMove コルーチン
    IEnumerator SmoothMove(Transform obj, Vector3 target, float duration, System.Action onComplete = null)
    {
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }

        obj.position = target;
        onComplete?.Invoke(); // 終了後にコールバックを実行
    }
}
