using Assets.Scripts.Data;
using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

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

    public GameObject EndScreen;
    public Image GameEndPanel;
    public Color[] GameEndPanelColors;
    public Text _gameTextMessage;
    public int _currentLevel = 0;
    public byte _playerLives;
    public int _gameScores;
    public static TextAsset RecordsFile;

    public List<Record> Records;

    /// <summary>Game state flag.</summary>
    private GameState _gameStateFlag;

    public static event Action OnPlayerLooseLive;

    #region Class constants
    /// <summary>Text displayer on END SCRREN if player win.</summary>
    private const string GAMEEND_VICTORY_TEXT = "ZWYCIESTWO!";
    /// <summary>Text displayer on END SCRREN if player loose.</summary>
    private const string GAMEEND_DEATH_TEXT = "PRZEGRYWASZ";
    /// <summary>Text tp display in game area in stop mode.</summary>
    private const string STATE_STOPPED_TEXT = "Nacisnij, aby zaczac!";
    /// <summary>Text to display in game area in pause mode.</summary>
    private const string STATE_PAUSED_TEXT = "Nacisnij, aby wznowic!";
    /// <summary>Initial ammount of player lifes.</summary>
    public const byte INITIAL_LIVES = 3;
    /// <summary>Limit of records saved to file.</summary>
    public const int STATS_LIMIT = 10;
    #endregion

    /// <summary>
    ///     Start method by Unity Engine.
    /// </summary>
    private void Start()
    {
        Records = LoadRecords();

        Screen.SetResolution(1080 , 1920 , true);
        GameEndPanel.GetComponent<Image>();
        _playerLives = INITIAL_LIVES;
        _gameScores = 0;

        SetState(GameState.GS_STOPPED);

        Ball.OnBallDie += OnBallDie;
        Block.OnBlockDestroy += this.OnBlockDestroy;

        UIManager.Instance.UpdateScoresText();
        UIManager.Instance.UpdateLives();
    }

    /// <summary>
    ///     Disable method by Unity Engine.
    /// </summary>
    private void OnDisable()
    {
        Ball.OnBallDie -= OnBallDie;
        Block.OnBlockDestroy -= this.OnBlockDestroy;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public static string GetAppDataPath()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            return "/storage/emulated/0/Arkadroid_Data";
        #else
            return Application.persistentDataPath + "/Data";
        #endif
    }

    /// <summary>
    ///     Method whitch subscribe block destruction event.
    /// </summary>
    /// <see cref="Block.OnBlockDestroy"/>
    private void OnBlockDestroy(Block obj)
    {
        if (BlocksManager.Instance.LevelBlocks.Count <= 0)
        {
            BallsManager.Instance.ResetBalls();
            this._gameStateFlag = GameState.GS_STOPPED;
            BlocksManager.Instance.LoadNext();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnBallDie(Ball obj)
    {
        if (BallsManager.Instance.BallsCollection.Count <= 0)
        {
            --_playerLives;
            OnPlayerLooseLive?.Invoke();

            if (_playerLives <= 0)
            {
                this.ShowGameEndScreen(false);
            }
            else
            {
                BallsManager.Instance.ResetBalls();
                _gameStateFlag = GameState.GS_STOPPED;
            }
        }
    }

    public void SetState(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GS_STOPPED:
                _gameTextMessage.text = STATE_STOPPED_TEXT;
                _gameTextMessage.enabled = true;
                break;
            case GameState.GS_PAUSED:
                _gameTextMessage.text = STATE_PAUSED_TEXT;
                _gameTextMessage.enabled = true;
                break;
            default:
                _gameTextMessage.enabled = false;
                break;
        }

        this._gameStateFlag = gameState;
    }

    public GameState GetState()
    {
        return this._gameStateFlag;
    }

    public void ShowGameEndScreen(bool isWinner)
    {
        _gameStateFlag = GameState.GS_ENDED;

        var _panel_text = EndScreen.GetComponentsInChildren<Text>();

        if (isWinner)
        {
            EndScreen.GetComponent<Image>().color = new Color32(0 , 184 , 64 , 255);
            _panel_text[0].text = string.Format("{0}\n{1} pkt" , GAMEEND_VICTORY_TEXT , this._gameScores);
        }
        else
        {
            EndScreen.GetComponent<Image>().color = new Color32(219 , 54 , 49 , 255);
            _panel_text[0].text = string.Format("{0}\n{1} pkt", GAMEEND_DEATH_TEXT, this._gameScores);
        }

        EndScreen.SetActive(true);
    }

    public void AddRecord(Record record, bool save)
    {
        Records.Add(record);

        if (save)
        {
            SaveStats(Records);
        }
    }
    public static List<Record> LoadRecords()
    {
        //TextAsset file = Resources.Load("records") as TextAsset;
        string file_name = GetAppDataPath() + "/records.txt";

        var lines = File.ReadAllLines(file_name);
        //string[] lines = file.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        List<Record> result = new List<Record>();

        for (int i = 0; i < lines.Length; i++)
        {
            Record tmp_record = new Record();
            string[] line_data = lines[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            tmp_record.Name = line_data[0];
            tmp_record.Scores = int.Parse(line_data[1]);
            tmp_record.Timestamp = long.Parse(line_data[2]);

            result.Add(tmp_record);
        }

        return result == null ? result : result.OrderByDescending(r => r.Scores).ToList();
    }

    public static void SaveStats(List<Record> records)
    {
        string file_name = GetAppDataPath() + "/records.txt";

        if (!File.Exists(file_name))
        {
            File.Create(file_name);
        }

         if (records.Count > STATS_LIMIT)
         {
            records = records.OrderByDescending(r => r.Scores).ToList().GetRange(0, 5);
         }

         string lines = "";
         foreach (Record r in records)
         {
             lines += r.ToString() + "\n";
         }

         File.WriteAllText(file_name , lines);
    }
}
