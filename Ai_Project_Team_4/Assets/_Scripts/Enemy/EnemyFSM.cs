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
    /// �⺻
    /// </summary>
    void Idle() 
    {

    }

    /// <summary>
    /// ��ȸ : �ֺ� ����Ÿ�
    /// </summary>
    void Patrol()
    {

    }

    /// <summary>
    /// �߰� : �÷��̾� ����
    /// </summary>
    void Detect()
    {

    }

    /// <summary>
    /// ��ȿ
    /// </summary>
    void Roar()
    {

    }

    /// <summary>
    /// �߰� : �÷��̾ ���� �̵�
    /// </summary>
    void Chase()
    {

    }

    /// <summary>
    /// �ĳ��� : �÷��̾� ���� �ĳ���
    /// </summary>
    void Block()
    {

    }

    /// <summary>
    /// ���� : �÷��̾� ���� �°� �ٿ�
    /// </summary>
    void Stun()
    {

    }
}
