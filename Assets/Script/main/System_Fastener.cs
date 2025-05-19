using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Fastener : MonoBehaviour
{
    /*========= Inspector で選択 =========*/
    [Header("初期状態 (編集モードで変更可)")]
    [SerializeField] private State stateInEditor = State.Solid;   // プルダウン表示

    /*========= 列挙型 =========*/
    public enum State { Solid, Ghost }   // Solid＝当たり有効 / Ghost＝当たり無効

    /*========= パラメータ =========*/
    [Header("スライド速度")]
    public float slideSpeed = 8f;        // 取っ手が利用する参考値

    /*========= プロパティ =========*/
    public State CurrentState { get; private set; }

    /*========= キャッシュ =========*/
    EdgeCollider2D edgeCol;              // 踏み判定
    SpriteRenderer sr;                   // 透明度変更

    /*--------------------------------------------------------------
     *  OnValidate : エディタ上で値が変わったら即反映
     *------------------------------------------------------------*/
#if UNITY_EDITOR
    void OnValidate()
    {
        // まだ Awake 前のことがあるので毎回 GetComponent
        if (edgeCol == null) edgeCol = GetComponent<EdgeCollider2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        SetState(stateInEditor, true);   // 強制反映
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
        // ゲーム開始時に Inspector で設定された状態で初期化
        SetState(stateInEditor, true);
    }

    /*--------------------------------------------------------------
     *  公開メソッド : 状態トグル
     *------------------------------------------------------------*/
    public void Toggle()
    {
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);
    }

    /*--------------------------------------------------------------
     *  内部実装 : 状態適用
     *------------------------------------------------------------*/
    void SetState(State newState, bool force = false)
    {
        if (!force && newState == CurrentState) return;

        CurrentState = newState;

        // コライダー有効／無効
        edgeCol.enabled = (newState == State.Solid);

        // スプライト透明度
        var c = sr.color;
        c.a = (newState == State.Solid ? 1f : 0.4f);
        sr.color = c;
    }

    /*--------------------------------------------------------------
     *  レール両端座標を返す（取っ手が使用）
     *------------------------------------------------------------*/
    public (Vector2, Vector2) GetEndPoints()
    {
        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);
        return (p0, p1);
    }
}
