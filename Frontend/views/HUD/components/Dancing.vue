<script lang="ts" setup>
import { useI18n } from '../js/i18n'

const { t } = useI18n()
const texture = 'coui://CharacterPreview'
</script>

<template>
  <div class="character-preview">
    <img
        class="character-preview__image"
        :src="texture"
        :alt="t('dancing.alt')"
    />

    <span class="character-preview__label">{{ t('dancing.label') }}</span>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.character-preview {
  width: 200px;
  height: 200px;
  border-radius: $radius-md;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.45);
  border: 1px solid rgba(255, 255, 255, 0.12);
  position: relative;

  &__image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
  }

  &__label {
    position: absolute;
    top: 6px;
    left: 8px;
    font-size: 10px;
    font-weight: 700;
    letter-spacing: 0.1em;
    color: $color-muted;
    text-transform: uppercase;
  }
}
</style>
