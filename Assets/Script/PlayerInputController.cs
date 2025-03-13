
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputController : TopDownCharacterController
{

    bool _isDashing = false;
    bool _isGathering = false;
    public void OnMove(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        CallMoveEvent(direction);
    }

    public void OnLook(InputValue value)
    {
        Vector2 mousePos = value.Get<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = mousePos - (Vector2)transform.position;
        CallLookEvent(direction);
    }

    public void OnDashStart()
    {
        if (!_isDashing)
        {
            _isDashing = true;
        }
        CallDashEvent(_isDashing);
    }
    public void OnDashEnd()
    {
        if (!_isDashing)
        {
            _isDashing = false;
        }
        CallDashEvent(_isDashing);

    }

    public void OnGatherPress()
    {
        if (!_isGathering)
        {
            _isGathering = true;
        }
        CallGatherEvent(_isGathering);
    }

    public void OnGatherRelease()
    {
        if (_isGathering)
        {
            _isGathering = false;
        }
        CallGatherEvent(_isGathering);
    }
}
