# План выступления: GameFace в Unity

## 0. Вступление: что мы сегодня показываем

**Ключевой вопрос:**
Что команда должна понять после презентации?

**Ответ:**
GameFace — это не просто “HTML внутри Unity”. Это отдельный runtime-слой для сложного игрового UI, где интерфейс можно делать на HTML/CSS/JS/Vue, а Unity остаётся источником игровой логики.

**Главный тезис:**

```text
Unity отвечает за игру.
GameFace отвечает за интерфейс.
Bridge/Presenter связывает их между собой.
```

---

# 1. Что такое GameFace

**Ключевой вопрос:**
Что такое Coherent GameFace?

**Ответ:**
GameFace — это middleware для создания игрового UI на веб-технологиях: HTML, CSS, JavaScript. В Unity он представлен через `CohtmlView`, который загружает HTML-страницу и рендерит её как игровой интерфейс.

**Что сказать команде:**

```text
Это не браузер в классическом смысле.
Это специализированный UI runtime для игр.
Он похож на web frontend, но работает внутри game engine.
```

**Простой пример:**
HUD, инвентарь, магазин, настройки или nameplate можно сверстать как frontend-компоненты.

---

# 2. Зачем использовать GameFace

**Ключевой вопрос:**
Почему не Unity UI / UI Toolkit / uGUI?

**Ответ:**
GameFace особенно полезен, когда интерфейс становится похож на полноценное приложение:

```text
сложный HUD
инвентарь
магазин
battle pass
настройки
социальные окна
чат
таблицы
лог событий
карточки предметов
динамические списки
анимированные панели
```

**Главный тезис:**

```text
Если UI сложный, часто меняется и требует богатой верстки,
web-подход может быть быстрее и гибче Unity UI.
```

**Важно честно сказать:**
Для простого UI из пары кнопок GameFace может быть избыточен.

---

# 3. Общая архитектура

**Ключевой вопрос:**
Как GameFace встраивается в Unity?

**Ответ:**
Архитектура такая:

```text
Unity Gameplay Systems
        ↓
C# Presenter / Bridge
        ↓
CohtmlView
        ↓
HTML / Vue / Tailwind UI
```

**Что объяснить:**
Unity не должна напрямую знать про конкретные кнопки, div-ы и CSS. Unity должна отдавать UI-состояние и принимать UI-команды.

**Правильная модель:**

```text
PlayerHealthSystem → HudPresenter → GameFace → Vue Store → HealthBar.vue
```

**Плохая модель:**

```text
Player.cs напрямую вызывает JS-кнопки и HTML-элементы
```

---

# 4. Что такое CohtmlView

**Ключевой вопрос:**
Что такое View в GameFace?

**Ответ:**
`CohtmlView` — это окно, слой или поверхность интерфейса. Он загружает HTML-страницу и рендерит её в Unity.

**Типы использования:**

```text
Screen HUD View — интерфейс поверх игры
Fullscreen Menu View — меню, инвентарь, магазин
World-space View — UI в 3D-мире
Live View — динамическая текстура/камера внутри UI
```

**Главный тезис:**
View — это не “одна кнопка”. View — это интерфейсный слой.

---

# 5. Как строить HUD

**Ключевой вопрос:**
HUD должен состоять из многих GameFace View или одной?

**Ответ:**
Обычно HUD лучше делать как **один CohtmlView**, внутри которого живёт Vue-приложение с компонентами.

```text
HudView
  App.vue
  HealthBar.vue
  Minimap.vue
  SkillPanel.vue
  CombatLog.vue
  SettingsModal.vue
```

**Почему так лучше:**

```text
меньше View
меньше render targets
проще input
проще z-index
проще data flow
проще профилировать
```

**Когда делать несколько View:**

```text
отдельный fullscreen inventory/menu
world-space терминал
nameplate над персонажем
live texture preview
debug overlay
```

---

# 6. Где хранить UI-файлы

**Ключевой вопрос:**
Где должны лежать Vue/Tailwind файлы?

