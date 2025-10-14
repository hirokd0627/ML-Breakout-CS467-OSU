# CS467 Capstone Project - Machine Learning Breakout Game

---

### Summary: TBD...

### Group Members:
Matt Villa
Hayden Greiner
Takahiro Kudo
Ethan Van Hao
Liam Maguire

---

# License

This project makes use of the MIT License, see the license section for applicable information.

---

# Unity Project Setup & Workflow Guide

This guide explains how to **clone, open, and work on this Unity project**, including expectations for commits, formatting, and pull requests.

---

## <ins>Cloning and Opening the Project</ins>

### Step 1 — Clone the repository
Clone the project from GitHub:
```bash
git clone https://github.com/mv805/ML-Breakout-CS467-OSU.git
cd <repo>
```

### Step 2 — Open in Unity
1. Open **Unity Hub**.
2. Click **Add project** → select the cloned folder.
3. Unity will read `ProjectSettings/ProjectVersion.txt` and prompt to install the matching version.
4. Open the project. The first load will take longer as Unity rebuilds its local `Library/` cache.

### Step 3 — Verify Unity settings
These are already configured in the repo and must remain this way:
- **Version Control Mode:** Visible Meta Files  
- **Asset Serialization:** Force Text  

(Found under **Edit → Project Settings → Editor**)

---

## <ins>Expectations for Work & Commits</ins>

- **Never push directly to `main`**. Always create a feature branch and open a Pull Request.
- **Keep commits small and focused.**
- **Do not commit build artifacts** (`Library/`, `Temp/`, `Logs/`, etc. are ignored automatically, you dont commit these).
- **CSharpier** enforces code formatting — all `.cs` files must be formatted before merging.
- The repository includes a GitHub Action that runs the formatting check automatically on every PR.

---

## <ins>Daily Workflow</ins>

### Start new work (from terminal in project folder)
Always branch from the latest `main`:
```bash
git checkout main
git pull origin main
git checkout -b feat/your-feature-name
```

### Work in Unity
- Open the project in Unity Hub.
- Make code or asset changes, test, and save.
- Do not rename or move files without committing related `.meta` files.

### Commit and push
```bash
git add -A
git commit -m "Implement new feature or fix"
git push -u origin feat/your-feature-name
```

### Open a Pull Request
1. Go to GitHub → your branch → **Compare & pull request**.
2. Add a clear title and description.
3. Wait for CI checks to complete (formatting, etc.).
4. Request a review from a teammate by assigning someone.
5. Once approved and checks pass → click **Merge**.

NOTE: Approvers are best practice, everyone has the admin rights to ignore this but we should strive to at least get some concurrance on commits to protect the integrity of the code. 

---

## <ins>CSharpier Formatting (Required)</ins>

### Local usage (from repo tool manifest)
The formatter is included as a **local dotnet tool** in `.config/dotnet-tools.json`.

Run from the repo root:

#### Check formatting only
```bash
dotnet tool restore
dotnet csharpier check .
```

#### Apply formatting fixes
```bash
dotnet csharpier format .
```

Any unformatted file will cause the CI check (`format-check / csharp-format`) to fail on pull requests.

---

## <ins>Keeping Your Local Repo Updated</ins>

Before starting new work or after a merge:
```bash
git checkout main
git pull origin main
```

To clean old branches:
```bash
git fetch --all --prune
git branch -d feat/old-branch
```

To reset your local copy completely (discarding uncommitted work):
```bash
git restore .
git clean -fd
git fetch --all --prune
git checkout main
git reset --hard origin/main
```

---

## <ins>Expectations Summary</ins>

| Area | Expectation |
|------|--------------|
| Branching | All work done on feature branches |
| Direct commits to main | ❌ Never |
| Code formatting | Must pass `dotnet csharpier check .` |
| Reviews | Each PR requires 1 approval |
| Unity version | Use version in `ProjectVersion.txt` |
| Meta files | Always commit with their assets |
| Commits | Small, focused, descriptive |

---

## TL;DR

> **Clone → Open in Unity → Branch → Work → Commit → Push → PR → Review → Merge → Pull main**
