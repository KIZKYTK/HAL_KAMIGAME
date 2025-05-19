using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

/// <summary>
/// �擾�\�A�C�e���i��F�� / �t�b�N�j  
/// Player ���G�ꂽ�玩�g�� keyId �� PlayerController �ɓn���A���g��j������
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemEntity : MonoBehaviour
{
    [Header("���̃A�C�e�������� keyId")]
    public string keyId = "red";    // ��: "red", "blue" �Ȃ�

    /*--------------------------------------------------------------
     * �v���n�u�������Ɏ����ݒ�iCollider �� Trigger�ATag �� Item�j
     *------------------------------------------------------------*/
    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;      // �g���K�[�Ƃ��ċ@�\������
        gameObject.tag = "Item";    // Tag �Ŕ���p
    }

    /*--------------------------------------------------------------
     * Player ���G�ꂽ�u�ԂɌĂ΂��
     *------------------------------------------------------------*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;       // �v���C���[�łȂ���Ζ���

        Move_Player pc = other.GetComponent<Move_Player>();
        if (pc == null) return;                        // �O�̂��߃`�F�b�N

        pc.currentItemKey = keyId;                     // �v���C���[�� keyId ��n��
        Debug.Log($"{pc.name} ���� [{keyId}] ���擾");

        Destroy(gameObject);                           // �A�C�e��������
        // TODO: ������ SE �Đ��� UI �t�B�[�h�o�b�N��ǉ�����Ɨǂ�
    }
}
