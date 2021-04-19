using UnityEngine.SceneManagement;
using UnityEngine;

public class SmartGame1GM : MonoBehaviour
{
    public void CloseScene()
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync("SmartGame_1");
    }

    public void Win()
    {
        CloseScene();
        Head.Instance.HackOver(true);
    }

    public void Fail()
    {
        CloseScene();
        Head.Instance.HackOver(false);
    }
}
