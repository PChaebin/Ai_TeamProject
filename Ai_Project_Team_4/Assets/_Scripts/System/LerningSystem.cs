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
    public List<float> fitness;
    [Header("current Seed")]
    public int seedIndex = 1;

    //개체수
    [Header("generaton n")]
    public int genN;
    [Header("gen I")]
    public int genI = 35;
    [Header("gen O")]
    public int genO = 20;

    //최대세대
    [Header("seed n")]
    public int seedN;
    //엘리트 수
    [Header("eleitm")]
    public int ele;

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

    public int GetSeedIndex()
    {
        return seedIndex;
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
        //Debug.Log("select count : "+corrosGeneratons.Count);
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
        //Debug.Log("tonement count : " + tonement.Count);
        return tonement;
    }

    /// <summary>
    /// 엘리트, 토너먼트 선택과 교배 함수 
    /// </summary>
    public List<List<float[,]>> SelecteGen(List<List<float[,]>> tonement, List<List<float[,]>> corrosGeneratons)
    {
        //Debug.Log("eletism count : "+corrosGeneratons.Count);
        // 목표 크기
        int targetSize = generatons.Count;
        //Debug.Log("targetSize : " + targetSize);
        while (corrosGeneratons.Count < targetSize)
        {
            // 부모 두 개 랜덤 선택 (서로 다른 인덱스)
            int i1 = Random.Range(0, tonement.Count);
            int i2 = Random.Range(0, tonement.Count - 1);
            if (i2 >= i1) i2++;
            var parent1 = tonement[i1];
            var parent2 = tonement[i2];

            // 절단점 개수는 parent 길이에 맞게 조정
            int maxCuts = Mathf.Max(1, parent1.Count - 1);
            int numCuts = Mathf.Min(2, maxCuts);

            // 1) 가능한 절단점(1 ~ Count-1) 리스트
            var possibleCuts = Enumerable.Range(1, parent1.Count - 1).ToList();
            // 2) 섞어서 numCuts개 뽑고 정렬
            var cuts = possibleCuts
                .OrderBy(_ => Random.value)
                .Take(numCuts)
                .OrderBy(x => x)
                .ToList();
            cuts.Add(parent1.Count);  // 끝 지점 추가

            // 3) 교차 생성
            var child1 = new List<float[,]>();
            var child2 = new List<float[,]>();
            int last = 0;
            bool takeP1 = true;
            foreach (int cut in cuts)
            {
                int len = cut - last;
                if (takeP1)
                {
                    child1.AddRange(parent1.GetRange(last, len));
                    child2.AddRange(parent2.GetRange(last, len));
                }
                else
                {
                    child1.AddRange(parent2.GetRange(last, len));
                    child2.AddRange(parent1.GetRange(last, len));
                }
                takeP1 = !takeP1;
                last = cut;
            }

            corrosGeneratons.Add(child1);
            corrosGeneratons.Add(child2);
        }
        if(corrosGeneratons.Count > targetSize)
        {
            for(int i = corrosGeneratons.Count -1; i >= targetSize; i--)
            {
                corrosGeneratons.RemoveAt(i);
            }
        }
        //Debug.Log("corrosGeneratons count : " + corrosGeneratons.Count);
        return corrosGeneratons;
    }

    public List<List<float[,]>> Mutate(List<List<float[,]>> corrosGeneratons)
    {
        float mutate = 0;
        int idx = 0;
        int x = 0;
        int y = 0;
        for (int j = ele; j < genN; j++)
        {
            mutate = UnityEngine.Random.value * 100;
            //Debug.Log("mutate : " + mutate);
            if (mutate <= mutate1)
            {
                idx = UnityEngine.Random.Range(0, genI);
                //Debug.Log(idx);
                x = idx % hiddenN;
                y = idx % inputN;
                corrosGeneratons[j][0][x, y] = Mathf.Floor(UnityEngine.Random.Range(-1f, 1f) * 100f) / 100f;
            }
            if (mutate <= mutate2)
            {
                idx = UnityEngine.Random.Range(0, genO);
                //Debug.Log(idx);
                x = idx % outputN;
                y = idx % hiddenN;
                corrosGeneratons[j][1][x, y] = Mathf.Floor(UnityEngine.Random.Range(-1f, 1f) * 100f) / 100f;
            }
        }

        fitness.Clear();
        for(int i = 0; i < genN; i++)
        {
            fitness.Add(0);
        }
        //Debug.Log("fitness count : "+fitness.Count);
        return corrosGeneratons;
    }

    /// <summary>
    /// 평가함수 
    /// </summary>
    public void SetFitness(GameObject leftPoint, GameObject rightPoint, GameObject projectileObj)
    {
        float leftdist = maxDist - (projectileObj.transform.position - leftPoint.transform.position).magnitude;
        if (leftdist < 0)
        {
            leftdist = 0;
        }
        float rightdist = maxDist - (projectileObj.transform.position - rightPoint.transform.position).magnitude;
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
            //Debug.Log(fitness.Count);
        }
       //Debug.Log("init");
    }
    public List<float[,]> InitGeneratons()
    {
        float[,] ih = new float[hiddenN, inputN];
        float[,] ho = new float[outputN, hiddenN];
        for (int h = 0; h < hiddenN; h++)
        {
            for (int i = 0; i < inputN; i++)
            {
                ih[h, i] = Mathf.Floor(UnityEngine.Random.Range(-1f, 1f) * 100f ) / 100f;
                //Debug.Log("ih : "+ih[h, i]);
            }
        }
        for (int o = 0; o < outputN; o++)
        {
            for (int h = 0; h < hiddenN; h++)
            {
                ho[o, h] = Mathf.Floor(UnityEngine.Random.Range(-1f, 1f) *100f) / 100f;
                //Debug.Log("ho : " + ho[o, h]);
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
        Debug.Log("result : "+PlayerPrefs.GetString("best"));
    }
}
