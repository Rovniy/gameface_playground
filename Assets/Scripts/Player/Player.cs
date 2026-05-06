using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -20f;

    [Header("Attack")]
    [SerializeField] private float _attackCooldown = 0.5f;

    private CharacterController _characterController;

    private Vector3 _velocity;
    private float _nextAttackTime;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector2 inputDirection)
    {
        var direction = new Vector3(inputDirection.x, 0f, inputDirection.y);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        var movement = direction * _moveSpeed;

        if (_characterController.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += _gravity * Time.deltaTime;

        var finalMove = movement + new Vector3(0f, _velocity.y, 0f);

        _characterController.Move(finalMove * Time.deltaTime);
    }

    public void Attack()
    {
        if (Time.time < _nextAttackTime)
            return;

        _nextAttackTime = Time.time + _attackCooldown;

        Debug.Log("[Player] Attack!");

        // Тут позже можно вызвать:
        // - анимацию атаки
        // - спавн projectile
        // - hitbox
        // - звук
        // - VFX
    }
}