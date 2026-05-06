export interface GamefaceEngine {
  on(event: string, handler: (...args: unknown[]) => void): void
  off(event: string, handler: (...args: unknown[]) => void): void
  trigger(event: string, ...args: unknown[]): void
  call<T = unknown>(event: string, ...args: unknown[]): Promise<T>
}

declare global {
  // `cohtml.js` (loaded as a classic script before main.js) ensures `engine`
  // is always defined — in Cohtml it's wired to native, in a plain browser
  // it's a stub whose methods no-op. Either way our code can call it
  // unconditionally without null checks.
  const engine: GamefaceEngine
}
