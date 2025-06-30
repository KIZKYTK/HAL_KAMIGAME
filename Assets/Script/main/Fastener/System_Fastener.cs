using UnityEngine;

/// <summary>
/// �t�@�X�i�[�{�́F��Ԃɉ����� Collider �Ɠ����x�ASprite ��ؑ�
/// </summary>
[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class System_Fastener : MonoBehaviour
{
    public enum State { Solid, Ghost }

    [Header("������� (�ҏW�ŕύX��)")]
    [SerializeField] private State stateInEditor = State.Solid;

    [Header("��Ԃ��Ƃ� Sprite")]
    public Sprite solidSprite;              // ���̗p�摜
    public Sprite ghostSprite;              // ���̗p�摜

    [Header("��Ԑ؂�ւ���SE")]
    private SE sePlayer;

    public State CurrentState { get; private set; }
    public float slideSpeed = 8f;           // ����葤���Q�Ɨp�Ɏg���ꍇ����

    EdgeCollider2D edgeCol;
    SpriteRenderer sr;

#if UNITY_EDITOR
    void OnValidate()                       // Inspector �l���ς�����u�ԌĂ΂��
    {
        if (!edgeCol) edgeCol = GetComponent<EdgeCollider2D>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        SetState(stateInEditor, true);      // ���̏�Ŕ��f
    }
#endif

    void Awake()
    {
        edgeCol = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        sePlayer = GetComponentInChildren<SE>();
    }

    void Start() => SetState(stateInEditor, true); // ���s��������

    /*--- �O������̃g�O���Ăяo�� ---*/
    public void Toggle() =>
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);

    /*--------------------------------------------------------------
     * ��Ԃ�K�p�FCollider�E�����x�ESprite �S�Ă����ŊǗ�
     *------------------------------------------------------------*/
    void SetState(State s, bool force = false)
    {
        if (!force && s == CurrentState) return;

        CurrentState = s;

        /* �����蔻�� ON/OFF */
        edgeCol.enabled = (s == State.Solid);

        /* �����x�ύX */
        var c = sr.color;
        c.a = (s == State.Solid ? 1f : 0.4f);
        sr.color = c;

        /* Sprite �ؑցinull �`�F�b�N���݁j*/
        if (s == State.Solid && solidSprite)
            sr.sprite = solidSprite;
        else if (s == State.Ghost && ghostSprite)
            sr.sprite = ghostSprite;

        // ���ʉ����Đ��iforce = true �̏��������͖炳�Ȃ��j
        if (!force && sePlayer != null)
        {
            sePlayer.PlaySE();
        }
    }

    /// <summary>���[�����[�̃��[���h���W��Ԃ�</summary>
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
