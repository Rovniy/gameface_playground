<script setup lang="ts">
import {computed, onMounted, ref} from 'vue'
import { useEngineEvent } from '@shared/js/engine'

const health = ref(100)
const maxHealth = ref(100)
const fillStyle = computed(() => ({
  width: `${Math.max(0, Math.min(1, health.value / maxHealth.value)) * 100}%`,
}))

useEngineEvent<[number, number]>('HUD_OnHealthChanged', (value, max) => {
  health.value = value
  maxHealth.value = max
})

onMounted(() => {
  setInterval(() => {
    health.value = Math.ceil(Math.random() * 100)
  }, 2000)
})
</script>

<template>
  <div class="healthbar">
    <div class="healthbar__fill" :style="fillStyle"></div>
    <span class="healthbar__label">{{ health }} / {{ maxHealth }}</span>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.healthbar {
  position: relative;
  width: 240px;
  height: 16px;
  background: rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-sm;
  overflow: hidden;

  &__fill {
    height: 100%;
    position: absolute;
    background: linear-gradient(90deg, $color-danger, $color-warn);
    transition: width 0.5s ease;
  }

  &__label {
    position: absolute;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 11px;
    font-weight: 700;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -50%);
    color: $color-fg;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.6);
  }
}
</style>
