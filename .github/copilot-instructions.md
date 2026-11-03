# Copilot code review instructions

Project overview, layout, commands and coding conventions are in [AGENTS.md](../AGENTS.md); this file only lists what to focus on in review. Write review comments in English.

Keep comments specific and actionable.

## Review priorities

- **Secret handling.** The Discord webhook URL (`ConfigData.DiscordWebhookUrl`) is user-provided and sensitive. Flag any code that logs it, writes it to error/telemetry output, or embeds a real webhook/token as a literal.
- **XML documentation.** StyleCop (`stylecop.json`) requires doc comments on *every* type and member, including `private` ones; CI's `dotnet format --verify-no-changes` fails otherwise. Flag new/changed members that lack one, and `<param>` / `<returns>` tags that no longer match the signature.
- **Log-parsing regex.** Detection depends on VRChat's exact log format (`NewAuraDetectionService.cs`, `AuthenticatedDetectionService.cs`). Flag regex changes that look narrower/broader than intended or that could silently stop matching real log lines.
- **Watcher resilience.** `LogWatcher` and notification code run in background loops. Flag exceptions that can escape and kill the loop; network/notification calls should be wrapped in try/catch with logging.
- **Nullability.** `Nullable` is enabled. Flag dereferences of possibly-null values and unjustified `!` null-forgiving operators.
- **File / IO and process handling.** The updater deletes directories, kills processes, downloads assets, and verifies a sha256 digest before extracting. Flag removal of the digest verification, path handling that could escape the target folder, or unhandled IO failures.
- **Resource lifetime.** `IDisposable` types (`LogWatcher`, mutex, streams, `CancellationTokenSource`) must be disposed. Flag leaks or double-dispose.
- **Language.** New or edited code comments, docs, and commit/PR text should be English (see AGENTS.md).

## Known non-issues — do not flag

- Existing Japanese XML doc / inline comments are legacy; do not request bulk translation (only flag new or edited comments that are not in English).
- `docs/ja/` and `README-ja.md` are intentionally Japanese.
- Windows-only APIs (WinForms, registry access, `LibraryImport`/P/Invoke, UWP toast) are expected — do not suggest cross-platform alternatives.
- `AllowUnsafeBlocks` and the `Console.WriteLine` debug logging throughout are intentional.
- Duplicated `SemanticVersion` / `ReleaseInfo` / `GitHubReleaseService` between the app and the Updater are intentional (the Updater is standalone).
- Absence of unit tests is known; do not request adding a test project as a blocking review item.
- `Auras.json` is a large generated data catalog; do not review it line by line.
