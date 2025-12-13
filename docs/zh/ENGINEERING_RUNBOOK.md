# 工程维护手册 (Engineering Runbook)

**文档定位**：本项目核心架构的“操作说明书”。
**适用场景**：日常开发、新人入职、紧急故障排查。
**核心原则**：先说怎么做 (Action)，再说为什么 (Context)。

---

## 🔴 紧急速查 (Quick Actions)

| 场景 (Scenario) | 动作 (Action) | 详细文档 |
| :--- | :--- | :--- |
| **我新写了一个数据类，存档读不出来** | 给类加上 `[BsonDeserializerRegister]` 标签，按 `Ctrl+S`。 | [Bson AOT 指南](bson_aot_maintenance_guide_cn.md) |
| **我需要加一个新的角色属性 (如: 护甲)** | **严禁**直接改 `eNumericType`！去 `AttributeSchema.json` 添加字段，然后运行生成菜单。 | [魔数陷阱分析](magic_number_fragility_lesson_cn.md) |
| **手机包启动崩溃 / 报 AOT 错误** | 检查 `RoslynAnalyzers` 是否编译正常，重新运行 `./build_bson_generator.sh`。 | [Bson AOT 指南](bson_aot_maintenance_guide_cn.md) |

---

## 1. 数据序列化系统 (BSON AOT)

### 🟢 日常开发：添加新数据类型
当你创建新的 `SkillData`, `BuffData`, `ItemData` 时：

1.  **操作**：在类定义上方添加 `[BsonDeserializerRegister]`。
    ```csharp
    [BsonDeserializerRegister] // <--- 必须加这个！
    public class MyNewData : NodeDataBase { ... }
    ```
2.  **验证**：保存代码。Unity 编译完成后，该类型即自动注册。
3.  **禁止**：不要手动去改 `GeneratedTypeRegistry.cs`，那是机器生成的。

### 🟡 维护操作：修改生成器逻辑
如果你发现生成的代码格式不对，或者需要支持新的功能：

1.  **修改**：编辑 `RoslynAnalyzers/BsonMapGenerator.cs`。
2.  **构建**：在终端运行构建脚本。
    ```bash
    cd RoslynAnalyzers
    ./build_bson_generator.sh
    ```
3.  **生效**：切回 Unity，等待编译结束。

---

## 2. 数值属性系统 (Attribute Schema)

### 🟢 日常开发：添加/修改属性
当你需要增加一个新的属性（例如 `CriticalChance` 暴击率）时：

1.  **操作**：打开 `Assets/Config/AttributeSchema.json` (假设路径)。
2.  **编辑**：
    ```json
    {
      "Name": "CriticalChance",
      "HasBase": true,   // 需要基础值
      "HasAdd": true,    // 需要装备加成
      "HasMult": false   // 不需要百分比加成
    }
    ```
3.  **生成**：点击 Unity 菜单栏 `Tools -> Generate Numeric Types`。
4.  **结果**：`eNumericType.cs` 会自动更新，分配新的 ID。

### 🔴 红色警报：绝对禁止的操作
*   ❌ **禁止** 手动修改 `eNumericType.cs` 文件。你的修改会被下次生成覆盖。
*   ❌ **禁止** 在代码里写死 `(int)type * 10 + 1` 这种逻辑。请使用 `AttributeUtils.GetBaseType(type)` 等辅助函数。

---

## 3. 常见故障排查 (Troubleshooting)

### Q: 生成的代码报错 "Duplicate Definition"
*   **原因**：旧的手动生成文件 (`TypeRegistryGenerator.cs`) 没删干净。
*   **解决**：全盘搜索 `GeneratedTypeRegistry`，只保留 `RoslynAnalyzers` 生成的那份（它在内存里，或者在 `Temp` 目录），删除 Assets 目录下的物理文件。

### Q: 属性 ID 错乱 / 数值不对
*   **原因**：有人绕过 Schema 直接改了枚举，或者多人协作时 Schema 冲突。
*   **解决**：
    1.  回滚 `eNumericType.cs`。
    2.  解决 `AttributeSchema.json` 的 Git 冲突。
    3.  重新点击 `Generate Numeric Types`。

---

> **架构师寄语**：
> 人的记忆是不可靠的，流程和工具才是可靠的。
> 遇到问题先看本手册，不要凭直觉“硬改”代码。
