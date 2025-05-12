using System.Collections;                    // C# �� IEnumerator �Ȃǂ��g�p
using UnityEngine;                           // UnityEngine API

/// <summary>
/// �t�@�X�i�[�{�̐���                                        // �N���X�̖ړI
/// �EEdgeCollider2D �Ńv���C���[�����߂�^���߂Ȃ���ؑ�   // ��@�\1
/// �E���[���W�𑼃X�N���v�g�֒�                          // ��@�\2
/// TODO: �K�v�Ȃ特�E�G�t�F�N�g�Ǘ��������ōs��            // �g���|�C���g
/// </summary>
[RequireComponent(typeof(EdgeCollider2D),      // �K�{: ���ݔ���
                  typeof(SpriteRenderer))]     // �K�{: ���X�v���C�g
public class FastenerController : MonoBehaviour
{
    /*===== ���J�񋓌^ =====*/
    public enum State { Solid, Ghost }       // Solid=���� / Ghost=����

    /*===== �C���X�y�N�^�\�� =====*/
    [Header("�X���C�h���x (unit/�b)")]
    public float slideSpeed = 8f;            // TODO: ��Փx�ʂɕς���ꍇ�����𒲐�

    /*===== �v���p�e�B =====*/
    public State CurrentState { get; private set; } = State.Solid; // �����

    /*=====  =====*/
    private EdgeCollider2D edgeCol;          // ���߂�R���C�_�[
    private SpriteRenderer sr;               // �����x�ύX�p

    /*----------------- ���C�t�T�C�N�� -----------------*/
    void Awake()                             // �Q�[���J�n���Ɉ�x�Ă΂��
    {
        edgeCol = GetComponent<EdgeCollider2D>(); // Component �擾
        sr = GetComponent<SpriteRenderer>();

        if (edgeCol == null)                 // Error �`�F�b�N
            Debug.LogError("[Fastener] EdgeCollider2D ��������܂���");

        SetState(State.Solid, true);         // �������i���̏�ԁj
    }

    /*----------------- ���J API -----------------*/
    /// <summary>����?���̂��g�O������</summary>
    public void Toggle()
    {
        // TODO: SE �Đ���A�j���ؑւ�ǉ��������ꍇ������
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);
    }

    /*----------------- ��Ԑݒ� -----------------*/
    private void SetState(State newState, bool force = false)
    {
        if (!force && newState == CurrentState) return; // �ω��Ȃ��Ȃ�I��

        CurrentState = newState;            // ��ԍX�V

        edgeCol.enabled = (newState == State.Solid); // ���̂Ȃ瓖����L��

        // �X�v���C�g���ߓx�����i����=�s���� / ����=�������j
        var c = sr.color;                   // ���݂̐F�擾
        c.a = (newState == State.Solid ? 1f : 0.4f);
        sr.color = c;

        // TODO: �p�[�e�B�N�����̉��o��ǉ�����Ȃ炱�̉���
    }

    /*-----------------  -----------------*/
    /// <summary>
    /// EdgeCollider2D �̗��[���[���h���W��Ԃ�
    /// ���X�N���v�g�iHandleController�j�����[���I�[��m�邽�߂Ɏg�p
    /// </summary>
    public (Vector2 p0, Vector2 p1) GetEndPoints()
    {
        if (edgeCol == null)                // ������ null �̏ꍇ
            return (Vector2.zero, Vector2.zero);

        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);       // 0�Ԗ�
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);      // �Ō�
        return (p0, p1);
    }
}
