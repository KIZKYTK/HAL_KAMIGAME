using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Fastener : MonoBehaviour
{
    /*========= Inspector �őI�� =========*/
    [Header("������� (�ҏW���[�h�ŕύX��)")]
    [SerializeField] private State stateInEditor = State.Solid;   // �v���_�E���\��

    /*========= �񋓌^ =========*/
    public enum State { Solid, Ghost }   // Solid��������L�� / Ghost�������薳��

    /*========= �p�����[�^ =========*/
    [Header("�X���C�h���x")]
    public float slideSpeed = 8f;        // ����肪���p����Q�l�l

    /*========= �v���p�e�B =========*/
    public State CurrentState { get; private set; }

    /*========= �L���b�V�� =========*/
    EdgeCollider2D edgeCol;              // ���ݔ���
    SpriteRenderer sr;                   // �����x�ύX

    /*--------------------------------------------------------------
     *  OnValidate : �G�f�B�^��Œl���ς�����瑦���f
     *------------------------------------------------------------*/
#if UNITY_EDITOR
    void OnValidate()
    {
        // �܂� Awake �O�̂��Ƃ�����̂Ŗ��� GetComponent
        if (edgeCol == null) edgeCol = GetComponent<EdgeCollider2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        SetState(stateInEditor, true);   // �������f
    }
#endif

    /*--------------------------------------------------------------*/
    void Awake()
    {
        edgeCol = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // �Q�[���J�n���� Inspector �Őݒ肳�ꂽ��Ԃŏ�����
        SetState(stateInEditor, true);
    }

    /*--------------------------------------------------------------
     *  ���J���\�b�h : ��ԃg�O��
     *------------------------------------------------------------*/
    public void Toggle()
    {
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);
    }

    /*--------------------------------------------------------------
     *  �������� : ��ԓK�p
     *------------------------------------------------------------*/
    void SetState(State newState, bool force = false)
    {
        if (!force && newState == CurrentState) return;

        CurrentState = newState;

        // �R���C�_�[�L���^����
        edgeCol.enabled = (newState == State.Solid);

        // �X�v���C�g�����x
        var c = sr.color;
        c.a = (newState == State.Solid ? 1f : 0.4f);
        sr.color = c;
    }

    /*--------------------------------------------------------------
     *  ���[�����[���W��Ԃ��i����肪�g�p�j
     *------------------------------------------------------------*/
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
