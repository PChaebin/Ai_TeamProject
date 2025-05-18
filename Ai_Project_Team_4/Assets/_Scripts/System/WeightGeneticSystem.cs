using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightGeneticSystem : MonoBehaviour
{
    [Header("Generaton")]
    public List<List<float[,]>> generatons = new List<List<float[,]>>();

    private void Start()
    {
        InitObject();
    }

    public void InitObject()
    {
        for(int c = 0; c < 64; c++)
        {
            generatons.Add(InitGeneratons());
        }
        Print(generatons);
    }

    public void Print(List<List<float[,]>> objList)
    {
        for(int i = 0; i < 64; i++)
        {
            List<float[,]> gen = objList[i];
            float[,] ihObj = gen[0];
            float[,] hoObj = gen[1];

            for (int h = 0; h < 5; h++)
            {
                for(int k =0; k < 7; k++)
                {
                    Debug.Log(i + " obj : " + h + " 열 : "+ k + " 줄 : " +ihObj[h,k]);
                }
            }
            for (int h = 0; h < 4; h++)
            {
                for (int k = 0; k < 5; k++)
                {
                    Debug.Log(i + " obj : " + h + " 열 : " + k + " 줄 : " + hoObj[h, k]);
                }
            }
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
                ih[h, i] = Random.value;
            }
        }
        for (int o = 0; o < 4; o++)
        {
            for (int h = 0; h < 5; h++)
            {
                ho[o, h] = Random.value;
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
    public float[] NerualNetwork(float[] inputs, float[,] weightIH, float[,] weightHO)
    {
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
    private static float Sigmoid(float x)
    {
        // e^(-x) 계산
        return 1f / (1f + Mathf.Exp(-x));
    }
}
