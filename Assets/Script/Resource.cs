using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] int itemCode, numResource;

    int _remaining;
    public int ItemCode => itemCode;
    public int Remaining
    {   get{return _remaining;}
        set
        {
            _remaining = value;
            if(_remaining == 0){
                gameObject.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        _remaining = numResource;
    }
}

