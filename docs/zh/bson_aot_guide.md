# BSON AOT 与 Source Generator 维护指南

**目标读者：** 游戏逻辑程序员、技术美术 (TA)、构建工程师
**文档目的：** 阐述如何在 IL2CPP 环境下安全地使用 BSON 序列化系统，以及如何维护 Source Generator 工作流。

---

## 1. 核心原理：为什么要这么做？

### 问题：IL2CPP 与反射 (Reflection)
在 Unity 的 IL2CPP (AOT) 后端中，运行时的反射（例如 `MakeGenericMethod`）是非常危险的。因为编译器会进行**代码裁剪 (Code Stripping)**，如果它发现某些泛型组合在编译时没有被显式调用，它就会把这些代码删掉。
结果就是：你在编辑器里跑得好好的，打包到手机上就会**崩溃**。

### 解决方案：Source Generators
我们引入了 **Roslyn Source Generator** (`BsonMapGenerator`)，它运行在**编译时 (Compile Time)**。
*   它会自动扫描所有标记了 `[BsonDeserializerRegister]` 的类。
*   它会自动生成一个静态类 `GeneratedTypeRegistry`。
*   这确保了所有必要的 BSON 映射都在编译时被显式注册，彻底杜绝了代码裁剪导致的崩溃问题。

---

## 2. 游戏逻辑程序员：如何添加新数据？

**场景：** 你写了一个新的技能数据类，并且希望它能被保存/读取。

### 第一步：添加标签
只需要在你的类上添加 `[BsonDeserializerRegister]` 标签即可。

```csharp
using Ebonor.DataCtrl;

[BsonDeserializerRegister] // <--- 只需要加这一行
public class MyNewSkillData : UnitAttributesNodeDataBase
{
    public int ManaCost;
}
```

### 第二步：完成了！
*   **不需要** 点击任何菜单按钮。
*   **不需要** 手动修改任何注册表文件。
*   只需要按下 `Ctrl+S` (保存)。Source Generator 会在后台自动运行，瞬间更新注册逻辑。

---

## 3. 引擎/工具程序员：如何维护生成器？

**场景：** 你需要修改生成器的逻辑（例如支持新的属性，或者修改生成代码的格式）。

### 源码位置
生成器的源码位于工程根目录：
`ProjectRoot/RoslynAnalyzers/`

### 如何修改并重新编译
由于 Generator 是一个被 Unity 加载的已编译 DLL，如果你修改了它的 C# 源码，必须重新编译它。

**方法 A：一键构建脚本 (Mac/Linux)**
1.  在终端中打开 `ProjectRoot/RoslynAnalyzers/` 目录。
2.  运行构建脚本：
    ```bash
    ./build_bson_generator.sh
    ```
3.  切回 Unity。DLL 会自动更新并重新编译。

**方法 B：手动构建 (Windows/IDE)**
1.  使用 Rider 或 Visual Studio 打开 `RoslynAnalyzers/BsonGenerator.csproj`。
2.  选择 **Release** 模式编译项目。
3.  将输出的 DLL 复制到 Unity 工程中：
    *   源文件：`RoslynAnalyzers/bin/Release/netstandard2.0/BsonGenerator.dll`
    *   目标位置：`Assets/Plugins/SourceGenerators/BsonGenerator/`

---

## 4. 常见问题排查 (Troubleshooting)

### Q: "我新写的类没有被保存下来！"
*   **检查：** 你加了 `[BsonDeserializerRegister]` 标签吗？
*   **检查：** 这个类是 `public` 的吗？
*   **检查：** 控制台有没有关于 `BsonGenerator` 的报错？

### Q: "报错说 'Multiple definitions of GeneratedTypeRegistry' (重复定义)。"
*   **原因：** 你可能没有删除旧的手动生成器文件。
*   **解决：** 请删除 `Assets/Scripts/DataCtrl/Editor/TypeRegistryGenerator.cs` 以及 `Assets/Scripts/DataCtrl/Generated/GeneratedTypeRegistry.cs`。

### Q: "Unity 报错说找不到 'System.Collections.Immutable' 或其他 DLL。"
*   **解决：** 确保 Unity 中的 `BsonGenerator.dll` 的 **Asset Labels** 已经设置为 `RoslynAnalyzer`，并且 **Platforms** 只勾选了 **Editor**。

---

## 5. 底层机制 (Under the Hood)

1.  **输入：** `BsonMapGenerator.cs` 扫描代码语法树，寻找所有带有 `[BsonDeserializerRegister]` 的类。
2.  **输出：** 它在内存中生成如下的 C# 代码字符串：
    ```csharp
    namespace Ebonor.DataCtrl {
        public static class GeneratedTypeRegistry {
            public static void RegisterAllBsonClassMaps() {
                BsonClassMap.LookupClassMap(typeof(MyNewSkillData));
                // ... 其他类 ...
            }
        }
    }
    ```
3.  **执行：** `DataCtrl.cs` 在游戏启动时调用 `GeneratedTypeRegistry.RegisterAllBsonClassMaps()`，完成注册。