**Ответ:**
Исходники Vue лучше держать отдельно, а в `StreamingAssets` класть только собранный билд.

**Рекомендуемая структура:**

```text
Frontend/
  Gameface/
    Hud/
      src/
      App.vue
      package.json
      vite.config.ts
      tailwind.config.ts

Assets/
  StreamingAssets/
    Cohtml/
      UIResources/
        Hud/
          index.html
          assets/
```

**Главный тезис:**
GameFace должен получать готовые `index.html`, `.js`, `.css`, а не Vue-исходники.

---

# 7. Vue 3 + Tailwind

**Ключевой вопрос:**
Можно ли использовать Vue 3 и Tailwind?

**Ответ:**
Да. GameFace получает уже собранный HTML/CSS/JS. Tailwind на выходе — это обычный CSS.

**Что важно сказать:**

```text
GameFace — не полноценный Chrome.
Нужно использовать safe subset CSS.
```

**Безопасно использовать:**

```text
flex
absolute/fixed positioning
padding/margin
border-radius
opacity
transform
simple transitions
simple backgrounds
```

**Осторожно использовать:**

```text
CSS Grid
filter
backdrop-filter
mix-blend-mode
сложные SVG filters
сложные form controls
native select/range
```

---

# 8. Unity → UI: отправка данных

**Ключевой вопрос:**
Как Unity передаёт данные в GameFace?

**Ответ:**
Самый простой способ — события через `TriggerEvent`.

**C#:**

```csharp
view.View.TriggerEvent("HUD_OnHealthChanged", health, maxHealth);
```

**Vue:**

```ts
useEngineEvent<[number, number]>('HUD_OnHealthChanged', (health, maxHealth) => {
  hud.health = health
  hud.maxHealth = maxHealth
})
```

**Что объяснить:**
Для GameFace лучше передавать простые типы:

```text
number
string
bool
```

Со сложными C# объектами, `List<T>`, вложенными DTO иногда возникают проблемы. Для сложных данных безопаснее использовать JSON string.

---

# 9. UI → Unity: команды из интерфейса

**Ключевой вопрос:**
Как кнопка в UI вызывает код в Unity?

**Ответ:**
Vue вызывает `engine.trigger`, Unity принимает через `RegisterForEvent`.

**Vue:**

```ts
triggerEngine('HUD_OnButtonClick', 'attack')
```

**Unity:**

```csharp
view.NativeView.RegisterForEvent(
    "HUD_OnButtonClick",
    (Action<string>)OnHudButtonClick
);
```

**Что сказать:**
UI не должен напрямую менять игровое состояние. Он отправляет команду, а Unity решает, что делать.

**Пример:**

```text
Attack button → HUD_OnButtonClick("attack") → PlayerInputController.AttackFromUi()
```

---

# 10. `trigger` vs `call`

**Ключевой вопрос:**
Когда использовать `trigger`, а когда `call`?

**Ответ:**

```text
engine.trigger — событие без ответа
engine.call — запрос с ответом
```

**Примеры `trigger`:**

```text
Attack
Cancel
OpenInventory
CloseModal
RotatePreview
```

**Примеры `call`:**

```text
GetSettings
GetInventory
CanBuyItem
GetPlayerProfile
```

**Главный тезис:**
`trigger` — для действий.
`call` — для запросов, где UI ждёт ответ.

---

# 11. Data Binding

**Ключевой вопрос:**
Что такое официальный GameFace Data Binding?

**Ответ:**
Это механизм, где Unity создаёт C# модель, а HTML привязывается к её полям через `data-bind-*`.

**HTML:**

```html
<div data-bind-value="{{Nameplate.username}}">None</div>
```

**C#:**

```csharp
view.NativeView.CreateModel("Nameplate", model);
view.NativeView.UpdateWholeModel(model);
view.NativeView.SynchronizeModels();
```

**Когда полезно:**

```text
простые nameplates
статусы
числа
небольшие модели состояния
простые debug panels
```

