# 📦 Stage 5: Release Governance & Versioning

## 1. Objective
Package and store production-ready, security-audited artifacts in centralized artifact repositories (Artifactory / GitHub Container Registry).

## 2. Semantic Versioning (SemVer 2.0.0)
Format: `v<MAJOR>.<MINOR>.<PATCH>` (e.g. `v1.2.0`)
- **MAJOR**: Breaking changes or API contract shifts.
- **MINOR**: Backward-compatible new features (e.g., IoT telemetry actuator).
- **PATCH**: Backward-compatible bug fixes and security patches.

## 3. Container Image Tagging Policy
1. `ghcr.io/spacepulse/<service>:latest` -> Latest stable production build.
2. `ghcr.io/spacepulse/<service>:v1.2.0` -> Immutable release tag.
3. `ghcr.io/spacepulse/<service>:<commit-sha>` -> Specific build artifact for rollbacks.

## 4. Manual Approval Gates
Production releases require explicit manual approval from Release Managers in the GitHub Environment protection rules before deployment to production.
