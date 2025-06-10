using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LerningSystem : MonoBehaviour
{
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

    public float[,] Getmaterix(int i)
    {
        return generatons[genIndex][i];
    }

    public int genIndexUP()
    {
        genIndex++;
        if(genIndex >= 128)
        {
            return -1;
        }
        return genIndex;
    }

    public List<List<float[,]>> SelectEleti(List<float> fitList)
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

        return corrosGeneratons;
    }

    public List<List<float[,]>> Tournament()
    {
        int firstIndex;
        int secondIndex;

        float gen1;
        float gen2;

        List<List<float[,]>> tonement = new List<List<float[,]>>();

        // ��ʸ�Ʈ����, 64���� ������ 32�� ����
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
    /// ����Ʈ, ��ʸ�Ʈ ���ð� ���� �Լ� 
    /// </summary>
    public List<List<float[,]>> SelecteGen(List<List<float[,]>> tonement, List<List<float[,]>> corrosGeneratons)
    {
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
            par2indx = UnityEngine.Random.Range(0, tonement.Count - 1);
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
                y = idx % 5;
                x = idx / 5;
                corrosGeneratons[j][0][y, x] = UnityEngine.Random.Range(10, 80);
            }
            if (mutate < mutate2)
            {
                idx = UnityEngine.Random.Range(0, genO);
                y = idx % 4;
                x = idx / 4;
                corrosGeneratons[j][1][y, x] = UnityEngine.Random.Range(10, 80);
            }
        }
        seed++;
        genIndex = 0;

        return corrosGeneratons;
    }

    /// <summary>
    /// ���Լ� 
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
                ih[h, i] = UnityEngine.Random.Range(10, 80);
            }
        }
        for (int o = 0; o < 4; o++)
        {
            for (int h = 0; h < 5; h++)
            {
                ho[o, h] = UnityEngine.Random.Range(10, 80);
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

}
