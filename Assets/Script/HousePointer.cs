using UnityEngine;

public class HousePointer : MonoBehaviour
{

    [SerializeField] Transform _pointPos;

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Atan2(_pointPos.position.y - transform.position.y, _pointPos.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
