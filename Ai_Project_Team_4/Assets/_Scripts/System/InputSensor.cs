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
    // ��������!
    [Header("isTurning")]
    public bool isTurning = false;

    //���� ������ �ճ�
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
    [Header("current Generaton Index")]
    public int genIndex = 0;
    [Header("Fitness")]
    public List<float> fitness = new List<float>();
    [Header("Genraton Seed")]
    public int seed = 1;

    //��ü��
    [Header("generaton n 64")]
    public int genN = 64;
    //�ִ뼼��
    [Header("seed n 100")]
    public int seedN = 100;
    //����Ʈ ��
    [Header("eleitm 4")]
    public int ele = 4;

    //�Ȱ�źȯ�ִ�Ÿ�
    [Header("maxDist")]
    public float maxDist = 1.5f;

    // Update is called once per frame
    void Start()
    {
        InitObject();//������ ��ü 64�� ����  
        outputLayerNode = new float[4] { 10,0,0,0 };
    }

    private void FixedUpdate()
    {
        if(isTurning)
        {
            // ��ȯ �� �� ������ 
            rotateAction(outputLayerNode);
        }
    }

    /// <summary>
    /// �߻�ü ������ �Է°� �����ϰ� �� ������ ��� �� ��ȯ 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            isTurning = true;
            SetDistanceToProjectile(GetDistanceToProjectile(collision.gameObject));
            SetDistanceToLeftArm(GetDistanceToLeftArm(collision.gameObject));
            SetDistanceToRightArm(GetDistanceToRightArm(collision.gameObject));
            SetPositionPlusToLeftArm(GetPositionPlusToLeftArm(collision.gameObject));
            SetPositionMinusToLeftArm(GetPositionMinusToLeftArm(collision.gameObject));
            SetPositionPlusToRightArm(GetPositionPlusToRightArm(collision.gameObject));
            SetPositionMinusToRightArm(GetPositionMinusToRightArm(collision.gameObject));

            outputLayerNode = NerualNetwork(inputLayerNode);
            SetFitness(leftArmPoint, rightArmPoint, collision.gameObject);
        }
        isTurning = false;
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
    /// ����Ʈ, ��ʸ�Ʈ ���ð� ���� �Լ� 
    /// </summary>
    IEnumerator SelecteGen()
    {
        //���� ���뿡 ���� ��
        List<List<float[,]>> corrosGeneratons = new List<List<float[,]>>();

        // ����Ʈ ���� �� ���� ���뿡 �ٷ� �־��� (4��)
        for (int i = 0; i < 4; i++)
        {
            float bestValue = fitness.Max();
            int bestIndex = fitness.FindIndex(x => x == bestValue);
            corrosGeneratons.Add(generatons[bestIndex]);
        }

        int firstIndex;
        int secondIndex;

        float gen1;
        float gen2;

        List<List<float[,]>> tonement = new List<List<float[,]>>();

        // ��ʸ�Ʈ����, 64���� ������ 32�� ����
        for (int i = 0; i < 32; i++)
        {
            // Get first random index
            firstIndex = UnityEngine.Random.Range(0, generatons.Count);

            // Get second random index ensuring it's different from firstIndex
            secondIndex = UnityEngine.Random.Range(0, generatons.Count - 1);
            if (secondIndex >= firstIndex)
                secondIndex++;

            gen1 = fitness[firstIndex];
            gen2 = fitness[secondIndex];

            if (gen1 >= gen2)
            {
                tonement.Add(generatons[firstIndex]);
            }
            else
            {
                tonement.Add(generatons[secondIndex]);
            }
        }

        int par1indx;
        int par2indx;

        List<float[,]> parent1;
        List<float[,]> parent2;

        // ���� ���� ����
        while (corrosGeneratons.Count < generatons.Count)
        {
            List<float[,]> child1 = new List<float[,]>();
            List<float[,]> child2 = new List<float[,]>();

            par1indx = UnityEngine.Random.Range(0, tonement.Count);
            par2indx = UnityEngine.Random.Range(0, tonement.Count -1 );
            if (par2indx >= par1indx)
                par2indx++;

            parent1 = tonement[par1indx];
            parent2 = tonement[par2indx];

            // 1) ������ ���� ����
            int numPoints = 2;

            // 2) [1, parent1.Count) �������� �ߺ� ���� ������ ����
            List<int> points = new List<int>();
            while (points.Count < numPoints)
            {
                int p = UnityEngine.Random.Range(1, parent1.Count);
                if (!points.Contains(p))
                    points.Add(p);
            }
            points.Sort();            // �������� ����
            points.Add(parent1.Count); // �������� ��ü ����

            // 4) ������ ���� ������ ������ ���� �ٿ��ֱ�
            int last = 0;
            bool takeFromP1 = true;
            foreach (int cut in points)
            {
                int len = cut - last;
                if (takeFromP1)
                {
                    // parent1 �� child1, parent2 �� child2
                    child1.AddRange(parent1.GetRange(last, len));
                    child2.AddRange(parent2.GetRange(last, len));
                }
                else
                {
                    // parent2 �� child1, parent1 �� child2
                    child1.AddRange(parent2.GetRange(last, len));
                    child2.AddRange(parent1.GetRange(last, len));
                }
                takeFromP1 = !takeFromP1;
                last = cut;
            }
            corrosGeneratons.Add(child1);
            corrosGeneratons.Add(child2);
            yield return new WaitForSeconds(0.02f);
        }
        generatons = corrosGeneratons;
        seed++;
        genIndex = 0;
    }

    /// <summary>
    /// ���Լ� 
    /// </summary>
    public void SetFitness(GameObject leftPoint, GameObject rightPoint, GameObject projectileObj)
    {
        float leftdist = maxDist - (leftPoint.transform.position - projectileObj.transform.position).magnitude;
        if(leftdist < 0)
        {
            leftdist = 0;
        }
        float rightdist = maxDist - (rightPoint.transform.position - projectileObj.transform.position).magnitude;
        if (rightdist < 0)
        {
            rightdist = 0;
        }
        float total = rightdist + leftdist;
        if (total >= 30f)
        {
            total = 30f;
        }
        fitness[genIndex] = total;
    }

    public void UpGenIndex()
    {
        genIndex++;
        if (genIndex >= 64)
        {
            StartCoroutine(SelecteGen());
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
            fitness.Add(0);
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
                ih[h, i] = UnityEngine.Random.Range(10,80);
            }
        }
        for (int o = 0; o < 4; o++)
        {
            for (int h = 0; h < 5; h++)
            {
                ho[o, h] = UnityEngine.Random.Range(10,80);
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
