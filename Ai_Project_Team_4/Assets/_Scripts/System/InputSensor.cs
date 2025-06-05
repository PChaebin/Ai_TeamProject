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
            // 반환 값 팔 돌리기 
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

            outputLayerNode = NerualNetwork(inputLayerNode, weightIH, weightHO);
        }
        isTurning = false;
        if(system != null)
        {
            system.SetFitness(leftArmPoint, rightArmPoint, collision.gameObject);
        }
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
    public float[] NerualNetwork(float[] inputs,float[,] weightInstIH, float[,] weightInstHO)
    {
        // 1) 은닉층 계산: hidden = weightIH × inputs
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
        // 2) 출력층 계산: output = weightHO × hidden
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
            Debug.LogWarning($"파일이 존재하지 않습니다: {path}");
            return result;
        }

        // 모든 라인을 한꺼번에 읽어온다 (UTF8)
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        int i = 0;
        while (i < lines.Length)
        {
            string line = lines[i].Trim();

            // 빈 줄(또는 공백)인 경우 다음 라인으로 넘어감
            if (string.IsNullOrEmpty(line))
            {
                i++;
                continue;
            }

            // 헤더 형식: "# Array {index} (행: {rows}, 열: {cols})"
            if (line.StartsWith("# Array"))
            {
                // 헤더를 건너뛴 뒤, 실제 데이터 라인을 수집한다.
                i++;

                // 한 배열의 각 행(row)을 담을 리스트
                List<string> rowLines = new List<string>();

                // 빈 줄이 나오기 전까지 계속 읽는다.
                while (i < lines.Length && !string.IsNullOrEmpty(lines[i].Trim()))
                {
                    rowLines.Add(lines[i].Trim());
                    i++;
                }

                // rowLines.Count가 곧 행 개수
                int rowCount = rowLines.Count;
                if (rowCount == 0)
                {
                    // 만약 빈 배열(헤더만 있고 데이터 없음)인 경우, 0×0 배열로 처리하거나 건너뛸지 결정
                    result.Add(new float[0, 0]);
                    continue;
                }

                // 첫 행에서 콤마 개수로 열(column) 개수를 추정
                string[] firstTokens = rowLines[0].Split(',');
                int colCount = firstTokens.Length;

                // 2D 배열 생성
                float[,] array2D = new float[rowCount, colCount];

                // 실제 데이터 파싱
                for (int r = 0; r < rowCount; r++)
                {
                    string[] tokens = rowLines[r].Split(',');

                    // (안정성) 열 개수가 다를 경우, 작은 쪽만 처리
                    int tokensToRead = Mathf.Min(tokens.Length, colCount);

                    for (int c = 0; c < tokensToRead; c++)
                    {
                        // float.Parse 로 문자열을 실수로 변환
                        if (float.TryParse(tokens[c], out float parsed))
                        {
                            array2D[r, c] = parsed;
                        }
                        else
                        {
                            // 파싱 실패 시 로그 남기고 0으로 채움
                            Debug.LogWarning($"[{r},{c}] 위치 값 '{tokens[c]}'를 float로 파싱할 수 없습니다. 0으로 설정합니다.");
                            array2D[r, c] = 0f;
                        }
                    }

                    // 만약 토큰 수가 colCount보다 적다면 나머지 열은 기본 0으로 남음
                }

                // 완성된 2D 배열을 결과 리스트에 추가
                result.Add(array2D);

                // now i는 빈 줄(또는 파일 끝)위치에 있으므로, 빈 줄을 건너뛸 것
                // (while 루프 시작부에서 빈 줄을 걸러줌)
            }
            else
            {
                // 헤더가 아닌(예상치 못한) 일반 라인이 온 경우 건너뛴다.
                Debug.LogWarning($"헤더('# Array')가 아닌 예기치 않은 라인: '{line}' (라인 {i + 1})");
                i++;
            }
        }
        return result;
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
