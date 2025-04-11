using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    public float speedFloat;
    [SerializeField, Header("ジャンプ力")]
    public float jumpPower;
    [SerializeField, Header("Transform")]
    public new Transform transform;
    [SerializeField, Header("LayerMask")]
    public LayerMask layer;
    [SerializeField, Header("Raduis")]
    private float radius = 0.3f;

    // リジットボディ情報取得用変数
    private Rigidbody2D rb;
    // 地面との接触フラグ
    private bool touchFg;

    // Start is called before the first frame update
    void Start()
    {
        // rigidbodyの情報を取得
        rb=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        touchFg = Physics2D.OverlapCircle(transform.position, radius, layer);
        Debug.Log("touchFg: " + touchFg);

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

    // 判定範囲を可視化
    private void OnDrawGizmosSelected()
    {
        if (transform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
