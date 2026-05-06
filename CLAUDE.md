# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Unity 6 (`6000.3.2f1`) + URP playground for **Coherent Labs Gameface** (`Cohtml`) — an HTML/CSS/JS UI runtime that renders web pages as Unity textures and binds them to C#. The repository is essentially the official Cohtml sample bundle wired into a fresh URP template; treat it as a sandbox for experimenting with Gameface integration patterns rather than a shipping game.

## Critical: Gameface package path

`Packages/manifest.json` references the trial package by an **absolute Windows path on this machine**:

```
"com.coherent-labs.cohtml": "file:C:/Users/OS/Downloads/com.coherent-labs.gameface.trial@3.0.0-build3/com.coherent-labs.gameface.trial@3.0.0-build3"
```

If that directory is missing, the project will not load — Unity reports the package as missing and every Cohtml sample script fails to compile. Before touching anything Cohtml-related, verify that path resolves. When working on a different machine, the manifest must be rewritten to point at the local copy of the trial package.

The Cohtml runtime/editor/rendering-backend code itself lives **inside that external package**, not in this repo. The `Cohtml.*.csproj` files in the working directory are Unity-generated IDE shims that reference back into it. Do not try to edit Cohtml core files in those csprojs — open the package source under that Downloads path.

## Repository layout

- `Assets/Samples/Cohtml/3.0.0/<SampleName>/` — C# MonoBehaviours and `.unity` scenes for each sample (Sample Hub, Mad Rabbits, Moba, Leaderboard, Live Views, Event Binding, Input Propagation, In-World Views, Localization, View in Canvas UI, Surface Partitioning HUD, Websockets). Files carry the Coherent Labs proprietary header — do not relicense.
- `Assets/StreamingAssets/Cohtml/UIResources/<SampleName>/` — the HTML/CSS/JS for each sample. Cohtml `View` components reference these via `coui://uiresources/<SampleName>/<file>.html`. The `SampleHub` directory has its own `package.json` and `node_modules` (run `npm install` there to refresh `coherent-gameface-interaction-manager`).
- `Assets/Scenes/SampleScene.unity` — the only scene listed in `EditorBuildSettings`. The Cohtml sample scenes are **not** in the build list by default; if you need to load them in a player build, add them via `File → Build Settings`.
- `Assets/Samples/Cohtml/3.0.0/Sample Hub/samples-extended-data.json` — the source of truth for what shows up in the in-game Sample Hub UI. Each entry's `"link"` is the **scene name** to load (no `coui://` prefix), wired through `SampleHubNavigation.LoadSample`.

## How the Sample Hub flow works

This is the architectural backbone you need to understand before changing anything sample-related:

1. The Sample Hub scene contains a `CohtmlView` pointing at `coui://uiresources/SampleHub/index.html`.
2. `SampleHub.cs` waits for `viewComponent.Listener.ReadyForBindings`, then registers two JS-triggered events: `Hub_OnLoadSample` (string scene name) and `Hub_OnScriptingReady` (signals JS is ready). On the latter, C# calls `viewComponent.NativeView.ExecuteScript("addSamples(...)")` injecting `samples-extended-data.json` into the page.
3. When the user clicks a sample tile, the JS fires `engine.trigger('Hub_OnLoadSample', name)`, which routes to `SampleHubNavigation.LoadSample(name)` → `SceneManager.LoadScene(name)`.
4. `SampleHubNavigation` is a `DontDestroyOnLoad` singleton; pressing **H** in any sample scene returns to the hub.

Adding a new sample therefore means: (a) create the scene and add it to Build Settings, (b) drop UI under `Assets/StreamingAssets/Cohtml/UIResources/<Name>/`, (c) add a `CohtmlView` pointing at it, (d) append an entry to `samples-extended-data.json` whose `link` matches the scene name.

## Cohtml binding API conventions

The samples (especially `Event Binding/EventBinding.cs`) demonstrate the full surface; reuse these patterns rather than inventing new ones:

- **C# subscribes to JS events:** `view.NativeView.RegisterForEvent("Name", (Action<T>)Handler)` — fired from JS via `engine.trigger('Name', payload)`. Multiple handlers per event are allowed.
- **C# exposes a callable returning a value:** `view.NativeView.BindCall("Name", (Func<...>)Handler)` — invoked from JS via `engine.call('Name').then(result => ...)`. Bind only one handler per call name.
- **C# pushes to JS:** `view.NativeView.TriggerEvent("Name", args...)` (fire-and-forget) or `view.NativeView.ExecuteScript("...")` (raw JS eval).
- **Lifecycle hooks:** `view.Listener.ReadyForBindings` (register everything here, **not** in `Start`), `view.Listener.BindingsReleased` (clean up — used in `EventBinding.cs` to null out static data).
- **Input system gating:** sample code branches on `#if ENABLE_INPUT_SYSTEM` (e.g., `SampleHubNavigation.cs`) because this project ships with `com.unity.inputsystem` 1.17 enabled and the legacy input manager disabled.

## Building & running

Open the project root in Unity Hub with version `6000.3.2f1` (the version is pinned in `ProjectSettings/ProjectVersion.txt`). The `.csproj` and `.sln` files at the root are regenerated by Unity on import — do not edit them by hand and do not commit them as the source of truth.

There is no CI, no test harness wired up beyond `com.unity.test-framework`, and **no commits on `main` yet** (`git log` errors with "branch does not have any commits"). There is also no `.gitignore`, which is why Unity's `Library/`, `Logs/`, `Temp/`, `obj/`, and the generated `*.csproj`/`.sln` show as untracked. Before the first commit, add a Unity-flavored `.gitignore` so those don't get checked in.

Tests, when present, run via Unity's Test Runner window (Window → General → Test Runner) or `Unity -batchmode -runTests -testPlatform EditMode|PlayMode`. There are no project-specific test suites in `Assets/` today.

## URP / rendering notes

Two URP asset pairs in `Assets/Settings/` — `PC_RPAsset.asset` / `PC_Renderer.asset` and `Mobile_RPAsset.asset` / `Mobile_Renderer.asset` — selected by quality level. The Cohtml rendering backend lives in `Cohtml.RenderingBackend.*` (inside the trial package); if a sample renders black or with the wrong sRGB curve, that's where to look, not in URP settings.
