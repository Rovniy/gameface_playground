# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

This file scopes to `Frontend/`. The repo-wide `../CLAUDE.md` covers the Unity / Cohtml side (sample hub flow, C#↔JS binding API, package paths). Read both before making cross-cutting changes — Frontend produces the HTML/JS/CSS bundles that Unity loads; the Cohtml binding contract (`engine.trigger` / `engine.call` / `RegisterForEvent` / `BindCall`) is documented there.

## What this directory is

`Frontend/` is a Vue 3 + TypeScript + SCSS source tree (`package.json` name: `chillbase`) that builds **one self-contained Cohtml UI bundle per view** and writes each bundle into `../Assets/StreamingAssets/Cohtml/UIResources/<view-path>/`. Unity loads them via `coui://uiresources/<view-path>/index.html`. Sources live here; only built output goes into StreamingAssets.

Views currently shipped: `views/HUD/`, `views/InWorld/nameplate/`, `views/InWorld/video-player-local/`.

## Commands

- `npm run dev` — run `sync-cohtml` then start Vite dev server on port 5173. Visit `http://localhost:5173/HUD/`, `http://localhost:5173/InWorld/nameplate/`, etc., to load a view in a real browser with HMR. The global `engine` is stubbed by `shared/cohtml.js` so handlers no-op outside Cohtml.
- `npm run build` — sync `cohtml.js` and run a one-shot per-view production build into StreamingAssets. Wipes each `outDir`.
- `npm run build:watch` — same, but keeps a Rollup watcher per view alive; only changed views are rebuilt and `outDir` is **not** wiped (so Unity's `.meta` files survive).
- `npm run sync` — copy `shared/cohtml.js` into every `views/*/index.html`-bearing directory. Runs implicitly before `dev` / `build`.
- `npm run test` / `npm run test:run` — Vitest with `happy-dom`, picking up `{shared,views}/**/*.{test,spec}.ts`. Single test: `npx vitest run shared/js/engine.test.ts` or `npx vitest -t 'subscribes on mount'`.
- `npm run type-check` — `vue-tsc --noEmit`. There is no separate lint step.

## Architecture: the per-view atomic-build model

The single most important thing to understand here is that **views do not share a build**. Each directory under `views/` that contains an `index.html` is treated as an independent Vite project:

1. `scripts/build-views.mjs` walks `views/` recursively for `index.html` files.
2. For each view it calls `vite.build()` with `root` = the view dir and `outDir` = `Assets/StreamingAssets/Cohtml/UIResources/<view-rel-path>/`, reusing `vite.config.ts` via `loadConfigFromFile` so plugins / aliases / SCSS config stay in one place.
3. Per-view Rollup output is pinned: `js/main.js`, `js/[name].js` for chunks, `styles/main.css`, `assets/[name][extname]`. `cssCodeSplit: false` and `inlineDynamicImports: true` are deliberate — Cohtml loads one HTML file with a fixed set of script/style references, so we don't want chunk splitting.
4. `cohtml.js` is a **classic (non-module) script** that Vite refuses to bundle from a `<script src>` tag. It's copied two ways: `scripts/sync-cohtml.mjs` lays it next to each `index.html` for dev, and `copyCohtmlPlugin` in `build-views.mjs` copies it into each view's `outDir` on `closeBundle`. Both are needed — don't try to import it as an ES module.
5. `vite.config.ts` is shared across **dev server, Vitest, and build**. The dev server's `root` is `views/`; Vitest pins its own `root` back to `Frontend/` because tests live under `shared/`.

Adding a new view: create `views/<name>/index.html` (with `<script src="./cohtml.js"></script>` before the module entry — see `views/HUD/index.html`), `views/<name>/js/main.ts` mounting a Vue app, and `views/<name>/components/App.vue`. Run `npm run sync` once and it becomes a buildable view automatically. To wire it into Unity, point a `CohtmlView` at `coui://uiresources/<name>/index.html`.

## Shared code conventions

- `@shared/*` alias resolves to `Frontend/shared/*` (configured in both `tsconfig.json` paths and Vite `resolve.alias`). Use it in TS imports; SCSS uses `@use 'shared/styles' as *;` instead because SCSS `loadPaths` is set to the Frontend root.
- `shared/js/engine.ts` is the only sanctioned way for Vue components to talk to Cohtml. `useEngineEvent<[T1, T2]>(name, handler)` auto-binds on mount and **unbinds on unmount** — always use it instead of calling `engine.on`/`off` directly so cleanup isn't forgotten. `useEngineReady()` returns a reactive `ready` flag tied to Cohtml's `Ready` event. `triggerEngine` / `callEngine` wrap the outbound side.
- `shared/types/gameface.d.ts` declares `engine` as a non-nullable global. This is safe because `cohtml.js` always defines it (real impl in Cohtml, no-op stub in browsers) — components must not null-check it.
- SCSS: `@forward` in `shared/styles/_index.scss` exposes only `tokens` + `mixins`. `_reset.scss` and any future component CSS must be opted in explicitly per view (see `views/HUD/components/App.vue`'s `@use 'shared/styles/reset';`). Design tokens (`$color-*`, `$space-*`, `$radius-*`, `$shadow-*`) live in `_tokens.scss` — use these instead of literal values.

## Cohtml runtime constraints (CSS subset)

Cohtml is **not a full browser**. When writing styles or component logic for a view:
- No CSS Grid in this project — flex only. Convert any `display: grid` you encounter.
- Avoid `MutationObserver`, `calc()`, and `-webkit-*` prefixes — assume a subset of Web APIs.
- Don't use Vue `<Teleport>` — it crashes Unity. Render modals/overlays inline with `position: fixed`.
- Native form controls (`input[type=range]`, `select`, `input[type=number]`, date/color/file) are unstable. Build custom controls with `div` + mouse events.
- The HUD root must be click-through except for actual interactive widgets. The `pass-through-pointer-events` mixin in `shared/styles/_mixins.scss` and the `.hud { pointer-events: none } .hud__cell > * { pointer-events: auto }` pattern in `views/HUD/components/App.vue` are the canonical way — copy that pattern, don't put `pointer-events: auto` on whole panels.

## Testing notes

`shared/js/engine.test.ts` shows the pattern for testing engine-bound code: stub the global `engine` with an in-memory pub/sub in `beforeEach`, mount the component with `@vue/test-utils`, then assert on subscribe/unsubscribe calls and on handler delivery via `triggerEngine`. New composables that touch `engine` should follow the same fake-engine pattern rather than mocking modules.
