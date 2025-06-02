using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [Tooltip("�������[�V�����p�X�v���C�g�����Ԃɓ���Ă�������")]
    public Sprite[] walkSprites;
    [Tooltip("1�b�Ԃɐ؂�ւ���R�}��")]
    public float framesPerSecond = 10f;
    [Tooltip("�ړ����x")]
    public float moveSpeed = 5f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private int frameIndex;
    private float timer;

    [Header("�蓮�ݒ�")]
    [Tooltip("�X�v���C�g�����ڃT�C�Y�i���E�����j")]
    public Vector2 targetVisualSize = new Vector2(1f, 1f);

    [Tooltip("�I�u�W�F�N�g�̌����ڃT�C�Y�i���E�����j�BAwake���Ɏ����擾���ȍ~�͕ς��܂���B")]
    public Vector2 objectVisualSize = new Vector2(1f, 1f);

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (walkSprites == null || walkSprites.Length == 0)
        {
            Debug.LogWarning("walkSprites �ɃX�v���C�g���Z�b�g���Ă��������B");
            return;
        }

        // Awake���ɃI�u�W�F�N�g��localScale���L�^���ČŒ�
        objectVisualSize = new Vector2(transform.localScale.x, transform.localScale.y);

        sr.sprite = walkSprites[0];
        AdjustScaleToTargetSize(walkSprites[0]);
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (h > 0f) sr.flipX = false;
        else if (h < 0f) sr.flipX = true;

        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        if (Mathf.Approximately(h, 0f))
        {
            frameIndex = 0;
            timer = 0f;
            sr.sprite = walkSprites[0];
            AdjustScaleToTargetSize(walkSprites[0]);
        }
        else
        {
            timer += Time.deltaTime;
            float interval = 1f / framesPerSecond;
            if (timer >= interval)
            {
                timer -= interval;
                frameIndex = (frameIndex + 1) % walkSprites.Length;
                Sprite nextSprite = walkSprites[frameIndex];
                sr.sprite = nextSprite;
                AdjustScaleToTargetSize(nextSprite);
            }
        }
    }

    void AdjustScaleToTargetSize(Sprite sprite)
    {
        if (sprite == null || sprite.bounds.size.x == 0 || sprite.bounds.size.y == 0)
            return;

        Vector2 spriteSize = sprite.bounds.size;
        Vector3 newScale = transform.localScale;

        newScale.x = targetVisualSize.x / spriteSize.x;
        newScale.y = targetVisualSize.y / spriteSize.y;

        transform.localScale = newScale;
    }

    void OnDrawGizmos()
    {
        Vector3 center = transform.position;

        // targetVisualSize�i�V�A���j
        Gizmos.color = Color.cyan;
        Vector3 halfTarget = new Vector3(targetVisualSize.x / 2f, targetVisualSize.y / 2f, 0f);
        DrawRectangle(center, halfTarget);

        // objectVisualSize�i���F�j
        Gizmos.color = Color.yellow;
        Vector3 halfObject = new Vector3(objectVisualSize.x / 2f, objectVisualSize.y / 2f, 0f);
        DrawRectangle(center, halfObject);
    }

    void DrawRectangle(Vector3 center, Vector3 halfSize)
    {
        Vector3 topLeft = center + new Vector3(-halfSize.x, halfSize.y, 0f);
        Vector3 topRight = center + new Vector3(halfSize.x, halfSize.y, 0f);
        Vector3 bottomLeft = center + new Vector3(-halfSize.x, -halfSize.y, 0f);
        Vector3 bottomRight = center + new Vector3(halfSize.x, -halfSize.y, 0f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        sr = GetComponent<SpriteRenderer>();
        if (walkSprites != null && walkSprites.Length > 0 && walkSprites[0] != null)
        {
            sr.sprite = walkSprites[0];
            AdjustScaleToTargetSize(walkSprites[0]);
        }
    }
#endif
}
