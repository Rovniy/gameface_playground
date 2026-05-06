<script setup lang="ts">
import {triggerEngine} from "@shared/js/engine.ts";
import {onMounted} from "vue";

onMounted(() => triggerEngine('HUD_NameplateReady'))
</script>

<template>
  <div class="nameplate">
    <span class="nameplate__name" data-bind-value="{{HudUsername.username}}">None</span>
    <div class="nameplate__healthbar">
      <div class="nameplate__healthbar-fill"></div>
    </div>
  </div>
</template>

<style lang="scss">
@use 'shared/styles/_reset';
</style>

<style lang="scss" scoped>
@use 'shared/styles/tokens' as *;

.nameplate {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex-grow: 0;
  gap: $space-xs;
  padding: $space-xs;
  border-radius: $radius-md;
  background: rgba(0,0,0, .4);

  &__name {
    font-size: 16px;
    color: white;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.8);
  }

  &__healthbar {
    width: 96px;
    height: 4px;
    background: rgba(0, 0, 0, 0.6);
    border-radius: 2px;
    overflow: hidden;
  }

  &__healthbar-fill {
    height: 100%;
    background: $color-accent;
    transition: width 0.15s ease;
  }
}
</style>
