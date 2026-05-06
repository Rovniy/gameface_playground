<script setup lang="ts">
import { triggerEngine } from '@shared/js/engine'
import arrowIcon from '@shared/assets/arrow.svg'

type Direction = 'up' | 'left' | 'down' | 'right'

function startMove(direction: Direction) {
  triggerEngine('HUD_OnMoveStart', direction)
}

function stopMove() {
  triggerEngine('HUD_OnMoveStop')
}
</script>

<template>
  <div class="movement-pad" aria-label="WASD movement">
    <div class="movement-pad__row">
      <button
        class="movement-pad__btn"
        type="button"
        aria-label="W — forward"
        title="W"
        @mousedown="startMove('up')"
        @mouseup="stopMove"
        @mouseleave="stopMove"
      >
        <img :src="arrowIcon" alt="Forward arrow" class="movement-pad__icon" />
      </button>
    </div>

    <div class="movement-pad__row">
      <button
        class="movement-pad__btn"
        type="button"
        aria-label="A — left"
        title="A"
        @mousedown="startMove('left')"
        @mouseup="stopMove"
        @mouseleave="stopMove"
      >
        <img :src="arrowIcon" alt="Forward arrow" class="movement-pad__icon movement-pad__icon_270" />
      </button>

      <button
        class="movement-pad__btn"
        type="button"
        aria-label="S — back"
        title="S"
        @mousedown="startMove('down')"
        @mouseup="stopMove"
        @mouseleave="stopMove"
      >
        <img :src="arrowIcon" alt="Forward arrow" class="movement-pad__icon movement-pad__icon_180" />
      </button>

      <button
        class="movement-pad__btn"
        type="button"
        aria-label="D — right"
        title="D"
        @mousedown="startMove('right')"
        @mouseup="stopMove"
        @mouseleave="stopMove"
      >
        <img :src="arrowIcon" alt="Forward arrow" class="movement-pad__icon movement-pad__icon_90" />
      </button>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;
@use 'shared/styles/mixins' as *;

.movement-pad {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: $space-xs;
}

.movement-pad__row {
  display: flex;
  gap: $space-xs;
}

.movement-pad__btn {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $color-surface;
  color: $color-fg;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: $radius-md;
  font-size: 18px;
  font-weight: 700;
  line-height: 1;
  transition: background 0.15s ease, transform 0.05s ease;

  &:active {
    transform: translateY(2px);

    .movement-pad__icon {
      filter: brightness(1);
    }
  }
}

.movement-pad__icon {
  width: 24px;
  height: 24px;
  filter: brightness(0.8);

  &_90 {
    transform: rotate(90deg);
  }
  &_180 {
    transform: rotate(180deg);
  }
  &_270 {
    transform: rotate(270deg);
  }
}
</style>
