using System.Linq;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    private float previousTimescale;
    private CursorLockMode previousLockMode;
    private bool previousVisibility;

    public static bool isPaused { get; set; }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseAction();
        }
    }

    private void PauseAction()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        previousTimescale = Time.timeScale;
        previousLockMode = Cursor.lockState;
        previousVisibility = Cursor.visible;

        pauseMenu.SetActive(true);

        //Time.timeScale = 0f;

        var targets = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IPausable>();

        foreach (var target in targets)
        {
            target.Pause();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        //Time.timeScale = previousTimescale;

        var targets = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IPausable>();

        foreach (var target in targets)
        {
            target.Continue();
        }

        Cursor.lockState = previousLockMode;
        Cursor.visible = previousVisibility;

        isPaused = false;
    }

    public void Restart()
    {
        //Time.timeScale = 1f;
        GameManager.i.Restart();
    }
}
