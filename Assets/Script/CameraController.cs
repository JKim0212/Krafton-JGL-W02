using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] Transform player;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.position + offset;
    }
}
