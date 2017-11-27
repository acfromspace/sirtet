using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*  Author: AC De Leon
 *  Game: Sirtet
 * */

public class MusicManager : MonoBehaviour {

    private static MusicManager instance = null;
    public static MusicManager Instance { get { return instance; } }

	void Start ()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Level")
        {
            GameObject menuMusic = GameObject.FindGameObjectWithTag("music");
            Destroy(menuMusic);
        }

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
	}
}
