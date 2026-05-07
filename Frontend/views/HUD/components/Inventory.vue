<script setup lang="ts">
import { ref } from 'vue'
import { useEngineEvent } from '@shared/js/engine'
import { useI18n } from '../js/i18n'

interface Skill {
  name: string
  icon?: string
}

const { t } = useI18n()
const SLOT_COUNT = 4

// Always render exactly 4 cells so the strip's footprint stays stable. C#
// pushes a fresh array via `HUD_OnInventoryUpdate`; missing entries land as
// empty slots, extras past index 3 are ignored.
const slots = ref<(Skill | null)[]>(Array.from({ length: SLOT_COUNT }, () => null))

useEngineEvent<[(Skill | null)[]]>('HUD_OnInventoryUpdate', (next) => {
  const padded: (Skill | null)[] = Array.from({ length: SLOT_COUNT }, () => null)
  for (let i = 0; i < Math.min(SLOT_COUNT, next.length); i++) padded[i] = next[i]
  slots.value = padded
})
</script>

<template>
  <div class="inventory" :aria-label="t('inventory.aria')">
    <div
      v-for="(slot, i) in slots"
      :key="i"
      class="inventory__slot"
      :class="{ 'inventory__slot--empty': !slot }"
      :title="slot?.name ?? t('inventory.slot', { n: i + 1 })"
    >
      <span v-if="slot" class="inventory__icon">{{ slot.icon ?? '✦' }}</span>
      <span class="inventory__hotkey">{{ i + 1 }}</span>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.inventory {
  display: flex;
  align-items: center;
  gap: $space-sm;
  padding: $space-sm;
  background: rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: $radius-md;
  box-shadow: $shadow-md;
}

.inventory__slot {
  position: relative;
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $color-surface;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: $radius-sm;

  &--empty {
    background: rgba(0, 0, 0, 0.25);
    border-style: dotted;
    border-color: rgba(255, 255, 255, 0.08);
  }
}

.inventory__icon {
  font-size: 24px;
  line-height: 1;
  color: $color-fg;
}

.inventory__hotkey {
  position: absolute;
  bottom: 4px;
  right: 6px;
  font-size: 10px;
  font-weight: 700;
  color: $color-muted;
}
</style>
