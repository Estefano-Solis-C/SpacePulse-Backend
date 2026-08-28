# 💻 Stage 2: Code & Version Control Standards

## 1. Branching Strategy: Trunk-Based Development
```text
  main (Production)
   │
   ├── develop (Integration / Staging)
   │     │
   │     ├── feature/SP-101-iot-telemetry
   │     ├── bugfix/SP-102-auth-timeout
   │     └── release/v1.0.0
```

- `main`: Protected production branch. Requires Pull Request + 2 Approvals + Green CI.
- `develop`: Staging integration branch.
- `feature/<ticket-id>-<short-description>`: Short-lived feature branches (< 2 days).
- `bugfix/<ticket-id>-<short-description>`: Bug resolution branches.
- `release/v<version>`: Preparation branch for SemVer tagging.

## 2. Conventional Commit Messages
Follow the Angular / Conventional Commits specification:
- `feat(spaces)`: Add price filter to space list
- `fix(iot)`: Correct temperature sensor threshold parsing
- `test(iam)`: Add Jasmine unit test for user assembler
- `refactor(core)`: Modularize HTTP interceptor handling
- `ci(devops)`: Configure GitHub Actions deployment workflow
- `docs(readme)`: Update architectural diagrams

## 3. Pull Request Review Rules
1. Automated CI Pipeline must pass (Build, Lint, Unit Tests, Security Scan).
2. Code coverage must not decrease.
3. All PR discussions must be resolved before merging (Squash & Merge).
