using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        MusicManager musicManager = FindAnyObjectByType<MusicManager>();
        musicManager.OnSceneUpdate();
    }
}
