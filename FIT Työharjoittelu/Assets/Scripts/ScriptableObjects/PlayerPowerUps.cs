using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpList", menuName = "PowerUpList")]
public class PlayerPowerUps : ScriptableObject
{
    public bool Undying;
    public float DamageBoostAmount;
    public float SpeedBoostAmount;
    public float JumpBoostAmount;
    public float DashBoostAmount;
    public float HitforceBoostAmount;
    public float kbBoostAmount;

    public void ResetPowerUps()
    {
        Undying = false;
        DamageBoostAmount = 1;
        SpeedBoostAmount = 1;
        JumpBoostAmount = 1;
        DashBoostAmount = 1;
        HitforceBoostAmount = 1;
        kbBoostAmount = 1;        
    }
}