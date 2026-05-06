using System.Collections.Generic;
using UnityEngine;
using cohtml;

public sealed class MinimapTrackPresenter : MonoBehaviour
{
    [Header("GameFace")]
    [SerializeField] private CohtmlView _view;

    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Terrain")]
    [SerializeField] private float _terrainWidth = 100f;
    [SerializeField] private float _terrainHeight = 100f;

    [Header("Trail")]
    [SerializeField] private float _minDistanceBetweenPoints = 0.5f;
    [SerializeField] private int _maxPoints = 200;

    [Header("Sending")]
    [SerializeField] private float _sendInterval = 0.1f;

    private bool _isViewReady;

    private readonly List<MinimapPointDto> _trail = new();

    private Vector3 _lastWorldPoint;
    private float _nextSendTime;

    private bool _hasLastPoint;

    private void Awake()
    {
        if (_view == null)
            _view = FindObjectOfType<CohtmlView>();

        if (_view != null && _view.Listener != null)
            _view.Listener.ReadyForBindings += OnReadyForBindings;
    }
    
    private void OnDestroy()
    {
        if (_view != null && _view.Listener != null)
            _view.Listener.ReadyForBindings -= OnReadyForBindings;
    }
    
    private void OnReadyForBindings()
    {
        _isViewReady = true;
        Debug.Log("[Minimap] GameFace view ready");
    }
    
    private void Update()
    {
        if (!_isViewReady)
            return;
        
        if (_player == null || _view == null || _view.NativeView == null)
            return;

        var normalized = WorldToMinimap(_player.position);

        TryAddTrailPoint(normalized, _player.position);

        if (Time.unscaledTime < _nextSendTime)
            return;

        SendPosition(normalized);
        SendTrailPoint(normalized);

        _nextSendTime = Time.unscaledTime + _sendInterval;
    }

    private Vector2 WorldToMinimap(Vector3 worldPosition)
    {
        float normalizedX = worldPosition.x / _terrainWidth;
        float normalizedY = 1f - worldPosition.z / _terrainHeight;

        return new Vector2(
            Mathf.Clamp01(normalizedX),
            Mathf.Clamp01(normalizedY)
        );
    }

    private void TryAddTrailPoint(Vector2 normalizedPoint, Vector3 worldPoint)
    {
        if (!_hasLastPoint)
        {
            AddTrailPoint(normalizedPoint, worldPoint);
            return;
        }

        var distance = Vector3.Distance(_lastWorldPoint, worldPoint);

        if (distance < _minDistanceBetweenPoints)
            return;

        AddTrailPoint(normalizedPoint, worldPoint);
    }

    private void AddTrailPoint(Vector2 normalizedPoint, Vector3 worldPoint)
    {
        _trail.Add(new MinimapPointDto(normalizedPoint.x, normalizedPoint.y));

        while (_trail.Count > _maxPoints)
            _trail.RemoveAt(0);

        _lastWorldPoint = worldPoint;
        _hasLastPoint = true;
    }
    
    private void SendPosition(Vector2 normalized)
    {
        if (!_isViewReady || _view == null || _view.View == null)
            return;

        _view.NativeView.TriggerEvent(
            "HUD_OnMinimapPositionChanged",
            normalized.x,
            normalized.y
        );
    }
    
    private void SendTrailPoint(Vector2 normalized)
    {
        if (!_isViewReady || _view == null || _view.View == null)
            return;

        _view.NativeView.TriggerEvent(
            "HUD_OnMinimapTrailPointAdded",
            normalized.x,
            normalized.y
        );
    }
}