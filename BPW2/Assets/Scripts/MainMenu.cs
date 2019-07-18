using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        Controller.Instance.LoadNextLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
