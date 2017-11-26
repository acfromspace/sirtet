using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {

    public Text levelText;

    void Start ()
    {
        levelText.text = "0";
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
