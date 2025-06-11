using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LerningSystem : MonoBehaviour
{
    //유전자개체 리스트와 객체인덱스와 세대수와 평가함수
    [Header("Generaton")]
    public List<List<float[,]>> generatons = new List<List<float[,]>>();
    [Header("current Generaton Index")]
    public int genIndex = 0;
    [Header("Fitness")]
    public List<float> fitness = new List<float>();
    [Header("current Seed")]
    public int seedIndex = 1;

    //개체수
    [Header("generaton n")]
    public int genN = 8;
    [Header("gen I")]
    public int genI = 35;
    [Header("gen O")]
    public int genO = 20;

    //최대세대
    [Header("seed n")]
    public int seedN = 4;
    //엘리트 수
    [Header("eleitm")]
    public int ele = 2;

    //인풋
    [Header("inputN")]
    public int inputN = 7;
    //히든
    [Header("hiddenN")]
    public int hiddenN = 5;
    //아웃풋
    [Header("outputN")]
    public int outputN = 4;

    [Header("Mutate1")]
    public float mutate1 = 55f;
    [Header("Mutate2")]
    public float mutate2 = 35f;

    //팔과탄환최대거리
    [Header("maxDist")]
    public float maxDist = 1.5f;

    public float[,] Getmaterix(int i)
    {
        return generatons[genIndex][i];
    }

    public int genIndexAndSeedUP()
    {
        genIndex++;
        if(genIndex >= genN)
        {
            seedIndex++;
            genIndex = 0;
            return -1;
        }
        return genIndex;
    }

    public bool IsOverSeed()
    {
        if(seedIndex > seedN)
        {
            return true;
        }
        return false;
    }

    public void NextGens()
    {
        generatons = Mutate(SelecteGen(Tournament(), SelectEleti()));
    }

    public List<List<float[,]>> SelectEleti()
    {
        //다음 세대에 넣을 것
        List<List<float[,]>> corrosGeneratons = new List<List<float[,]>>();

        // 엘리트 선택 후 다음 세대에 바로 넣어줌 (4개)
        for (int i = 0; i < ele; i++)
        {
            float bestValue = fitness.Max();
            int bestIndex = fitness.FindIndex(x => x == bestValue);
            corrosGeneratons.Add(generatons[bestIndex]);
        }

        return corrosGeneratons;
    }

    public List<List<float[,]>> Tournament()
    {
        int firstIndex;
        int secondIndex;

        float gen1;
        float gen2;

        List<List<float[,]>> tonement = new List<List<float[,]>>();

        // 토너먼트진행, 64개의 절반인 32개 선택
        for (int i = 0; i < genN / 2; i++)
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
        return tonement;
    }

    /// <summary>
    /// 엘리트, 토너먼트 선택과 교배 함수 
    /// </summary>
    public List<List<float[,]>> SelecteGen(List<List<float[,]>> tonement, List<List<float[,]>> corrosGeneratons)
    {
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
            par2indx = UnityEngine.Random.Range(0, tonement.Count - 1);
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
        }
        return corrosGeneratons;
    }

    public List<List<float[,]>> Mutate(List<List<float[,]>> corrosGeneratons)
    {
        float mutate = 0;
        int idx = 0;
        int x = 0;
        int y = 0;

        for (int j = 3; j < genN; j++)
        {
            mutate = UnityEngine.Random.value * 100;
            if (mutate < mutate1)
            {
                idx = UnityEngine.Random.Range(0, genI);
                x = idx % hiddenN;
                y = idx % inputN;
                corrosGeneratons[j][0][x, y] = UnityEngine.Random.Range(-1f, 1f);
            }
            if (mutate < mutate2)
            {
                idx = UnityEngine.Random.Range(0, genO);
                x = idx % outputN;
                y = idx % hiddenN;
                corrosGeneratons[j][1][x, y] = UnityEngine.Random.Range(-1f, 1f);
            }
        }

        fitness.Clear();

        return corrosGeneratons;
    }

    /// <summary>
    /// 평가함수 
    /// </summary>
    public void SetFitness(GameObject leftPoint, GameObject rightPoint, GameObject projectileObj)
    {
        float leftdist = maxDist - (leftPoint.transform.position - projectileObj.transform.position).magnitude;
        if (leftdist < 0)
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

    /// <summary>
    /// 신경망 가중치 유전자조합을 가진 객체 64개 생성 
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
        float[,] ih = new float[hiddenN, inputN];
        float[,] ho = new float[outputN, hiddenN];
        for (int h = 0; h < hiddenN; h++)
        {
            for (int i = 0; i < inputN; i++)
            {
                ih[h, i] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
        for (int o = 0; o < outputN; o++)
        {
            for (int h = 0; h < hiddenN; h++)
            {
                ho[o, h] = UnityEngine.Random.Range(-1f, 1f);
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
    public void WriteFileGreateGen()
    {
        float mx = fitness.Max();
        int idx = fitness.IndexOf(mx);
        List<float[,]> eleGen = generatons[idx];

        string bestGen = "";
        for(int i = 0; i < hiddenN; i++)
        {
            for (int j = 0; j < inputN; j++)
            {
                bestGen += eleGen[0][i, j].ToString();
                if (j == inputN - 1) break;
                bestGen += ", ";
            }
            if (i == hiddenN - 1) break;
            bestGen += " % ";
        }
        bestGen += " / ";
        for (int i = 0; i < outputN; i++)
        {
            for (int j = 0; j < hiddenN; j++)
            {
                bestGen += eleGen[1][i, j].ToString();
                if (j == hiddenN - 1) break;
                bestGen += ", ";
            }
            if (i == outputN - 1) break;
            bestGen += " % ";
        }
        PlayerPrefs.SetString("best", bestGen);
    }
}
