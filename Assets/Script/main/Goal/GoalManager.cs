using UnityEngine;
using UnityEngine.SceneManagement; // シーンの読み込みに必要

public class GoalManager : MonoBehaviour
{
    // シーンを移動するためのターゲットシーン名
    public string targetSceneName;

    // プレイヤーのレイヤーと接触したとき
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player"レイヤーに属するオブジェクトか確認
        if (other.CompareTag("Player"))
        {
            // シーンのロード
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
