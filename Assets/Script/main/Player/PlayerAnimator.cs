using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("通常画像")]
    public Sprite idleSprite;

    [Header("歩行アニメーション")]
    public List<Sprite> walkSprites;

    [Header("アニメーション速度 (秒)")]
    public float frameDuration = 0.1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private int frameIndex = 0;
    private float frameTimer = 0f;

    private Vector2 fixedSize; // 固定表示サイズ（ワールド単位）

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 描画サイズ固定用（初期スプライトのサイズ）
        if (idleSprite != null)
        {
            fixedSize = idleSprite.bounds.size;
        }

        // スプライトを伸縮可能にする
        sr.drawMode = SpriteDrawMode.Sliced;
    }

    void Update()
    {
        if (sr == null) return;

        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.01f;
        bool isJumping = !isGrounded;
        bool isJumpingAndMoving = isJumping && isMoving;

        bool doAnimate = (isGrounded && isMoving) || isJumpingAndMoving;

        if (rb.velocity.x > 0.01f)
            sr.flipX = false;
        else if (rb.velocity.x < -0.01f)
            sr.flipX = true;

        Sprite targetSprite = idleSprite;

        if (doAnimate && walkSprites.Count > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameDuration)
            {
                frameIndex = (frameIndex + 1) % walkSprites.Count;
                frameTimer = 0f;
            }
            targetSprite = walkSprites[frameIndex];
        }
        else
        {
            frameIndex = 0;
            frameTimer = 0f;
        }

        sr.sprite = targetSprite;

        // スプライトごとに描画サイズを固定
        ApplyFixedSize(targetSprite);
    }

    void ApplyFixedSize(Sprite sprite)
    {
        if (sprite == null || fixedSize == Vector2.zero) return;

        Vector2 originalSize = sprite.bounds.size;

        // 描画サイズを固定サイズに合わせる
        sr.size = fixedSize;

        
    }
}
