using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 鍵アイテム：Player の keyRing に keyId を追加し、自身を破棄
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemEntity : MonoBehaviour
{
    [Header("この鍵の keyId")]
    public string keyId = "red";

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.tag = "Item";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Move_Player pc = other.GetComponent<Move_Player>();
        if (pc == null) return;

        if (pc.keyRing.Add(keyId))
            Debug.Log($"{pc.name} が鍵 [{keyId}] を取得");
        else
            Debug.Log($"鍵 [{keyId}] は既に所持済み");

        Destroy(gameObject);
    }
}
