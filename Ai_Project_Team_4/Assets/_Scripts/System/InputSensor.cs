using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

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

    [Header("Uses I")]
    float[,] weightIH;
    [Header("Uses O")]
    float[,] weightHO;
    [Header("final gen")]
    List<float[,]> finalGen;

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
    [Header("generaton n")]
    public int genN = 128;
    [Header("gen I")]
    public int genI = 35;
    [Header("gen O")]
    public int genO = 20;
    //�ִ뼼��
    [Header("seed n")]
    public int seedN = 100;
    //����Ʈ ��
    [Header("eleitm")]
    public int ele = 4;

    [Header("Mutate1")]
    public float mutate1 = 55f;
    [Header("Mutate2")]
    public float mutate2 = 35f;

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
        weightIH = generatons[genIndex][0];
        weightHO = generatons[genIndex][1];
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
        for (int i = 0; i < ele; i++)
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
        for (int i = 0; i < genN/2; i++)
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

        float mutate = 0;
        int idx = 0;
        int x = 0;
        int y = 0;

        for (int j =3; j < genN; j++)
        {
            mutate = UnityEngine.Random.value * 100;
            if(mutate < mutate1)
            {
                idx = UnityEngine.Random.Range(0, genI);
                y = idx % 5;
                x = idx / 5;
                corrosGeneratons[j][0][y,x] = UnityEngine.Random.Range(10,80);
            }
            if (mutate < mutate2)
            {
                idx = UnityEngine.Random.Range(0, genO);
                y = idx % 4;
                x = idx / 4;
                corrosGeneratons[j][1][y, x] = UnityEngine.Random.Range(10, 80);
            }
        }

        generatons = corrosGeneratons;

        seed++;
        genIndex = 0;
    }

    public void WriteFileGreateGen()
    {
        float mx = fitness.Max();
        int idx = fitness.IndexOf(mx);
        List<float[,]> eleGen = generatons[idx];
        
        string path = Path.Combine("D:\\UnityHub\\UnityGame\\Ai_TeamProject\\Ai_Project_Team_4\\Assets\\_Scripts\\System", "eletism");
        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            for (int arrayIndex = 0; arrayIndex < eleGen.Count; arrayIndex++)
            {
                float[,] array2D = eleGen[arrayIndex];
                int rows = array2D.GetLength(0);
                int cols = array2D.GetLength(1);

                // �迭 ������ ���� ��� ���� ���� (���� ����)
                writer.WriteLine($"# Array {arrayIndex} (��: {rows}, ��: {cols})");

                // �� ��(row)���� �����͸� �� �ٿ� �޸�(�Ǵ� ��)�� �����ؼ� �ۼ�
                for (int r = 0; r < rows; r++)
                {
                    StringBuilder lineBuilder = new StringBuilder();
                    for (int c = 0; c < cols; c++)
                    {
                        lineBuilder.Append(array2D[r, c].ToString());

                        // ������ ���� �ƴϸ� ������ �߰�
                        if (c < cols - 1)
                            lineBuilder.Append(",");  // �޸�(,)�� ����. �ʿ� �� '\t'�� ���� ����
                    }

                    writer.WriteLine(lineBuilder.ToString());
                }

                // �迭 ���� �� �� �ϳ� �߰� (���� ����)
                writer.WriteLine();
            }
        }
        Debug.Log($"�����͸� ���� ��ο� �����߽��ϴ�: {path}");

    }

    public List<float[,]> ReadFileGreateGen()
    {
        List<float[,]> result = new List<float[,]>();
        string path = Path.Combine("D:\\UnityHub\\UnityGame\\Ai_TeamProject\\Ai_Project_Team_4\\Assets\\_Scripts\\System", "eletism");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"������ �������� �ʽ��ϴ�: {path}");
            return result;
        }

        // ��� ������ �Ѳ����� �о�´� (UTF8)
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        int i = 0;
        while (i < lines.Length)
        {
            string line = lines[i].Trim();

            // �� ��(�Ǵ� ����)�� ��� ���� �������� �Ѿ
            if (string.IsNullOrEmpty(line))
            {
                i++;
                continue;
            }

            // ��� ����: "# Array {index} (��: {rows}, ��: {cols})"
            if (line.StartsWith("# Array"))
            {
                // ����� �ǳʶ� ��, ���� ������ ������ �����Ѵ�.
                i++;

                // �� �迭�� �� ��(row)�� ���� ����Ʈ
                List<string> rowLines = new List<string>();

                // �� ���� ������ ������ ��� �д´�.
                while (i < lines.Length && !string.IsNullOrEmpty(lines[i].Trim()))
                {
                    rowLines.Add(lines[i].Trim());
                    i++;
                }

                // rowLines.Count�� �� �� ����
                int rowCount = rowLines.Count;
                if (rowCount == 0)
                {
                    // ���� �� �迭(����� �ְ� ������ ����)�� ���, 0��0 �迭�� ó���ϰų� �ǳʶ��� ����
                    result.Add(new float[0, 0]);
                    continue;
                }

                // ù �࿡�� �޸� ������ ��(column) ������ ����
                string[] firstTokens = rowLines[0].Split(',');
                int colCount = firstTokens.Length;

                // 2D �迭 ����
                float[,] array2D = new float[rowCount, colCount];

                // ���� ������ �Ľ�
                for (int r = 0; r < rowCount; r++)
                {
                    string[] tokens = rowLines[r].Split(',');

                    // (������) �� ������ �ٸ� ���, ���� �ʸ� ó��
                    int tokensToRead = Mathf.Min(tokens.Length, colCount);

                    for (int c = 0; c < tokensToRead; c++)
                    {
                        // float.Parse �� ���ڿ��� �Ǽ��� ��ȯ
                        if (float.TryParse(tokens[c], out float parsed))
                        {
                            array2D[r, c] = parsed;
                        }
                        else
                        {
                            // �Ľ� ���� �� �α� ����� 0���� ä��
                            Debug.LogWarning($"[{r},{c}] ��ġ �� '{tokens[c]}'�� float�� �Ľ��� �� �����ϴ�. 0���� �����մϴ�.");
                            array2D[r, c] = 0f;
                        }
                    }

                    // ���� ��ū ���� colCount���� ���ٸ� ������ ���� �⺻ 0���� ����
                }

                // �ϼ��� 2D �迭�� ��� ����Ʈ�� �߰�
                result.Add(array2D);

                // now i�� �� ��(�Ǵ� ���� ��)��ġ�� �����Ƿ�, �� ���� �ǳʶ� ��
                // (while ���� ���ۺο��� �� ���� �ɷ���)
            }
            else
            {
                // ����� �ƴ�(����ġ ����) �Ϲ� ������ �� ��� �ǳʶڴ�.
                Debug.LogWarning($"���('# Array')�� �ƴ� ����ġ ���� ����: '{line}' (���� {i + 1})");
                i++;
            }
        }
        return result;
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
        fitness[genIndex] += total;
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
        for (int c = 0; c < genN; c++)
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
