using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _playUI, _hideoutUI;
    [SerializeField] Slider _dash;
    [SerializeField] TextMeshProUGUI _timer, _scrapText, _batteryText, _moneyText, _dayText;

    public void UpdateUI(float _dashMax, float time, int scrapNum, int batteryNum, int moneyNum, int day)
    {
        _dash.maxValue = _dashMax;
        _dash.value = _dashMax;
        int min = (int)(time / 60f);
        int sec = (int)Mathf.Ceil(time % 60f) - 1;
        _timer.text = min.ToString() + ":" + sec.ToString("D2");
        _dayText.text = $"Day {day}";
        AddScrap(scrapNum);
        AddBattery(batteryNum);
        AddMoney(moneyNum);
    }

    public void UpdateDashSliderandTimer(float dashGage, float time)
    {
        _dash.value = dashGage;
        int min = (int)(time / 60f);
        int sec = (int)Mathf.Ceil(time % 60f) - 1;
        _timer.text = min.ToString() + ":" + sec.ToString("D2");
    }

    public void AddScrap(int scrapNum)
    {
        _scrapText.text = $"X {scrapNum}";
    }

    public void AddBattery(int batteryNum)
    {
        _batteryText.text = $"X {batteryNum}";
    }
    public void AddMoney(int moneyNum)
    {
        _moneyText.text = $"X {moneyNum}";
    }

    public void EndDay(){
        _playUI.SetActive(false);
        _hideoutUI.SetActive(true);
    }
    public void NextDay(){
        _hideoutUI.SetActive(false);
        _playUI.SetActive(true);
    }


}
