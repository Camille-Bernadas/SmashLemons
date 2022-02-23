using UnityEngine;
public interface ICharacter
{
    void Move(Vector3 direction);
    void Jump();
    void Dash();
    void Attack();
    void SpecialAttack();
    void UltimateAttack();
    void Block();
    void Taunt();
}
