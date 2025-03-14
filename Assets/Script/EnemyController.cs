using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Transform player;
    NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;
    GameManager gameManager;

    [Header("Movement")]
    [SerializeField] float _patrolSpeed = 5;
    [SerializeField] float _trackSpeed = 10;
    Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;
    [SerializeField] Transform[] patrolPosition;
    [HideInInspector] public int patrolNumber = 0;
    public bool _detectedPlayer = false;

    public enum EnemyState { Patrol, Track, Night }
    EnemyState currentState = EnemyState.Patrol;


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
        _agent.updatePosition = false;
        _rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.instance;
    }

    void Start()
    {
        _agent.speed = _patrolSpeed;
    }

    void FixedUpdate()
    {
        if (gameManager.IsPlaying)
        {
            //플레이어와 접근할 시 플레이어 잡음 
            if (Vector2.Distance(player.position, transform.position) <= 1f)
            {
                Capture();
            }
            _rb.position = _agent.nextPosition;
            //현재 패턴에 따라 행동
            switch (currentState)
            {
                //순찰
                case EnemyState.Patrol:
                    Patrol();
                    break;
                //추적
                case EnemyState.Track:
                    Track();
                    break;
                //밤 추적
                case EnemyState.Night:
                    NightTrack();
                    break;
            }
        }
    }

    //순찰 패턴
    //일정 루트를 따라 순찰한다
    void Patrol()
    {
        if (gameManager.IsNight)
        {
            _agent.speed = _trackSpeed;
            currentState = EnemyState.Night;
            return;
        }
        Vector3 walkPoint = patrolPosition[patrolNumber].position;
        _agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;


        if (distanceToWalkPoint.magnitude < 1f)
        {

            patrolNumber++;
            if (patrolNumber >= patrolPosition.Length)
            {
                patrolNumber = 0;
            }
        }
    }

    //추적
    //플레이어를 일정 시간 추적한다
    //일정 시간 후 플레이어가 거리 내에 없으면 순찰로 복귀
    void Track()
    {
        if (gameManager.IsNight)
        {
            _agent.speed = _trackSpeed;
            currentState = EnemyState.Night;
            return;
        }
        _agent.SetDestination(player.position);
        Invoke(nameof(Search), 5f);
    }
    void Search()
    {
        _agent.speed = _patrolSpeed;
        ReturnToPatrol();
    }

    //밤이 됬을 때 추적
    //거리 상관없이 플레이어를 추적
    void NightTrack()
    {
        _agent.SetDestination(player.position);
    }
    public void ReturnToPatrol()
    {
        // _agent.SetDestination(patrolPosition[patrolNumber].position);
        currentState = EnemyState.Patrol;
    }

    //일정거리 내(트리거)로 플레이어가 접근 시 플레이어 추적 시작
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = EnemyState.Track;
            _agent.speed = _trackSpeed;
        }
    }

    //플레이어 접촉 시 잡음 상태 시작
    void Capture()
    {
        gameManager.Captured();
    }

}
