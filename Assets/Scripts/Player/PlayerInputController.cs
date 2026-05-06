using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public sealed class PlayerInputController : MonoBehaviour
{
    private Player _player;

    private Vector2 _keyboardMoveInput;
    private Vector2 _uiMoveInput;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        ReadKeyboardInput();

        var finalMoveInput = GetFinalMoveInput();
        _player.Move(finalMoveInput);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _player.Attack();
        }
    }

    private void ReadKeyboardInput()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null)
        {
            _keyboardMoveInput = Vector2.zero;
            return;
        }

        var x = 0f;
        var y = 0f;

        if (keyboard.aKey.isPressed)
            x -= 1f;

        if (keyboard.dKey.isPressed)
            x += 1f;

        if (keyboard.wKey.isPressed)
            y += 1f;

        if (keyboard.sKey.isPressed)
            y -= 1f;

        _keyboardMoveInput = new Vector2(x, y);

        if (_keyboardMoveInput.sqrMagnitude > 1f)
            _keyboardMoveInput.Normalize();
    }

    private Vector2 GetFinalMoveInput()
    {
        if (_uiMoveInput.sqrMagnitude > 0.01f)
            return _uiMoveInput;

        return _keyboardMoveInput;
    }

    public void SetUiMoveInput(Vector2 direction)
    {
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        _uiMoveInput = direction;
    }

    public void StopUiMoveInput()
    {
        _uiMoveInput = Vector2.zero;
    }

    public void AttackFromUi()
    {
        _player.Attack();
    }
}