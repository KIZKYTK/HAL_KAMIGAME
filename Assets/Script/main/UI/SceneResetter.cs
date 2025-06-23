using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetter : MonoBehaviour
{
    // Œ»İ‚ÌƒV[ƒ“‚ğÄ“Ç‚İ‚İ‚µ‚Ä‰Šú‰»‚·‚é
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
