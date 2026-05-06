<script setup lang="ts">
import { computed, ref } from 'vue'
import { useEngineEvent } from '@shared/js/engine'

type MinimapPoint = {
  x: number
  y: number
}


const MAP_SIZE = 160

const heroX = ref(0.5)
const heroY = ref(0.5)
const trail = ref<MinimapPoint[]>([])

function clamp01(value: number) {
  return Math.max(0, Math.min(1, value))
}

const heroStyle = computed(() => ({
  left: `${heroX.value * 100}%`,
  top: `${heroY.value * 100}%`,
}))

const trailPolylinePoints = computed(() => {
  return trail.value
      .map((point) => {
        const x = clamp01(point.x) * MAP_SIZE
        const y = clamp01(point.y) * MAP_SIZE

        return `${x},${y}`
      })
      .join(' ')
})

useEngineEvent<[number, number]>('HUD_OnMinimapPositionChanged', (x, y) => {
  heroX.value = Math.max(0, Math.min(1, x))
  heroY.value = Math.max(0, Math.min(1, y))
})

useEngineEvent<[number, number]>('HUD_OnMinimapTrailPointAdded', (x, y) => {
  trail.value.push({
    x: clamp01(x),
    y: clamp01(y),
  })

  if (trail.value.length > 200) {
    trail.value.shift()
  }
})
</script>

<template>
  <div class="minimap" aria-label="minimap">
    <div class="minimap__viewport">
      <svg
          class="minimap__trail"
          :viewBox="`0 0 ${MAP_SIZE} ${MAP_SIZE}`"
          preserveAspectRatio="none"
      >
        <polyline
            v-if="trailPolylinePoints.length > 0"
            :points="trailPolylinePoints"
            class="minimap__trail-line"
        />
      </svg>

      <div class="minimap__hero" :style="heroStyle"></div>
    </div>

    <span class="minimap__label">MAP</span>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.minimap {
  position: relative;
  width: 200px;
  height: 200px;
  background: rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: $radius-md;
  box-shadow: $shadow-md;
  overflow: hidden;

  &__viewport {
    position: absolute;
    width: 100%;
    height: 100%;
    background: radial-gradient(circle at center, rgba(124, 92, 255, 0.12), transparent 70%);
  }

  &__trail {
    position: absolute;
    width: 100%;
    height: 100%;
    pointer-events: none;
  }

  &__trail-line {
    fill: none;
    stroke: rgb(248 19 19 / 0.9);
    stroke-width: 3;
    stroke-linecap: round;
    stroke-linejoin: round;
  }

  &__hero {
    position: absolute;
    width: 10px;
    height: 10px;
    background: $color-accent;
    border: 2px solid white;
    border-radius: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 0 10px $color-accent;
    transition: left 0.15s ease, top 0.15s ease;
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
