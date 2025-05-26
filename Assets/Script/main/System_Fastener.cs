using UnityEngine;

/// <summary>
/// ファスナー本体：Collider 有効 / 無効 と透明度を切替。  
/// Inspector のプルダウンで初期状態（Solid / Ghost）を選べる。
/// </summary>
[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class System_Fastener : MonoBehaviour
{
    public enum State { Solid, Ghost }

    [Header("初期状態 (編集で変更可)")]
    [SerializeField] private State stateInEditor = State.Solid;

    public State CurrentState { get; private set; }
    public float slideSpeed = 8f;          // 取っ手側が参照する場合がある

    EdgeCollider2D edgeCol;
    SpriteRenderer sr;

#if UNITY_EDITOR
    void OnValidate()                      // エディタで値を変えた瞬間に呼ばれる
    {
        if (!edgeCol) edgeCol = GetComponent<EdgeCollider2D>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        SetState(stateInEditor, true);     // その場で反映
    }
#endif

    void Awake()
    {
        edgeCol = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() => SetState(stateInEditor, true);      // 実行時に初期化

    public void Toggle() =>
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);

    void SetState(State s, bool force = false)
    {
        if (!force && s == CurrentState) return;
        CurrentState = s;
        edgeCol.enabled = (s == State.Solid);           // 当たり判定
        var c = sr.color; c.a = (s == State.Solid ? 1f : 0.4f); sr.color = c;
    }

    /// <summary>レール両端のワールド座標を返す</summary>
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
