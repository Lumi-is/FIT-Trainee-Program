using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUps;
    public void StartGame()
    {
        playerPowerUps.ResetPowerUps();
        SceneManager.LoadScene("Level 1");
    }

    public void StartTutorial()
    {
        playerPowerUps.ResetPowerUps();
        SceneManager.LoadScene("Tutorial");
    }
}
