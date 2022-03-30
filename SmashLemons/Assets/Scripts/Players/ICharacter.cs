using UnityEngine;
using UnityEngine.InputSystem;
public interface ICharacter
{
    void Move(Vector3 direction);
    void Jump();
    void Dash();
    void Attack(Vector2 direction);
    void SpecialAttack();
    void UltimateAttack();
    void Block(InputAction.CallbackContext context);
    void Taunt();
}
