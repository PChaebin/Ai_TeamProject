using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class WeightGeneticSystem : MonoBehaviour
{
    [Header("Generaton")]
    public List<List<float[,]>> generatons = new List<List<float[,]>>();

    [Header("Generaton Index")]
    public int genIndex = 0;

    [Header("Fitness")]
    public List<float> fitness = new List<float>();

    [Header("Genraton Seed")]
    public int seed = 1;

    private void Start()
    {
        InitObject();
    }

    public void SelecteGen()
    {
        List<List<float[,]>> bester = new List<List<float[,]>>();
        for(int i = 0; i < 4; i++)
        {
            float bestValue = fitness.Max();
            int bestIndex = fitness.FindIndex(x => x == bestValue );
            bester.Add(generatons[bestIndex]);
            generatons.Remove(generatons[bestIndex]);
            fitness.Remove(fitness[bestIndex]);
        }

        for (int i =0; i < 30; i++)
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

    public void CrossOver()
    {
        seed++;
    }

    public void SetFitness(bool isProtected, float distance = 0)
    {
        genIndex++;
        if(genIndex >= 64)
        {
            genIndex = 0;
        }
    }

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

    public void InitObject()
    {
        for(int c = 0; c < 64; c++)
        {
            generatons.Add(InitGeneratons());
        }
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
        // e^(-x) 계산
        return 1f / (1f + Mathf.Exp(-x));
    }
}
