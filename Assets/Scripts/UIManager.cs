using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///     User Interface Manager (text objects in top of game screen).
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager _instance = null;
    public static UIManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    /// <summary>UI element for displaying scores.</summary>
    public Text Scores;
    /// <summary>UI element for displaying player lives.</summary>
    public Text Lives;
    /// <summary>UI element for displaying level count.</summary>
    public Text LevelCount;

    public Button ButtonSave;
    public Button ButtonClose;
    public InputField InputUserName;

    /// <summary>
    ///     Start method by Unity Engine.
    /// </summary>
    private void Start()
    {
        Block.OnBlockDestroy += OnBlockDestroyed;
        GameManager.OnPlayerLooseLive += OnPlayerLooseLive;
        BlocksManager.OnLevelLoaded += OnLevelLoaded;
    }

    /// <summary>
    ///     Disable method by Unity Engine
    /// </summary>
    private void OnDisable()
    {
        Block.OnBlockDestroy -= OnBlockDestroyed;
        GameManager.OnPlayerLooseLive -= OnPlayerLooseLive;
        BlocksManager.OnLevelLoaded -= OnLevelLoaded;
    }

    /// <summary>
    ///     Method whitch subscribe block destruction event.
    /// </summary>
    /// <seealso cref="Block.OnBlockDestroy"/>
    private void OnBlockDestroyed(Block obj)
    {
        // Updating game scores
        GameManager.Instance._gameScores++;

        // Update suitable unity's Text object
        this.UpdateScoresText();
    }


    /// <summary>
    ///     Callback.
    /// </summary>
    private void OnPlayerLooseLive()
    {
        UpdateLives();
    }

    /// <summary>
    ///     Callback.
    /// </summary>
    private void OnLevelLoaded()
    {
        UpdateLevelCounter();
    }

    /// <summary>
    ///     Update UI <c>Scores</c> with correct format (6 digits).
    /// </summary>
    public void UpdateScoresText()
    {
        Scores.text = string.Format("{0:D6}", GameManager.Instance._gameScores);
    }

    /// <summary>
    ///     Update UI <c>Lives</c> with heart symbols.
    /// </summary>
    public void UpdateLives()
    {
        // Getting current lives
        var livesCount = GameManager.Instance._playerLives;

        // Formattig text
        string result = "";
        if (livesCount > 0)
        {
            for (byte i = 0; i < livesCount; i++)
            {
                result += "♥";
            }
        }

        // Update
        Lives.text = result;
    }

    /// <summary>
    ///     Update UI <c>LevelCount</c>.
    /// </summary>
    public void UpdateLevelCounter()
    {
        // Update text
        LevelCount.text = string.Format("{0:D2}/{1:D2}" , GameManager.Instance._currentLevel ,
            BlocksManager.Instance.LevelBlocksCount);
    }

    public void AddNewRecord()
    {
        Record rec = new Record();
        rec.Name = InputUserName.text;
        rec.Scores = GameManager.Instance._gameScores;
        rec.Timestamp = new DateTime(1970, 1, 1).Ticks;
        GameManager.Instance.AddRecord(rec , true);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
