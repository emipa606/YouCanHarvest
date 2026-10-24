# GitHub Copilot Instructions for RimWorld Mod: You Can Harvest! (Continued)

## Mod Overview and Purpose
You Can Harvest! (Continued) is a mod for the game RimWorld that provides users with the ability to set alerts when specific plants are ready for harvest. This mod is an update to 民火's mod, enhancing its features by fixing mod-settings with icons and list-sorting, adding an option to ignore plants already designated for harvest, and allowing users to specify which plants should receive harvest alerts in mod settings.

## Key Features and Systems
### Plant Harvest Alerts
- **Notification System**: Users can receive alerts when specific plants are ready for harvest.
- **Customizable Settings**: Users can specify which plants should receive harvest alerts in the mod settings.
- **Compatibility**: Works with plants added by other mods.

### Mod Settings Enhancements
- **Icons and List-Sorting**: Fixed the mod-settings interface to include icons and list-sorting for better usability.
- **Ignore Designated Plants**: Added an option to ignore plants that have already been designated for harvest.

### Language Support
- **Multilingual**: The mod supports multiple languages including English, Japanese, Chinese Simplified, and Chinese Traditional. Users can contribute translations by providing links to translation files.

## Coding Patterns and Conventions
### C# Code
- **Class Structure**: The mod uses a class-based approach for managing alerts and settings.
- **Methods**: Methods are used to handle specific tasks such as exposing data, resolving item definitions, and drawing UI elements.

### XML Integration
- **Mod Metadata**: The mod's metadata is stored in the `About.xml` file, which provides information about the mod's name, author, and version.

## Harmony Patching
Harmony patching is not explicitly mentioned in the provided summary, but it is a common practice in RimWorld modding. Harmony allows mods to modify existing game code without altering the original game files, enabling developers to add new features or fix bugs.

## Suggestions for Copilot
### XML Integration
- **Provide XML Structure**: Copilot should understand the structure of XML files used in RimWorld modding, such as `About.xml`, and be able to generate or modify these files as needed.

### Harmony Patching
- **Generate Patch Code**: Copilot should be able to generate Harmony patches for modifying existing game code, ensuring compatibility with the latest game updates.

### C# Code
- **Class and Method Generation**: Copilot should be able to generate C# classes and methods for managing alerts, settings, and UI elements.
- **UI Element Drawing**: Copilot should be able to generate code for drawing UI elements, such as buttons and lists, in the mod settings window.

### Language Support
- **Translation File Handling**: Copilot should be able to generate code for handling translation files, allowing users to contribute translations easily.

By following these guidelines, Copilot can be effectively utilized to enhance the development of the You Can Harvest! (Continued) mod, ensuring that new features are added efficiently and that existing functionality is maintained.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

