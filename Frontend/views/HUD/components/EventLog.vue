<script setup lang="ts">
import { useI18n } from '../js/i18n'

const { t } = useI18n()
</script>

<template>
  <div class="binding-demo__log-card">
    <div class="binding-demo__log-header">
      <span>{{ t('eventLog.title') }}</span>
      <span class="binding-demo__log-status">{{ t('eventLog.status') }}</span>
    </div>

    <div class="binding-demo__log-list">
      <div class="binding-demo__log-item" data-bind-for="event:{{HudDemo.events}}">
        <span class="binding-demo__log-time" data-bind-value="{{event.time}}"></span>
        <span class="binding-demo__log-text" data-bind-value="{{event.text}}"></span>
        <span class="binding-demo__log-damage" data-bind-value="{{event.damage}}"></span>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.binding-demo__log-card {
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 18px;
  background: rgba(5, 8, 18, 0.78);
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.35);
  overflow: hidden;
  max-width: 400px;
}

.binding-demo__log-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.14em;
  color: rgba(255, 255, 255, 0.7);
  border-bottom: 1px solid rgba(255, 255, 255, 0.07);
}

.binding-demo__log-status {
  color: #58ff9a;
}

// Pure CSS auto-pin to bottom: a fixed-height flex column with
// justify-content: flex-end always sticks its content against the bottom
// edge. When more rows arrive than the box can fit, the surplus overflows
// the *top* and is clipped by overflow:hidden — exactly the "old logs slide
// up off-screen, newest stays at the bottom" behaviour, with zero JS and
// nothing Cohtml is fussy about (no calc, no -webkit-, no MutationObserver).
.binding-demo__log-list {
  height: 240px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  gap: 8px;
  padding: 10px;
}

.binding-demo__log-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 9px 10px;
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.055);
  // Don't let flexbox squish rows when the list outgrows the box, otherwise
  // every row would shrink instead of clipping cleanly off the top.
  flex: 0 0 auto;
  // New rows fade up into place so their arrival reads softly even though
  // the rows above shift instantly.
  animation: logItemEnter 0.28s ease both;
}

.binding-demo__log-time {
  flex: 0 0 54px;
  font-size: 11px;
  color: rgba(255, 255, 255, 0.45);
  font-variant-numeric: tabular-nums;
}

.binding-demo__log-text {
  flex: 1 1 auto;
  font-size: 12px;
  line-height: 1.25;
  color: rgba(255, 255, 255, 0.88);
}

.binding-demo__log-damage {
  flex: 0 0 44px;
  font-size: 12px;
  font-weight: 700;
  text-align: right;
  color: #ff6b6b;
}

@keyframes logItemEnter {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
