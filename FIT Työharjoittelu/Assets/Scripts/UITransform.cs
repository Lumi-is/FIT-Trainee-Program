using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UITransform : MonoBehaviour
{
    [SerializeField] Transform follow;
    [SerializeField] Camera cam;

    void Start()
    {
        transform.position = follow.position + Vector3.up * 0.2f;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.gameObject.transform);
        transform.position = Vector3.MoveTowards(transform.position, follow.position + Vector3.up * 0.2f, 0.2f*Time.deltaTime);
    }
}
