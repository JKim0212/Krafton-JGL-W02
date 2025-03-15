using UnityEngine;

public class Fade : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Disable(bool endDay){
        gameObject.SetActive(false);
        if(endDay){
            GameManager.instance.EndDay();
        }else{
            GameManager.instance.NextDay();
        }
    }
}
