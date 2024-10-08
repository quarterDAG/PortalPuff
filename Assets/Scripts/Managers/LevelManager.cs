using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public List<GameObject> levelButtons = new List<GameObject>();

    [SerializeField] private WorldDefaultState worldsDefaultState;

    public List<WorldData> _worlds;

    public GameObject levelButtonPrefab;

    public int currentLevelNumber = 0;
    public int currentWorldIndex = 0;
    private int currentLevelStarsCollected;

    public LevelSummary levelSummary;


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


    public void InitializeLevelMenu ( Transform topRowContainer, Transform bottomRowContainer, int selectedWorld )
    {
        if (selectedWorld < 0 || selectedWorld > _worlds.Count || !_worlds[selectedWorld - 1].levels.Any())
        {
            Debug.LogWarning("Selected world is out of range or contains no levels.");
            return;
        }


        currentWorldIndex = selectedWorld - 1;
        levelButtons.Clear();

        foreach (Transform child in topRowContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in bottomRowContainer)
        {
            Destroy(child.gameObject);
        }

        int levelIndex = 0;

        for (int i = 0; i < _worlds[currentWorldIndex].levels.Count; i++)
        {
            if (_worlds[currentWorldIndex].levels[i].worldNumber == selectedWorld)
            {
                Transform parentContainer = levelIndex < 5 ? topRowContainer : bottomRowContainer;

                GameObject buttonObj = Instantiate(levelButtonPrefab, parentContainer);

                LevelButton button = buttonObj.GetComponent<LevelButton>();

                button.SetLevelData(_worlds[currentWorldIndex].levels[i].levelNumber, _worlds[currentWorldIndex].levels[i].isLocked, _worlds[currentWorldIndex].levels[i].starsEarned, _worlds[currentWorldIndex].levels[i].sceneName);

                button.LevelData = _worlds[currentWorldIndex].levels[i];

                levelButtons.Add(buttonObj);

                levelIndex++;

                if (levelIndex >= 10) break;
            }
        }


    }

    public void UpdateLevelSummary ()
    {
        if (levelSummary == null) return;

        string levelText = $"LEVEL {currentLevelNumber}";

        levelSummary.UpdateLevelText(levelText);
        levelSummary.SetStars(currentLevelStarsCollected);

        levelSummary.ShowLevelSummary(true);
    }

    public void UnlockLevelBySceneName ( string sceneName )
    {
        foreach (LevelData level in _worlds[currentWorldIndex].levels)
        {
            if (level.sceneName == sceneName)
            {
                level.isLocked = false;
                break;
            }
        }

        SaveProgress();
    }

    public void UnlockNextLevel ()
    {
        LevelData nextLevel = GetLevelDataByNumber(currentLevelNumber + 1);
        if (nextLevel != null)
            nextLevel.isLocked = false;

        SaveProgress();
    }

    public void LoadProgress ()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SaveDataCollection collection = JsonUtility.FromJson<SaveDataCollection>(json);
            _worlds = collection.worlds;
        }
        else
        {
            Debug.LogWarning("Save file not found, loading default data.");
            // Handle case where no save file exists
        }
    }

    public void SaveProgress ()
    {
        SaveDataCollection collection = new SaveDataCollection
        {
            worlds = _worlds,
        };

        string json = JsonUtility.ToJson(collection);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void DeleteSaveFile ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            ResetLevelsData();
        }
    }

    public void ResetLevelsData ()
    {
        _worlds = worldsDefaultState.worlds;

        SaveProgress();
    }

    public void SetWorldsDefaultState ( WorldDefaultState _defaultState )
    {
        worldsDefaultState = _defaultState;
    }


    public void CollectStar ()
    {
        currentLevelStarsCollected++;
    }

    public void CompleteLevel ()
    {
        LevelData currentLevel = GetLevelDataByNumber(currentLevelNumber);
        currentLevel.isTutorialShown = true;

        if (currentLevelStarsCollected > currentLevel.starsEarned)
            currentLevel.starsEarned = currentLevelStarsCollected;


        switch (currentLevelNumber)
        {
            case 10:
                UnlockWorld(1);
                break;

            case 20:
                UnlockWorld(2);
                break;
        }


        UpdateLevelSummary();

        SaveProgress();
    }


    public void NextLevel ()
    {
        ScreenFader.Instance?.FadeOut(() =>
        {
            if (currentLevelNumber != 30)
            {
                string nextLevelName = GetLevelDataByNumber(currentLevelNumber + 1).sceneName;
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                SceneManager.LoadScene("Thank You");
            }

            AudioManager.Instance?.PlayMusic("TrickyFox");
            currentLevelNumber++;
            ResetStars();
            ScreenFader.Instance?.FadeIn();
        });
    }

    public void ResetStars ()
    {
        currentLevelStarsCollected = 0;
    }

    public void UnlockWorld ( int worldToUnlock )
    {
        _worlds[worldToUnlock].isLocked = false;
    }

    public void UnlockAll ()
    {
        foreach (var world in _worlds)
        {
            world.isLocked = false;
            foreach (var level in world.levels)
            {
                level.isLocked = false;
            }
        }

        SaveProgress();
        SceneManager.LoadScene("Level Menu");

    }

    public void LevelHasPlayed ()
    {
        GetLevelDataByNumber(currentLevelNumber).isTutorialShown = false;
    }

    public LevelData GetLevelDataByNumber ( int levelNumber )
    {
        foreach (WorldData world in _worlds)
        {
            foreach (LevelData level in world.levels)
            {
                if (level.levelNumber == levelNumber)
                {
                    return level;
                }
            }
        }

        return null;
    }

    public LevelData GetLastUnlockedLevel ( int? worldIndex = null )
    {
        LevelData lastUnlockedLevel = null;

        // If a specific world index is provided, restrict the search to that world.
        if (worldIndex.HasValue)
        {
            if (worldIndex.Value >= 0 && worldIndex.Value < _worlds.Count)
            {
                WorldData specificWorld = _worlds[worldIndex.Value];
                foreach (LevelData level in specificWorld.levels)
                {
                    if (!level.isLocked)
                    {
                        if (lastUnlockedLevel == null || level.levelNumber > lastUnlockedLevel.levelNumber)
                        {
                            lastUnlockedLevel = level;
                        }
                    }
                }
            }
        }
        else
        {
            // The original behavior, searching through all worlds.
            foreach (WorldData world in _worlds)
            {
                if (world.isLocked) continue;

                foreach (LevelData level in world.levels)
                {
                    if (!level.isLocked)
                    {
                        if (lastUnlockedLevel == null || level.levelNumber > lastUnlockedLevel.levelNumber)
                        {
                            lastUnlockedLevel = level;
                        }
                    }
                }
            }
        }

        return lastUnlockedLevel;
    }

    public GameObject GetButtonOfLevelData ( LevelData _levelData )
    {
        if (_levelData == null) return null;

        foreach (var levelButton in levelButtons)
        {
            LevelButton levelButtonScript = levelButton.GetComponent<LevelButton>();
            if (levelButtonScript.LevelData == _levelData)
            {
                return levelButton;
            }
        }

        return null;
    }
}



[System.Serializable]
public class SaveDataCollection
{
    public List<WorldData> worlds;
}

[System.Serializable]
public class WorldData
{
    public int worldNumber;
    public List<LevelData> levels;
    public bool isLocked = true;

}

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public string sceneName;
    public int worldNumber;
    public bool isLocked = true;
    public int starsEarned;
    public bool isTutorialShown;
    public bool isDialogShown;
}
