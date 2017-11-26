using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {

    public Text levelText;

    public Text highScoreText;

    void Start ()
    {
        levelText.text = "0";
        highScoreText.text = PlayerPrefs.GetInt("highScore").ToString();
    }

    public void Credits ()
    {

    }

	public void PlayGame ()
    {
        if (Game.startingLevel == 0)
        {
            Game.startingAtLevelZero = true;
        }
        else
        {
            Game.startingAtLevelZero = false;
        }
        SceneManager.LoadScene("Main");
    }

    public void QuitGame ()
    {
        Application.Quit();
    }

    public void ChangedValue (float value)
    {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchGameMenu ()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
