using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerController player;
    public PlayerController Player => player;
    [SerializeField] EnemyController enemy;
    public EnemyController Enemy => enemy;
    [SerializeField] Transform _resourcePositions;
    [SerializeField] Transform _scraps;
    [SerializeField] UIManager _ui;
    bool _isPlaying = true;
    public bool IsPlaying => _isPlaying;

    [Header("Player Stats")]
    float _lightLengthModifier = 1;
    float _dashMaxModifier = 1;
    float _gatherTimeModifier = 1;

    float _dashRecoveryTimeModifier = 1;
    [Header("Game Play")]
    [SerializeField] Transform playerStartPosition;
    [SerializeField] Transform enemyStartPosition;
    int day = 0;
    [SerializeField] Transform _hideoutLocation;
    [SerializeField] GameObject _hideout;

    bool _isNight = false;
    float time = 10f;
    public bool IsNight => _isNight;
    [Header("Resource")]
    [SerializeField] int scrapNum, batteryNum, moneyNum = 0;
    private int tempScrap, tempBattery, tempMoney = 0;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player.Gathered += AddResource;
        player.CharacterController.OnEndDayEvent += EndDay;
        NextDay();
    }

    void Update()
    {
        if (_isPlaying)
        {
            if (!IsNight)
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    _isNight = true;
                    time = 0;
                }
            }
            _ui.UpdateDashSliderandTimer(player.DashGage, time);
        }


    }
    public void AddResource(GameObject resource)
    {
        int resourceCode = resource.GetComponent<Resource>().ItemCode;
        switch (resourceCode)
        {
            case 0:
                tempScrap += 1;
                _ui.AddScrap(tempScrap);
                break;
            case 1:
                tempBattery += 1;
                _ui.AddBattery(tempBattery);
                break;
            case 2:
                tempMoney += 1;
                _ui.AddMoney(tempMoney);
                break;
        }
        resource.GetComponent<Resource>().Remaining -= 1;
    }

    public void EndDay()
    {
        _isPlaying = false;
        DeSpawnResources();
        player.Rb.linearVelocity = Vector2.zero;
        scrapNum += tempScrap;
        batteryNum += tempBattery;
        moneyNum += tempMoney;
        tempScrap = 0;
        tempBattery = 0;
        tempMoney = 0;
        _hideout.SetActive(true);
        player.Torch.gameObject.SetActive(false);
        player.Pointers.SetActive(false);
        player.transform.position = _hideoutLocation.position;
        player.transform.rotation = Quaternion.Euler(0, 0, -180);
        enemy.Agent.enabled = false;
        enemy.transform.position = enemyStartPosition.position;
        _ui.EndDay();

    }

    public void Captured()
    {
        tempScrap = 0;
        tempBattery = 0;
        tempMoney = 0;
        EndDay();
    }

    void DeSpawnResources()
    {
        for (int i = 0; i < _resourcePositions.childCount; i++)
        {
            _resourcePositions.GetChild(i).gameObject.SetActive(false);
        }

    }
    void SpawnResources()
    {
        for (int i = 0; i < _resourcePositions.childCount; i++)
        {
            float spawnProb = Random.Range(0, 100);
            if (spawnProb >= 50)
            {
                _resourcePositions.GetChild(i).gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < _scraps.childCount; i++)
        {
            _scraps.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void NextDay()
    {
        day += 1;
        _isNight = false;
        SpawnResources();
        player.UpdateStats(_dashMaxModifier, _gatherTimeModifier, _lightLengthModifier, _dashRecoveryTimeModifier);
        player.Rb.position = playerStartPosition.position;
        player.Torch.gameObject.SetActive(true);
        player.Pointers.SetActive(true);
        time = 60f;
        enemy.patrolNumber = 0;
        enemy.Agent.enabled = true;
        enemy.ReturnToPatrol();
        _ui.UpdateUI(player.CurDashMax, time, tempScrap, tempBattery, tempMoney, day);
        _ui.NextDay();
        _isPlaying = true;

    }
}
