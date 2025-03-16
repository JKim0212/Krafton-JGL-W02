using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _playUI, _hideoutUI, _torchUpgrade, _charcterUpgrade, _transmuter;
    public GameObject HideoutUI => _hideoutUI;
    [SerializeField] Slider _dash;
    [SerializeField] TextMeshProUGUI _timer, _scrapText, _batteryText, _moneyText, _dayText;
    [SerializeField] TextMeshProUGUI _hideoutScrapText, _hideoutBatteryText, _hideoutMoneyText, _hideoutDayText, _endingText;
    [SerializeField] GameObject _fade, _endingScreen, _startGameScreen;
    [SerializeField] GameObject _tutorial, _backgroundText, _backgroundText1, _backgroundText2, _backgroundText3, _control, _scrapImage, _startButton;
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
        if(time <= 0f){
            _timer.text = "0:00";    
        } else {
            _timer.text = min.ToString() + ":" + sec.ToString("D2");
        }
        
    }

    public void UpdateUpgradeText(int scrapNum, int batteryNum, int moneyNum, int day)
    {
        _hideoutScrapText.text = $"X {scrapNum}";
        _hideoutBatteryText.text = $"X {batteryNum}";
        _hideoutMoneyText.text = $"X {moneyNum}";
        _hideoutDayText.text = $"Day {day}";
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

    public IEnumerator EndDay()
    {
        _playUI.SetActive(false);
        _fade.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        _fade.SetActive(false);
        _hideoutUI.SetActive(true);
    }
    public IEnumerator NextDay()
    {
        _hideoutUI.SetActive(false);
        _torchUpgrade.SetActive(false);
        _charcterUpgrade.SetActive(false);
        _fade.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        _fade.SetActive(false);
        _transmuter.SetActive(false);
        _playUI.SetActive(true);
    }

    public void Upgrade(int upgradeCode)
    {
        GameManager.instance.Upgrade(upgradeCode);
    }

    public void StartGame()
    {
        _startGameScreen.SetActive(false);
        StartCoroutine(Tutorial());
    }
    public void Ending(int day)
    {
        _endingText.text = $"총 {day} 일이 걸렸습니다.";
        _endingScreen.SetActive(true);
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator Tutorial()
    {
        _tutorial.SetActive(true);
        _backgroundText.SetActive(true);
        yield return new WaitForSeconds(2f);
        _backgroundText.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _backgroundText1.SetActive(true);
        yield return new WaitForSeconds(2f);
        _backgroundText1.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _backgroundText2.SetActive(true);
        yield return new WaitForSeconds(2f);
        _backgroundText2.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _backgroundText3.SetActive(true);
        _control.SetActive(true);
        _scrapImage.SetActive(true);
        _startButton.SetActive(true);
    }

    public void EndTutorial(){
        _tutorial.SetActive(false);
        GameManager.instance.NextDay();
    }
}
