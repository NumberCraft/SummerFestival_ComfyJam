using UnityEngine;

public class MenuButtonsManager : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.i.StartGame();
    }

    public void ExitToMainMenu()
    {
        //Time.timeScale = 1f;
        GameManager.i.ExitToMainMenu();
    }

    public void QuitGame()
    {
        GameManager.i.QuitGame();
    }
}
