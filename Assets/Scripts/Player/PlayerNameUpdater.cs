using System.Collections;
using cohtml;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNameUpdater : MonoBehaviour
{
    [Header("GameFace")]
    [SerializeField] private CohtmlView view;

    private readonly HudUsername _model = new();

    private bool _isModelReady;

    private static readonly string[] UserNames =
    {
        "ShadowRift",
        "NovaByte",
        "IronMantis",
        "PixelWarden",
        "GhostLynx",
        "RogueCircuit",
        "FrostViper",
        "EmberWolf",
        "VoidRunner",
        "StormKite",
        "CyberMoth",
        "NightHarbor",
        "AshenFox",
        "CrimsonPulse",
        "VortexSaint",
        "BladeOtter",
        "SolarWraith",
        "HexFalcon",
        "MoonDrifter",
        "ToxicPanda",
        "ArcaneToast",
        "NeonGoblin",
        "SilentRook",
        "OmegaSprout",
        "ThunderMoss",
        "RustyComet",
        "VelvetDagger",
        "GlitchBunny",
        "DarkPebble",
        "RapidKoala",
        "QuantumFrog",
        "MistyRaven",
        "TurboCactus",
        "CrystalVandal",
        "EchoBadger",
        "PlasmaDuck",
        "FrozenNoodle",
        "WildCipher",
        "CopperDragon",
        "LuckySpecter",
        "ZeroWalrus",
        "MangoReaper",
        "DustyPhoenix",
        "LunarFerret",
        "AtomicSnail",
        "SilverKraken",
        "BurningPixel",
        "TacticalHamster",
        "SonicMushroom",
        "BlueRonin",
        "GoldenWisp",
        "ChaosKiwi",
        "MechaSparrow",
        "HiddenJaguar",
        "CandyVortex",
        "SteelPumpkin",
        "WarpedWizard",
        "FeralButton",
        "ObsidianBee",
        "PanicWizard",
        "TinyTitan",
        "VioletShark",
        "RocketMole",
        "AncientBug",
        "PrimalCookie",
        "MidnightSloth",
        "SavageTeapot",
        "MagicScarab",
        "JungleSyntax",
        "RubyHawk",
        "BananaKnight",
        "CosmicLizard",
        "PhantomMuffin",
        "PixelSamurai",
        "ElectricYeti",
        "NeonTurtle",
        "GrimMango",
        "TurboGhost",
        "ShadowNugget",
        "FrozenBandit",
        "SolarPickle",
        "CyberFalcon",
        "IronCupcake",
        "VoidPigeon",
        "StormyWizard",
        "QuantumMoose",
        "LuckyRaptor",
        "ToxicWizard",
        "MoonPancake",
        "DarkFlamingo",
        "EchoTiger",
        "CrimsonOtter",
        "SilentMammoth",
        "GlitchKnight",
        "NovaBadger",
        "RustyShadow",
        "VelvetComet",
        "PlasmaRonin",
        "GoldenMantis",
        "ArcanePigeon"
    };

    private void Awake()
    {
        if (view == null)
            view = GetComponent<CohtmlView>();

        _model.Username = "TestPlayer";

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
        Debug.Log("[PlayerNameUpdater] OnReadyForBindings");
        
        var createdModel = view.NativeView.CreateModel("HudUsername", _model);

        if (createdModel == null)
        {
            Debug.LogError("[PlayerNameUpdater] Failed to create GameFace data model.");
            return;
        }

        _isModelReady = true;

        PushModelToUi();

        StartCoroutine(ChangeUsername());

        Debug.Log("[PlayerNameUpdater] Data model registered.");
    }

    private IEnumerator ChangeUsername()
    {
        while (_isModelReady)
        {
            var delay = Random.Range(1, 6);
            yield return new WaitForSeconds(delay);

            var index = Random.Range(0, UserNames.Length);
            _model.Username = UserNames[index];

            Debug.Log($"[PlayerNameUpdater] _model.Username={_model.Username}");
            
            PushModelToUi();
        }
    }

    private void PushModelToUi()
    {
        if (!_isModelReady || view == null || view.NativeView == null)
            return;

        view.NativeView.UpdateWholeModel(_model);
        view.NativeView.SynchronizeModels();
        
        Debug.Log("[PlayerNameUpdater] Model synchronized");
    }
}
