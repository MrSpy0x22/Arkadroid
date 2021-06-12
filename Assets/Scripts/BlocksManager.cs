using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlocksManager : MonoBehaviour
{
    #region Singleton
    private static BlocksManager _instance = null;
    public static BlocksManager Instance => _instance;

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

    #region BlockManager constants
    public const int MAX_ROWS = 10;
    public const int MAX_COLS = 11;
    public const float START_X = -2.14f;
    public const float START_Y = 3.78f;
    public const float SHIFT_X = 0.43f;
    public const float SHIFT_Y = 0.43f;
    #endregion

    /// <summary>Prefab for blocks [passed in Unity Editor]</summary>
    public Block _blockPrefab;
    /// <summary>References to available blocks sprites [passed in Unity Editor].</summary>
    public Sprite[] _spriteTextures;
    /// <summary>References to blocks sprites color [passed in Unity Editor].</summary>
    public Color[] _spriteColors;
    /// <summary>Current level map.</summary>
    public List<int[,]> LevelsData { get; set; }
    /// <summary>Current level blocks map.</summary>
    public List<Block> LevelBlocks { get; set; }
    /// <summary>GameObject for blocks container.</summary>
    public GameObject BlockContainer;
    /// <summary>CUrrent level blocks count.</summary>
    public int LevelBlocksCount = 0;

    /// <summary>Invoked before loading next level.</summary>
    public static event Action OnLevelLoaded;

    /// <summary>
    ///     Start method by Unity Engine.
    /// </summary>
    private void Start()
    {
        this.LevelsData = LoadLevelsDataFromFiles();
        this.BlockContainer = new GameObject("BlockContainer");

        // Render block area (level 0)
        RenderBlocks();

        // Force updating text element
        UIManager.Instance.UpdateLevelCounter();
    }

    /// <summary>
    ///     Doing stuffs like updating level number and re-render block area.
    /// </summary>
    /// <param name="level">Level index to load.</param>
    /// <seealso cref="RemoveExistingBlocks"/>
    /// <seealso cref="RenderBlocks"/>
    public void LoadLevel(int level)
    {
        // Updating level number in PlayManager
        GameManager.Instance._currentLevel = level;
        
        // Recreating blocks area
        RemoveExistingBlocks();
        RenderBlocks();
    }

    private void RenderBlocks()
    {
        this.LevelBlocks = new List<Block>();
        int[,] levelData = this.LevelsData[GameManager.Instance._currentLevel];
        float spawnPositionX, spawnPositionY = START_Y;

        for (int row = 0; row < MAX_ROWS; row++)
        {
            spawnPositionX = START_X;

            for (int col = 0; col < MAX_COLS; col++)
            {
                int blockType = levelData[row, col];

                if (blockType > 0)
                {
                    Block block = Instantiate(this._blockPrefab, new Vector2(spawnPositionX, spawnPositionY), Quaternion.identity);
                    block.Prepare(this.BlockContainer.transform, this._spriteTextures[blockType - 1], this._spriteColors[blockType - 1], blockType);
                    
                    // Render layer
                    block.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GameScene";

                    this.LevelBlocks.Add(block);
                }

                spawnPositionX += SHIFT_X;
            }

            spawnPositionY -= SHIFT_Y;
        }

        this.LevelBlocksCount = this.LevelsData.Count;
    }

    public void LoadNext()
    {
        GameManager.Instance._currentLevel++;

        if (GameManager.Instance._currentLevel >= this.LevelBlocksCount)
        {
            GameManager.Instance.ShowGameEndScreen(true);
        }
        else
        {
            // Calling all subscribers
            OnLevelLoaded?.Invoke();
            this.LoadLevel(GameManager.Instance._currentLevel);
        }
    }

    private void RemoveExistingBlocks()
    {
        foreach (var block in LevelBlocks.ToList())
        {
            Destroy(block);
        }
    }

    /// <summary>
    ///     Load levels map from text file 'levels.txt'.
    /// </summary>
    private List<int[,]> LoadLevelsDataFromFiles()
    {
        List<int[,]> result = new List<int[,]>();
        TextAsset file = Resources.Load("levels") as TextAsset;
        string[] lines = file.text.Split(new string[] { Environment.NewLine } , StringSplitOptions.RemoveEmptyEntries);
        int[,] parsedLevelsData = new int[MAX_ROWS , MAX_COLS];
        int currentRow = 0;

        for (int row = 0; row < lines.Length; row++)
        {
            string currentLine = lines[row];

            // Comment
            if (currentLine.StartsWith("#"))
            {
                continue;
            }
            // Command
            else if (currentLine.StartsWith("@"))
            {
                currentRow = 0;
                result.Add(parsedLevelsData);
                parsedLevelsData = new int[MAX_ROWS, MAX_COLS];
            }
            // Content
            else
            {
                string[] dataAsString = currentLine.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int col = 0; col < dataAsString.Length; col++)
                {
                    parsedLevelsData[currentRow, col] = int.Parse(dataAsString[col]);
                }

                currentRow++;
            }
        }

        return result;
    }
}
