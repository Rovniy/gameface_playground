using System.Collections;
using UnityEngine;
using cohtml;

public sealed class HudBindingDemoController : MonoBehaviour
{
    [Header("GameFace")]
    [SerializeField] private CohtmlView view;

    [Header("Counter")]
    [SerializeField] private int initialCounter = 1;
    [SerializeField] private int minDelaySeconds = 1;
    [SerializeField] private int maxDelaySeconds = 5;
    [SerializeField] private int minIncrement = 10;
    [SerializeField] private int maxIncrement = 999;

    [Header("Event Log")]
    [SerializeField] private int maxEvents = 30;

    private readonly HudBindingDemoModel _model = new();

    private bool _isModelReady;
    private int _eventId;

    private static readonly string[] UserNames =
    {
        "User 1",
        "User 2",
        "User 3",
        "User 4"
    };

    private void Awake()
    {
        if (view == null)
            view = GetComponent<CohtmlView>();

        _model.Counter = initialCounter;

        if (view != null && view.Listener != null)
            view.Listener.ReadyForBindings += OnReadyForBindings;
    }

    private void OnDestroy()
    {
        if (view != null && view.Listener != null)
            view.Listener.ReadyForBindings -= OnReadyForBindings;
    }

    private void OnReadyForBindings()
    {
        var createdModel = view.NativeView.CreateModel("HudDemo", _model);

        if (createdModel == null)
        {
            Debug.LogError("[HudBindingDemo] Failed to create GameFace data model.");
            return;
        }

        _isModelReady = true;

        PushModelToUi();

        StartCoroutine(SimulateCounter());
        StartCoroutine(SimulateEvents());

        Debug.Log("[HudBindingDemo] Data model registered.");
    }

    private IEnumerator SimulateCounter()
    {
        while (_isModelReady)
        {
            var delay = Random.Range(minDelaySeconds, maxDelaySeconds + 1);
            yield return new WaitForSeconds(delay);

            var increment = Random.Range(minIncrement, maxIncrement + 1);
            _model.Counter += increment;

            AddEvent($"System increased score by {increment}", increment);

            PushModelToUi();
        }
    }

    private IEnumerator SimulateEvents()
    {
        while (_isModelReady)
        {
            var delay = Random.Range(1f, 3f);
            yield return new WaitForSeconds(delay);
    
            var attacker = UserNames[Random.Range(0, UserNames.Length)];
            var victim = UserNames[Random.Range(0, UserNames.Length)];
    
            if (attacker == victim)
                victim = "User Boss";
    
            var damage = Random.Range(10, 100);
    
            AddEvent($"{attacker} dealt {damage} damage to {victim}", damage);
    
            PushModelToUi();
        }
    }

    private void AddEvent(string text, int damage)
    {
        _eventId++;
    
        Debug.Log($"[HudBindingDemo] Event added: {text}");
        
        _model.Events.Add(new HudEventLogItem(
            id: _eventId,
            text: text,
            damage: damage,
            time: System.DateTime.Now.ToString("HH:mm:ss")
        ));
    
        while (_model.Events.Count > maxEvents)
            _model.Events.RemoveAt(0);
    }

    private void PushModelToUi()
    {
        if (!_isModelReady || view == null || view.NativeView == null)
            return;

        view.NativeView.UpdateWholeModel(_model);
        view.NativeView.SynchronizeModels();
    }
}