using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string levelMenu;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; }
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private ScreenFader screenFader;

    public void ActivateCanvas ( bool _isActive )
    {
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
        screenFader.FadeOut();

        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        await Task.Delay(1000);

        // Reload the current scene
        SceneManager.LoadScene(currentScene.buildIndex);

    }

    public void StartNewGame ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene(levelMenu);
    }

    public void QuitLevel ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene(levelMenu);
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
