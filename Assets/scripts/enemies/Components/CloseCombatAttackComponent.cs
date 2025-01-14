using UnityEngine;

public class CloseCombatAttackComponent : MonoBehaviour, IAttackComponent
{
    private SimpleAI _entity;
    private bool _isCoolingDown = false;
    private float _bodyCheckSpeed;
    private bool _bodyCheck = false;
    private bool _finnishedAttacking;
    private bool _isAttacking;
    private Vector2 _chargeDir;
    private bool _clearedForce = false; 
    private AttackPhases _curPhase = AttackPhases.Charge;
    private float _maxJumpHight; 

    private bool _doJump; 

    private enum AttackPhases
    {
        Charge,
        Attack,
        BackUp
    }
    public void Attack()
    {
        switch (_curPhase)
        {
            case AttackPhases.Charge:
                Charge();
                break;
            case AttackPhases.Attack:
                BodyCheck();
                break;
            case AttackPhases.BackUp:
                FinnishState();
                break;
            default:
                Debug.LogError("This isnt a defined Phase...");
                break; 
        }
    }

    private void FixedUpdate()
    {
        Jump();
    }

    public bool FinnishedAttack()
    {
        return _finnishedAttacking; 
    }

    public void Init(SimpleAI entity)
    {
        _entity = entity;
        _bodyCheckSpeed = _entity.Speed * 13f; 
    }

    private void Jump()
    {
        if (!_doJump) //early out if not jumping 
            return;

        if (_entity.transform.position.y < _maxJumpHight)
        {
            Debug.Log("jumping...");
            Vector2 jumpVel = Vector2.up * _entity.JumpForce - Physics2D.gravity * (Time.fixedDeltaTime * _entity.TimeScale);
            _entity.RB.velocity += jumpVel;
        }
        else
        {
            _doJump = false;
            _entity.RB.velocity = Vector2.zero;
            _curPhase = AttackPhases.Attack;
        }
    }

    private void Charge()
    {
        if (_clearedForce)
            Unfreeze();
        _isAttacking = true;
        _finnishedAttacking = false; 
        Vector2 pos = _entity.transform.position;
        _chargeDir = (_entity.PlayerPos - pos).normalized;

        _entity.RB.velocity += _chargeDir * _entity.Speed * (Time.fixedDeltaTime * _entity.TimeScale);
        bool isGrounded = IsGrounded(1.5f); 
        if ((_entity.PlayerPos - pos).sqrMagnitude <= (4 * 4) && isGrounded && !_doJump)
        {
            _doJump = true;
            _maxJumpHight = _entity.transform.position.y + _entity.JumpHight;
            _isCoolingDown = false;
            Debug.Log("Springe");
        }
        else
        {
            Debug.Log("Didnt jump! Distance^2: " + (_entity.PlayerPos - pos).sqrMagnitude  + "max Distance^2: " +  (4*4) + " is grounded: " + isGrounded + "is jumping: " + _doJump); 
        }
    } 

    private void BodyCheck()
    {
        if (!IsGrounded() && !_isCoolingDown)
        {
            _entity.RB.velocity += _chargeDir * _bodyCheckSpeed * (Time.fixedDeltaTime * _entity.TimeScale);
        }
        else
        {
            ClearForce();
            _entity.RB.gravityScale = 1; 
            _curPhase = AttackPhases.BackUp;
        }
    }

    void FinnishState()//name is wip lol
    {
        if (_clearedForce)
            Unfreeze();
        else
            BackUp(); 

    }

    private void BackUp()
    {
        Vector2 backUpDirection = ((Vector2)_entity.transform.position - _entity.PlayerPos);
        //To-DO change Addforce to MovePosition 
        _entity.RB.velocity += backUpDirection.normalized * _entity.Speed * (Time.fixedDeltaTime * _entity.TimeScale);
        float distanceSqr = backUpDirection.sqrMagnitude;
        if (distanceSqr > (4 * 4))
        {
            _curPhase = AttackPhases.Charge;
            _isCoolingDown = false;
            _finnishedAttacking = true;
            ClearForce();
        }
    }
    private bool IsGrounded(float groundDist = 1.25f)
    {
        if (Physics2D.Raycast(_entity.transform.position, -Vector2.up, groundDist, ~_entity.IgnoreLayer))
        {
            return true;
        }
        return false;
    }

    private void ClearForce()
    {
        _clearedForce = true;
        _doJump = false; 
        _entity.RB.constraints = RigidbodyConstraints2D.FreezePosition;
        _entity.RB.velocity = Vector2.zero;
    }

    private void Unfreeze()
    {
        _clearedForce = false; 
        _entity.RB.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation; 
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
        _doJump = false; 
        Unfreeze(); 
    }
}
