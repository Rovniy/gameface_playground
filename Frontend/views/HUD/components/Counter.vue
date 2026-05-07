<script setup lang="ts">
import { useI18n } from '../js/i18n'

const { t } = useI18n()
</script>

<template>
  <div class="counter">
    <span class="counter__label">{{ t('counter.label') }}</span>
    <span class="counter__value" data-bind-value="{{HudDemo.counter}}">0</span>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.counter {
  display: flex;
  flex-grow: 0;
  flex-direction: column;
  pointer-events: none;
  color: white;
  padding: 5px 10px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-md;
  background: rgba(5, 8, 18, 0.78);
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.35);
  align-self: flex-start;

  &__label {
    font-size: 11px;
    letter-spacing: 0.16em;
    color: rgba(255, 255, 255, 0.55);
    margin-bottom: 8px;
  }
  &__value {
    font-size: 54px;
    line-height: 1;
    font-weight: 700;
    letter-spacing: -0.04em;
    color: #7c5cff;
    text-shadow: 0 0 22px rgba(124, 92, 255, 0.55);
    animation: counterPulse 0.35s ease;
  }
}

@keyframes counterPulse {
  0% {
    transform: scale(1);
    filter: brightness(1);
  }

  45% {
    transform: scale(1.08);
    filter: brightness(1.35);
  }

  100% {
    transform: scale(1);
    filter: brightness(1);
  }
}
</style>
