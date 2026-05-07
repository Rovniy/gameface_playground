<script setup lang="ts">
import { LANGS, useI18n, type Lang } from '../js/i18n'

const { lang, dir, t, setLang } = useI18n()

function switchTo(code: Lang) {
  setLang(code)
}
</script>

<template>
  <section class="loc" :dir="dir" :lang="lang">
    <header class="loc__header">
      <h2 class="loc__title">{{ t('loc.title') }}</h2>
      <p class="loc__subtitle">{{ t('loc.subtitle') }}</p>
      <span class="loc__dir-badge" dir="ltr">dir = {{ dir }}</span>
    </header>

    <!-- Switcher row pinned LTR so the EN→RU→AR→ZH order doesn't flip in RTL -->
    <div class="loc__buttons" dir="ltr">
      <button
        v-for="entry in LANGS"
        :key="entry.code"
        type="button"
        class="loc__btn"
        :class="{ 'loc__btn--active': lang === entry.code }"
        :lang="entry.code"
        :dir="entry.dir"
        @click="switchTo(entry.code)"
      >
        {{ entry.name }}
      </button>
    </div>

    <div class="loc__row">
      <span class="loc__label">{{ t('loc.shortLabel') }}</span>
      <span class="loc__value">{{ t('loc.monospace') }}</span>
    </div>

    <p class="loc__paragraph">{{ t('loc.paragraph') }}</p>

    <ul class="loc__list">
      <li class="loc__chip">{{ t('loc.actions.attack') }}</li>
      <li class="loc__chip">{{ t('loc.actions.defend') }}</li>
      <li class="loc__chip">{{ t('loc.actions.retreat') }}</li>
    </ul>

    <p class="loc__hint">{{ t('loc.hint') }}</p>
  </section>
</template>

<style scoped lang="scss">
@use 'shared/styles/tokens' as *;

.loc {
  display: flex;
  flex-direction: column;
  gap: $space-sm;
  padding: $space-md;
  width: 300px;
  max-width: 100%;
  background: $color-surface;
  color: $color-fg;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-md;
}

.loc__header {
  display: flex;
  flex-direction: column;
  gap: 2px;
  position: relative;
}

.loc__title {
  margin: 0;
  font-size: 14px;
  font-weight: 700;
}

.loc__subtitle {
  margin: 0;
  font-size: 11px;
  color: $color-muted;
  line-height: 1.4;
}

.loc__dir-badge {
  align-self: flex-start;
  margin-block-start: $space-xs;
  padding: 2px 6px;
  background: $color-bg;
  color: $color-muted;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-sm;
  font-family: $font-mono;
  font-size: 10px;
  letter-spacing: 0.05em;
}

.loc__buttons {
  display: flex;
  gap: $space-xs;
}

.loc__btn {
  flex: 1;
  padding: 6px 4px;
  background: $color-bg;
  color: $color-muted;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-sm;
  font-size: 12px;
  font-weight: 700;
  line-height: 1.2;
  transition: background 0.12s ease, color 0.12s ease, border-color 0.12s ease;

  &:hover { color: $color-fg; }

  &--active {
    background: $color-accent;
    color: white;
    border-color: $color-accent;
  }
}

.loc__row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $space-sm;
  padding: $space-xs $space-sm;
  background: $color-bg;
  border-radius: $radius-sm;
  white-space: break-spaces;
}

.loc__label {
  font-size: 13px;
  font-weight: 700;
}

.loc__value {
  font-size: 12px;
  color: $color-muted;

}

.loc__paragraph {
  margin: 0;
  font-size: 12px;
  line-height: 1.55;
  color: $color-muted;
  overflow-wrap: break-word;
}

.loc__list {
  display: flex;
  flex-wrap: wrap;
  gap: $space-xs;
  margin: 0;
  padding: 0;
  list-style: none;
}

.loc__chip {
  padding: 2px 8px;
  background: $color-bg;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 999px;
  font-size: 11px;
  font-weight: 700;
}

.loc__hint {
  margin: 0;
  font-size: 10px;
  color: $color-muted;
  line-height: 1.45;
}
</style>
