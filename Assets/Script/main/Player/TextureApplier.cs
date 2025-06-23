using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TextureApplier : MonoBehaviour
{
    [Tooltip("�\��t�������e�N�X�`��")]
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
        // �e�N�X�`���̃T�C�Y���擾
        int texWidth = texture.width;
        int texHeight = texture.height;

        // Sprite���쐬�iPivot�𒆐S�Ɂj
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texWidth, texHeight),
            new Vector2(0.5f, 0.5f), // pivot
            100f                     // Pixels Per Unit
        );

        spriteRenderer.sprite = sprite;

        // Sprite�̃T�C�Y�ɍ��킹�ăX�P�[���𒲐�
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
