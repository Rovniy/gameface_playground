using System;
using UnityEngine;
using cohtml;

public sealed class HudInputReceiver : MonoBehaviour
{
    [SerializeField] private CohtmlView view;
    [SerializeField] private PlayerInputController playerInput;

    private void Awake()
    {
        if (view == null)
            view = GetComponent<CohtmlView>();

        view.Listener.ReadyForBindings += RegisterBindings;
    }

    private void OnDestroy()
    {
        if (view != null && view.Listener != null)
            view.Listener.ReadyForBindings -= RegisterBindings;
    }

    private void RegisterBindings()
    {
        Debug.Log("[HUD] ReadyForBindings");

        view.NativeView.RegisterForEvent(
            HudEventNames.OnButtonClick,
            (Action<string>)OnButtonClick
        );

        view.NativeView.RegisterForEvent(
            HudEventNames.OnMoveStart,
            (Action<string>)OnMoveStart
        );

        view.NativeView.RegisterForEvent(
            HudEventNames.OnMoveStop,
            (Action)OnMoveStop
        );

        Debug.Log("[HUD] Bindings registered");
    }

    private void OnButtonClick(string action)
    {
        Debug.Log($"[HUD] Button click: {action}");

        switch (action)
        {
            case "attack":
                playerInput.AttackFromUi();
                break;

            case "cancel":
                Debug.Log("[HUD] Cancel");
                break;

            default:
                Debug.LogWarning($"[HUD] Unknown action: {action}");
                break;
        }
    }

    private void OnMoveStart(string direction)
    {
        Debug.Log($"[HUD] Move start: {direction}");

        playerInput.SetUiMoveInput(DirectionToVector(direction));
    }

    private void OnMoveStop()
    {
        Debug.Log("[HUD] Move stop");

        playerInput.StopUiMoveInput();
    }

    private static Vector2 DirectionToVector(string direction)
    {
        return direction switch
        {
            "up" => Vector2.up,
            "down" => Vector2.down,
            "left" => Vector2.left,
            "right" => Vector2.right,

            "up-left" => new Vector2(-1f, 1f).normalized,
            "up-right" => new Vector2(1f, 1f).normalized,
            "down-left" => new Vector2(-1f, -1f).normalized,
            "down-right" => new Vector2(1f, -1f).normalized,

            _ => Vector2.zero
        };
    }
}