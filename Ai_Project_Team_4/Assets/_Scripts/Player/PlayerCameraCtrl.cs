using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraCtrl : MonoBehaviour
{
    public Transform target;

    float smoothing = 10f;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, position, smoothing * Time.deltaTime);
    }
}
