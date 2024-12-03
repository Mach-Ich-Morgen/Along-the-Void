using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarCombatAttackComponent : MonoBehaviour, IAttackComponent
{
    private SimpleAI _entity;
    private float _fireRangeSQR;
    private GameObject _attackEffect;
    private float _speed = 100f;
    private Rigidbody2D _rb;
    private bool _isCoolingDown = false;
    private bool _finnishedAttacking;
    public bool _isAttacking = false;

    private AttackPhases _curPhase = AttackPhases.Charge;
    private bool _clearedForce;
    private bool _switchedFromeOtherState = true; 
    private enum AttackPhases
    {
        Charge,
        Attack
    }

    public void Init(SimpleAI entity)
    {
        _entity = entity;
        _fireRangeSQR = _entity.MaxRange / 2 * (_entity.MaxRange / 2);
    }
    public void Attack()
    {
        switch (_curPhase)
        {
            case AttackPhases.Charge:
                Debug.Log("Huh...Charging"); 
                Charge();
                break;
            case AttackPhases.Attack:
                Debug.Log("Huh...Fireing");
                Shoot();
                break;
            default:
                Debug.LogError("This isnt a defined Phase...");
                break;
        }
    }

    private void Charge()
    {
        if (_switchedFromeOtherState)
        {
            _switchedFromeOtherState = false; 
            ClearForce();
        }
        if (_clearedForce)
            Unfreeze(); 
        Vector2 direction = _entity.PlayerPos - (Vector2)transform.position;
        if (direction.sqrMagnitude > _fireRangeSQR)
        {
            _entity.RB.AddForce(direction.normalized * _entity.Speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
        else if (direction.sqrMagnitude <= _fireRangeSQR)
        {
            _curPhase = AttackPhases.Attack;
            Debug.Log("switched to Attack state");
            ClearForce();
            return;

        }
        else
        {
            Debug.LogError("ich sollte hier gar nicht rein..."); 
        }
    }

    private void Shoot()
    {
        Vector2 direction = _entity.PlayerPos - (Vector2)transform.position;
        if (direction.sqrMagnitude <= _fireRangeSQR)
        {
            if (!_isCoolingDown)
            {
                _isAttacking = true;
                _isCoolingDown = true;
                _attackEffect = Instantiate(_entity.AttackVFX, _entity.transform.position, Quaternion.identity);
                _attackEffect.GetComponent<EnemyCollisionHandler>().Init(_entity.StatusEffect, _entity); 
                _rb = _attackEffect.GetComponent<Rigidbody2D>();
                if (_rb == null)
                {
                    _rb = _attackEffect.AddComponent<Rigidbody2D>();
                }
                FireSlimeBall();
                _isAttacking = false;
                StartCoroutine(CoolDown());
            }
        }
        else
        {
            _curPhase = AttackPhases.Charge; 
        }
    }
    private void FireSlimeBall()
    {
        Vector2 targetPos = _entity.PlayerPos;
        Vector2 force = (targetPos - (Vector2)_entity.transform.position + CalculateAimOffset(targetPos - (Vector2)_entity.transform.position)).normalized * _speed;
        Debug.Log(force); 
        _rb.AddForce(force, ForceMode2D.Impulse); 

    }

    private Vector2 CalculateAimOffset(Vector2 linearDir)
    {
        Vector2 gravity = Physics2D.gravity * _rb.gravityScale;
        float estimateFlyDuration = linearDir.magnitude / _speed; 
        return ((gravity*-1) * estimateFlyDuration) * Time.fixedDeltaTime; 
    }

    private void ClearForce()
    {
        Debug.LogWarning("Cleare force");  
        _clearedForce = true;
        _entity.RB.constraints = RigidbodyConstraints2D.FreezePosition;
        _entity.RB.velocity = Vector2.zero;
    }

    private void Unfreeze()
    {
        _clearedForce = false;
        _entity.RB.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(1.5f);
        _isCoolingDown = false; 
    }

    public bool FinnishedAttack()
    {
        return _finnishedAttacking; 
    }

    public void ResetAttackStatus()
    {
        _finnishedAttacking = false; 
    }

    public bool IsAttacking()
    {
        return _isAttacking; 
    }

    public void Exit()
    {
        Unfreeze();
        Debug.LogWarning("Exit mexit"); 
        _isCoolingDown = false;
        _isAttacking = false;
        _switchedFromeOtherState = true;

    }
}

