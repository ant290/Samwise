```markdown
# PROJECT_BRIEF.md — [Samwise]

> Last updated: [date] | Sprint [N] | Status: [In Progress / Complete]

## 1. Project Overview

Samwise is a homesteading / garden automation project. It consists of a blazor web app which is used for dashboarding and configuration, alongside embedded software which will run on arduino or esp32 style devices.

The web app will use .net blazor with bootstrap for styling, nlog for logging and sqlite for data persistance.

The embedded software will be for: reading moisture levels in the soil, managing solar charging of batteries, switching on and off the pump for the watering system.

## 2. Concept / Product Description

Samwise, a homesteading or garden automation project. It's main aim is to help stay on top of watering in the garden, with various moisture sensors spread around to read the soil moisture levels.

The main responsibilities.
1: Reading and reporting soil moisture levels.
2: Switching on / off the watering system.
3: Alerting when water levels are low.

## 3. Tech Stack

- **Frontend:** [.net blazor, c#, html, bootstrap]
- **Backend:** [.net blazor, sqlite]
- **Hosting:** [self hosted on a local network device]
- **Testing:** [bUnit]
<!-- - **CI/CD:** [pipeline tool] -->

## 4. Architecture

```
┌─────────────────────────────────────────┐
│              Frontend                   │
│  [Main Component] → [Sub Components]    │
└──────────────┬──────────────────────────┘
               │ HTTPS
┌──────────────▼──────────────────────────┐
│              Backend API                │
│  [Endpoints and their purpose]          │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│              Storage / Database         │
│  [Tables, collections, env vars]        │
└─────────────────────────────────────────┘
```

## 5. Key Files Map

| Area | Path | Contents |
|------|------|----------|
| Entry point | `BlazorWebApp/SamwiseBlazor/Components/App.razor` | App bootstrap |
| API | `BlazorWebApp/SamwiseBlazor/Controllers` | Server-side logic |
| Config | `BlazorWebApp/SamwiseBlazor/appsettings.json` | Server-only configuration |
| Tests | `BlazorWebApp/SamwiseBlazor.tests` | E2E and API tests |

## 6. Team Roles

| Agent | Name | Role |
|-------|------|------|
| Producer | Remy | Sprint plans, coordination, merging |
| Frontend | Nova | UI components, state, client logic |
| Backend | Sage | API, auth, database, security |
| Art/CSS | Milo | Visual design, animations, polish |
| QA | Ivy | Testing, bug filing, sign-off |

## 7. Sprint Status

| Sprint | Name | Status | Scope |
|--------|------|--------|-------|
| 0 | Architecture | ✅ Done | Tech stack, project structure, design guide |
| 1 | Core Features | 🔨 In Progress | [scope description] |

## 8. Current State (rewrite every sprint)

**What works:**
- [List of working features]

**What doesn't work yet:**
- [Known issues]

**What's next:**
- [Next sprint goals]

## 9. Security Rules

1. Secrets live in environment variables only — never in code or git.
2. [Auth approach]
3. [Additional security rules]

## 10. How to Run Locally

```bash
cd BlazorWebApp/SamwiseBlazor && dotnet restore
dotnet run
```

## 11. How to Deploy

[Pipeline description, env var locations, deployment steps]

## 12. Cross-Chat Handoff Protocol

Every sprint chat must do these before finishing:

1. Write `docs/sprint-N/done.md` — what was built, what's not done, what needs manual setup, files changed/created
2. Update PROJECT_BRIEF.md: Section 7 (mark sprint done) + Section 8 (rewrite current state)
3. Commit all changes with descriptive message: `sprint-N: <summary>`

This is how context survives across chats. If skipped, the next chat starts blind and may overwrite or duplicate work. The repo is the shared memory — keep it accurate.

## 13. Bug & Fix Tracking

Bugs are tracked as GitHub Issues on the repo. Single source of truth for all teams.

**For QA:** File bugs as GitHub Issues with labels (`bug`, `severity:blocker/major/minor`). Include: component, steps to reproduce, expected vs actual. When no blockers found: write `docs/qa/sprint-N-signoff.md` with test count, pass rate, explicit "no blockers" statement.

**For Dev Team:** Check GitHub Issues before starting work. Fix blockers and majors before polish. Use GitHub closing keywords in commits: `fix: description (Fixes #42)`. For reference-only, use `Refs #42`.

**For DevOps:** File infrastructure issues with label `infra`.

**For feature ideas:** add to `docs/ideas-backlog.md`.

## 14. Multi-Repo Setup

Each team works in their own separate clone of the repo. No worktrees. Everyone works on their own branch, pushes to origin, creates PRs.

**Teams:**
- Producer on `main` (coordination hub)
- Dev Team on `feature/sprint-N`
- QA on `feature/qa-N`

**Setup:**
```bash
git clone <repo> <folder-name>
cd <folder-name>
git checkout -b <branch-name>
npm install
```

**Branch strategy:** Feature branches → PR → regular merge to main. Never push directly to main. Never squash. Never rebase feature branches (causes commit loss).
```
