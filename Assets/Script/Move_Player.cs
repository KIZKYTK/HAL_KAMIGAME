// KIZK Created

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    public float speedFloat = 5.0f;
    [SerializeField, Header("ジャンプ力")]
    public float jumpPower = 10.0f;
    [SerializeField, Header("Raduis")]
    private float radius = 0.5f;
    [SerializeField, Header("接触判定")]
    public Transform groundCheck;
    [SerializeField, Header("LayerMask")]
    public LayerMask groundLayer;
    [SerializeField, Header("ログ表示")]
    public bool showLog = true;

    // リジットボディ情報取得用変数
    private Rigidbody2D rb;
    // 地面との接触フラグ
    private bool touchFg;

    // Start is called before the first frame update
    void Start()
    {
        // rigidbodyの情報を取得
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // touchFg = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
        // Debug.Log("touchFg: " + touchFg);

        // A/D or ←/→ で横移動
        float mx = Input.GetAxisRaw("Horizontal");
        // 横移動のみ更新
        rb.velocity = new Vector2(mx * speedFloat, rb.velocity.y);

        // ジャンプ処理
        if(Input.GetButtonDown("Jump") && touchFg)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }

    private void FixedUpdate()
    {
        touchFg = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
        if(showLog) Debug.Log("touchFg: " + touchFg);
    }

    // 判定範囲を可視化
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = touchFg ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, radius);
        }
    }
}
