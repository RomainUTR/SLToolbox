# 🛠 SL Toolbox (Better Unity)
A collection of *"Quality of Life"* tools for **Unity**, designed to speed up your editor workflow and simplify daily scripting.

## 🚀 Installation (Clean & Isolated)
This tool is packaged for the Unity Package Manager. It stays isolated in your Packages folder and keeps your Assets clean.

In Unity, open the Package Manager **(Window > Package Manager)**.

Click the + button in the top left corner.

Select **"Add package from git URL..."**.

Paste the repository URL: *https://github.com/RomainUTR/SLToolbox.git*

Click Add.

## 💎 What's Inside?
### ⚡ Editor Tools (Work Faster)
**Scene Switcher:** A quick popup window to switch scenes in 2 seconds. You can trigger it by clicking the scene name in the Hierarchy.

**Smart Grouper (Ctrl+G):** Select multiple objects and press Ctrl+G. It groups them cleanly under a new parent object positioned exactly at the center of your selection (Barycenter). Fully supports Undo.

**Scene Bootloader:** Enable it via Tools > SL Toolbox > Play From First Scene. Forces the game to start from your Scene 0 (e.g., Main Menu) even if you are currently working inside a Level scene.

**High-Res Screenshot Tool (Ctrl+Alt+K):** Capture crispy, high-resolution screenshots directly from the editor game view, perfect for your portfolio or marketing materials. Access it via the menu Tools > SL Toolbox > Take High-Res Screenshot.

**Drop To Ground (Alt+D):** Drop selected Game Object(s) to the ground of the scene. Fully supports Undo.

### 📜 Runtime Extensions (Code Faster)
**.GetRandom() Extension:** No need to type Random.Range(0, list.Count) anymore.

Usage: var myItem = myList.GetRandom();

__Note: Don't forget to add using **RomainUTR.SLToolbox.Runtime;** at the top of your script.__

## ⚠️ The "Deal" (Read this!)
I'm sharing this tool to help out, but keep in mind I am a student just like you:

**It's Robust:** I used Assembly Definitions. This means even if your own project has compilation errors, my Toolbox will keep working to help you navigate scenes.

**No Warranty:** Always make a Git commit before installing new packages. I am not responsible if your project catches fire (though I really hope it won't).

*Made with ❤️ by Romain UTR*
