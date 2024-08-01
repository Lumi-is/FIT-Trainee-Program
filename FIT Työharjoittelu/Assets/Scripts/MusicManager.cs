using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource Music1;
    [SerializeField] AudioSource Music2;
    [SerializeField] AudioSource Music3;
    [SerializeField] AudioSource Music4;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Level 9") return;
        
        DontDestroyOnLoad(gameObject);
            
    }

    void OnDestroy()
    {
        Debug.Log("Scene change");
    }
    public void OnSceneUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Level 1")
        {
            Music1.Stop();
            Music1.Play();
            Music2.Stop();
            Music3.Stop();
            Music4.Stop();
        }

        if(SceneManager.GetActiveScene().name == "Level 3")
        {
            Music1.Stop();
            Music2.Stop();
            Music2.Play();
            Music3.Stop();
            Music4.Stop();
        }

        if(SceneManager.GetActiveScene().name == "Level 6")
        {
            Music1.Stop();
            Music2.Stop();
            Music3.Stop();
            Music3.Play();
            Music4.Stop();
        }

        if(SceneManager.GetActiveScene().name == "Level 9")
        {
            Music1.Stop();
            Music2.Stop();
            Music3.Stop();
            Music4.Stop();
            Music4.Play();
        }

        if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            Music1.Stop();
            Music2.Stop();
            Music3.Stop();
            Music4.Stop();
        }

        if(SceneManager.GetActiveScene().name == "Menu")
        {
            Music1.Stop();
            Music2.Stop();
            Music3.Stop();
            Music4.Stop();
        }

        if(SceneManager.GetActiveScene().name == "DeathScreen")
        {
            Music1.Stop();
            Music2.Stop();
            Music3.Stop();
            Music4.Stop();
        }
    }
}
