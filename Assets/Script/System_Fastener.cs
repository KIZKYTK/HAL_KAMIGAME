// KIZK Created

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Fastener : MonoBehaviour
{
    // 当たり判定の箇所(Horizontal＝ 左右, Vertical ＝ 上下, Both ＝ 左右上下)
    public enum EdgeDir { Horizontal, Vertical, Both }

    [Header("端の判定")]
    [Range(0f, 0.5f)]
    public float edgePercent = 0.2f; // 端の幅（0.2 = 20%）
    public EdgeDir edgeDirection = EdgeDir.Horizontal;

    // 長押しと判定される時間
    private float longPress_time = 0.3f;
    // 入力時間
    private float press_time = 0.0f;
    // 入力判定フラグ
    private bool isPressing = false;

    private bool canInput = false;

    private Collider2D triggerCollider;

    // Start is called before the first frame update
    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (!triggerCollider || !triggerCollider.isTrigger)
        {
            Debug.LogWarning("このオブジェクトには Trigger に設定された Collider2D が必要です！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInput) return;

        // 入力処理(入力時)
        if (Input.GetKeyDown(KeyCode.F))
        {
            press_time = Time.time;
            isPressing = true;
        }

        // 入力処理(入力中)
        if (Input.GetKeyUp(KeyCode.F))
        {
            float heldDuration=Time.time- press_time;
            isPressing = true;

            if(heldDuration < longPress_time)
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
            Vector2 playerPos = other.transform.position;
            Bounds bounds = triggerCollider.bounds;

            bool isOnHorizontalEdge = false;
            bool isOnVerticalEdge = false;

            if (edgeDirection == EdgeDir.Horizontal || edgeDirection == EdgeDir.Both)
            {
                float width = bounds.size.x;
                float leftEdge = bounds.min.x + width * edgePercent;
                float rightEdge = bounds.max.x - width * edgePercent;
                isOnHorizontalEdge = (playerPos.x <= leftEdge || playerPos.x >= rightEdge);
            }

            if (edgeDirection == EdgeDir.Vertical || edgeDirection == EdgeDir.Both)
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
        }
    }

    // 短押し処理
    void ShortPressAction()
    {

    }
    
    // 長押し処理
    void LongPressAction()
    {

    }
}
