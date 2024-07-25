using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUpManager : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] PlayerPowerUps playerPowerUps;

    [SerializeField] GameObject[] powerUps = new GameObject[6];
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            playerPowerUps.ResetPowerUps();
        }
    }

    public void PresentOptions()
    {
        int option1Index = Random.Range(0,6);
        int option2Index = Random.Range(0,6);

        while(option1Index == option2Index)
        {
            option2Index = Random.Range(0,6);
        }
        GameObject option1 = powerUps[option1Index];
        GameObject option2 = powerUps[option2Index];

        Instantiate(option1, player.position + Vector3.forward + Vector3.right * -1, Quaternion.identity);
        Instantiate(option2, player.position + Vector3.forward + Vector3.right, Quaternion.identity);
    }
}
