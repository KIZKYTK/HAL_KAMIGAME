using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ���A�C�e���FPlayer �� keyRing �� keyId ��ǉ����A���g��j��
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemEntity : MonoBehaviour
{
    [Header("���̌��� keyId")]
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
            Debug.Log($"{pc.name} ���� [{keyId}] ���擾");
        else
            Debug.Log($"�� [{keyId}] �͊��ɏ����ς�");

        Destroy(gameObject);
    }
}
