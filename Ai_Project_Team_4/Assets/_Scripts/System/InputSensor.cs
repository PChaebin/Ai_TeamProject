using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;

public class InputSensor : MonoBehaviour
{
    [Header("TestLearning")]
    public LerningSystem system;

    //감지영역
    [Header("collider")]
    public Collider2D Collider;
    // 돌리세요!
    [Header("isTurning")]
    public bool isTurning = false;

    //왼팔 오른팔 손끝
    [Header("LeftArmPoint")]
    public GameObject leftArmPoint;
    [Header("RightArmPoint")]
    public GameObject rightArmPoint;

    //왼팔 오른팔 
    [Header("LeftArm")]
    public GameObject leftArm;
    [Header("RightArm")]
    public GameObject rightArm;

    //입력값과 출력값 리스트들
    [Header("InputLayerNode")]
    [SerializeField]
    private float[] inputLayerNode;
    [Header("OutputLayerNode")]
    [SerializeField]
    private float[] outputLayerNode;

    [Header("Uses I")]
    float[,] weightIH;
    [Header("Uses O")]
    float[,] weightHO;
    [Header("final gen")]
    List<float[,]> finalGen;

    //인풋
    [Header("inputN")]
    public int inputN = 7;
    //히든
    [Header("hiddenN")]
    public int hiddenN = 5;
    //아웃풋
    [Header("outputN")]
    public int outputN = 4;

    // Update is called once per frame
    void Awake()
    {
        //Debug.Log("start");
        outputLayerNode = new float[4] { 0,0,0,0 };
        inputLayerNode = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
        if(system != null)
        {
            system.InitObject();
            weightIH = system.Getmaterix(0);
            weightHO = system.Getmaterix(1);
            return;
        }
        
        finalGen = ReadFileGreateGen();
        weightIH = finalGen[0];
        weightHO = finalGen[1];
    }

    public void Turning(bool set)
    {
        isTurning = set;
    }

    public bool GetTurn()
    {
        return isTurning;
    }

    private void FixedUpdate()
    {
        if(isTurning)
        {
            // 반환 값 팔 돌리기 
            rotateAction(outputLayerNode);
        }
    }

    /// <summary>
    /// 발사체 감지후 입력값 생성하고 팔 돌리는 출력 값 반환 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            SetDistanceToProjectile(GetDistanceToProjectile(collision.gameObject));
            SetDistanceToLeftArm(GetDistanceToLeftArm(collision.gameObject));
            SetDistanceToRightArm(GetDistanceToRightArm(collision.gameObject));
            SetPositionPlusToLeftArm(GetPositionPlusToLeftArm(collision.gameObject));
            SetPositionMinusToLeftArm(GetPositionMinusToLeftArm(collision.gameObject));
            SetPositionPlusToRightArm(GetPositionPlusToRightArm(collision.gameObject));
            SetPositionMinusToRightArm(GetPositionMinusToRightArm(collision.gameObject));

