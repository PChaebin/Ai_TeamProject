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
    [Header("TestLearning")]
    public LerningSystem system;

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

    // Update is called once per frame
    void Start()
    {
        outputLayerNode = new float[4] { 0,0,0,0 };
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

    private void FixedUpdate()
    {
        if(isTurning)
        {
            // ��ȯ �� �� ������ 
            rotateAction(outputLayerNode);
        }
    }
    public void UpGenIndex()
    {
        int a = system.genIndexUP();
        if (a == -1)
        {
            system.WriteFileGreateGen();
            return;
        }
        weightIH = system.Getmaterix(0);
        weightHO = system.Getmaterix(1);
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

            outputLayerNode = NerualNetwork(inputLayerNode, weightIH, weightHO);
        }
        isTurning = false;
        if(system != null)
        {
            system.SetFitness(leftArmPoint, rightArmPoint, collision.gameObject);
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
    public float[] NerualNetwork(float[] inputs,float[,] weightInstIH, float[,] weightInstHO)
    {
        // 1) ������ ���: hidden = weightIH �� inputs
        var hidden = new float[5];
        for (int h = 0; h < 5; h++)
        {
            float sum = 0f;
            for (int i = 0; i < 7; i++)
            {
                sum += weightInstIH[h, i] * inputs[i];
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
                sum += weightInstHO[o, h] * hidden[h];
            }
            output[o] = Sigmoid(sum);
        }

        return output;
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

    private static float Sigmoid(float x)
    {
        // e^(-x) ���
        return 1f / (1f + Mathf.Exp(-x));
    }
}
