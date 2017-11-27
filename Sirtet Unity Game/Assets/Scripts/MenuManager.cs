using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*  Author: AC De Leon
 *  Game: Sirtet
 * */

public class MenuManager : MonoBehaviour {

    public Text levelText;

    public Text highScoreText;

    public Text lastScore;

    void Start()
    {
        // Resets scores
        // PlayerPrefs.SetInt("highScore", 0);

        // Check if objects exist to avoid null pointer exceptions
        if (levelText != null)
        {
            levelText.text = "0";
        }

        if (highScoreText != null)
        {
            highScoreText.text = PlayerPrefs.GetInt("highScore").ToString();
        }

        if (lastScore != null)
        {
            lastScore.text = PlayerPrefs.GetInt("lastScore").ToString();
        }
    }

    public void LaunchPlayGame ()
    {
        if (GameManager.startingLevel == 0)
        {
            GameManager.startingAtLevelZero = true;
        }
        else
        {
            GameManager.startingAtLevelZero = false;
        }
        SceneManager.LoadScene("Level");
    }

    public void LaunchGameMenu ()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LaunchHowToPlay ()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void LaunchCredits ()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LaunchQuitGame ()
    {
        Application.Quit();
    }

    public void ChangedValue (float value)
    {
        GameManager.startingLevel = (int)value;
        levelText.text = value.ToString();
    }
}
