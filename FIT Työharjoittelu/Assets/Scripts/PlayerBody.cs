using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUps;
    [SerializeField] Transform playerHead;
    [SerializeField] Vector3 bodyOffset;

    void Start()
    {
        Volume global = FindAnyObjectByType<Volume>();
        VolumeProfile profile = global.sharedProfile;

        Vignette vignette;
        if(profile.TryGet(out vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0;
        }
    }

    void Update()
    {
        transform.position = playerHead.position + bodyOffset;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("EnemySword"))
        {
            Die();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.CompareTag("EnemySword"))
        {
            Die();
        }
    }
    public void Die()
    {
        if(SceneManager.GetActiveScene().name == "Tutorial") return;
        
        Debug.Log("Test");
        if(playerPowerUps.Undying)
        {
            playerPowerUps.Undying = false;
        } else {

            Volume global = FindAnyObjectByType<Volume>();
            VolumeProfile profile = global.sharedProfile;

            Vignette vignette;
            if(profile.TryGet(out vignette))
            {
                vignette.intensity.overrideState = true;
                vignette.intensity.value = 1;
            }

            Invoke(nameof(DeathScreen),1f);
        }
    }

    private void DeathScreen()
    {
        SceneManager.LoadScene("DeathScreen");
    }
}
