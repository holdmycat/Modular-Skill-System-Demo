# Modular Skill System Demo (SLG Architecture Prototype)

> **针对 SLG 游戏开发痛点的架构解决方案**
> *展示如何通过工业化管线将技能制作效率提升 80%，并实现确定性的战斗逻辑同步。*

---

## 核心价值 (Why This Matters ?)

本工程展示了一个**主程级 (Tech Lead)** 的架构方案，旨在解决 SLG 项目中两个最核心的难题：

1. **战斗逻辑的确定性与同步**：如何在分离逻辑与表现的前提下，实现复杂的战斗指令同步？
2. **内容生产的工业化**：如何让策划在不依赖程序的情况下，快速量产数百种技能与数值变种？

### 关键成果数据

* **效率提升 80%**: 通过 NPData 可视化管线，技能变种制作时间从 **小时级** 缩短至 **秒级** (Copy & Tweak)。
* **0 代码依赖**: 策划可独立完成技能配置、行为树编排及热更测试。
* **100% 逻辑分离**: 实现了“双平行世界”架构，服务端逻辑与客户端表现完全解耦。

---

## 1. 架构设计：双平行世界与 Zenject (Architecture)

针对 SLG 战斗的高频交互与强同步需求，本项目搭建了一套基于 **Zenject** 的**双平行世界 (Dual-World)** 架构。

### 核心特性

* **严格分层 (Clean Architecture)**:
  * **Data Layer (纯数据)**: `NumericComponent`, `ConfigData`。无逻辑，易序列化。
  * **Logic Layer (纯逻辑)**: `ServerCommander`, `CombatSystem`。运行在独立世界，仅处理数值与状态。
  * **View Layer (纯表现)**: `ClientCommander`, `UnitView`。仅负责渲染与动画播放。
* **双世界模拟 (Server vs Client)**
  * 利用 Zenject 的 `SubContainer` 和 `ProjectContext`，在单机可同时模拟 Server 和 Client 两个环境。
  * **Command & RPC**: 封装了统一的指令层。Client 发送 Command，Server 处理后通过 RPC 广播状态快照。这确保了未来扩展到真实网络同步时，底层逻辑无需重写。
* **依赖注入 (DI)**: 全面使用 Zenject 管理单例、工厂与对象池，避免了“Manager 满天飞”的耦合地狱。

* **依赖注入 (DI)**: 全面使用 Zenject 管理单例、工厂与对象池，避免了“Manager 满天飞”的耦合地狱。

## 3. UI 架构：响应式 MVVM 框架 (UI Architecture)

针对 SLG 复杂的多级界面与海量数据展示，构建了基于 **MVVM + UniTask + Zenject + SignalBus** 的现代 UI 框架。

* **状态同步难题的终结**:
  * **ViewModel (Logic)**: 负责业务逻辑与状态持有，通过 `SignalBus` 驱动视图更新。
  * **Context (Data)**: 区分“服务器数据”(`ShowcaseContext`)与“客户端状态”(`ClientRuntimeContext`)，确保数据流向清晰。
  * **Nested UI 支持**: 实现了泛型基类 `BaseWindow<T>` 与 `BaseWidget<T>`，自动化处理子界面的依赖注入与生命周期管理。
* **性能优化**: 全异步加载 (UniTask)，告别卡顿；严格的 View/Logic 分离使得 UI 逻辑可独立单元测试。

## 4. 工具链：可视化技能生产管线 (Toolchain)

基于 **NPData Behaviour** 框架，实现了一套“所见即所得”的技能编辑器。

### 工业化生产流程

1. **可视化编排**: 策划在节点编辑器中组合原子行为（如“造成伤害”、“播放特效”、“施加Buff”）。
2. **变种量产 (Copy & Tweak)**:
    * **传统痛点**: 新技能通常需要程序写新脚本，或者复制一份代码改参数。
    * **本方案**: 策划只需 Duplicate 现有节点图，修改数值（伤害系数、范围），点击 "Compile" 即可。
    * **热更支持**: 编译生成二进制数据 (`.bytes`)，支持运行时热重载，无需重启游戏即可验证数值。
3. **数据驱动**: 所有的属性（攻防血）和行为逻辑（AI 决策）皆为数据。程序仅负责维护原子节点的逻辑（Processor），彻底从重复的配置工作中解放。

---

## 3. 面向技术负责人的快速评估 (Technical Highlights)

如果您正在寻找能驾驭复杂 SLG 项目的技术负责人，本工程展示了以下关键能力：

* **架构规划能力**: 能够设计解耦、可测试、易扩展的系统架构（Zenject, MVVM, Command Pattern）。
* **工具与管线意识**: 深刻理解“工具提升效率”的价值，能够开发针对性的编辑器工具（NodeEditor）来赋能策划团队。
* **性能敏感度**: 采用二进制序列化 (MongoDB.Bson) 替代低效的 JSON/XML；使用对象池 (MemoryPool) 管理战斗单位；UniTask 零 GC 异步处理。
* **代码质量**: 严格的命名规范，清晰的目录结构，以及对 SOLID 原则的坚持。

### 快速体验

1. 打开 `Assets/Scenes/ShowcaseScene.unity`。
2. 运行游戏，体验基于指令的单位生成与战斗。
3. 打开 `Assets/Resources/SkillData/AllSkillAttributesDataGraph`，尝试修改数值并重新编译，体验热更流程。

---

> *此工程不仅是一个 Demo，更是我在 SLG 领域多年积累的最佳实践缩影。期待能将这套高效、稳健的工程体系带入您的团队。*
