using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSensor : MonoBehaviour
{
    [Header("collider")]
    public Collider2D Collider;
    [Header("LeftArm")]
    public GameObject leftArm;
    [Header("RightArm")]
    public GameObject rightArm;

    [Header("InputLayerNode")]
    [SerializeField]
    private List<float> inputLayerNode;

    // Update is called once per frame
    void Start()
    {
        inputLayerNode = new List<float>(7) { 0,0,0,0,0,0,0 };

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            SetPositionPlusToLeftArm(GetPositionPlusToLeftArm(collision.gameObject));
            SetPositionMinusToLeftArm(GetPositionMinusToLeftArm(collision.gameObject));
            SetPositionPlusToRightArm(GetPositionPlusToRightArm(collision.gameObject));
            SetPositionMinusToRightArm(GetPositionMinusToRightArm(collision.gameObject));
            Debug.Log("Left-right : " + inputLayerNode[2]);
            Debug.Log("Left-left : " + inputLayerNode[3]);
            Debug.Log("Right-right : " + inputLayerNode[5]);
            Debug.Log("Right-left : " + inputLayerNode[6]);
        }
    }

    public float GetDistanceToProjectile(GameObject projectile)
    {
        float distanceToProjectile = (projectile.transform.position - this.transform.position).magnitude;
        return distanceToProjectile;
    }

    public void SetDistanceToProjectile(float distanceToProjectile)
    {
        inputLayerNode[0] = distanceToProjectile;
    }

    public float GetDistanceToLeftArm(GameObject projectile)
    {
        float distanceToLeftArm = (projectile.transform.position - leftArm.transform.position).magnitude;
        return distanceToLeftArm;
    }

    public void SetDistanceToLeftArm(float distanceToLeftArm)
    {
        inputLayerNode[1] = distanceToLeftArm;
    }

    public float GetPositionPlusToLeftArm(GameObject projectile)
    {
        Vector3 localPos = leftArm.transform.InverseTransformPoint(projectile.transform.position);

        if(localPos.x > 0f)
        {
            return 1f;
        }
        return 0f;
    }

    public void SetPositionPlusToLeftArm(float positionPlusToLeftArm)
    {
        inputLayerNode[2] = positionPlusToLeftArm;
    }

    public float GetPositionMinusToLeftArm(GameObject projectile)
    {
        Vector3 localPos = leftArm.transform.InverseTransformPoint(projectile.transform.position);

        if (localPos.x < 0f)
        {
            return 1f;
        }
        return 0f;
    }

    public void SetPositionMinusToLeftArm(float positionPlusToLeftArm)
    {
        inputLayerNode[3] = positionPlusToLeftArm;
    }

    public float GetDistanceToRightArm(GameObject projectile)
    {
        float distanceToRightArm = (projectile.transform.position - rightArm.transform.position).magnitude;
        return distanceToRightArm;
    }

    public void SetDistanceToRightArm(float distanceToRightArm)
    {
        inputLayerNode[4] = distanceToRightArm;
    }
    public float GetPositionPlusToRightArm(GameObject projectile)
    {
        Vector3 localPos = rightArm.transform.InverseTransformPoint(projectile.transform.position);

        if (localPos.x > 0f)
        {
            return 1f;
        }
        return 0f;
    }

    public void SetPositionPlusToRightArm(float positionPlusToRightArm)
    {
        inputLayerNode[5] = positionPlusToRightArm;
    }

    public float GetPositionMinusToRightArm(GameObject projectile)
    {
        Vector3 localPos = rightArm.transform.InverseTransformPoint(projectile.transform.position);

        if (localPos.x < 0f)
        {
            return 1f;
        }
        return 0f;
    }

    public void SetPositionMinusToRightArm(float positionPlusToLeftArm)
    {
        inputLayerNode[6] = positionPlusToLeftArm;
    }
}