**С Vue использовать осторожно:**
Vue и GameFace Data Binding оба управляют DOM. Лучше не смешивать их в одном и том же элементе без необходимости.

**Практичный вывод:**
Для Vue-проекта основной поток лучше строить так:

```text
Unity → TriggerEvent/JSON → Vue Store → Vue Components
```

А Data Binding использовать точечно.

---

# 12. Как организовать C# данные для HUD

**Ключевой вопрос:**
Как правильно держать HUD-состояние в Unity?

**Ответ:**
Через `HudState`, `HudStore`, `HudPresenter`, `GamefaceBridge`.

```text
Gameplay Systems
  ↓
HudStore
  ↓
HudPresenter
  ↓
GamefaceBridge
  ↓
Vue Store
```

**Что сказать:**
Не надо, чтобы каждый gameplay-класс напрямую вызывал GameFace.

**Хорошая схема:**

```text
PlayerHealth → event → HudStore.SetHealth()
HudPresenter → flush dirty state → GameFace
```

**Главный тезис:**
GameFace-интеграция должна быть отдельным presentation layer.

---

# 13. Combat log / event log

**Ключевой вопрос:**
Как делать лог игровых событий?

**Ответ:**
Лог — это поток событий. Его лучше делать через `TriggerEvent`, а не через Data Binding.

**Unity:**

```csharp
view.View.TriggerEvent("HUD_OnCombatLogEvent", json);
```

**Vue:**

```ts
events.value.push(JSON.parse(json))
```

**Что показать:**
Красивый список наподобие чата:

```text
12:01:10 ShadowRift dealt 99 damage to NovaByte
12:01:12 FrostViper healed EmberWolf
12:01:14 GhostLynx picked up Crystal
```

**Главный тезис:**
Data Binding — для состояния.
Event stream — для событий.

---

# 14. Minimap / radar

**Ключевой вопрос:**
Как рисовать мини-карту и путь игрока?

**Ответ:**
Unity считает координаты, GameFace визуализирует.

```text
World X/Z
  ↓
Normalize to 0..1
  ↓
TriggerEvent
  ↓
Vue SVG/Canvas
```

**Пример:**

```text
Terrain 100x100
Player at X=50, Z=50
Minimap x=0.5, y=0.5
```

**Про трек пути:**
Лучше не пересылать весь массив точек каждый кадр. Отправлять только новые точки:

```text
HUD_OnMinimapPositionChanged(x, y)
HUD_OnMinimapTrailPointAdded(x, y)
HUD_OnMinimapTrailCleared()
```

---

# 15. SVG vs Canvas vs RenderTexture

**Ключевой вопрос:**
Чем рисовать динамику в UI?

**Ответ:**

```text
SVG — удобно для простых линий, иконок, path
Canvas — лучше для частой дорисовки, например trail
RenderTexture — лучше для сложной карты или 3D-сцены
```

**Простой вывод:**

```text
маленький path на карте → SVG
часто обновляемый trail → Canvas
сложная 3D/2D карта → Unity RenderTexture
```

---

# 16. 3D-объекты в UI

**Ключевой вопрос:**
Можно ли показать 3D-персонажа в инвентаре?

**Ответ:**
GameFace сам не рендерит 3D-модель. 3D рендерит Unity.

**Схема:**

```text
3D Character
  ↓
Preview Camera
  ↓
RenderTexture / Cohtml Live View
  ↓
GameFace img / UI panel
```

**Vue:**

```html
<img src="coui://CharacterPreview">
```

**Что сказать:**
GameFace показывает результат рендера Unity, а не саму модель.

---

# 17. World-space UI и nameplates

**Ключевой вопрос:**
Как сделать никнейм над персонажем?

**Ответ:**
Для одного персонажа можно сделать world-space `CohtmlView` над головой.

```text
Player
  NameplateAnchor
    CohtmlView
    Billboard script
```

**Важно:**

```text
View должна смотреть в камеру
input лучше отключить
модель создаётся именно в этой CohtmlView
```

**Для множества персонажей:**
Если юнитов много, лучше один HUD View и screen-space позиции:

