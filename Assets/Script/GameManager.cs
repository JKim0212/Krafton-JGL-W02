using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
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
    [SerializeField] GameObject _slash;
    bool _isPlaying = false;
    public bool IsPlaying => _isPlaying;
    [SerializeField] GameObject _endingFade, _endingScene;
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
    int tempScrap, tempBattery, tempMoney = 0;

    [SerializeField] CinemachineCamera _vcam;
    float _initialSize = 10;
    float _modifiedSize = 10;

    Coroutine footprinting;
  

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player.Gathered += AddResource;
        player.CharacterController.OnEndDayEvent += EndDay;
        _vcam.Lens.OrthographicSize = _initialSize;
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
        _vcam.Lens.OrthographicSize = _initialSize;
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
        enemy.Danger(false);
        if(footprinting != null){
            enemy.StopCoroutine(footprinting);
        }
        player.Torch.gameObject.SetActive(false);
        player.Pointers.SetActive(false);
        player.transform.position = _hideoutLocation.position;
        player.transform.rotation = Quaternion.Euler(0, 0, -180);
        enemy.Agent.enabled = false;
        enemy.transform.position = enemyStartPosition.position;
        _ui.UpdateUpgradeText(scrapNum, batteryNum, moneyNum, day);
        StartCoroutine(_ui.EndDay());

    }

    public IEnumerator Captured()
    {
        tempScrap = 0;
        tempBattery = 0;
        tempMoney = 0;
        _slash.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _slash.SetActive(false);
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

        if (scrapNum >= 50)
        {
            _vcam.Lens.OrthographicSize = _initialSize;
            StartCoroutine(Ending());
            return;
        }
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
        _vcam.Lens.OrthographicSize = _modifiedSize;
        StartCoroutine(_ui.NextDay());
        footprinting = enemy.StartCoroutine(enemy.Footprinting());
        _isPlaying = true;
    }

    IEnumerator Ending()
    {
        _ui.HideoutUI.SetActive(false);
        _endingFade.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(318, 0, 0);
        player.gameObject.SetActive(false);
        _endingScene.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        _endingScene.SetActive(false);
        _endingFade.SetActive(false);
        _ui.Ending(day);

    }

    public void Upgrade(int upgradeCode)
    {
        switch (upgradeCode)
        {
            case 0:
                if (batteryNum >= 1)
                {
                    batteryNum -= 1;
                    _lightLengthModifier *= 1.2f;
                    _modifiedSize *= 1.1f;
                    if (_modifiedSize > 15)
                    {
                        _modifiedSize = 15;
                    }
                }
                break;
            case 1:
                if (moneyNum >= 1)
                {
                    moneyNum -= 1;
                    _dashMaxModifier *= 1.2f;
                }
                break;
            case 2:
                if (moneyNum >= 1)
                {
                    moneyNum -= 1;
                    _dashRecoveryTimeModifier *= 1.5f;
                }
                break;
            case 3:
                if (moneyNum >= 1)
                {
                    moneyNum -= 1;
                    _gatherTimeModifier *= 0.9f;
                }
                break;
            case 4:
                if (scrapNum >= 2)
                {
                    scrapNum -= 2;
                    batteryNum += 1;
                }
                break;
            case 5:
                if (scrapNum >= 2)
                {
                    scrapNum -= 2;
                    moneyNum += 1;
                }
                break;
        }
        _ui.UpdateUpgradeText(scrapNum, batteryNum, moneyNum, day);
    }
}
