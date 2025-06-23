using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TextureApplier : MonoBehaviour
{
    [Tooltip("貼り付けたいテクスチャ")]
    public Texture2D texture;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (texture != null)
        {
            ApplyTexture();
        }
    }

    void ApplyTexture()
    {
        // テクスチャのサイズを取得
        int texWidth = texture.width;
        int texHeight = texture.height;

        // Spriteを作成（Pivotを中心に）
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texWidth, texHeight),
            new Vector2(0.5f, 0.5f), // pivot
            100f                     // Pixels Per Unit
        );

        spriteRenderer.sprite = sprite;

        // Spriteのサイズに合わせてスケールを調整
        float unitsX = texWidth / 100f;
        float unitsY = texHeight / 100f;
        Vector3 scale = transform.localScale;

        transform.localScale = new Vector3(
            scale.x / unitsX,
            scale.y / unitsY,
            1f
        );
    }
}
