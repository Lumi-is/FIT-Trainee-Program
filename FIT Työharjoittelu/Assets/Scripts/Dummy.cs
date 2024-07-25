using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "PlayerKatana")
        {
            if(levelManager.hasDashed && levelManager.HasMoved)
            {
                levelManager.DummyHitAmount++;
            }
        } else if(col.gameObject.tag == "PlayerFist")
        {
            if(levelManager.DummyHitAmount > 2)
            {
                levelManager.hasPunchedDummy = true;
            }
        }
    }
}
