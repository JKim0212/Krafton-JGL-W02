using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] int itemCode, numResource;
    public int ItemCode => itemCode;
    public int NumResource
    {   get{return numResource;}
        set
        {
            numResource = value;
            if(numResource == 0){
                gameObject.SetActive(false);
            }
        }
    }
}

