
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
    public TopDownCharacterController CharacterController
    {
        get { return _characterController; }
        private set { _characterController = value; }
    }
    Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;
    [SerializeField] GameObject _pointers;
    public GameObject Pointers => _pointers;
    Vector2 _direction;
    float angle = 0;
    [SerializeField] Light2D torch;
    public Light2D Torch => torch;
    [SerializeField] float startLightLength;
    [SerializeField] float _moveSpeed;


    [Header("Dash")]
    [SerializeField] float _dashMult;
    float _dashModifier = 1;
    bool _isDashing = false;
    public bool IsDashing => _isDashing;
    [SerializeField] float _dashMax = 100f;
    float _curDashMax = 100f;
    public float CurDashMax => _curDashMax;
    float _dashGage = 100f;
    public float DashGage => _dashGage;
    float _dashRecoveryTime = 5f;
    [SerializeField] float _startDashRecoveryTime = 5f;
    bool _dashGageInCoolDown = false;
    public bool DashGageInCooldown => _dashGageInCoolDown;

    [Header("Resource")]
    bool _isAtResource = false;
    bool _isGathering = false;
    float gatherGage = 0;
    [SerializeField] float _gatherTime = 3f;
    float _curGatherTime = 3f;
    [SerializeField] GameObject interactionIndicator;
    [SerializeField] Slider interactionGage;
    GameObject _gatheredResource = null;

    [Header("Game Play")]
    bool _isAtDoor = false;
    public bool IsAtDoor => _isAtDoor;
    [SerializeField] GameObject _atDoorIndicator;
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
        startLightLength = torch.pointLightOuterRadius;
    }

    void FixedUpdate()
    {
        if (gameManager.IsPlaying)
        {
            if (_isDashing && _rb.linearVelocity != Vector2.zero)
            {
                _dashGage -= Time.deltaTime * 10f;
                if (_dashGage <= 0)
                {
                    _characterController._isDashing = false;
                    Dash(false);
                }
            }
            else if (_dashGage <= _curDashMax && !_dashGageInCoolDown)
            {
                _dashGage += Time.deltaTime * _dashRecoveryTime;
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

    }

    //Movement
    void Look(Vector2 direction)
    {
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
        else if (collision.gameObject.CompareTag("Door"))
        {
            _isAtDoor = true;
            _atDoorIndicator.SetActive(true);
        } else if(collision.gameObject.CompareTag("Scrap Location")){
            _pointers.transform.GetChild(collision.gameObject.GetComponent<ScrapLocation>().locationNum).gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Resource"))
        {
            interactionIndicator.SetActive(false);
            _isAtResource = false;
        }
        else if (collision.gameObject.CompareTag("Door"))
        {
            _isAtDoor = false;
            _atDoorIndicator.SetActive(false);
        }
    }

    //Gathering Resource
    void Gather(bool isGathering)
    {
        _isGathering = isGathering;
    }

    public void UpdateStats(float dashMaxModifier, float gatherTimeModifier, float lightLengthModifier, float dashRecoveryTimeModifier)
    {
        _curDashMax = _dashMax * dashMaxModifier;
        _dashGage = _curDashMax;
        _curGatherTime = _gatherTime *= gatherTimeModifier;
        interactionGage.maxValue = _curGatherTime;
        torch.pointLightOuterRadius = startLightLength * lightLengthModifier;
        _dashRecoveryTime = _startDashRecoveryTime * dashRecoveryTimeModifier;
    }
}
