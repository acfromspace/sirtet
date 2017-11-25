using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour {

	public void PlayAgain ()
    {
        // Used to be Application.LoadLevel
        SceneManager.LoadScene("Main");
    }
}
