import { beforeEach, describe, expect, it, vi } from 'vitest'
import { defineComponent, h } from 'vue'
import { mount } from '@vue/test-utils'
import { triggerEngine, useEngineEvent, useEngineReady } from './engine'

// In Cohtml the global `engine` is wired to native by cohtml.js. In tests we
// stub it with an in-memory pub/sub so component code can stay unchanged and
// we can assert on subscription/unsubscription and event delivery.
type Handler = (...args: unknown[]) => void

function createFakeEngine() {
  const listeners = new Map<string, Set<Handler>>()
  return {
    on: vi.fn((e: string, h: Handler) => {
      if (!listeners.has(e)) listeners.set(e, new Set())
      listeners.get(e)!.add(h)
    }),
    off: vi.fn((e: string, h: Handler) => {
      listeners.get(e)?.delete(h)
    }),
    trigger: vi.fn((e: string, ...args: unknown[]) => {
      listeners.get(e)?.forEach((h) => h(...args))
    }),
    call: vi.fn(),
    _listeners: listeners,
  }
}

let fakeEngine: ReturnType<typeof createFakeEngine>

beforeEach(() => {
  fakeEngine = createFakeEngine()
  ;(globalThis as unknown as { engine: unknown }).engine = fakeEngine
})

describe('useEngineEvent', () => {
  it('subscribes on mount and unsubscribes on unmount', () => {
    const handler = vi.fn<(value: number) => void>()
    const Comp = defineComponent({
      setup() {
        useEngineEvent<[number]>('HUD_OnHealthChanged', handler)
        return () => h('div')
      },
    })

    const wrapper = mount(Comp)
    expect(fakeEngine.on).toHaveBeenCalledWith('HUD_OnHealthChanged', expect.any(Function))

    triggerEngine('HUD_OnHealthChanged', 42)
    expect(handler).toHaveBeenCalledWith(42)

    wrapper.unmount()
    expect(fakeEngine.off).toHaveBeenCalledWith('HUD_OnHealthChanged', expect.any(Function))
    expect(fakeEngine._listeners.get('HUD_OnHealthChanged')?.size ?? 0).toBe(0)
  })
})

describe('useEngineReady', () => {
  it('flips to true once engine fires Ready', () => {
    let api!: ReturnType<typeof useEngineReady>
    const Comp = defineComponent({
      setup() {
        api = useEngineReady()
        return () => h('div')
      },
    })

    const wrapper = mount(Comp)
    expect(api.ready.value).toBe(false)

    triggerEngine('Ready')
    expect(api.ready.value).toBe(true)

    wrapper.unmount()
  })
})
