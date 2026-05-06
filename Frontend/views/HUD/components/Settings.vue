<script setup lang="ts">
import { onBeforeUnmount, onMounted, reactive, ref } from 'vue'
import { triggerEngine } from '@shared/js/engine'
import cogsIcon from '@shared/assets/cogs.svg'

const isOpen = ref(false)

// Local UI state. Every control writes to this object via v-model and the
// emitChange call pushes a snapshot to C# so the engine can persist /apply
// settings out-of-band.
const settings = reactive({
  showDamageNumbers: true,
  showFps: false,
  quality: 'high' as 'low' | 'medium' | 'high' | 'ultra',
  fov: 90,
  language: 'en' as 'en' | 'ru' | 'es' | 'de',
})

function emitChange() {
  triggerEngine('HUD_OnSettingsChanged', { ...settings })
}

function close() { isOpen.value = false }
function toggle() { isOpen.value = !isOpen.value }

function onKeydown(e: KeyboardEvent) {
  if (e.key === 'Escape' && isOpen.value) {
    e.preventDefault()
    e.stopPropagation()
    close()
  }
}

onMounted(() => window.addEventListener('keydown', onKeydown))
onBeforeUnmount(() => window.removeEventListener('keydown', onKeydown))
</script>

<template>
  <button
    class="settings-trigger"
    type="button"
    :aria-expanded="isOpen"
    aria-label="Settings"
    title="Settings (Esc)"
    @click="toggle"
  >
    <img :src="cogsIcon" class="settings-trigger__icon" alt="" aria-hidden="true" />
  </button>

  <div v-if="isOpen" class="settings-overlay" @click.self="close">
    <div class="settings-panel" role="dialog" aria-modal="true" aria-label="Settings">
        <header class="settings-panel__header">
          <h2 class="settings-panel__title">Settings</h2>
          <button
            class="settings-panel__close"
            type="button"
            aria-label="Close"
            @click="close"
          >×</button>
        </header>

        <div class="settings-panel__body">
          <section class="settings-section">
            <h3 class="settings-section__title">Graphics</h3>

            <div class="setting">
              <span class="setting__label">Quality preset</span>
              <div class="radio-group">
                <label
                  v-for="q in (['low', 'medium', 'high', 'ultra'] as const)"
                  :key="q"
                  class="radio-group__item"
                  :class="{ 'radio-group__item--active': settings.quality === q }"
                >
                  <input
                    type="radio"
                    name="quality"
                    :value="q"
                    v-model="settings.quality"
                    @change="emitChange"
                  />
                  <span class="radio-group__label">{{ q }}</span>
                </label>
              </div>
            </div>

            <label class="setting setting--row">
              <span class="setting__label">FOV</span>
              <input
                class="setting__input setting__input--number"
                type="number"
                min="60"
                max="120"
                step="1"
                v-model.number="settings.fov"
                @change="emitChange"
              />
            </label>
          </section>
        </div>

      <footer class="settings-panel__footer">
        <button class="settings-panel__btn" type="button" @click="close">Done</button>
      </footer>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;
@use 'shared/styles/mixins' as *;

// ---- trigger button (sits in HUD, bottom-left cell) ----
.settings-trigger {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $color-surface;
  color: $color-fg;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-md;
  font-size: 22px;
  line-height: 1;
  transition: background 0.15s ease, transform 0.05s ease;

  &:hover { background: $color-surface-soft; }
  &:active { transform: translateY(2px); }
}

.settings-trigger__icon {
  display: block;
  width: 22px;
  height: 22px;
  // <img> can't honour CSS `color` (SVG renders standalone). Filter approximates
  // $color-fg on a monochrome SVG; if you need real currentColor, inline the
  // <svg> markup or load it as a Vue component instead of via <img>.
  filter: invert(94%) sepia(7%) saturate(91%) hue-rotate(193deg) brightness(96%) contrast(89%);
}

