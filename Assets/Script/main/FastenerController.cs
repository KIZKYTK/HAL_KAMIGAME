using System.Collections;                    // C# の IEnumerator などを使用
using UnityEngine;                           // UnityEngine API

/// <summary>
/// ファスナー本体制御                                        // クラスの目的
/// ・EdgeCollider2D でプレイヤーが踏める／踏めないを切替   // 主機能1
/// ・両端座標を他スクリプトへ提供                          // 主機能2
/// TODO: 必要なら音・エフェクト管理もここで行う            // 拡張ポイント
/// </summary>
[RequireComponent(typeof(EdgeCollider2D),      // 必須: 踏み判定
                  typeof(SpriteRenderer))]     // 必須: 可視スプライト
public class FastenerController : MonoBehaviour
{
    /*===== 公開列挙型 =====*/
    public enum State { Solid, Ghost }       // Solid=実体 / Ghost=虚体

    /*===== インスペクタ表示 =====*/
    [Header("スライド速度 (unit/秒)")]
    public float slideSpeed = 8f;            // TODO: 難易度別に変える場合ここを調整

    /*===== プロパティ =====*/
    public State CurrentState { get; private set; } = State.Solid; // 現状態

    /*=====  =====*/
    private EdgeCollider2D edgeCol;          // 踏めるコライダー
    private SpriteRenderer sr;               // 透明度変更用

    /*----------------- ライフサイクル -----------------*/
    void Awake()                             // ゲーム開始時に一度呼ばれる
    {
        edgeCol = GetComponent<EdgeCollider2D>(); // Component 取得
        sr = GetComponent<SpriteRenderer>();

        if (edgeCol == null)                 // Error チェック
            Debug.LogError("[Fastener] EdgeCollider2D が見つかりません");

        SetState(State.Solid, true);         // 初期化（実体状態）
    }

    /*----------------- 公開 API -----------------*/
    /// <summary>実体?虚体をトグルする</summary>
    public void Toggle()
    {
        // TODO: SE 再生やアニメ切替を追加したい場合ここで
        SetState(CurrentState == State.Solid ? State.Ghost : State.Solid);
    }

    /*----------------- 状態設定 -----------------*/
    private void SetState(State newState, bool force = false)
    {
        if (!force && newState == CurrentState) return; // 変化なしなら終了

        CurrentState = newState;            // 状態更新

        edgeCol.enabled = (newState == State.Solid); // 実体なら当たり有効

        // スプライト透過度調整（実体=不透明 / 虚体=半透明）
        var c = sr.color;                   // 現在の色取得
        c.a = (newState == State.Solid ? 1f : 0.4f);
        sr.color = c;

        // TODO: パーティクル等の演出を追加するならこの下に
    }

    /*-----------------  -----------------*/
    /// <summary>
    /// EdgeCollider2D の両端ワールド座標を返す
    /// 他スクリプト（HandleController）がレール終端を知るために使用
    /// </summary>
    public (Vector2 p0, Vector2 p1) GetEndPoints()
    {
        if (edgeCol == null)                // 万が一 null の場合
            return (Vector2.zero, Vector2.zero);

        Vector2 p0 = transform.TransformPoint(edgeCol.points[0]);       // 0番目
        Vector2 p1 = transform.TransformPoint(edgeCol.points[^1]);      // 最後
        return (p0, p1);
    }
}
