using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Transform player;
    NavMeshAgent agent;
    GameManager gameManager;

    [Header("Movement")]
    float _patrolSpeed = 5;
    float _trackSpeed = 10;
    Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;
    [SerializeField] Transform[] patrolPosition;
    [HideInInspector] public int patrolNumber = 0;
    public bool _detectedPlayer = false;

    public enum EnemyState { Patrol, Track }
    EnemyState currentState = EnemyState.Patrol;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;
        _rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.instance;
    }

    void Start()
    {
        agent.speed = _patrolSpeed;
    }

    void FixedUpdate()
    {
        if (gameManager.IsPlaying)
        {
            _rb.position = agent.nextPosition;
            switch (currentState)
            {
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Track:
                    Track();
                    break;
            }
        }
    }

    void Patrol()
    {

        Vector3 walkPoint = patrolPosition[patrolNumber].position;
        agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // walkPoint 도달 시
        if (distanceToWalkPoint.magnitude < 1f)
        {
            // 다음 포인트로 이동
            patrolNumber++;
            if (patrolNumber >= patrolPosition.Length)
            {
                patrolNumber = 0; // 마지막 포인트에 도달하면 처음으로 돌아감
            }
        }
    }

    void Track()
    {
        agent.SetDestination(player.position);
        Invoke(nameof(Search), 5f);
    }
    void Search()
    {
        if (Vector3.Distance(transform.position, player.position) < 1f)
        {
            Invoke(nameof(Capture), 3f);
            return;
        }
        ReturnToPatrol();
    }
    void ReturnToPatrol()
    {
        currentState = EnemyState.Patrol;
    }
    // public IEnumerator Patrol()
    // {


    //     while (!_detectedPlayer && gameManager.IsNight && gameManager.IsPlaying)
    //     {

    //         agent.SetDestination(patrolPosition[patrolNumber].position);


    //         if (Vector3.Distance(transform.position, patrolPosition[patrolNumber].position) <= 1f)
    //         {
    //             patrolNumber += 1;
    //             if (patrolNumber >= patrolPosition.Length)
    //             {
    //                 patrolNumber = 0;
    //             }
    //         }
    //         yield return new WaitForSeconds(1f);
    //     }
    //     agent.speed = _trackSpeed;
    //     StartCoroutine(Track());
    // }

    // IEnumerator Track()
    // {

    //     while (_detectedPlayer || gameManager.IsNight)
    //     {
    //         agent.SetDestination(player.position);

    //         yield return null; // Prevent freezing by allowing the loop to yield execution
    //         StartCoroutine(CheckDetection());
    //         if (Vector2.Distance(player.position, transform.position) <= 0.1f)
    //         {
    //             StartCoroutine(Capture());
    //         }
    //     }

    //     StartCoroutine(Patrol());
    // }

    // IEnumerator CheckDetection()
    // {
    //     yield return new WaitForSeconds(3f);
    //     if (Vector3.Distance(transform.position, player.position) > 10f)
    //     {
    //         _detectedPlayer = false;
    //     }
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = EnemyState.Track;
        }
    }
    void Capture()
    {
        currentState = EnemyState.Patrol;
        gameManager.Captured();
    }

}
