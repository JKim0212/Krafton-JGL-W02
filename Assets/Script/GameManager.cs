using System;
using UnityEditor.Rendering;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerController player;
    public PlayerController Player => player;

    float dashMaxModifier = 1;

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
    }


    public void AddResource(GameObject resource)
    {
        int resourceCode = resource.GetComponent<Resource>().ItemCode;
        switch(resourceCode){
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

}
