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

    private int ind = 0;
    private int count = 0;

    private float currentTime = 0f;
    private float time = 3f;

    public bool shot = false;
    private int write = 0;

    [Header("end mix")]
    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source.Play();
        vecList = new List<Vector2>();
        vecList.Add(new Vector2(8f, 0.5f));
        vecList.Add(new Vector2(8f, -3.5f));
        vecList.Add(new Vector2(1f, -3.5f));
        vecList.Add(new Vector2(-6f, -3.5f));
        vecList.Add(new Vector2(-6f, 0.5f));
        vecList.Add(new Vector2(-6f, 4.5f));
        vecList.Add(new Vector2(1f, 4.6f));
        vecList.Add(new Vector2(8f, 4.5f));
        currentTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(!shot) return;
        if(Time.time - currentTime < time) return;
        ind = Random.Range(0, 8);
        inst = Instantiate(pro, goList[ind].transform.position, goList[ind].transform.rotation);
        instpro = inst.GetComponent<Projectile>();
        instpro.Fire(goList[ind].transform.position, vecList[ind]);
        currentTime = Time.time;
        count++;
        if (count >= 16)
        {
            write = senc.UpGenIndex();
            count = 0;
            source.Play();
        }
        if (write == -1)
        {
            source.Play();
            shot = false;
        }
    }
}
