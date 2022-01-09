using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenBehaviour : MainMenuBehaviour
{
    public static bool paused;

    public GameObject pauseMenu;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;

        Time.timeScale = (paused) ? 0 : 1;
        pauseMenu.SetActive(paused);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPauseMenu(false);
    }
}
