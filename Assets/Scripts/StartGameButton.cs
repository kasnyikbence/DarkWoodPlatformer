using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public int gameStartScene;

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
}
