using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Transform player;
    NavMeshAgent agent;

    [Header("Movement")]
    float _patrolSpeed = 5;
    float _trackSpeed = 10;
    Rigidbody2D _rb;
    [SerializeField] Transform[] patrolPosition;
    int patrolNumber = 0;
    public bool _detectedPlayer = false;
    public bool _isNight = false;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Patrol());
    }

    void FixedUpdate()
    {
        _rb.position = agent.nextPosition;
    }
    IEnumerator Patrol()
    {
        agent.speed = _patrolSpeed;

        while (!_detectedPlayer && !_isNight)
        {
            agent.SetDestination(patrolPosition[patrolNumber].position);


            if (Vector3.Distance(transform.position, patrolPosition[patrolNumber].position) <= 1f)
            {
                patrolNumber += 1;
                if (patrolNumber >= patrolPosition.Length)
                {
                    patrolNumber = 0;
                }
            }
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Track());
    }

    IEnumerator Track()
    {
        agent.speed = _trackSpeed;
        while (_detectedPlayer || _isNight)
        {
            agent.SetDestination(player.position);

            yield return null; // Prevent freezing by allowing the loop to yield execution
            StartCoroutine(CheckDetection());
        }

        StartCoroutine(Patrol());
    }

    IEnumerator CheckDetection()
    {
        yield return new WaitForSeconds(3f);
        if (Vector3.Distance(transform.position, player.position) > 5f)
        {
            _detectedPlayer = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _detectedPlayer = true;
        }
    }


}
