﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Author: AC De Leon
 *  Game: Sirtet
 * */

public class Tetromino : MonoBehaviour {

    // How fast the tetrominos fall
    float fall = 0;
    private float fallSpeed;

    // Rotation syntax for specific tetrominos, for example, squares don't rotate
    public bool allowRotation = true;
    public bool limitRotation = false;

    // Used for time difference between users, faster = more points, slower = less points
    public int individualScore = 100;
    private float individualScoreTime;

    // Audio files for movement
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    // Speed at when tetromino moves when button is held down
    private float continuousVeritcalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;

    // How long to wait before tetromino recognizes that a button is being held down
    private float buttonDownWaitMax = 0.2f;        

    // Puts time in between key presses for user experience
    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;
    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!GameManager.isPaused)
        {
            CheckUserInput();
            UpdateIndividualScore();
            UpdateFallSpeed();
        }
	}

    void UpdateFallSpeed ()
    {
        fallSpeed = GameManager.fallSpeed;
    }

    void UpdateIndividualScore ()
    {
        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;

            // Avoids score to drop below 0, so it'll be around individualScore ~ 0 points
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput() {

        // Smoother movements by resetting timers
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            movedImmediateVertical = false;

            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        // RIGHT ARROW
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        // LEFT ARROW
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        // UP ARROW
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        // DOWN ARROW
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    void MoveLeft ()
    {
        if (movedImmediateHorizontal)
        {

            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            // Controls speed of movement through time
            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        // Moves piece by x-axis, 1 space to the left
        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight ()
    {
        if (movedImmediateHorizontal)
        {

            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            // Controls speed of movement through time
            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        // Moves piece by x-axis, 1 space to the right
        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown ()
    {
        if (movedImmediateVertical)
        {

            if (buttonDownWaitTimerVertical < buttonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVeritcalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
        {
            movedImmediateVertical = true;
        }

        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);

            FindObjectOfType<GameManager>().DeleteRow();

            if (FindObjectOfType<GameManager>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<GameManager>().GameOver();
            }

            PlayLandAudio();

            FindObjectOfType<GameManager>().SpawnNextTetromino();

            GameManager.currentScore += individualScore;

            // FindObjectOfType<GameManager>().UpdateHighScore();

            enabled = false;

            tag = "Untagged";
        }

        fall = Time.time;
    }

    void Rotate ()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                // Every rotation will be 90 degrees
                transform.Rotate(0, 0, 90);
            }

            if (CheckIsValidPosition())
            {
                FindObjectOfType<GameManager>().UpdateGrid(this);
                PlayRotateAudio();
            }
            else
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }

    public void HardDrop ()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<GameManager>().UpdateGrid(this);

            FindObjectOfType<GameManager>().DeleteRow();

            if (FindObjectOfType<GameManager>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<GameManager>().GameOver();
            }

            PlayLandAudio();

            FindObjectOfType<GameManager>().SpawnNextTetromino();

            GameManager.currentScore += individualScore;

            // FindObjectOfType<GameManager>().UpdateHighScore();
            
            enabled = false;

            tag = "Untagged";
        }
    }

    void PlayMoveAudio ()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void PlayRotateAudio ()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    void PlayLandAudio ()
    {
        audioSource.PlayOneShot(landSound);
    }

    bool CheckIsValidPosition ()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<GameManager>().Round(mino.position);
            
            if (FindObjectOfType<GameManager>().CheckIsInsideGrid (pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<GameManager>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<GameManager>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
