using System;
using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{

    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2> OnLookEvent;
    public event Action<bool> OnDashEvent;
    public event Action<bool> OnGatherEvent;
    public void CallMoveEvent(Vector2 direction)
    {
        OnMoveEvent?.Invoke(direction);
    }

    public void CallLookEvent(Vector2 direction)
    {
        OnLookEvent?.Invoke(direction);
    }

    public void CallDashEvent(bool isDashing){
        OnDashEvent?.Invoke(isDashing);
    }

    public void CallGatherEvent(bool isGathering){
        OnGatherEvent?.Invoke(isGathering);
    }
}
