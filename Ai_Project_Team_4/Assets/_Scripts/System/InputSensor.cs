using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSensor : MonoBehaviour
{
    [Header("collider")]
    public Collider2D Collider;
    [Header("LeftArmPoint")]
    public GameObject leftArmPoint;
    [Header("RightArmPoint")]
    public GameObject rightArmPoint;

    [Header("LeftArm")]
    public GameObject leftArm;
    [Header("RightArm")]
    public GameObject rightArm;

    [Header("InputLayerNode")]
    [SerializeField]
    private float[] inputLayerNode;

    [Header("WeightGeneticSystem")]
    [SerializeField]
    private float[] outputLayerNode;

    // Update is called once per frame
    void Start()
    {
        inputLayerNode = new float[4] { 10,0,0,0 };
    }

    private void FixedUpdate()
    {
        rotateAction(false, inputLayerNode);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            SetPositionPlusToLeftArm(GetPositionPlusToLeftArm(collision.gameObject));
            SetPositionMinusToLeftArm(GetPositionMinusToLeftArm(collision.gameObject));
            SetPositionPlusToRightArm(GetPositionPlusToRightArm(collision.gameObject));
            SetPositionMinusToRightArm(GetPositionMinusToRightArm(collision.gameObject));
        }
    }

    public void rotateAction(bool status, float[] outPut)
    {
        if(status)
        {
            return;
        }
        float actionValue = outPut.Max();
        int actionIndex = Array.IndexOf(outPut, actionValue);
        //Debug.Log("index : " + actionIndex);
        switch (actionIndex)
        {
            case 0:
                rightArm.transform.Rotate(Vector3.forward, 3f);
                break;
            case 1:
                rightArm.transform.Rotate(Vector3.forward, -3f);
                break;
            case 2:
                leftArm.transform.Rotate(Vector3.forward, 3f);
                break;
            case 3:
                leftArm.transform.Rotate(Vector3.forward, -3f);
                break;
            default:
                Debug.Log("notting");
                break;
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
        float distanceToLeftArm = (projectile.transform.position - leftArmPoint.transform.position).magnitude;
        return distanceToLeftArm;
    }

    public void SetDistanceToLeftArm(float distanceToLeftArm)
    {
        inputLayerNode[1] = distanceToLeftArm;
    }

    public float GetPositionPlusToLeftArm(GameObject projectile)
    {
        Vector3 localPos = leftArmPoint.transform.InverseTransformPoint(projectile.transform.position);

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
        Vector3 localPos = leftArmPoint.transform.InverseTransformPoint(projectile.transform.position);

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
        float distanceToRightArm = (projectile.transform.position - rightArmPoint.transform.position).magnitude;
        return distanceToRightArm;
    }

    public void SetDistanceToRightArm(float distanceToRightArm)
    {
        inputLayerNode[4] = distanceToRightArm;
    }
    public float GetPositionPlusToRightArm(GameObject projectile)
    {
        Vector3 localPos = rightArmPoint.transform.InverseTransformPoint(projectile.transform.position);

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
        Vector3 localPos = rightArmPoint.transform.InverseTransformPoint(projectile.transform.position);

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
