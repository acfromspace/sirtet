using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtLevelZero;
    public static int startingLevel;

    public int scoreOneLine = 100;
    public int scoreTwoLine = 200;
    public int scoreThreeLine = 400;
    public int scoreFourLine = 800;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public static float fallSpeed = 1.0f;

    public AudioClip clearedLineSound;
    float volume = 1.5f;

    public Text hud_score;
    public Text hud_level;
    public Text hud_lines;

    private int numberOfRowsThisTurn = 0;

    private AudioSource audioSource;

    public static int currentScore = 0;

    private GameObject previewTetromino;
    private GameObject nextTetromino;

    private bool gameStarted = false;

    private Vector2 previewTetrominoPosition = new Vector2 (-6.5f, 16);

	// Use this for initialization
	void Start ()
    {
        currentLevel = startingLevel;

        SpawnNextTetromino();

        audioSource = GetComponent<AudioSource>();
	}

    void Update ()
    {
        UpdateScore();

        UpdateUI();

        UpdateLevel();

        UpdateSpeed();
    }

    void UpdateLevel ()
    {
        if (startingAtLevelZero == true || (startingAtLevelZero == false && numLinesCleared / 10 > startingLevel))
        {
            // Every 10 lines increases the level and there's no floats due to being strictly int
            currentLevel = numLinesCleared / 10;
        }

    }

    void UpdateSpeed ()
    {
        // Level 1, 1 - 0.1 = 0.9 which is the time to take to go down
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI ()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore ()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLines();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLines();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLines();
            }

            numberOfRowsThisTurn = 0;

            PlayLineClearedSound();
        }
    }

    public void ClearedOneLine ()
    {
        currentScore += scoreOneLine + (currentLevel * 25);
        numLinesCleared += 1;
    }

    public void ClearedTwoLines ()
    {
        currentScore += scoreTwoLine + (currentLevel * 50);
        numLinesCleared += 2;
    }

    public void ClearedThreeLines ()
    {
        currentScore += scoreThreeLine + (currentLevel * 75);
        numLinesCleared += 3;
    }

    public void ClearedFourLines ()
    {
        currentScore += scoreFourLine + (currentLevel * 100);
        numLinesCleared += 4;
    }

    public void PlayLineClearedSound ()
    {
        audioSource.PlayOneShot(clearedLineSound, volume);
    }

    public bool CheckIsAboveGrid (Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsFullRowAt (int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            // If row is not full
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        
        // Since we found a full row, increment the full row variable
        numberOfRowsThisTurn++;

        // Row is full
        return true;
    }

    public void DeleteMinoAt (int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    public void MoveRowDown (int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown (int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow ()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);

                MoveAllRowsDown(y + 1);

                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform) {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition (Vector2 pos)
    {
        if (pos.y > gridHeight)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    // Disregard error as it is made during the dynamic gameplay
    public void SpawnNextTetromino ()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // Makees preview tetromino not move
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            // Puts the preview tetromino as the piece in play
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // Makees preview tetromino not move
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
    }

    public bool CheckIsInsideGrid (Vector2 pos)
    {
        // Checks piece is within  the grid
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round (Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino ()
    {
        int randomTetromino = Random.Range(1, 8);

        string randomTetrominoName = "Prefabs/randomTetromino";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_Long";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_Square";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
        }

        return randomTetrominoName;
    }

    public void GameOver ()
    {
        SceneManager.LoadScene("GameOver");
    }
}
