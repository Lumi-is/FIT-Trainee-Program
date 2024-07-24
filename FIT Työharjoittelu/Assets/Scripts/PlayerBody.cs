using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUps;
    [SerializeField] Transform playerHead;

    [SerializeField] Vector3 bodyOffset;
    void Update()
    {
        transform.position = playerHead.position + bodyOffset;
    }
    public void Die()
    {
        if(playerPowerUps.Undying)
        {
            playerPowerUps.Undying = false;
        } else {
            //Death
        }
    }
}
