using UnityEngine;

/// <summary>
/// ファスナー本体：状態に応じて Collider と透明度、Sprite を切替
/// </summary>
[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class System_Fastener : MonoBehaviour
{
    public enum State { Solid, Ghost }

    [Header("初期状態 (編集で変更可)")]
    [SerializeField] private State stateInEditor = State.Solid;

    [Header("状態ごとの Sprite")]
    public Sprite solidSprite;              // 実体用画像
    public Sprite ghostSprite;              // 虚体用画像

    [Header("状態切り替えのSE")]
    private SE sePlayer;

    public State CurrentState { get; private set; }
    public float slideSpeed = 8f;           // 取っ手側が参照用に使う場合あり

    EdgeCollider2D edgeCol;
    SpriteRenderer sr;

#if UNITY_EDITOR
    void OnValidate()                       // Inspector 値が変わった瞬間呼ばれる
    {
        if (!edgeCol) edgeCol = GetComponent<EdgeCollider2D>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        SetState(stateInEditor, true);      // その場で反映
    }
#endif

    void Awake()
    {
        edgeCol = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        sePlayer = GetComponentInChildren<SE>();
    }

    void Start() => SetState(stateInEditor, true); // 実行時初期化

    /*--- 外部からのトグル呼び出し ---*/
    public void Toggle() =>
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);

    /*--------------------------------------------------------------
     * 状態を適用：Collider・透明度・Sprite 全てここで管理
     *------------------------------------------------------------*/
    void SetState(State s, bool force = false)
    {
        if (!force && s == CurrentState) return;

        CurrentState = s;

        /* 当たり判定 ON/OFF */
        edgeCol.enabled = (s == State.Solid);

        /* 透明度変更 */
        var c = sr.color;
        c.a = (s == State.Solid ? 1f : 0.4f);
        sr.color = c;

        /* Sprite 切替（null チェック込み）*/
        if (s == State.Solid && solidSprite)
            sr.sprite = solidSprite;
        else if (s == State.Ghost && ghostSprite)
            sr.sprite = ghostSprite;

        // 効果音を再生（force = true の初期化時は鳴らさない）
        if (!force && sePlayer != null)
        {
            sePlayer.PlaySE();
        }
    }

    /// <summary>レール両端のワールド座標を返す</summary>
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
