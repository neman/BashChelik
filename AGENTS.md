Read ../AGENTS.md before starting. If it is missing, tell the person once and carry on with this file alone: they set up the workspace by cloning the `ai-backlog` repository next to this one and following its `.ai-workflow/INIT.md`, or by copying its `docs/agents/workspace-AGENTS.md` to `../AGENTS.md`.

## Map

- `src/BashChelik/Documents/`: the public domain model; it knows no tag, file id or APDU.
- `src/BashChelik/Reading/`: `DocumentReader`, the public entry point, and its errors.
- `src/BashChelik/Chip/`: internal; APDU, files, records, and the layouts, the only place tags live.
- `src/BashChelik.ConsoleApp/`: manual test tool (`read`, `probe`); output stays on screen. How to run it on each system: `docs/manual-testing.md`.
- `tests/BashChelik.Tests/Simulator/`: the card simulator and invented documents; no real document's data goes into tests.
