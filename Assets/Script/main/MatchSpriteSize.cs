using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MatchSpriteSize : MonoBehaviour
{
    [Tooltip("スプライトが表示されるワールドユニットでの幅")]
    public float targetWidth = 1f;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null)
        {
            Debug.LogError("MatchSpriteSize: SpriteRenderer にスプライトが設定されていません！");
            return;
        }

        // スプライトの元サイズ（ワールド単位・スケール1換算）
        Vector2 spriteSize = sr.sprite.bounds.size;  // e.g. (2, 3)

        // 目標幅に合わせる Scale を計算（アスペクト比維持）
        float scale = targetWidth / spriteSize.x;

        // オブジェクトの Scale を設定
        transform.localScale = new Vector3(scale, scale, 1f);

        // （オプション）Collider2D があれば、サイズを合わせる例
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = spriteSize;      // unscaled size
            bc.size *= scale;          // adjust to new scale
            bc.offset = Vector2.zero;  // center offset
        }
    }
}