// ---- modal overlay ----
// Renders inline as a sibling of the trigger button (no <Teleport> — Cohtml's
// runtime crashes when Vue moves nodes out of the component tree). Position
// the rest of the HUD. The overlay is a direct child of the bottom-left
// hud__cell, so App.vue's `.hud__cell > * { pointer-events: auto }` rule
// re-enables interaction on top of the HUD's click-through canvas.
.settings-overlay {
  position: fixed;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.6);
  z-index: 100;
  pointer-events: auto;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.settings-panel {
  display: flex;
  flex-direction: column;
  width: 560px;
  max-height: 80vh;
  background: $color-surface;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-lg;
  box-shadow: $shadow-lg;
  overflow: hidden;
}

.settings-panel__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $space-md;
  padding: $space-md $space-lg;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

.settings-panel__title {
  margin: 0;
  font-size: 16px;
  font-weight: 700;
  letter-spacing: 0.02em;
}

.settings-panel__close {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  color: $color-muted;
  border: none;
  border-radius: $radius-sm;
  font-size: 20px;
  line-height: 1;

  &:hover { background: rgba(255, 255, 255, 0.06); color: $color-fg; }
}

.settings-panel__body {
  display: flex;
  flex-direction: column;
  gap: $space-lg;
  padding: $space-lg;
  overflow-y: auto;
}

.settings-panel__footer {
  display: flex;
  justify-content: flex-end;
  padding: $space-md $space-lg;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
}

.settings-panel__btn {
  padding: $space-sm $space-md;
  background: $color-accent;
  color: white;
  font-weight: 700;
  border: none;
  border-radius: $radius-md;
}

// ---- sections & shared row ----
.settings-section {
  display: flex;
  flex-direction: column;
  gap: $space-sm;
}

.settings-section__title {
  margin: 0;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: $color-muted;
}

.setting {
  display: flex;
  flex-direction: column;
  gap: $space-xs;

  &--row {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    gap: $space-md;
  }
}

.setting__row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $space-md;
}

.setting__label {
  color: $color-fg;
  font-size: 13px;
}

.setting__value {
  color: $color-muted;
  font-size: 12px;
  min-width: 40px;
  text-align: right;
}

// ---- controls ----
.setting__checkbox {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.setting__slider {
  width: 100%;
}

.setting__input {
  padding: 4px 8px;
  background: $color-bg;
  color: $color-fg;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-sm;
  font-size: 13px;

  &--number {
    width: 72px;
    text-align: right;
  }

  &--key {
    width: 60px;
    text-align: center;
    text-transform: uppercase;
    font-family: $font-mono;
    font-weight: 700;
    letter-spacing: 0.05em;
  }
}

.setting__select {
  padding: 4px 8px;
  background: $color-bg;
  color: $color-fg;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-sm;
  font-size: 13px;
  cursor: pointer;
}

// radio "segmented" group
.radio-group {
  display: flex;
  gap: $space-xs;
  background: $color-bg;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-md;
  padding: 2px;
}

.radio-group__item {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 6px 8px;
  border-radius: $radius-sm;
  cursor: pointer;
  font-size: 12px;
  font-weight: 700;
  text-transform: capitalize;
  color: $color-muted;
  transition: background 0.15s ease, color 0.15s ease;

  input {
    position: absolute;
    opacity: 0;
    pointer-events: none;
  }

  &:hover { color: $color-fg; }
  &--active {
    background: $color-accent;
    color: white;
  }
}

.radio-group__label { line-height: 1; }

// custom toggle switch (input is visually hidden but still focusable)
.switch {
  position: relative;
  display: flex;
  align-items: center;
  width: 36px;
  height: 20px;

  input {
    position: absolute;
    margin: 0;
    opacity: 0;
    cursor: pointer;
    z-index: 1;
  }

  &__track {
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 999px;
    position: relative;
    transition: background 0.15s ease;

    &::after {
      content: '';
      position: absolute;
      top: 2px;
      left: 2px;
      width: 16px;
      height: 16px;
      background: $color-fg;
      border-radius: 50%;
      transition: transform 0.15s ease, background 0.15s ease;
    }
  }

  &__track {
    background: $color-accent;

    &::after {
      transform: translateX(16px);
      background: white;
    }
  }

  input:focus-visible + &__track {
    box-shadow: 0 0 0 2px rgba($color-accent, 0.6);
  }
}
</style>
