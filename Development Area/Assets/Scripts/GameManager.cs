using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*  Author: AC De Leon
 *  Game: Sirtet
 * */

public class GameManager : MonoBehaviour {

    // How big the grid is
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    // Enables the game grid
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    private bool gameStarted = false;

    // Enables difficulty
    public static bool startingAtLevelZero;
    public static int startingLevel;
    
    // Amount of points to be earned
    public int scoreOneLine;
    public int scoreTwoLine;
    public int scoreThreeLine;
    public int scoreFourLine;
    private int numberOfRowsThisTurn = 0;

    // Starting of the game
    public int currentLevel = 0;
    private int numLinesCleared = 0;

    // How fast the tetrominos fall
    public static float fallSpeed = 1.0f;
    public static bool isPaused = false;

    // Audio
    public AudioSource audioSource;
    public AudioClip clearedLineSound;
    float volume = 1.5f;

    // HUD
    public Canvas hud_canvas;
    public Canvas pause_canvas;
    public Canvas dontLeaveMe_canvas;
    public Text hud_score;
    public Text hud_level;
    public Text hud_lines;
    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private GameObject savedTetromino;

    // Keeps Score
    public static int currentScore = 0;
    private int startingHighScore;

    // Location of preview next tetromino
    private Vector2 previewTetrominoPosition = new Vector2 (-9.5f, 14.35f);
    private Vector2 savedTetrominoPosition = new Vector2(-9.5f, 5);

    // Game element Save Tetromino not working properly
    public int maxSwaps = 2;
    private int currentSwaps = 0;

    // Camera control
    [SerializeField]
    Camera camera1;
    [SerializeField]
    Camera camera2;

	void Start ()
    {
        currentScore = 0;

        hud_score.text = "0";

        currentLevel = startingLevel;

        hud_level.text = currentLevel.ToString();

        hud_lines.text = "0";

        SpawnNextTetromino();

        audioSource = GetComponent<AudioSource>();

        startingHighScore = PlayerPrefs.GetInt("highScore");

        camera2.GetComponent<Camera>().enabled = false;
        camera1.GetComponent<Camera>().enabled = true;
    }

    void Update ()
    {
        UpdateScore();

        UpdateUI();

        UpdateLevel();

        UpdateSpeed();

        UpdateHighScore();

        CheckUserInput();
    }

    void CheckUserInput ()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                camera1.GetComponent<Camera>().enabled = false;
                camera2.GetComponent<Camera>().enabled = true;
                PauseGame();
            }
            else
            {
                camera2.GetComponent<Camera>().enabled = false;
                camera1.GetComponent<Camera>().enabled = true;
                ResumeGame();
            }
        }

        // CODEISGOOD 28:33
        /*
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNextTetromino.transform);
        }
        */
    }

    public void PauseGame ()
    {
        Time.timeScale = 0;
        audioSource.Pause();
        isPaused = true;
        hud_canvas.enabled = false;
        pause_canvas.enabled = true;
        dontLeaveMe_canvas.enabled = false;
}

    public void ResumeGame ()
    {
        Time.timeScale = 1;
        audioSource.Play();
        isPaused = false;
        hud_canvas.enabled = true;
        pause_canvas.enabled = false;
        dontLeaveMe_canvas.enabled = false;
    }

    public void RestartGame ()
    {
        Time.timeScale = 1;
        isPaused = false;
        hud_canvas.enabled = false;
        pause_canvas.enabled = false;
        dontLeaveMe_canvas.enabled = true;
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

            FindObjectOfType<GameManager>().UpdateHighScore();

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

    public void UpdateHighScore ()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highScore", currentScore); 
        }

        PlayerPrefs.SetInt("lastScore", currentScore);
    }

    // Game element Save Tetromino not working properly
    bool CheckIsValidPosition (GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (!CheckIsInsideGrid(pos))
            {
                return false;
            }

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckIsAboveGrid (Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight - 1) // Check this is it's causing the problem in the game
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

    // Game element Save Tetromino not working properly
    public void SpawnNextTetromino ()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // Makes preview tetromino not move
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";
        }
        else
        {
            // Puts the preview tetromino as the piece in play
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // Makes preview tetromino not move
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }

        currentSwaps = 0;
    }

    // Game element Save Tetromino not working properly
    public void SaveTetromino (Transform t)
    {
        currentSwaps++;

        if (currentSwaps > maxSwaps)
        {
            return;
        }

        if (savedTetromino != null)
        {
            // There is currently a tetromino being held
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");
            // Put tetromino at starting point due to glitch if a piece did not fit where the previous tetromino was
            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);
        }
        else
        {
            // There is currently no tetromino being held
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
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
