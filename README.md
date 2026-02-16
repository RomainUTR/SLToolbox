# 🛠 SL Toolbox (Better Unity)

A collection of **"Quality of Life"** tools for Unity, designed to speed up your editor workflow, clean up your inspectors, and simplify daily scripting.

## 🚀 Installation

You have two options to install SL Toolbox:

### Option A: The "Pro" Way (Recommended)
*Keeps your Assets folder clean and allows easy updates via Package Manager.*

1. Open Unity **Package Manager** (Window > Package Manager).
2. Click **+** > **Add package from git URL...**
3. Paste: `https://github.com/RomainUTR/SLToolbox.git`
4. Click **Add**.

### Option B: The Classic Way (.unitypackage)
*Downloads the files directly into your project's Assets folder.*

1. Go to the **[Releases Page](https://github.com/RomainUTR/SLToolbox/releases/latest)**.
2. Download the latest `.unitypackage` file.
3. Double-click it to import it into your project.

---

## 💎 What's Inside?

### ✨ Inspector Magic (Attributes)
Turn your inspector into a powerful dashboard without writing custom editors. Just add `using RomainUTR.SLToolbox;`.

#### 🎨 Visuals & Organization
* **`[SLTitle("Title", "Color")]`**: Adds a colored header and separator to organize your variables.
* **`[SLInfoBox("Message")]`**: Displays a help box (Info, Warning, or Error) above a field.
* **`[SLInline]`**: Edit **ScriptableObjects** or other references directly inside the current inspector (nested view).

#### 🛡️ Safety & Debug
* **`[SLReadOnly]`**: Shows a variable in the inspector but prevents editing (great for debug values).
* **`[SLRequired]`**: Turns the field **RED** if the reference is missing. Stops `NullReferenceException` before they happen!
* **`[SLButton("Label")]`**: Adds a button to trigger any function directly from the editor.

#### 🧠 Logic & Dropdowns
* **`[SLDropdown("MethodName")]`**: Creates a dropdown menu populated by a function (supports `int` and `string`).
* **`[SLCallback("MethodName")]`**: Automatically calls a function whenever the variable value is changed in the inspector.

**Example:**
```csharp
[SLTitle("Combat Settings", "red")]
[SLRequired]
public GameObject weaponPrefab;

[SLInfoBox("Select a weapon type")]
[SLDropdown("GetWeaponTypes")]
public string currentWeapon;

[SLButton("Test Attack")] 
public void PerformAttack() { ... }

[SLReadOnly]
public int currentHP;
```

### 🔗 Robust Scene Management

Stop using strings for Scene Management! They break when you rename assets.

`SceneReference`: A custom type that stores the Asset in the editor *(safe renaming)* and the **Path at runtime**.
**Auto-Validation:** If the scene is not in the Build Settings, a **"Fix" button** appears automatically in the inspector to add it in one click.

```csharp
public SceneReference nextLevel; // Drag & Drop the scene file here!

void Load() {
    // Works like magic, converts to string automatically
    SceneManager.LoadScene(nextLevel); 
}
```

### ⚡ Editor Tools (Work Faster)
* **`Scene Switcher`:** A quick popup window to switch scenes in 2 seconds. Trigger it by clicking the scene name in the Hierarchy.

* **`Smart Grouper (Ctrl+G)`:** Groups selected objects under a new parent positioned exactly at the center of your selection (Barycenter). Fully supports Undo.

* **`Drop To Ground (Alt+D)`:** Instantly drops selected objects to the nearest surface below them.

* **`High-Res Screenshot Tool (Ctrl+Alt+K)`:** Capture crispy, high-resolution screenshots directly from the Game View. (Tools > SL Toolbox > Take High-Res Screenshot).

* **`Scene Bootloader`:** Forces the game to start from Scene 0 (e.g., Main Menu) even if you are currently working inside a Level scene. (Tools > SL Toolbox > Play From First Scene).

### 📜 Runtime Extensions (Code Faster)

* **`.GetRandom()`:** No need to type Random.Range(0, list.Count) anymore.

```csharp
using RomainUTR.SLToolbox;
// ...
var myItem = myList.GetRandom();
```

## ⚠️ The "Deal" (Read this!)
I'm sharing this tool to help out, but keep in mind I am a student just like you:

**It's Robust:** I used Assembly Definitions. This means even if your own project has compilation errors, the Toolbox will keep working to help you navigate scenes.

**No Warranty:** Always make a Git commit before installing new packages. I am not responsible if your project catches fire (though I really hope it won't).

## Made with ❤️ by Romain UTR
