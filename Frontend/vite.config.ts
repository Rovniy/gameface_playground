import {defineConfig} from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import {fileURLToPath, URL} from 'node:url'

// Single config for dev server + Vitest. The per-view production build lives
// in scripts/build-views.mjs and reuses this file via vite's loadConfigFromFile,
// overriding `root` and `build` per view so each one becomes a self-contained
// bundle in StreamingAssets.
export default defineConfig({
    // @ts-ignore
    plugins: [vue()],
    resolve: {
        alias: {
            '@shared': fileURLToPath(new URL('./shared', import.meta.url)),
        },
    },
    css: {
        preprocessorOptions: {
            scss: {
                // Lets view stylesheets do `@use 'shared/styles'` etc., resolved
                // against the Frontend/ root rather than the per-view file's dir.
                loadPaths: [fileURLToPath(new URL('.', import.meta.url))],
            },
        },
    },
    // Dev: serve views/ as the document root. Visit http://localhost:5173/HUD/
    // or http://localhost:5173/InWorld/nameplate/ to load a view in the browser
    // — Vite handles SCSS/Vue/TS on the fly with HMR.
    root: fileURLToPath(new URL('./views', import.meta.url)),
    server: {
        port: 5173,
        fs: {
            // dev server needs to read SCSS partials and TS modules under shared/,
            // which lives one level above `root`.
            allow: [fileURLToPath(new URL('.', import.meta.url))],
        },
    },
    test: {
        // Vite's `root` is views/ for the dev server, but tests live under
        // shared/ as well, so Vitest gets its own root pinned at Frontend/.
        root: fileURLToPath(new URL('.', import.meta.url)),
        environment: 'happy-dom',
        globals: true,
        include: ['{shared,views}/**/*.{test,spec}.ts'],
    },
})
