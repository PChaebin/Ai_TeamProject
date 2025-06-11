using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnv : MonoBehaviour
{
    [Header("sensor")]
    [SerializeField]
    public InputSensor senc;

    [Header("Shooter")]
    [SerializeField]
    public List<GameObject> goList;

    [Header("pos")]
    [SerializeField]
    public List<Vector2> vecList;

    [Header("Projectil")]
    [SerializeField]
    public GameObject pro;

    private GameObject inst;
    private Projectile instpro;

    public int ind = 0;
    public int count = 0;

    public float current;
    public float cool = 5f;

    public string te;
    public List<float[,]> text = new List<float[,]>();

    // Start is called before the first frame update
    void Start()
    {
        vecList = new List<Vector2>();
        vecList.Add(new Vector2(8f, 0.5f));
        vecList.Add(new Vector2(8f, -3.5f));
        vecList.Add(new Vector2(1f, -3.5f));
        vecList.Add(new Vector2(-6f, -3.5f));
        vecList.Add(new Vector2(-6f, 0.5f));
        vecList.Add(new Vector2(-6f, 4.5f));
        vecList.Add(new Vector2(1f, 4.6f));
        vecList.Add(new Vector2(8f, 4.5f));
        current = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        return;
        if(Time.time - current < cool)
        {
            return;
        }
        inst = Instantiate(pro, goList[ind].transform.position, goList[ind].transform.rotation);
        instpro = inst.GetComponent<Projectile>();
        instpro.Fire(goList[ind].transform.position, vecList[ind]);
        ind++;
        count++;
        current = Time.time;
        if(ind >= 8)
        {
            ind = 0;
            if(count >= 16)
            {
                count = 0;
                senc.UpGenIndex();
            }
        }
    }
}
