using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusEffect : MonoBehaviour
{

    public enum EnemyStatus
    {
        None,
        Frozen,
        Burning
    }

    [SerializeField]
    private SimpleAI _entity;
    private EnemyStatus _status;
    private EnemyHealth _health; 
    public EnemyStatus Status { get { return _status; } }

    private void Start()
    {
        _health = GetComponent<EnemyHealth>(); 
    }
    private void FixedUpdate()
    {
        if (_status == EnemyStatus.Burning)
            _health.GetDamage((3.33f*Time.fixedDeltaTime) * _entity.TimeScale); 

    }

    public void FreezeEnemy()
    {
        Debug.Log("Ice Ice baby"); 
        _status = EnemyStatus.Frozen;
    }

    public void BurnEnemy()
    {
        _status = EnemyStatus.Burning;
    }

    public void ResetEnemyStatus()
    {
        _status = EnemyStatus.None; 
    }
}
