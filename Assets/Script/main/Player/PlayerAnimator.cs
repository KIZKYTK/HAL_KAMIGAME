using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("�ʏ�摜")]
    public Sprite idleSprite;

    [Header("���s�A�j���[�V����")]
    public List<Sprite> walkSprites;

    [Header("�A�j���[�V�������x (�b)")]
    public float frameDuration = 0.1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private int frameIndex = 0;
    private float frameTimer = 0f;

    private Vector2 fixedSize; // �Œ�\���T�C�Y�i���[���h�P�ʁj

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // �`��T�C�Y�Œ�p�i�����X�v���C�g�̃T�C�Y�j
        if (idleSprite != null)
        {
            fixedSize = idleSprite.bounds.size;
        }

        // �X�v���C�g��L�k�\�ɂ���
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

        // �X�v���C�g���Ƃɕ`��T�C�Y���Œ�
        ApplyFixedSize(targetSprite);
    }

    void ApplyFixedSize(Sprite sprite)
    {
        if (sprite == null || fixedSize == Vector2.zero) return;

        Vector2 originalSize = sprite.bounds.size;

        // �`��T�C�Y���Œ�T�C�Y�ɍ��킹��
        sr.size = fixedSize;

        
    }
}
