using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI worldTitle;
    [SerializeField] private TextMeshProUGUI worldName;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Sprite[] backgroundImages;

    [SerializeField] private Transform topRow;
    [SerializeField] private Transform bottomRow;
    [SerializeField] private int selectedWorld;

    [SerializeField] private ScreenFader screenFader;

    [SerializeField] private Button previousWorldButton;
    [SerializeField] private Button nextWorldButton;

    private LevelData lastUnlockedLevel;

    private void Start ()
    {
        lastUnlockedLevel = LevelManager.Instance.GetLastUnlockedLevel();
        selectedWorld = lastUnlockedLevel.worldNumber;

        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            BackToMainMenu();
    }

    public void BackToMainMenu ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        ScreenFader.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("Main Menu");
            ScreenFader.Instance?.FadeIn();

        });

    }

    public void NextWorld ()
    {
        if (selectedWorld >= LevelManager.Instance._worlds.Count) return;

        AudioManager.Instance?.PlaySFX("Click");

        selectedWorld++;
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    public void PreviousWorld ()
    {
        if (selectedWorld == 1) return;

        AudioManager.Instance?.PlaySFX("Click");

        selectedWorld--;
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    public void UnlockAll ()
    {
        LevelManager.Instance.UnlockAll();
    }

    public void UpdateWorldUI ()
    {
        int indexWorld = selectedWorld - 1;


        backgroundImage.sprite = backgroundImages[indexWorld];

        LevelData lastLevelInTheSelectedWorld = LevelManager.Instance.GetLastUnlockedLevel(indexWorld);
        GameObject lastLevelButton = LevelManager.Instance.GetButtonOfLevelData(lastLevelInTheSelectedWorld);

        EventSystem.current.SetSelectedGameObject(lastLevelButton);

        switch (selectedWorld)
        {
            case 1:
                worldTitle.text = "WORLD 1";
                worldName.text = "The Laboratory";
                previousWorldButton.gameObject.SetActive(false);
                nextWorldButton.gameObject.SetActive(true);

                if (LevelManager.Instance._worlds[1].isLocked)
                    nextWorldButton.interactable = false;
                else
                    nextWorldButton.interactable = true;

                break;

            case 2:
                worldTitle.text = "WORLD 2";
                worldName.text = "The Sewers";

                previousWorldButton.gameObject.SetActive(true);
                previousWorldButton.interactable = true;
                nextWorldButton.gameObject.SetActive(true);

                if (LevelManager.Instance._worlds[2].isLocked)
                    nextWorldButton.interactable = false;
                else
                    nextWorldButton.interactable = true;

                break;

            case 3:
                worldTitle.text = "WORLD 3";
                worldName.text = "The Swamp";

                previousWorldButton.interactable = true;

                nextWorldButton.gameObject.SetActive(false);

                break;

            default:
                worldTitle.text = "UNKNOWN WORLD";
                worldName.text = "Unknown";
                break;
        }

    }


}
