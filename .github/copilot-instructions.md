# Copilot code review instructions

Elite's RNG Aura Observer is a Windows-only C# / WinForms desktop app (.NET 9)
that tails VRChat logs and sends toast + Discord webhook notifications when an
Aura is obtained. A separate `ElitesRNGAuraObserver.Updater` project self-updates
from GitHub releases. There is no automated test project.

Review for the points below. Keep comments specific and actionable.

## Review priorities

- **Secret handling.** The Discord webhook URL (`ConfigData.DiscordWebhookUrl`) is user-provided and sensitive. Flag any code that logs it, writes it to error/telemetry output, or embeds a real webhook/token as a literal.
- **XML documentation.** StyleCop (`stylecop.json`) requires doc comments on *every* type and member, including `private` ones. Flag new/changed members that lack an XML doc comment — CI's `dotnet format --verify-no-changes` will otherwise fail.
- **Log-parsing regex.** Detection depends on VRChat's exact log format (`NewAuraDetectionService.cs`, `AuthenticatedDetectionService.cs`). Flag regex changes that look narrower/broader than intended or that could silently stop matching real log lines.
- **Watcher resilience.** `LogWatcher` and notification code run in background loops. Flag exceptions that can escape and kill the loop; network/notification calls should be wrapped in try/catch with logging, matching the existing pattern.
- **Nullability.** `Nullable` is enabled. Flag dereferences of possibly-null values and unjustified `!` null-forgiving operators.
- **File / IO and process handling.** The updater deletes directories, kills processes, downloads assets, and verifies a checksum digest before extracting. Flag removal of the checksum verification, path handling that could escape the target folder, or unhandled IO failures.
- **Resource lifetime.** `IDisposable` types (`LogWatcher`, mutex, streams, `CancellationTokenSource`) must be disposed. Flag leaks or double-dispose.

## Conventions enforced

- 4-space indentation (spaces, not tabs), file-scoped namespaces, `using` directives outside the namespace with `System.*` first, newline at end of file. These are enforced by StyleCop + `dotnet format`; only comment on them when a change clearly violates them.
- Conventional Commit prefixes (`feat:`, `fix:`, `chore:`, …) drive the release version bump.

## Known non-issues — do not flag

- Japanese XML doc comments and inline comments are intentional; do not ask to translate them to English.
- Windows-only APIs (WinForms, registry access, `LibraryImport`/P/Invoke, UWP toast) are expected — do not suggest cross-platform alternatives.
- `AllowUnsafeBlocks` and the console `Console.WriteLine` debug logging throughout are intentional design choices.
- Absence of unit tests is known; do not request adding a test project as a blocking review item.
- `Auras.json` is a large generated data catalog; do not review it line by line.
