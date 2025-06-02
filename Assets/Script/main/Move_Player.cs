using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// プレイヤー制御：複数鍵 + 鍵不要ハンドル対応版
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Move_Player : MonoBehaviour
{
    /*===== 基本移動 =====*/
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    /*===== Fキー =====*/
    public KeyCode interactKey = KeyCode.F;
    public float holdThreshold = 0.25f;

    /*===== 接地判定 =====*/
    public Transform groundCheck;
    public float groundCheckRadius = .08f;
    public LayerMask groundMask;

    /*===== 鍵セット =====*/
    public HashSet<string> keyRing = new HashSet<string>();

    /*===== UI (任意) =====*/
    public Image hookIcon;
    public Sprite iconGot, iconNot;

    /*===== 内部状態 =====*/
    Rigidbody2D rb;
    bool isGrounded, isSliding;
    HandleController currentHandle;
    bool isPressingF; float fTime; bool slideTrig;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!groundCheck) groundCheck = transform.Find("groundCheck");
        if (hookIcon && iconNot) hookIcon.sprite = iconNot;
    }

    void Update()
    {
        /*-- 接地判定 --*/
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        /*-- 移動 & ジャンプ --*/
        if (!isSliding)
        {
            float h = Input.GetKey(KeyCode.D) ? 1 :
                      Input.GetKey(KeyCode.A) ? -1 : 0;
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else rb.velocity = Vector2.zero;

        /*-- Fキー押下開始 --*/
        if (Input.GetKeyDown(interactKey))
        {
            isPressingF = true; fTime = 0; slideTrig = false;
        }

        /*-- 押下中（長押し判定） --*/
        if (isPressingF)
        {
            fTime += Time.deltaTime;
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||          // 鍵不要
                          keyRing.Contains(currentHandle.requiredKeyId)); // 鍵一致

            if (!slideTrig &&
                fTime >= holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                slideTrig = true; isSliding = true;
                currentHandle.StartMoveWithPlayer(gameObject);
            }
        }

        /*-- Fキー離す（短押し判定） --*/
        if (Input.GetKeyUp(interactKey))
        {
            bool keyOK = currentHandle &&
                         (currentHandle.requiredKeyId == "" ||
                          keyRing.Contains(currentHandle.requiredKeyId));

            if (!slideTrig &&
                fTime < holdThreshold &&
                currentHandle && !currentHandle.isMoving &&
                keyOK)
            {
                currentHandle.StartMoveAlone();
            }

            isPressingF = false; fTime = 0;
        }

        /*-- スライド終了 --*/
        if (isSliding && currentHandle && !currentHandle.isMoving)
            isSliding = false;
    }

    /*---------------- Trigger 判定 ----------------*/
    void OnTriggerEnter2D(Collider2D other)
    {
        /* 取っ手 */
        if (other.CompareTag("Handle"))
        {
            currentHandle = other.GetComponent<HandleController>();
            return;
        }

        /* 鍵アイテム */
        if (other.CompareTag("Item"))
        {
            ItemEntity it = other.GetComponent<ItemEntity>();
            if (it && keyRing.Add(it.keyId))
            {
                if (hookIcon && iconGot) hookIcon.sprite = iconGot;
                Debug.Log($"鍵 [{it.keyId}] を取得");
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Handle") &&
            other.GetComponent<HandleController>() == currentHandle)
            currentHandle = null;
    }
}
