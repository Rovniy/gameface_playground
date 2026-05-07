import { computed, type ComputedRef } from 'vue'
import { ref } from 'vue'
import { triggerEngine } from '@shared/js/engine'
import en from '../i18n/en.json'
import ru from '../i18n/ru.json'
import ar from '../i18n/ar.json'
import zh from '../i18n/zh.json'

// HUD-scoped i18n: a single source of truth shared by every component in this
// view. The active language lives in module-level state so any `useI18n()`
// caller reads the same reactive value — Localization.vue mutates it via
// `setLang` and the rest of the HUD re-renders. Frontend-wide concerns stay in
// shared/; this file is intentionally local to views/HUD/ because each Cohtml
// view ships its own bundle and doesn't share a runtime.

export type Lang = 'en' | 'ru' | 'ar' | 'zh'
export type Dir = 'ltr' | 'rtl'

interface Dictionary {
  meta: { name: string; dir: Dir }
  [key: string]: unknown
}

const dictionaries: Record<Lang, Dictionary> = {
  en: en as Dictionary,
  ru: ru as Dictionary,
  ar: ar as Dictionary,
  zh: zh as Dictionary,
}

export const LANGS: { code: Lang; name: string; dir: Dir }[] = (
  ['en', 'ru', 'ar', 'zh'] as Lang[]
).map((code) => ({
  code,
  name: dictionaries[code].meta.name,
  dir: dictionaries[code].meta.dir,
}))

const current = ref<Lang>('en')

function lookup(dict: Dictionary, path: string): string | undefined {
  const segments = path.split('.')
  let node: unknown = dict
  for (const segment of segments) {
    if (node && typeof node === 'object' && segment in (node as object)) {
      node = (node as Record<string, unknown>)[segment]
    } else {
      return undefined
    }
  }
  return typeof node === 'string' ? node : undefined
}

function interpolate(template: string, params?: Record<string, string | number>): string {
  if (!params) return template
  return template.replace(/\{(\w+)\}/g, (_, key: string) =>
    key in params ? String(params[key]) : `{${key}}`,
  )
}

export interface I18n {
  lang: ComputedRef<Lang>
  dir: ComputedRef<Dir>
  t: (key: string, params?: Record<string, string | number>) => string
  setLang: (lang: Lang) => void
}

export function useI18n(): I18n {
  return {
    lang: computed(() => current.value),
    dir: computed(() => dictionaries[current.value].meta.dir),
    // Translate `key` against the active dictionary. Falls back to the key
    // itself if the path is missing — easier to spot in the UI than rendering
    // an empty string.
    t: (key, params) => {
      const raw = lookup(dictionaries[current.value], key)
      return raw === undefined ? key : interpolate(raw, params)
    },
    setLang: (lang) => {
      if (current.value === lang) return
      current.value = lang
      triggerEngine('HUD_OnLanguageChanged', lang)
    },
  }
}

// Fire the initial language to Unity once, at module load. This runs before
// any `useI18n()` consumer mounts so C# always learns the starting locale
// without depending on a particular component's lifecycle.
triggerEngine('HUD_OnLanguageChanged', current.value)
