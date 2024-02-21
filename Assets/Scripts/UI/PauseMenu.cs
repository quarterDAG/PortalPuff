using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField] private string levelMenu;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; }
    private Canvas pauseCanvas;
    [SerializeField] private GameObject optionsMenu;

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start ()
    {
        pauseCanvas = GetComponent<Canvas>();
    }

    public void ActivateCanvas ( bool _isActive )
    {
        if (_isActive)
            AudioManager.Instance?.StopMusic();
        else
            AudioManager.Instance?.PlayMusic("TrickyFox");

        TouchController.Instance?.ActivateTouch(!_isActive);
        PlayersManager.Instance?.FreezeAllPlayers(_isActive);
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
        ActivateCanvas(false);
        LevelManager.Instance?.LevelHasPlayed();
        ScreenFader.Instance?.FadeOut();

        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        await Task.Delay(1000);

        // Reload the current scene
        SceneManager.LoadScene(currentScene.buildIndex);

    }

    public void StartNewGame ()
    {
        ScreenFader.Instance?.FadeOut();
        LevelManager.Instance.LoadProgress();
        //LevelManager.Instance.SaveProgress();

        SceneManager.LoadScene(levelMenu);
    }

    public void Options ( bool isActive )
    {
        optionsMenu.SetActive(isActive);
    }

    public void QuitLevel ()
    {
        ActivateCanvas(false);
        ScreenFader.Instance?.FadeOut();
        SceneManager.LoadScene(levelMenu);
        AudioManager.Instance?.PlayMusic("TrickyFox");

    }

    public void ResetLevelsData ()
    {
        LevelManager.Instance.ResetLevelsData();
    }

    public void ExitGame ()
    {
#if UNITY_STANDALONE
    // Quit the application
    Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID || UNITY_IOS
        Application.Quit();
#endif
    }


}