            outputLayerNode = NerualNetwork(inputLayerNode, weightIH, weightHO);
            //Debug.Log("output : " + outputLayerNode[0]);
            //Debug.Log("output : " + outputLayerNode[1]);
            //Debug.Log("output : " + outputLayerNode[2]);
            //Debug.Log("output : " + outputLayerNode[3]);
        }
        if(system != null)
        {
            system.SetFitness(leftArmPoint, rightArmPoint, collision.gameObject);
            //Debug.Log("fit");
        }
    }

    public void SetTurning(bool isTurn)
    {
        isTurning = isTurn;
    }

    public int UpGenIndex()
    {
        int a = system.genIndexAndSeedUP();
        Debug.Log("next index : " + a);
        if (a == -1)
        {
            system.NextGens();
        }
        if (system.IsOverSeed())
        {
            system.WriteFileGreateGen();
            return -1;
        }
        weightIH = system.Getmaterix(0);
        weightHO = system.Getmaterix(1);
        return system.GetSeedIndex();
    }

    /// <summary>
    /// 신경망에서 계산한 출력값 기반 팔 돌리기 함수 
    /// </summary>
    /// <param name="outPut"></param>
    public void rotateAction(float[] outPut)
    {
        float actionValue = outPut.Max();
        //Debug.Log("max : " + actionValue);
        int actionIndex = Array.IndexOf(outPut, actionValue);
        //Debug.Log("index : " + actionIndex);
        switch (actionIndex)
        {
            case 0:
                rightArm.transform.Rotate(Vector3.forward, 10f);
                break;
            case 1:
                rightArm.transform.Rotate(Vector3.forward, -10f);
                break;
            case 2:
                leftArm.transform.Rotate(Vector3.forward, 10f);
                break;
            case 3:
                leftArm.transform.Rotate(Vector3.forward, -10f);
                break;
            default:
                Debug.Log("notting");
                break;
        }
    }

    /// <summary>
    /// 가중치 유전자 기반 신경망으로 출력값 계산 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public float[] NerualNetwork(float[] inputs,float[,] weightInstIH, float[,] weightInstHO)
    {
        // 1) 은닉층 계산: hidden = weightIH × inputs
        var hidden = new float[hiddenN];
        for (int h = 0; h < hiddenN; h++)
        {
            float sum = 0f;
            for (int i = 0; i < inputN; i++)
            {
                sum += weightInstIH[h, i] * inputs[i];
            }
            hidden[h] = Sigmoid(sum);
        }
        // 2) 출력층 계산: output = weightHO × hidden
        var output = new float[outputN];
        for (int o = 0; o < outputN; o++)
        {
            float sum = 0f;
            for (int h = 0; h < hiddenN; h++)
            {
                sum += weightInstHO[o, h] * hidden[h];
            }
            output[o] = Sigmoid(sum);
        }

        return output;
    }

    public List<float[,]> ReadFileGreateGen()
    {
        float[,] ih = new float[hiddenN, inputN];
        float[,] ho = new float[outputN, hiddenN];

        string test = PlayerPrefs.GetString("best");
        string[] spl = test.Split(" / ");
        string[] x = spl[0].Split(" % ");
        string[] y = spl[1].Split(" % ");
        for (int a = 0; a < x.Length; a++)
        {
            string[] f = x[a].Split(", ");
            for (int b = 0; b < f.Length; b++)
            {
                ih[a, b] = float.Parse(f[b]);
            }
        }
        for (int a = 0; a < y.Length; a++)
        {
            string[] f = y[a].Split(", ");
            for (int b = 0; b < f.Length; b++)
            {
                ho[a, b] = float.Parse(f[b]);
            }
        }
        List<float[,]> text = new List<float[,]>();
        text.Add(ih);
        text.Add(ho);
        return text;
    }

    /// <summary>
    /// 입력값을 리스트에 입력하기 총 7개 탄환까지의 거리, 왼팔거리, 왼팔 좌, 왼팔 우, 오른팔거리, 오른팔 좌, 오른팔 우 
    /// </summary>
    /// <param name="distanceToProjectile"></param>
    public void SetDistanceToProjectile(float distanceToProjectile)
    {
        inputLayerNode[0] = distanceToProjectile;
    }
    public void SetDistanceToLeftArm(float distanceToLeftArm)
    {
        inputLayerNode[1] = distanceToLeftArm;
    }
    public void SetPositionPlusToLeftArm(float positionPlusToLeftArm)
    {
        inputLayerNode[2] = positionPlusToLeftArm;
    }
    public void SetPositionMinusToLeftArm(float positionPlusToLeftArm)
    {
        inputLayerNode[3] = positionPlusToLeftArm;
    }
    public void SetDistanceToRightArm(float distanceToRightArm)
    {
        inputLayerNode[4] = distanceToRightArm;
    }
    public void SetPositionPlusToRightArm(float positionPlusToRightArm)
    {
        inputLayerNode[5] = positionPlusToRightArm;
    }
    public void SetPositionMinusToRightArm(float positionPlusToLeftArm)
    {
        inputLayerNode[6] = positionPlusToLeftArm;
    }

    /// <summary>
    /// 발사체로부터 거리, 위치 왼팔, 오른팔, 히트박스 기준으로 각각 구하기 총 7개 
    /// </summary>
    /// <param name="projectile"></param>
    /// <returns></returns>
    public float GetDistanceToProjectile(GameObject projectile)
    {
        float distanceToProjectile = (projectile.transform.position - this.transform.position).magnitude;
        return distanceToProjectile;
    }
    public float GetDistanceToLeftArm(GameObject projectile)
    {
        float distanceToLeftArm = (projectile.transform.position - leftArmPoint.transform.position).magnitude;
        return distanceToLeftArm;
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
    public float GetPositionMinusToLeftArm(GameObject projectile)
    {
        Vector3 localPos = leftArmPoint.transform.InverseTransformPoint(projectile.transform.position);

        if (localPos.x < 0f)
        {
            return 1f;
        }
        return 0f;
    }
    public float GetDistanceToRightArm(GameObject projectile)
    {
        float distanceToRightArm = (projectile.transform.position - rightArmPoint.transform.position).magnitude;
        return distanceToRightArm;
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
    public float GetPositionMinusToRightArm(GameObject projectile)
    {
        Vector3 localPos = rightArmPoint.transform.InverseTransformPoint(projectile.transform.position);

        if (localPos.x < 0f)
        {
            return 1f;
        }
        return 0f;
    }

    private static float Sigmoid(float x)
    {
        // e^(-x) 계산
        return 1f / (1f + Mathf.Exp(-x));
    }
}
