using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;  // Inspector �Őݒ�

    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }
}
