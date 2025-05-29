using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSensor : MonoBehaviour
{
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

    //유전자개체 리스트와 객체인덱스와 세대수와 평가함수
    [Header("Generaton")]
    public List<List<float[,]>> generatons = new List<List<float[,]>>();
    [Header("current Generaton Index")]
    public int genIndex = 0;
    [Header("Fitness")]
    public List<float> fitness = new List<float>();
    [Header("Genraton Seed")]
    public int seed = 1;

    //개체수
    [Header("generaton n 64")]
    public int genN = 64;
    //최대세대
    [Header("seed n 100")]
    public int seedN = 100;
    //엘리트 수
    [Header("eleitm 4")]
    public int ele = 4;

    //팔과탄환최대거리
    [Header("maxDist")]
    public float maxDist = 1.5f;

    // Update is called once per frame
    void Start()
    {
        InitObject();//유전자 개체 64개 생성  
        outputLayerNode = new float[4] { 10,0,0,0 };
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
    /// 신경망에서 계산한 출력값 기반 팔 돌리기 함수 
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
    /// 가중치 유전자 기반 신경망으로 출력값 계산 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public float[] NerualNetwork(float[] inputs)
    {
        float[,] weightIH = generatons[genIndex][0];
        float[,] weightHO = generatons[genIndex][1];
        // 1) 은닉층 계산: hidden = weightIH × inputs
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

        // 2) 출력층 계산: output = weightHO × hidden
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
    /// 엘리트, 토너먼트 선택과 교배 함수 
    /// </summary>
    IEnumerator SelecteGen()
    {
        //다음 세대에 넣을 것
        List<List<float[,]>> corrosGeneratons = new List<List<float[,]>>();

        // 엘리트 선택 후 다음 세대에 바로 넣어줌 (4개)
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

        // 토너먼트진행, 64개의 절반인 32개 선택
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

        // 다점 교배 진행
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

            // 1) 절단점 개수 지정
            int numPoints = 2;

            // 2) [1, parent1.Count) 범위에서 중복 없이 절단점 생성
            List<int> points = new List<int>();
            while (points.Count < numPoints)
            {
                int p = UnityEngine.Random.Range(1, parent1.Count);
                if (!points.Contains(p))
                    points.Add(p);
            }
            points.Sort();            // 오름차순 정렬
            points.Add(parent1.Count); // 마지막은 전체 길이

            // 4) 절단점 사이 구간을 번갈아 가며 붙여넣기
            int last = 0;
            bool takeFromP1 = true;
            foreach (int cut in points)
            {
                int len = cut - last;
                if (takeFromP1)
                {
                    // parent1 → child1, parent2 → child2
                    child1.AddRange(parent1.GetRange(last, len));
                    child2.AddRange(parent2.GetRange(last, len));
                }
                else
                {
                    // parent2 → child1, parent1 → child2
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
    /// 평가함수 
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

    /// <summary>
    /// 신경망 가중치 유전자조합을 가진 객체 64개 생성 
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
        // e^(-x) 계산
        return 1f / (1f + Mathf.Exp(-x));
    }
}
