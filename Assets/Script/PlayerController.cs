
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    public Action<GameObject> Gathered;

    [Header("Basic Movement")]
    TopDownCharacterController _characterController;
    Rigidbody2D _rb;
    Vector2 _direction;
    [SerializeField] Light2D torch;
    [SerializeField] float _moveSpeed;


    [Header("Dash")]
    [SerializeField] float _dashMult;
    float _dashModifier = 1;
    bool _isDashing = false;
    [SerializeField] float _dashMax = 100f;
    float _curDashMax = 100f;
    float _dashGage = 100f;
    bool _dashGageInCoolDown = false;

    [Header("Resource")]
    bool _isAtResource = false;
    bool _isGathering = false;
    float gatherGage = 0;
    [SerializeField] float _gatherTime = 3f;
    float _curGatherTime = 3f;
    [SerializeField] GameObject interactionIndicator;
    [SerializeField] Slider interactionGage;
    GameObject _gatheredResource = null;

    void Awake()
    {
        gameManager = GameManager.instance;
        _rb = GetComponent<Rigidbody2D>();
        _characterController = GetComponent<TopDownCharacterController>();
    }

    void Start()
    {
        _characterController.OnMoveEvent += Move;
        _characterController.OnLookEvent += Look;
        _characterController.OnDashEvent += Dash;
        _characterController.OnGatherEvent += Gather;

        interactionGage.maxValue = _curGatherTime;
        _dashGage = _curDashMax;
    }

    void FixedUpdate()
    {
        if (_isDashing && _rb.linearVelocity != Vector2.zero)
        {
            _dashGage -= Time.deltaTime * 10f;
        }
        else if (_dashGage <= _curDashMax && !_dashGageInCoolDown)
        {
            _dashGage += Time.deltaTime * 5f;
        }
        if (_isAtResource && _isGathering)
        {
            gatherGage += Time.deltaTime;
            interactionGage.value = gatherGage;
            if (gatherGage >= _curGatherTime)
            {
                Gathered?.Invoke(_gatheredResource);
                gatherGage = 0;
                interactionGage.value = gatherGage;
            }
        }
        else
        {
            gatherGage = 0f;
            interactionGage.value = gatherGage;
        }
        ApplyMovement();
    }

    //Movement
    void Look(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Move(Vector2 direction)
    {
        _direction = direction;
    }

    void Dash(bool isDashing)
    {
        if (isDashing && _dashGage > 0)
        {
            _dashModifier = _dashMult;
            _isDashing = true;
        }
        else
        {
            _dashModifier = 1;
            _isDashing = false;
            _dashGageInCoolDown = true;
            StartCoroutine(DashCoolDown());
        }
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(1f);
        _dashGageInCoolDown = false;
    }

    void ApplyMovement()
    {
        _rb.linearVelocity = _direction * _moveSpeed * _dashModifier;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Resource"))
        {
            _gatheredResource = collision.gameObject; ;
            interactionIndicator.transform.position = collision.transform.position + Vector3.up * 1;
            interactionIndicator.SetActive(true);
            _isAtResource = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Resource"))
        {
            interactionIndicator.SetActive(false);
            _isAtResource = false;
        }
    }

    //Gathering Resource
    void Gather(bool isGathering)
    {
        _isGathering = isGathering;

    }

    public void UpdateStats(float dashMaxModifier, float gatherTimeModifier)
    {
        _curDashMax = _dashMax * dashMaxModifier;
        _curGatherTime = _gatherTime *= gatherTimeModifier;
        interactionGage.maxValue = _curGatherTime;

    }
}
