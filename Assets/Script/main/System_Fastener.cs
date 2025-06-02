using UnityEngine;

/// <summary>
/// �t�@�X�i�[�{�́FCollider �L�� / ���� �Ɠ����x��ؑցB  
/// Inspector �̃v���_�E���ŏ�����ԁiSolid / Ghost�j��I�ׂ�B
/// </summary>
[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class System_Fastener : MonoBehaviour
{
    public enum State { Solid, Ghost }

    [Header("������� (�ҏW�ŕύX��)")]
    [SerializeField] private State stateInEditor = State.Solid;

    public State CurrentState { get; private set; }
    public float slideSpeed = 8f;          // ����葤���Q�Ƃ���ꍇ������

    EdgeCollider2D edgeCol;
    SpriteRenderer sr;

#if UNITY_EDITOR
    void OnValidate()                      // �G�f�B�^�Œl��ς����u�ԂɌĂ΂��
    {
        if (!edgeCol) edgeCol = GetComponent<EdgeCollider2D>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        SetState(stateInEditor, true);     // ���̏�Ŕ��f
    }
#endif

    void Awake()
    {
        edgeCol = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() => SetState(stateInEditor, true);      // ���s���ɏ�����

    public void Toggle() =>
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);

    void SetState(State s, bool force = false)
    {
        if (!force && s == CurrentState) return;
        CurrentState = s;
        edgeCol.enabled = (s == State.Solid);           // �����蔻��
        var c = sr.color; c.a = (s == State.Solid ? 1f : 0.4f); sr.color = c;
    }

    /// <summary>���[�����[�̃��[���h���W��Ԃ�</summary>
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
