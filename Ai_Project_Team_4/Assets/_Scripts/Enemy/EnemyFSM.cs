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

    [Header("공통 설정")]
    public float speed = 2f;
    public float detectRange = 5f;
    public Transform player;

    [Header("Patrol 설정")]
    private Vector2 patrolDirection;
    private float patrolTimer = 0f;
    public float patrolChangeTime = 2f;
    public float idleToPatrolTime = 1f;

    [Header("Detect 설정")]
    private float detectDuration = 0.4f;
    private float detectTimer = 0f;

    [Header("Roar 설정")]
    private float roarDuration = 0.1f;

    [Header("Chase 설정")]
    public float chaseSpeed = 3f;
    public float wallCheckDistance = 0.5f;

    [Header("Block 설정")]
    private float blockDuration = 1f;

    [Header("Stun 설정")]
    private float stunDuration = 2f;

    [Header("Hit Box")]
    public Collider2D Collider;

    [Header("stay time")]
    public float stayTime = 3f;
    public float currentTime = 0f;
    public Vector3 lastPos;
    public Vector3 currentPos;

    void Start()
    {
        lastPos = this.transform.position;
        GameObject found = GameObject.Find("Player");

        if (found != null)
            player = found.transform;
        else
            UnityEngine.Debug.LogWarning("Player 오브젝트를 찾을 수 없습니다.");

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
    /// 기본
    /// </summary>
    void Idle() 
    {
        if (PlayerDetected())
        {
            ChangeState(State.Detect);
            return;
        }

        if (stateTimer >= idleToPatrolTime)
        {
            ChangeState(State.Patrol);
        }
    }

    /// <summary>
    /// 배회 : 주변 어슬렁거림
    /// </summary>
    void Patrol()
    {
        patrolTimer += Time.deltaTime;
        currentTime += Time.deltaTime;

        if (patrolTimer > patrolChangeTime)
        {
            patrolDirection = UnityEngine.Random.insideUnitCircle.normalized;
            patrolTimer = 0f;
        }
        if (PlayerDetected())
            ChangeState(State.Detect);

        if (!IsWallInDirection(patrolDirection, wallCheckDistance))
        {
            transform.Translate(patrolDirection * speed * Time.deltaTime);
        }
        else
        {
            // 벽에 부딪히면 반사되는 방향 바꾸기
            patrolDirection = -UnityEngine.Random.insideUnitCircle.normalized;
            patrolTimer = 0f;
        }
        if(currentTime > stayTime)
        {
            currentPos = this.transform.position;
            if (lastPos == currentPos)
            {
                Debug.Log("change");
                patrolDirection = -patrolDirection;
            }
            currentTime = 0f;
            lastPos = this.transform.position;
        }


    }

    /// <summary>
    /// 발견 : 플레이어 감지
    /// </summary>
    void Detect()
    {
        detectTimer += Time.deltaTime;

        if (player == null) return;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        //float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);

        if (detectTimer >= detectDuration)
        {
            detectDuration = 0.4f;
            ChangeState(State.Roar);
        }

        if (!PlayerDetected())
        {
            detectDuration = 0.4f;
            ChangeState(State.Patrol);
        }
    }

    /// <summary>
    /// 포효
    /// </summary>
    void Roar()
    {
        if (stateTimer >= roarDuration)
        {
            ChangeState(State.Chase);
        }
    }

    /// <summary>
    /// 추격 : 플레이어를 향해 이동
    /// </summary>
    void Chase()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        if (!IsWallInDirection(dir, wallCheckDistance))
        {
            transform.Translate(dir * chaseSpeed * Time.deltaTime);
        }

        if (!PlayerDetected())
        {
            detectDuration = 1.4f;
            ChangeState(State.Detect);
        }
    }

    /// <summary>
    /// 막기 : 플레이어 무기 쳐내기
    /// </summary>
    void Block()
    {
        if (stateTimer >= blockDuration)
        {
            ChangeState(State.Idle);
        }
    }

    /// <summary>
    /// 기절 : 플레이어 무기 맞고 다운
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

        // 벽에 막혔는지 검사 (Wall 레이어만 검사)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Wall"));


        // 벽에 막혔으면 감지 실패
        if (hit.collider != null)
        {            
            return false;
        }

        // 감지 거리 안에 있고 벽에 안 막혔으면 감지 성공
        return distance <= detectRange;
    }

    /// <summary>
    /// 발사체와 맞으면 기절하는 부분 호출 부분 - 콜라이더 엔터로 사용
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

    private bool IsWallInDirection(Vector2 direction, float checkDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

    #region 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (player != null)
        {
            Gizmos.color = PlayerDetected() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // 상태 이름 표시
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, currentState.ToString());
#endif
    }
    #endregion
}