```text
Camera.WorldToScreenPoint(headPosition)
  ↓
Vue рисует nameplates absolute-positioned
```

**Главный тезис:**
Один-два world views — нормально.
Сотни world views — опасно для performance.

---

# 18. Input handling

**Ключевой вопрос:**
Как не сломать управление игроком?

**Ответ:**
HUD должен пропускать input там, где не нужны клики.

**CSS:**

```html
<main class="pointer-events-none">
  <button class="pointer-events-auto">Attack</button>
</main>
```

**Что сказать:**
Весь HUD не должен перехватывать мышь, иначе можно сломать камеру, стрельбу, выделение объектов.

**Главный тезис:**
Интерактивными должны быть только конкретные кнопки и панели.

---

# 19. Нативные HTML-контролы

**Ключевой вопрос:**
Можно ли использовать `input range`, `select`, `number`?

**Ответ:**
Лучше осторожно. В GameFace нативные form controls могут работать нестабильно.

**Проблемные элементы:**

```text
input type="range"
select / option
input type="number"
date/color/file inputs
```

**Лучше делать кастомно:**

```text
Slider → div + mouse events
Select → список кнопок
Toggle → button with active state
Radio → segmented buttons
```

**Главный тезис:**
Для игрового UI лучше кастомные контролы, а не browser-native controls.

---

# 20. Видео

**Ключевой вопрос:**
Можно ли показать видео?

**Ответ:**
Да, через `<video>`, но нужно следить за кодеками.

**Безопасный вариант:**

```text
WebM VP8/VP9 + Vorbis
или WebM без аудио
```

**Частая ошибка:**

```text
Unable to find decoder for A_OPUS
```

**Решение:**
Перекодировать аудио из Opus в Vorbis или удалить аудиодорожку.

**Главный тезис:**
Видео возможно, но его надо готовить под поддерживаемые кодеки.

---

# 21. Шрифты

**Ключевой вопрос:**
Почему GameFace ругается на `ui-sans-serif` или weight 600?

**Ответ:**
Tailwind использует системные font-family и веса, но GameFace лучше явно подключать шрифты.

**Правильно:**

```css
@font-face {
  font-family: "GameUISans";
  src: url("./fonts/Inter-SemiBold.woff2") format("woff2");
  font-weight: 600;
}
```

**Что сказать:**
Если используешь `font-semibold`, должен быть подключён font-face для `600`.

**Главный тезис:**
Все используемые font-weight должны иметь реальные файлы шрифтов.

---

# 22. Debugging

**Ключевой вопрос:**
Как дебажить GameFace UI?

**Ответ:**
Через несколько уровней:

```text
Unity Console
GameFace logs
GameFace DevTools / Inspector
DOM inspector
JS console
Network/resources
Unity Profiler
```

**Что показать на демо:**

```text
сломанный CSS
битый путь к картинке
JS error
неподключенный шрифт
неработающий data-bind
событие из Unity в UI
событие из UI в Unity
```

**Главный тезис:**
GameFace удобно дебажить как frontend, но надо помнить, что он работает внутри Unity.

---

# 23. Профилирование

**Ключевой вопрос:**
Как понять, где тормозит?

**Ответ:**
Смотреть нужно три слоя:

```text
Unity Profiler — CPU, GPU, Memory, GC, Rendering
GameFace DevTools — DOM, JS, CSS, resources
GameFace settings/logs — memory tracking, logs, caches
```

**Что искать в Unity Profiler:**

```text
CohtmlView.Update
CohtmlPluginManager.Update
RenderTexture allocations
Camera.Render
GC Alloc spikes
Texture memory
```

**Что искать в GameFace DevTools:**

```text
много DOM-элементов
частые layout updates
тяжёлые CSS effects
JS errors
лишние resource loads
```

**Главный тезис:**
Профилировать надо не “GameFace вообще”, а отдельно: data flow, DOM/CSS/JS, Unity rendering, memory.

---

# 24. Memory Tracking

