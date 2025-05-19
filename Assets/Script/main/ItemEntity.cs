using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

/// <summary>
/// 取得可能アイテム（例：鍵 / フック）  
/// Player が触れたら自身の keyId を PlayerController に渡し、自身を破棄する
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemEntity : MonoBehaviour
{
    [Header("このアイテムが持つ keyId")]
    public string keyId = "red";    // 例: "red", "blue" など

    /*--------------------------------------------------------------
     * プレハブ生成時に自動設定（Collider → Trigger、Tag → Item）
     *------------------------------------------------------------*/
    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;      // トリガーとして機能させる
        gameObject.tag = "Item";    // Tag で判定用
    }

    /*--------------------------------------------------------------
     * Player が触れた瞬間に呼ばれる
     *------------------------------------------------------------*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;       // プレイヤーでなければ無視

        Move_Player pc = other.GetComponent<Move_Player>();
        if (pc == null) return;                        // 念のためチェック

        pc.currentItemKey = keyId;                     // プレイヤーへ keyId を渡す
        Debug.Log($"{pc.name} が鍵 [{keyId}] を取得");

        Destroy(gameObject);                           // アイテムを消去
        // TODO: ここで SE 再生や UI フィードバックを追加すると良い
    }
}
