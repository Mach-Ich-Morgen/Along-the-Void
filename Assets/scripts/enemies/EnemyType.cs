using UnityEngine;

public abstract class EnemyType : MonoBehaviour
{
    public abstract void Movement(Vector3 targetPos);

    public abstract void Hount(); 
    public abstract void Attack();
}
