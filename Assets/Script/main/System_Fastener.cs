using UnityEngine;
using System.Collections;

public class System_Fastener : MonoBehaviour
{
    [Header("端の判定")]
    [Range(0f, 0.5f)]
    public float edgePercent = 0.1f;

    public bool useHorizontal = true;
    public bool useVertical = false;

    private float longPress_time = 0.3f;
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
            isPressing = true;

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

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInput = false;
            isPressing = false;
            playerObj = null;
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
            DrawRect(new Rect(bounds.min.x, bounds.min.y, edgeWidth, bounds.size.y));
            DrawRect(new Rect(bounds.max.x - edgeWidth, bounds.min.y, edgeWidth, bounds.size.y));
        }

        if (useVertical)
        {
            float edgeHeight = bounds.size.y * edgePercent;
            DrawRect(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, edgeHeight));
            DrawRect(new Rect(bounds.min.x, bounds.max.y - edgeHeight, bounds.size.x, edgeHeight));
        }
    }

    void DrawRect(Rect rect)
    {
        Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin, 0f);
        Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin, 0f);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMax, 0f);
        Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, 0f);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        Vector3 center = rect.center;
        Vector3 size = new Vector3(rect.width, rect.height, 0.01f);
        Gizmos.DrawCube(center, size);
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

        Vector3 axis;
        float objHalfExtent;

        if (useHorizontal)
        {
            axis = triggerCollider.transform.right.normalized;
            objHalfExtent = objBounds.extents.magnitude;
        }
        else if (useVertical)
        {
            axis = triggerCollider.transform.up.normalized;
            objHalfExtent = objBounds.extents.magnitude;
        }
        else
        {
            return; // 無効な設定
        }

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

        Vector3 targetA = objCenter - axis * (objHalfExtent - maxProjection);
        Vector3 targetB = objCenter + axis * (objHalfExtent - maxProjection);

        Vector3 toPlayer = playerTrans.position - objCenter;
        float dot = Vector3.Dot(toPlayer, axis);

        Vector3 targetPos = (dot >= 0) ? targetA : targetB;

        StopAllCoroutines();
        StartCoroutine(SmoothMove(playerTrans, targetPos, 0.3f, () => {
            // 長押しの移動後に物理判定を切り替える
            if (triggerCollider)
            {
                triggerCollider.isTrigger = !triggerCollider.isTrigger;
                Debug.Log("LongPress: Collider trigger = " + triggerCollider.isTrigger);
            }
        }));
    }

    // 移動コルーチンにコールバック追加
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
