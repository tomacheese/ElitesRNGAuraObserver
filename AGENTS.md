# AGENTS.md

## Language

- Deliverables are written in **English**: commit messages (Conventional Commits), PR / Issue titles and bodies, review comments, code comments and XML doc comments, and documentation.
- Exceptions: `docs/ja/` and `README-ja.md` (Japanese user docs, kept in sync with the English ones), localized UI strings, quotations, technical identifiers, generated files and external source text.
- Much of the existing code still has Japanese XML doc / inline comments. Do not bulk-translate them; write new or edited comments in English and translate a comment when you touch it.

## Overview

Elite's RNG Aura Observer is a Windows desktop app (C# / WinForms, .NET 9) that
watches VRChat log files and, when the player obtains an Aura in the VRChat world
"Elite's RNG Land", sends a Windows toast notification and a Discord webhook
message. It runs in the background from the task tray and ships with a separate
auto-updater. Windows-only.

## Solution layout

Two projects in `ElitesRNGAuraObserver.sln`:

- `ElitesRNGAuraObserver/` — the main tray application (`WinExe`).
  - `Program.cs` — entry point: single-instance mutex, exception handlers, update check, then `Application.Run(new TrayIcon())`.
  - `Core/AuraObserverController.cs` — wires `LogWatcher` to the detection services.
  - `Core/VRChat/LogWatcher.cs` — polls the newest `output_log_*.txt` every 1s and raises `OnNewLogLine`.
  - `Core/VRChat/AuthenticatedDetectionService.cs`, `Core/Aura/NewAuraDetectionService.cs` — parse log lines (regex) into events.
  - `Core/Notification/UwpNotificationService.cs`, `DiscordNotificationService.cs` — toast and webhook delivery.
  - `Core/Config/` — `AppConfig` (config.json load/save), `ConfigData`, `RegistryManager` (startup registration).
  - `Core/Aura/` — `Aura` record, `AuraCategory` enum. `Core/Json/JsonData.cs` is the catalog loader. Aura catalog is `Resources/Auras.json` (embedded), overridable by a local `Auras.json`.
  - `Core/Updater/` — checks GitHub releases and launches `ElitesRNGAuraObserver.Updater.exe` (skip with `--skip-update`; `--debug` opens a console).
  - `UI/` — `SettingsForm`, `TrayIcon`, custom controls.
- `ElitesRNGAuraObserver.Updater/` — standalone self-contained (`win-x64`) console updater. Downloads the latest GitHub release asset, verifies its sha256 digest, closes (then kills) the running app, extracts, and relaunches. It deliberately shares no code with the main app, so `SemanticVersion`, `ReleaseInfo` and `GitHubReleaseService` exist in both projects; keep the copies in sync.

## Development commands

- `dotnet restore ElitesRNGAuraObserver.sln` — restore packages.
- `dotnet build ElitesRNGAuraObserver.sln /p:Configuration=Release` — build.
- `dotnet publish ElitesRNGAuraObserver.sln -p:PublishProfile=Publish` — publish single-file output to `bin/Publish/`.
- `dotnet format ElitesRNGAuraObserver.sln --verify-no-changes --severity warn` — style check. CI runs this exact command and fails on any diff; run it before committing.

CI builds on Windows (`net9.0-windows10.0.17763.0`, WinForms). On Linux, `build` works with `EnableWindowsTargeting=true` (env var or `-p:`), but `dotnet format` reports end-of-line errors for LF checkouts because `.editorconfig` requires CRLF; use `dotnet format analyzers --verify-no-changes --severity warn --no-restore` there.

## Testing

There is no automated test project. Verify changes manually: build, run the app,
and confirm detection/notification against real or sample VRChat logs. The
Aura-detection regex in `NewAuraDetectionService.cs` depends on VRChat's exact log
format — when changing it, test against an actual `output_log_*.txt` line.

## Coding conventions

- StyleCop.Analyzers is enabled with `stylecop.json`. Notably `documentPrivateElements`/`documentPrivateFields` are true, so **every** type and member — including `private` ones — needs an XML doc comment, or the style check fails.
- 4-space indentation, spaces (no tabs). File-scoped namespaces. `using` directives outside the namespace, `System.*` first. Newline required at end of file.
- `Nullable` and `ImplicitUsings` are enabled; respect nullable annotations.
- Keep `<param>` / `<returns>` tags in sync with the signature (StyleCop's doc rules check for a missing doc comment, not for stale tags).
- Wrap notification / network calls in try/catch and log failures via `Console.WriteLine` rather than letting them crash the watcher loop, as the Discord call in `AuraObserverController` does. The toast call is not wrapped today.

## Configuration & runtime data

- User config is `config.json` under `%LocalAppData%\tomacheese\ElitesRNGAuraObserver`. An optional `config.path` file next to the exe redirects the config directory.
- `ConfigData` holds the Discord webhook URL, toast on/off, VRChat log dir, and Auras.json dir. The webhook URL is a user secret — never log it or commit a real value.
- `RegistryManager.EnsureStartupRegistration()` writes a Windows registry key for launch-at-startup.

## CI / release

- `.github/workflows/ci.yml` — on push/PR to `main`/`master`: restore, build, publish, upload artifacts, then the `dotnet format` style check.
- `.github/workflows/release.yml` — on push to `main`/`master`: `mathieudutour/github-tag-action` bumps the version from Conventional Commit prefixes, then builds and publishes a GitHub Release zip.
- Commit messages follow Conventional Commits (`feat:`, `fix:`, `chore:`, …); the prefix drives the release bump, so keep it accurate.

## Documentation

- User-facing docs live in `docs/en/` and `docs/ja/`; `README.md` / `README-ja.md` are the entry points. Update both language variants when changing user-facing behavior (install steps, settings, notifications). UI labels quoted in the docs must match the strings in `SettingsForm.Designer.cs`.
