using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    public float speedFloat = 5.0f;
    [SerializeField, Header("ジャンプ力")]
    public float jumpPower = 10.0f;
    [SerializeField, Header("Radius")]
    public float radius = 0.5f;            // Inspector で調整できるよう public に
    [SerializeField, Header("接触判定")]
    public Transform groundCheck;
    [SerializeField, Header("LayerMask")]
    public LayerMask groundLayer;
    [SerializeField, Header("ログ表示")]
    public bool showLog = true;

    // Rigidbody2D 取得用
    private Rigidbody2D rb;
    // 地面接触フラグ
    private bool touchFg;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 念のため倒れ防止
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 左右移動
        float mx = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(mx * speedFloat, rb.velocity.y);

        // ジャンプ
        if (Input.GetButtonDown("Jump") && touchFg)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }

    void FixedUpdate()
    {
        // 地面接触チェック
        touchFg = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
        if (showLog) Debug.Log("touchFg: " + touchFg);
    }

    public bool IsGrounded()
    {
        return touchFg;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = touchFg ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, radius);
        }
    }
}
