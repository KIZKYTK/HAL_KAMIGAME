using System.Collections;
using UnityEngine;

public class VerticalFastener : MonoBehaviour
{
    [Header("端の判定")]
    [Range(0f, 0.5f)]
    public float edgePercent = 0.1f;

    [Header("有効軸")]
    public bool useHorizontal = false;  // 横方向を無効に設定
    public bool useVertical = true;    // 縦方向を有効にするフラグ

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
            Debug.LogWarning("Trigger 設定された Collider2D が必要です");
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
            isPressing = false;
            float held = Time.time - press_time;

            if (held < longPress_time)
                ShortPressAction();
            else
                LongPressAction();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerObj = other.gameObject;
        Vector2 playerPos = other.transform.position;
        Bounds bounds = triggerCollider.bounds;
        float height = bounds.size.y;

        float bottomEdge = bounds.min.y + height * edgePercent;
        float topEdge = bounds.max.y - height * edgePercent;

        canInput = (playerPos.y <= bottomEdge || playerPos.y >= topEdge);
    }

    void ShortPressAction()
    {
        triggerCollider.isTrigger = !triggerCollider.isTrigger;
    }

    void LongPressAction()
    {
        if (!playerObj) return;

        // 縦方向の処理のみ
        if (useVertical)
        {
            Transform playerTrans = playerObj.transform;
            Collider2D playerCol = playerObj.GetComponent<Collider2D>();
            if (!playerCol) return;

            Bounds objBounds = triggerCollider.bounds;
            Bounds playerBounds = playerCol.bounds;

            Vector3 axis = transform.up.normalized; // 縦方向
            float halfExtent = objBounds.extents.y;

            // ここでプレイヤーの投影範囲を計算
            float maxProj = GetMaxProjection(playerBounds, axis);

            Vector3 center = objBounds.center;
            Vector3 toPlayer = playerTrans.position - center;
            float dot = Vector3.Dot(toPlayer, axis);

            // 縦方向のターゲット位置
            Vector3 targetPos = (dot >= 0) ?
                center - axis * (halfExtent - maxProj) :
                center + axis * (halfExtent - maxProj);

            // 縦方向に動かす
            if (Mathf.Abs(playerTrans.position.x - targetPos.x) > Mathf.Epsilon)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothMove(playerTrans, targetPos, 0.3f, () =>
                {
                    triggerCollider.isTrigger = !triggerCollider.isTrigger;
                }));
            }
        }
    }

    float GetMaxProjection(Bounds bounds, Vector3 axis)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = bounds.min, max = bounds.max;
        int i = 0;
        for (int x = 0; x <= 1; x++)
            for (int y = 0; y <= 1; y++)
                for (int z = 0; z <= 1; z++)
                    corners[i++] = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z);

        float maxProj = 0;
        foreach (var c in corners)
            maxProj = Mathf.Max(maxProj, Mathf.Abs(Vector3.Dot(c - bounds.center, axis)));

        return maxProj;
    }

    IEnumerator SmoothMove(Transform obj, Vector3 target, float time, System.Action onDone)
    {
        Vector3 start = obj.position;
        float elapsed = 0;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            obj.position = Vector3.Lerp(start, target, elapsed / time);
            yield return null;
        }

        obj.position = target;
        onDone?.Invoke();
    }
}
