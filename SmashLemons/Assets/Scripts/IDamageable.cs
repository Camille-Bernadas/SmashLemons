using UnityEngine;
public interface IDamageable
{
    public float Heal(float amount);
    public float TakeDamage(Vector3 origin, float amount);

}
