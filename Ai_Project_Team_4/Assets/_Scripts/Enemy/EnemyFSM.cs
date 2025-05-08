using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public static EnemyFSM Instance;

    public enum State
    {
        Idle, 
        Patrol,
        Detect,
        Roar,
        Chase,
        Block,
        Stun
    }

    public State currentState = State.Idle;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 기본
    /// </summary>
    void Idle() 
    {

    }

    /// <summary>
    /// 배회 : 주변 어슬렁거림
    /// </summary>
    void Patrol()
    {

    }

    /// <summary>
    /// 발견 : 플레이어 감지
    /// </summary>
    void Detect()
    {

    }

    /// <summary>
    /// 포효
    /// </summary>
    void Roar()
    {

    }

    /// <summary>
    /// 추격 : 플레이어를 향해 이동
    /// </summary>
    void Chase()
    {

    }

    /// <summary>
    /// 쳐내기 : 플레이어 무기 쳐내기
    /// </summary>
    void Block()
    {

    }

    /// <summary>
    /// 기절 : 플레이어 무기 맞고 다운
    /// </summary>
    void Stun()
    {

    }
}
