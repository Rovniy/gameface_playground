#!/usr/bin/env node
/**
 * Build each view as an independent Vite project. For every directory under
 * views/ that contains an index.html we run vite.build() with the view as
 * `root` and the corresponding folder under
 *
 *   Assets/StreamingAssets/Cohtml/UIResources/<view-path>
 *
 * as `outDir`. Nothing is shared across views — each output is a self-contained
 * HTML + JS + CSS + cohtml.js bundle that Cohtml loads via
 * coui://uiresources/<view-path>/index.html. The shared vite.config.ts (used
 * by `vite` dev server and Vitest) is reused so plugins, aliases, and SCSS
 * stay configured in one place.
 *
 * Pass --watch to keep one Rollup watcher per view alive: edits to anything
 * in a view's import graph (TS/Vue/SCSS/SVG) trigger an incremental rebuild
 * of just that view, leaving the other views' StreamingAssets folders
 * untouched. Stop with Ctrl+C.
 */
import { build, mergeConfig, loadConfigFromFile } from 'vite'
import { readdirSync, statSync, existsSync, copyFileSync, mkdirSync } from 'node:fs'
import { join, dirname, relative } from 'node:path'
import { fileURLToPath } from 'node:url'

const watch = process.argv.includes('--watch')
const frontendRoot = join(dirname(fileURLToPath(import.meta.url)), '..')
const viewsRoot = join(frontendRoot, 'views')
const projectRoot = join(frontendRoot, '..')
const destRoot = join(projectRoot, 'Assets', 'StreamingAssets', 'Cohtml', 'UIResources')
const cohtmlSource = join(frontendRoot, 'shared', 'cohtml.js')
const fontsSource = join(frontendRoot, 'shared', 'assets', 'fonts')

// cohtml.js is a classic (non-module) script — Vite refuses to bundle those
// from <script src> tags, so we copy it ourselves as a static sibling of
// index.html. The HTML reference `<script src="./cohtml.js">` then resolves
// both in dev (served by vite) and in production (copied here).
//
// `addWatchFile` registers cohtml.js with Rollup's watcher so editing it
// triggers a rebuild even though it's outside the JS import graph.
function copyCohtmlPlugin(outDir) {
  return {
    name: 'gameface:copy-cohtml',
    apply: 'build',
    buildStart() {
      this.addWatchFile(cohtmlSource)
    },
    closeBundle() {
      mkdirSync(outDir, { recursive: true })
      copyFileSync(cohtmlSource, join(outDir, 'cohtml.js'))
    },
  }
}

// Mirror shared/assets/fonts/* into each view's <outDir>/assets/fonts/.
// We can't lean on Vite's asset pipeline here because SCSS partials don't
// rewrite url() paths — `url('../assets/fonts/X.ttf')` written inside
// shared/styles/_fonts.scss ends up resolved relative to the consuming
// view's CSS, which leaves Vite unable to find the source binary at build
// time. Copying the binaries ourselves into each view keeps the runtime
// path the SCSS expects, and means every per-view bundle is self-contained
// (matching how cohtml.js is handled).
function copyFontsPlugin(outDir) {
  return {
    name: 'gameface:copy-fonts',
    apply: 'build',
    buildStart() {
      if (!existsSync(fontsSource)) return
      for (const file of readdirSync(fontsSource)) {
        this.addWatchFile(join(fontsSource, file))
      }
    },
    closeBundle() {
      if (!existsSync(fontsSource)) return
      const dest = join(outDir, 'assets', 'fonts')
      mkdirSync(dest, { recursive: true })
      for (const file of readdirSync(fontsSource)) {
        const src = join(fontsSource, file)
        if (statSync(src).isFile()) {
          copyFileSync(src, join(dest, file))
        }
      }
    },
  }
}

function findViewDirs(dir, out = []) {
  for (const name of readdirSync(dir)) {
    const path = join(dir, name)
    if (!statSync(path).isDirectory()) continue
    if (existsSync(join(path, 'index.html'))) out.push(path)
    findViewDirs(path, out)
  }
  return out
}

const loaded = await loadConfigFromFile(
  { command: 'build', mode: 'production' },
  join(frontendRoot, 'vite.config.ts'),
)
const sharedConfig = loaded?.config ?? {}

const viewDirs = findViewDirs(viewsRoot)
if (viewDirs.length === 0) {
  console.error('No views with index.html under views/')
  process.exit(1)
}

for (const viewDir of viewDirs) {
  const rel = relative(viewsRoot, viewDir).replace(/\\/g, '/')
  const outDir = join(destRoot, rel)
  const tag = `[${rel}]`

  const result = await build(
    mergeConfig(sharedConfig, {
      configFile: false,
      root: viewDir,
      base: './',
      // Default publicDir = <viewDir>/public — per-view static assets land in
      // the build output as-is (no hashing, no path rewriting). Use this for
      // raw files referenced with relative paths from index.html.
      logLevel: 'warn',
      plugins: [copyCohtmlPlugin(outDir), copyFontsPlugin(outDir)],
      build: {
        outDir,
        // Wipe outDir only on one-shot builds. In watch mode keep it warm so
        // Unity's .meta files survive and only changed files get rewritten.
        emptyOutDir: !watch,
        assetsInlineLimit: 0,
        cssCodeSplit: false,
        reportCompressedSize: false,
        watch: watch ? {} : null,
        rollupOptions: {
          output: {
            entryFileNames: 'js/main.js',
            chunkFileNames: 'js/[name].js',
            assetFileNames: (asset) => {
              const name = asset.name || ''
              if (name.endsWith('.css')) return 'styles/main.css'
              return 'assets/[name][extname]'
            },
            inlineDynamicImports: true,
          },
        },
      },
    }),
  )

  if (watch) {
    // result is a RollupWatcher; attaching listeners keeps the loop alive.
    result.on('event', (e) => {
      if (e.code === 'BUNDLE_END') {
        console.log(`${tag} ✓ rebuilt in ${e.duration}ms`)
      } else if (e.code === 'ERROR') {
        const msg = e.error?.message ?? String(e.error)
        console.error(`${tag} ✗ ${msg}`)
      }
    })
    console.log(`${tag} initial build done — watching`)
  } else {
    console.log(`✓ ${rel} → ${relative(projectRoot, outDir).replace(/\\/g, '/')}`)
  }
}

if (watch) {
  console.log(`\nWatching ${viewDirs.length} view${viewDirs.length === 1 ? '' : 's'}. Ctrl+C to stop.`)
} else {
  console.log(`Built ${viewDirs.length} view${viewDirs.length === 1 ? '' : 's'}.`)
}
