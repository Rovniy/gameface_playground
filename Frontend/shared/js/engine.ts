import { onBeforeUnmount, onMounted, ref } from 'vue'

/**
 * Subscribe to an `engine` event for the lifetime of the component.
 * Auto-unbinds on unmount.
 */
export function useEngineEvent<T extends unknown[] = unknown[]>(
  event: string,
  handler: (...args: T) => void,
): void {
  const wrapped = handler as (...args: unknown[]) => void
  onMounted(() => engine.on(event, wrapped))
  onBeforeUnmount(() => engine.off(event, wrapped))
}

/**
 * Reactive flag that flips to `true` once Cohtml's `Ready` event fires
 * (which is when C# bindings are guaranteed to be attached).
 */
export function useEngineReady() {
  const ready = ref(false)
  const onReady = () => {
    ready.value = true
  }
  onMounted(() => engine.on('Ready', onReady))
  onBeforeUnmount(() => engine.off('Ready', onReady))
  return { ready }
}

export function triggerEngine(event: string, ...args: unknown[]): void {
  engine.trigger(event, ...args)
}

export function callEngine<T = unknown>(event: string, ...args: unknown[]): Promise<T> {
  return engine.call<T>(event, ...args)
}