**Ключевой вопрос:**
Как понять, сколько памяти потребляет GameFace?

**Ответ:**
В `Gameface → Configure Library` можно включить `Enable Memory Tracking`, но это не всегда даёт отдельное красивое окно. Обычно смотреть надо через:

```text
Unity Profiler → Memory
GameFace DevTools / Inspector
GameFace logs
resource statistics
```

**Практический тест:**

```text
1. Запустить сцену
2. Записать baseline memory
3. Открыть inventory/shop/settings
4. Закрыть
5. Повторить 10 раз
6. Проверить, стабилизируется ли память
```

**Что может жрать память:**

```text
большие картинки
шрифты
видео
RenderTexture
LiveView
много View
internal caches
DOM
```

**Главный тезис:**
Высокая память не всегда утечка. Иногда это кэш. Но постоянный рост после каждого открытия окна — проблема.

---

# 25. Performance bottlenecks

**Ключевой вопрос:**
Какие типичные узкие места?

**Ответ:**

```text
слишком частые Unity → UI события
большие JSON/DTO каждый кадр
огромный DOM
длинные списки без лимита
тяжёлый CSS
много CohtmlView
большие RenderTexture
видео
LiveView
неотключённые preview cameras
```

**Правила оптимизации:**

```text
слать только изменения
не слать данные каждый frame
использовать throttle/debounce
ограничивать списки
не держать скрытые тяжёлые View активными
анимировать transform/opacity вместо width/height/top/left
отключать камеры, когда UI закрыт
```

---

# 26. Как принимать архитектурные решения

**Ключевой вопрос:**
Что чем делать?

**Ответ:**

```text
HUD → один CohtmlView + Vue components
Inventory/Menu → отдельный fullscreen View
Combat log → TriggerEvent stream
Settings → Vue local state + JSON в Unity
Minimap → SVG/Canvas
3D preview → Unity Camera + RenderTexture/LiveView
Nameplate одного персонажа → world-space View
Много nameplates → один HUD View + screen positions
Simple state → Data Binding можно
Complex Vue UI → Vue Store лучше
```

---

# 27. Ограничения и риски

**Ключевой вопрос:**
Где GameFace может быть плохим выбором?

**Ответ:**

```text
очень простой UI
команда не знает frontend
много low-end devices
нет времени на pipeline
нужна максимальная нативная интеграция с Unity UI
очень много world-space UI объектов
```

**Что сказать честно:**
GameFace даёт мощь frontend-подхода, но требует дисциплины:

```text
data flow
performance
resource management
fonts
CSS subset
build pipeline
debugging
```

---

# 28. Что показать в живом демо

Я бы сделал 5 коротких сцен/блоков.

## Demo 1 — HUD

```text
HP
Coins
Attack button
Unity → UI health update
UI → Unity attack command
```

## Demo 2 — Combat log

```text
Unity генерирует события
Vue рисует лог как чат
анимация появления строк
```

## Demo 3 — Settings modal

```text
кастомные sliders/toggles
UI отправляет JSON настроек в Unity
```

## Demo 4 — Minimap

```text
Unity нормализует координаты
Vue рисует точку и trail
SVG/Canvas explanation
```

## Demo 5 — 3D preview

```text
Unity camera renders character
RenderTexture / LiveView
GameFace показывает preview в inventory
```

---

# 29. Финальный вывод для команды

**Главная мысль:**

```text
GameFace — это способ строить сложный игровой UI как frontend-приложение.
Unity остаётся источником игрового состояния.
GameFace/Vue отвечает за визуальный слой.
Между ними нужен аккуратный Bridge/Presenter.
```

**Что команда должна запомнить:**

```text
1. Не пихать GameFace напрямую в gameplay-код.
2. Не делать отдельную View на каждую кнопку.
3. Не слать огромные данные каждый кадр.
4. Не считать GameFace полноценным Chrome.
5. Обязательно профилировать Unity + GameFace вместе.
6. Vue/Tailwind использовать можно, но аккуратно.
7. Для production нужна чёткая архитектура data flow.
```
