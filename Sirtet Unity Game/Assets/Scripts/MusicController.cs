using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    void Awake()
    {
        {
            GameObject a = GameObject.FindGameObjectWithTag("music");
            Destroy(a);
        }
    }
}
