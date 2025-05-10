using System.Collections;
using UnityEngine;

public class System_Fastener : MonoBehaviour
{
    [Header("端の判定")]
    [Range(0f, 0.5f)]
    public float edgePercent = 0.1f;

    [Header("有効軸")]
    public bool useHorizontal = true;
    public bool useVertical = false;

    private float longPress_time = 0.2f;
    private float press_time = 0.0f;
    private bool isPressing = false;
    private bool canInput = false;

    private Collider2D triggerCollider;
    private GameObject playerObj;

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
        if (!canInput) return;

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
            bool isOnVerticalEdge = false;

            if (useHorizontal)
            {
                float width = bounds.size.x;
                float leftEdge = bounds.min.x + width * edgePercent;
                float rightEdge = bounds.max.x - width * edgePercent;
                isOnHorizontalEdge = (playerPos.x <= leftEdge || playerPos.x >= rightEdge);
            }

            if (useVertical)
            {
                float height = bounds.size.y;
                float bottomEdge = bounds.min.y + height * edgePercent;
                float topEdge = bounds.max.y - height * edgePercent;
                isOnVerticalEdge = (playerPos.y <= bottomEdge || playerPos.y >= topEdge);
            }

            canInput = isOnHorizontalEdge || isOnVerticalEdge;
        }
    }

    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (!col) return;

        Bounds bounds = col.bounds;
        Gizmos.color = new Color(1, 0, 0, 0.25f);

        if (useHorizontal)
        {
            float edgeWidth = bounds.size.x * edgePercent;
            Gizmos.DrawWireCube(new Vector3(bounds.min.x + edgeWidth / 2, bounds.center.y, 0), new Vector3(edgeWidth, bounds.size.y, 0));
            Gizmos.DrawWireCube(new Vector3(bounds.max.x - edgeWidth / 2, bounds.center.y, 0), new Vector3(edgeWidth, bounds.size.y, 0));
        }

        if (useVertical)
        {
            float edgeHeight = bounds.size.y * edgePercent;
            Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.min.y + edgeHeight / 2, 0), new Vector3(bounds.size.x, edgeHeight, 0));
            Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.max.y - edgeHeight / 2, 0), new Vector3(bounds.size.x, edgeHeight, 0));
        }
    }

    void ShortPressAction()
    {
        if (triggerCollider)
        {
            triggerCollider.isTrigger = !triggerCollider.isTrigger;
            Debug.Log("ShortPress: Collider trigger = " + triggerCollider.isTrigger);
        }
    }

    void LongPressAction()
    {
        if (!playerObj) return;

        Transform playerTrans = playerObj.transform;
        Collider2D playerCol = playerObj.GetComponent<Collider2D>();
        if (!playerCol) return;

        Bounds objBounds = triggerCollider.bounds;
        Bounds playerBounds = playerCol.bounds;

        Vector3 objCenter = objBounds.center;

        Vector3 axis = Vector3.zero;
        float objHalfExtent = 0f;

        // 横と縦の処理を厳密に分ける
        if (useHorizontal)
        {
            // 横方向の移動
            axis = triggerCollider.transform.right.normalized;
            objHalfExtent = objBounds.extents.x; // 横方向の半分の幅
            MoveHorizontally(playerTrans, objCenter, axis, objHalfExtent, playerBounds);
        }
        else if (useVertical)
        {
            // 縦方向の移動
            axis = triggerCollider.transform.up.normalized;
            objHalfExtent = objBounds.extents.y; // 縦方向の半分の高さ
            MoveVertically(playerTrans, objCenter, axis, objHalfExtent, playerBounds);
        }
    }

    void MoveHorizontally(Transform playerTrans, Vector3 objCenter, Vector3 axis, float objHalfExtent, Bounds playerBounds)
    {
        // プレイヤーの移動に関するコーナー計算
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

        // 横方向のターゲット位置を計算
        Vector3 targetA = objCenter - axis * (objHalfExtent - maxProjection);
        Vector3 targetB = objCenter + axis * (objHalfExtent - maxProjection);

        Vector3 toPlayer = playerTrans.position - objCenter;
        float dot = Vector3.Dot(toPlayer, axis);

        // 目標位置を決定（横方向）
        Vector3 targetPos = (dot >= 0) ? targetA : targetB;

        // 横方向の移動を実行
        if (Mathf.Abs(playerTrans.position.y - targetPos.y) > Mathf.Epsilon)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothMove(playerTrans, targetPos, 0.3f, () =>
            {
                if (triggerCollider)
                {
                    triggerCollider.isTrigger = !triggerCollider.isTrigger;
                    Debug.Log("LongPress (Horizontal): Collider trigger = " + triggerCollider.isTrigger);
                }
            }));
        }
    }

    void MoveVertically(Transform playerTrans, Vector3 objCenter, Vector3 axis, float objHalfExtent, Bounds playerBounds)
    {
        // プレイヤーの移動に関するコーナー計算
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

        // 縦方向のターゲット位置を計算
        Vector3 targetA = objCenter - axis * (objHalfExtent - maxProjection);
        Vector3 targetB = objCenter + axis * (objHalfExtent - maxProjection);

        Vector3 toPlayer = playerTrans.position - objCenter;
        float dot = Vector3.Dot(toPlayer, axis);

        // 目標位置を決定（縦方向）
        Vector3 targetPos = (dot >= 0) ? targetA : targetB;

        // 縦方向の移動を実行
        if (Mathf.Abs(playerTrans.position.x - targetPos.x) > Mathf.Epsilon)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothMove(playerTrans, targetPos, 0.3f, () =>
            {
                if (triggerCollider)
                {
                    triggerCollider.isTrigger = !triggerCollider.isTrigger;
                    Debug.Log("LongPress (Vertical): Collider trigger = " + triggerCollider.isTrigger);
                }
            }));
        }
    }

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
