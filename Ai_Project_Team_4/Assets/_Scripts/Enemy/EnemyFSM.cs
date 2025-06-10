using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    // public static EnemyFSM Instance;

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

    private float stateTimer = 0f;

    [Header("���� ����")]
    public float speed = 2f;
    public float detectRange = 5f;
    public Transform player;

    [Header("Patrol ����")]
    private Vector2 patrolDirection;
    private float patrolTimer = 0f;
    public float patrolChangeTime = 2f;

    [Header("Detect ����")]
    private float detectDuration = 1.5f;
    private float detectTimer = 0f;

    [Header("Roar ����")]
    private float roarDuration = 1f;

    [Header("Chase ����")]
    public float chaseSpeed = 3f;

    [Header("Block ����")]
    private float blockDuration = 1f;

    [Header("Stun ����")]
    private float stunDuration = 2f;

    [Header("Hit Box")]
    public Collider2D Collider;

    void Start()
    {
        GameObject found = GameObject.Find("Player");

        if (found != null)
            player = found.transform;
        else
            UnityEngine.Debug.LogWarning("Player ������Ʈ�� ã�� �� �����ϴ�.");

        ChangeState(State.Idle);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle: 
                Idle();
                break;
            case State.Patrol:
                Patrol(); 
                break;
            case State.Detect: 
                Detect(); 
                break;
            case State.Roar: 
                Roar();
                break;
            case State.Chase:
                Chase(); 
                break;
            case State.Block: 
                Block(); 
                break;
            case State.Stun: 
                Stun(); 
                break;
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        stateTimer = 0f;
        Debug.Log($"[FSM] State changed to: {newState}");

        switch (newState)
        {
            case State.Patrol:
                patrolTimer = 0f;
                patrolDirection = UnityEngine.Random.insideUnitCircle.normalized;
                break;
            case State.Detect:
                detectTimer = 0f;
                break;
        }
    }


    /// <summary>
    /// �⺻
    /// </summary>
    void Idle() 
    {
        if (PlayerDetected())
            ChangeState(State.Detect);
    }

    /// <summary>
    /// ��ȸ : �ֺ� ����Ÿ�
    /// </summary>
    void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolChangeTime)
        {
            patrolDirection = UnityEngine.Random.insideUnitCircle.normalized;
            patrolTimer = 0f;
        }

        transform.Translate(patrolDirection * speed * Time.deltaTime);

        if (PlayerDetected())
            ChangeState(State.Detect);
    }

    /// <summary>
    /// �߰� : �÷��̾� ����
    /// </summary>
    void Detect()
    {
        detectTimer += Time.deltaTime;

        if (player == null) return;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (detectTimer >= detectDuration)
        {
            ChangeState(State.Roar);
        }
    }

    /// <summary>
    /// ��ȿ
    /// </summary>
    void Roar()
    {
        if (stateTimer >= roarDuration)
        {
            ChangeState(State.Chase);
        }
    }

    /// <summary>
    /// �߰� : �÷��̾ ���� �̵�
    /// </summary>
    void Chase()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * chaseSpeed * Time.deltaTime);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            // ���� �ε����� �������� ����
            return;
        }

        if (!PlayerDetected())
        {
            ChangeState(State.Patrol);
        }
    }

    /// <summary>
    /// ���� : �÷��̾� ���� �ĳ���
    /// </summary>
    void Block()
    {
        if (stateTimer >= blockDuration)
        {
            ChangeState(State.Idle);
        }
    }

    /// <summary>
    /// ���� : �÷��̾� ���� �°� �ٿ�
    /// </summary>
    void Stun()
    {
        if (stateTimer >= stunDuration)
        {
            ChangeState(State.Idle);
        }
    }

    bool PlayerDetected()
    {
        if (player == null) return false;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        // ���� �������� �˻� (Wall ���̾ �˻�)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Wall"));


        // ���� �������� ���� ����
        if (hit.collider != null)
        {
            
            return false;
        }

        // ���� �Ÿ� �ȿ� �ְ� ���� �� �������� ���� ����
        return distance <= detectRange;
    }

    /// <summary>
    /// �߻�ü�� ������ �����ϴ� �κ� ȣ�� �κ� - �ݶ��̴� ���ͷ� ���
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile item = collision.gameObject.GetComponent<Projectile>();
            item.SetDestoryed();
            item.OffRender();
            ChangeState(State.Stun);
        }
    }
    #region ����� �׸���
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (player != null)
        {
            Gizmos.color = PlayerDetected() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // ���� �̸� ǥ��
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, currentState.ToString());
#endif
    }
    #endregion
}
