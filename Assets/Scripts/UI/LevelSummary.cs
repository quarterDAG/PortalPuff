using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;



public class LevelSummary : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject nextLevelButton;

    private void Awake ()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.levelSummary = this;
        LevelManager.Instance.ResetStars();
    }

    public void ShowLevelSummary ( bool isActive )
    {
        canvas.enabled = isActive;
        MainMenu.Instance.ShowPauseMenu(false);
        FadeOut();

        if (isActive)
        {
            EventSystem.current.SetSelectedGameObject(nextLevelButton);
        }
    }

    public void FadeIn ()
    {
        StartCoroutine(FadeScreen(0));
    }

    public void FadeOut ()
    {
        StartCoroutine(FadeScreen(0.3f));
    }

    private IEnumerator FadeScreen ( float targetAlpha )
    {
        float alpha = fadeImage.color.a;

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(alpha, targetAlpha, t));
            fadeImage.color = newColor;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }

    public void UpdateLevelText ( string text )
    {
        if (levelText != null)
            levelText.text = text;
    }

    public void SetStars ( int number )
    {
        StartCoroutine(ActivateStarsWithDelay(number));
    }

    private IEnumerator ActivateStarsWithDelay ( int number )
    {
        number = Mathf.Clamp(number, 0, stars.Length);

        for (int i = 0; i < number; i++)
        {
            stars[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance?.PlaySFX("StarUI");
        }

        for (int i = number; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }

    public void OpenPauseMenu ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        MainMenu.Instance?.ShowPauseMenu(true);
        TouchController.Instance?.ActivateTouch(false);
    }

    public void NextLevel ()
    {
        if (canvas.enabled)
        {
            canvas.enabled = false;
            AudioManager.Instance?.PlaySFX("Click");
            LevelManager.Instance.NextLevel();
            Debug.Log("NEXT");
        }

    }

    public void ResetLevel ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        MainMenu.Instance?.ReloadCurrentScene();
        LevelManager.Instance.ResetStars();
        AudioManager.Instance?.PlayMusic("TrickyFox");
    }
}
