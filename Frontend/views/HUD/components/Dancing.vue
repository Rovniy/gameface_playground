<script lang="ts" setup>
const texture = 'coui://CharacterPreview'
</script>

<template>
  <div class="character-preview">
    <img
        class="character-preview__image"
        :src="texture"
        alt="Character preview"
    />

    <span class="minimap__label">Render Texture</span>
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
  }
}
</style>
