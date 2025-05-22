using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSensor : MonoBehaviour
{
    //��������
    [Header("collider")]
    public Collider2D Collider;

    //���� ������ �߽���
    [Header("LeftArmPoint")]
    public GameObject leftArmPoint;
    [Header("RightArmPoint")]
    public GameObject rightArmPoint;

    //���� ������ 
    [Header("LeftArm")]
    public GameObject leftArm;
    [Header("RightArm")]
    public GameObject rightArm;

    //�Է°��� ��°� ����Ʈ��
    [Header("InputLayerNode")]
    [SerializeField]
    private float[] inputLayerNode;
    [Header("OutputLayerNode")]
    [SerializeField]
    private float[] outputLayerNode;

    //�����ڰ�ü ����Ʈ�� ��ü�ε����� ������� ���Լ�
    [Header("Generaton")]
    public List<List<float[,]>> generatons = new List<List<float[,]>>();
    [Header("Generaton Index")]
    public int genIndex = 0;
    [Header("Fitness")]
    public List<float> fitness = new List<float>();
    [Header("Genraton Seed")]
    public int seed = 1;

    // Update is called once per frame
    void Start()
    {
        InitObject();//������ ��ü 64�� ����  
        outputLayerNode = new float[4] { 10,0,0,0 };
    }

    private void FixedUpdate()
    {
        // ��ȯ �� �� ������ 
        rotateAction(outputLayerNode);
    }

    /// <summary>
    /// �߻�ü ������ �Է°� �����ϰ� �� ������ ��� �� ��ȯ 
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
            outputLayerNode = NerualNetwork(inputLayerNode);
        }
    }

    /// <summary>
    /// �Ű������ ����� ��°� ��� �� ������ �Լ� 
    /// </summary>
    /// <param name="outPut"></param>
    public void rotateAction(float[] outPut)
    {
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

    /// <summary>
    /// ����ġ ������ ��� �Ű������ ��°� ��� 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public float[] NerualNetwork(float[] inputs)
    {
        float[,] weightIH = generatons[genIndex][0];
        float[,] weightHO = generatons[genIndex][1];
        // 1) ������ ���: hidden = weightIH �� inputs
        var hidden = new float[5];
        for (int h = 0; h < 5; h++)
        {
            float sum = 0f;
            for (int i = 0; i < 7; i++)
            {
                sum += weightIH[h, i] * inputs[i];
            }
            hidden[h] = Sigmoid(sum);
        }

        // 2) ����� ���: output = weightHO �� hidden
        var output = new float[4];
        for (int o = 0; o < 4; o++)
        {
            float sum = 0f;
            for (int h = 0; h < 5; h++)
            {
                sum += weightHO[o, h] * hidden[h];
            }
            output[o] = Sigmoid(sum);
        }

        return output;
    }

    /// <summary>
    /// �����Լ� 
    /// </summary>
    public void SelecteGen()
    {
        List<List<float[,]>> bester = new List<List<float[,]>>();
        for (int i = 0; i < 4; i++)
        {
            float bestValue = fitness.Max();
            int bestIndex = fitness.FindIndex(x => x == bestValue);
            bester.Add(generatons[bestIndex]);
            generatons.Remove(generatons[bestIndex]);
            fitness.Remove(fitness[bestIndex]);
        }

        for (int i = 0; i < 30; i++)
        {
            // Get first random index
            int firstIndex = UnityEngine.Random.Range(0, generatons.Count);

            // Get second random index ensuring it's different from firstIndex
            int secondIndex = UnityEngine.Random.Range(0, generatons.Count - 1);
            if (secondIndex >= firstIndex)
                secondIndex++;
            generatons.Remove(generatons[secondIndex]);
            fitness.Remove(fitness[secondIndex]);
        }
    }
    
    /// <summary>
    /// �����Լ� 
    /// </summary>
    public void CrossOver()
    {
        seed++;
    }

    /// <summary>
    /// ���Լ� 
    /// </summary>
    /// <param name="isProtected"></param>
    /// <param name="distance"></param>
    public void SetFitness(bool isProtected, float distance = 0)
    {
        genIndex++;
        if (genIndex >= 64)
        {
            genIndex = 0;
        }
    }

    /// <summary>
    /// �Է°��� ����Ʈ�� �Է��ϱ� �� 7�� źȯ������ �Ÿ�, ���ȰŸ�, ���� ��, ���� ��, �����ȰŸ�, ������ ��, ������ �� 
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
    /// �߻�ü�κ��� �Ÿ�, ��ġ ����, ������, ��Ʈ�ڽ� �������� ���� ���ϱ� �� 7�� 
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

    /// <summary>
    /// �Ű�� ����ġ ������������ ���� ��ü 64�� ���� 
    /// </summary>
    public void InitObject()
    {
        for (int c = 0; c < 64; c++)
        {
            generatons.Add(InitGeneratons());
        }
    }
    public List<float[,]> InitGeneratons()
    {
        float[,] ih = new float[5, 7];
        float[,] ho = new float[4, 5];
        for (int h = 0; h < 5; h++)
        {
            for (int i = 0; i < 7; i++)
            {
                ih[h, i] = UnityEngine.Random.value;
            }
        }
        for (int o = 0; o < 4; o++)
        {
            for (int h = 0; h < 5; h++)
            {
                ho[o, h] = UnityEngine.Random.value;
            }
        }
        List<float[,]> generaton = new List<float[,]>();
        generaton.Add(ih);
        generaton.Add(ho);
        return generaton;
    }
    public float[,] DeepCopy(float[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        var dest = new float[rows, cols];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                dest[y, x] = source[y, x];
            }
        }
        return dest;
    }
    private static float Sigmoid(float x)
    {
        // e^(-x) ���
        return 1f / (1f + Mathf.Exp(-x));
    }
}
