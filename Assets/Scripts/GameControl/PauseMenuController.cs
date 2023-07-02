using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button onActivationSelectable;
    [SerializeField] private GameObject pauseMenu;

    public bool IsPaused { get; private set; }
    public static PauseMenuController Instance { get; private set; }

    private void Start()
    {
        PauseInputListener.OnPausePressed += OnPausePressed;
        Instance = this;
    }

    private void OnPausePressed(bool isActive)
    {
        IsPaused = !IsPaused;
        if (IsPaused)
        {
            pauseMenu.SetActive(isActive);
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(onActivationSelectable.gameObject);
        }
        else
        {
            Continue();
        }
    }

    public void Continue()
    {
        IsPaused = false;
        pauseMenu.SetActive(false);
        PlayerManager.Instance.NextSelectable();
        Time.timeScale = 1;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}