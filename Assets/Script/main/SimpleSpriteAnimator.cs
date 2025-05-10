using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [Tooltip("歩きモーション用スプライトを順番に入れてください")]
    public Sprite[] walkSprites;
    [Tooltip("1秒間に切り替えるコマ数")]
    public float framesPerSecond = 10f;
    [Tooltip("移動速度")]
    public float moveSpeed = 5f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private int frameIndex;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (walkSprites == null || walkSprites.Length == 0)
            Debug.LogWarning("walkSprites にスプライトをセットしてください。");
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // 方向に応じてスプライトを反転
        if (h > 0f) sr.flipX = false;
        else if (h < 0f) sr.flipX = true;

        // 移動
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        // アニメーション
        if (Mathf.Approximately(h, 0f))
        {
            frameIndex = 0;
            timer = 0f;
            sr.sprite = walkSprites[0];
        }
        else
        {
            timer += Time.deltaTime;
            float interval = 1f / framesPerSecond;
            if (timer >= interval)
            {
                timer -= interval;
                frameIndex = (frameIndex + 1) % walkSprites.Length;
                sr.sprite = walkSprites[frameIndex];
            }
        }
    }
}
