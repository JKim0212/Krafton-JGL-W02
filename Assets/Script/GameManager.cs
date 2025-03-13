using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerController player;
    public PlayerController Player => player;
    [SerializeField] EnemyController enemy;
    public EnemyController Enemy => enemy;
    bool _isPlaying = true;
    public bool IsPlaying => _isPlaying;
    [SerializeField] GameObject[] _scraps;
    Dictionary<string, Transform> scrapsFound;
    [Header("Player Stats")]
    float lightLengthModifier = 1;
    float dashMaxModifier = 1;
    float gatherTimeModifier = 1;
    [Header("Game Play")]
    [SerializeField] Transform playerStartPosition;
    [SerializeField] Transform enemyStartPosition;
    int day = 0;
    [SerializeField] Transform _hideoutLocation;
    [SerializeField] GameObject _hideout;
    [SerializeField] Canvas _hideoutUI;
    bool _isNight = false;
    float time = 60f;
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
        if (!IsNight)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                _isNight = true;
            }
        }

    }
    public void AddResource(GameObject resource)
    {
        int resourceCode = resource.GetComponent<Resource>().ItemCode;
        switch (resourceCode)
        {
            case 0:
                tempScrap += 1;
                break;
            case 1:
                tempBattery += 1;
                break;
            case 2:
                tempMoney += 1;
                break;
        }
        resource.GetComponent<Resource>().NumResource -= 1;
    }

    public void EndDay()
    {
        _isPlaying = false;
        scrapNum += tempScrap;
        batteryNum += tempBattery;
        moneyNum += tempMoney;
        tempScrap = 0;
        tempBattery = 0;
        tempMoney = 0;
        _hideout.SetActive(true);
        player.Torch.gameObject.SetActive(false);
        player.transform.position = _hideoutLocation.position;
        _hideoutUI.gameObject.SetActive(true);

    }

    public void Captured()
    {
        tempScrap = 0;
        tempBattery = 0;
        tempMoney = 0;
        EndDay();
    }

    public void NextDay()
    {
        day += 1;
        _hideoutUI.gameObject.SetActive(false);
        player.UpdateStats(dashMaxModifier, gatherTimeModifier, lightLengthModifier);
        player.Rb.position = playerStartPosition.position;
        player.Torch.gameObject.SetActive(true);
        enemy.Rb.position = enemyStartPosition.position;
        enemy.patrolNumber = 0;
        _isPlaying = true;
    }
}
