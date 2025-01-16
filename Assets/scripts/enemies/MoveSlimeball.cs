using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlimeball : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 25f; 
    private Vector2 _startVel;
    private float _startDistance;
    private SimpleAI _entity;
    private Rigidbody2D _rb;
    private bool _isFired = false;
    

    // Update is called once per frame
    private void Update()
    {
        if (!_isFired)
            return;
        Fly();
        ClampVelocity();
    }

    private void Fly()
    {
        _rb.gravityScale = _entity.TimeScale; 
        float currentDistance = (_entity.PlayerPos - _rb.position).magnitude;

        Vector2 velocity = (_startVel * (currentDistance / _startDistance)) * (Time.fixedDeltaTime * _entity.TimeScale);
        _rb.velocity += velocity; 
    }


    private Vector2 ClampVelocity()
    {
        if (_rb.velocity.magnitude > _maxSpeed)
        {
            return _rb.velocity.normalized * _maxSpeed; 
        }
        return _rb.velocity; 
    }

    public void Instantiate(Vector2 startVel, float startDistance, SimpleAI entity, Rigidbody2D rb)
    {
        _startVel = startVel;
        _startDistance = startDistance;
        _entity = entity;
        _rb = rb;
        _isFired = true; 
    }

    public void Deactivate()
    {
        _isFired = false; 
    }
}
