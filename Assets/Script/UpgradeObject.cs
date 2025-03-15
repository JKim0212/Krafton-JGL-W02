using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject upgradeUI;
    public void OnPointerClick(PointerEventData eventData)
    {
        upgradeUI.SetActive(true);
    }

    public void Exit()
    {
        upgradeUI.SetActive(false);
    }


}
