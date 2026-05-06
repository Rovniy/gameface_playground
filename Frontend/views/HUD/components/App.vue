<script setup lang="ts">
import Healthbar from './Healthbar.vue'
import MovementPad from './MovementPad.vue'
import Inventory from './Inventory.vue'
import Minimap from './Minimap.vue'
import Settings from './Settings.vue'
import Counter from "./Counter.vue";
import Dancing from "./Dancing.vue";
import EventLog from "./EventLog.vue";
</script>

<template>
  <main class="hud">
    <header class="hud__row hud__top">
      <div class="hud__cell hud__cell--start">
        <div class="hud__column">
          <Healthbar />
          <Counter />
          <EventLog />
        </div>
      </div>
      <div class="hud__cell hud__cell--center"></div>
      <div class="hud__cell hud__cell--end">
        <div class="hud__row">
          <Dancing />
          <Minimap />
        </div>
      </div>
    </header>

    <section class="hud__middle">
    </section>

    <footer class="hud__row">
      <div class="hud__cell hud__cell--start"><Settings /></div>
      <div class="hud__cell hud__cell--center"><Inventory /></div>
      <div class="hud__cell hud__cell--end"><MovementPad /></div>
    </footer>
  </main>
</template>

<style lang="scss">
@use 'shared/styles/reset';
</style>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

// Full-screen HUD canvas. Top row pinned to top, bottom row to bottom; the
// middle section grows to fill the gap between them. Each row is split into
// three equal flex cells so a centered widget (e.g. ActionPanel) stays at
// true screen center even when an end-aligned widget (e.g. MovementPad) is
// also present in the same row.
.hud {
  position: fixed;
  display: flex;
  flex-direction: column;
  padding: $space-lg;
  gap: $space-md;
  width: 100vw;
  height: 100vh;
  // Background is click-through; only the actual widgets take pointer events
  // (set on direct children of cells / middle, never on empty layout cells).
  pointer-events: none;

  &__row {
    display: flex;
    align-items: flex-start;
    gap: $space-md;

    &:last-child {
      align-items: flex-end;
    }
  }

  &__cell {
    flex: 1;
    display: flex;

    &--start  { justify-content: flex-start; }
    &--center { justify-content: center; }
    &--end    { justify-content: flex-end; }
  }

  &__middle {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  &__cell > *,
  &__middle > * {
    pointer-events: auto;
  }

  &__column {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 1vh;
  }

  &__top {
    .hud__row {
      &:last-child {
        align-items: flex-start;
      }
    }
  }
}
</style>
