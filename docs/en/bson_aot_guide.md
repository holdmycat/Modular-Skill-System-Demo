# BSON AOT & Source Generator Maintenance Guide

**Target Audience:** Gameplay Programmers, Technical Artists, Build Engineers
**Purpose:** Explain how to work with the BSON serialization system in an IL2CPP-safe manner using Source Generators.

---

## 1. The "Why": Understanding the Architecture

### The Problem: IL2CPP & Reflection
In Unity's IL2CPP (AOT) backend, runtime reflection (e.g., `MakeGenericMethod`) is dangerous because the compiler strips unused code. If you try to create a generic method at runtime that wasn't compiled, the game crashes.

### The Solution: Source Generators
We use a **Roslyn Source Generator** (`BsonMapGenerator`) that runs **at compile time**.
*   It scans your code for classes marked with `[BsonDeserializerRegister]`.
*   It automatically generates a static class `GeneratedTypeRegistry`.
*   This ensures all necessary BSON maps are registered statically, preventing code stripping and runtime crashes.

---

## 2. For Gameplay Programmers: How to Add New Data

**Scenario:** You created a new Skill Data class and want it to be savable/loadable.

### Step 1: Add the Attribute
Simply add `[BsonDeserializerRegister]` to your class.

```csharp
using Ebonor.DataCtrl;

[BsonDeserializerRegister] // <--- THIS IS ALL YOU NEED
public class MyNewSkillData : UnitAttributesNodeDataBase
{
    public int ManaCost;
}
```

### Step 2: Done!
*   **Do not** run any menu commands.
*   **Do not** edit any registry files manually.
*   Just press `Ctrl+S` (Save). The Source Generator runs in the background and updates the registration logic instantly.

---

## 3. For Engine/Tools Programmers: Maintaining the Generator

**Scenario:** You need to modify the generator logic (e.g., to support a new attribute or change the generated code format).

### Location
The generator source code is located in:
`ProjectRoot/RoslynAnalyzers/`

### How to Modify & Rebuild
Since the Generator is a compiled DLL loaded by Unity, you must rebuild it if you change its C# source.

**Option A: One-Click Build (Mac/Linux)**
1.  Open Terminal at `ProjectRoot/RoslynAnalyzers/`.
2.  Run the build script:
    ```bash
    ./build_bson_generator.sh
    ```
3.  Switch back to Unity. The DLL will update and recompile.

**Option B: Manual Build (Windows/IDE)**
1.  Open `RoslynAnalyzers/BsonGenerator.csproj` in Rider or Visual Studio.
2.  Build the project in **Release** configuration.
3.  Copy the output DLL:
    *   From: `RoslynAnalyzers/bin/Release/netstandard2.0/BsonGenerator.dll`
    *   To: `Assets/Plugins/SourceGenerators/BsonGenerator/`

---

## 4. Troubleshooting

### Q: "My new class isn't being saved!"
*   **Check:** Did you add `[BsonDeserializerRegister]`?
*   **Check:** Is the class `public`?
*   **Check:** Are there any console errors related to `BsonGenerator`?

### Q: "I get a 'Multiple definitions of GeneratedTypeRegistry' error."
*   **Cause:** You might still have the old manual generator file (`TypeRegistryGenerator.cs`) or its output.
*   **Fix:** Delete `Assets/Scripts/DataCtrl/Editor/TypeRegistryGenerator.cs` and `Assets/Scripts/DataCtrl/Generated/GeneratedTypeRegistry.cs`.

### Q: "Unity complains about 'System.Collections.Immutable' or other DLLs."
*   **Fix:** Ensure the `BsonGenerator.dll` in Unity has its **Asset Labels** set to `RoslynAnalyzer` and is only enabled for **Editor** platform.

---

## 5. Under the Hood (How it works)

1.  **Input:** `BsonMapGenerator.cs` scans syntax trees for `[BsonDeserializerRegister]`.
2.  **Output:** It generates a string in memory:
    ```csharp
    namespace Ebonor.DataCtrl {
        public static class GeneratedTypeRegistry {
            public static void RegisterAllBsonClassMaps() {
                BsonClassMap.LookupClassMap(typeof(MyNewSkillData));
                // ... others ...
            }
        }
    }
    ```
3.  **Execution:** `DataCtrl.cs` calls `GeneratedTypeRegistry.RegisterAllBsonClassMaps()` at startup.
