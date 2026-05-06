#!/usr/bin/env node
/**
 * Copies shared/cohtml.js into every view directory under views/ so each
 * view stays a self-contained Cohtml UI bundle (HTML + CSS + JS + cohtml.js).
 * A "view" is any directory containing an index.html.
 */
import { readdirSync, statSync, copyFileSync, existsSync } from 'node:fs'
import { join, dirname, relative } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = join(dirname(fileURLToPath(import.meta.url)), '..')
const source = join(root, 'shared', 'cohtml.js')
const viewsRoot = join(root, 'views')

if (!existsSync(source)) {
  console.error(`✗ Source not found: ${source}`)
  process.exit(1)
}

function findViewDirs(dir) {
  const out = []
  for (const name of readdirSync(dir)) {
    const path = join(dir, name)
    if (!statSync(path).isDirectory()) continue
    if (existsSync(join(path, 'index.html'))) {
      out.push(path)
    } else {
      out.push(...findViewDirs(path))
    }
  }
  return out
}

const dirs = findViewDirs(viewsRoot)
for (const dir of dirs) {
  copyFileSync(source, join(dir, 'cohtml.js'))
  const rel = relative(root, dir).replace(/\\/g, '/')
  console.log(`  cohtml.js → ${rel}/`)
}
console.log(`Synced cohtml.js into ${dirs.length} view${dirs.length === 1 ? '' : 's'}.`)
