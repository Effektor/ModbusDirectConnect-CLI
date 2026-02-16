# Release Workflow Fix

## Problem
The release-please CI workflow was failing with the error:
```
Resource not accessible by integration
```

This error occurred when trying to create a GitHub release using a token.

## Changes Made

### 1. Token Reference Update
Changed from `secrets.GITHUB_TOKEN` to `github.token` throughout all workflow files:
- `.github/workflows/release-please.yml`
- `.github/workflows/build-and-release.yml`

**Why this matters:**
- `github.token` is the modern, recommended way to reference the automatic GitHub token
- It properly inherits permissions from the workflow's `permissions` block
- `secrets.GITHUB_TOKEN` is the older method that may not have all necessary permissions

### 2. Removed Dead Code
Removed references to non-existent PAT (Personal Access Token) environment variables in the release-please workflow:
- `RP_CREATED_PAT`
- `RP_TAG_PAT`
- `RP_VERSION_PAT`

These variables were never set, causing the workflow to unnecessarily fall back through conditional logic.

### 3. Simplified Output Resolution
The "Resolve release outputs" step was simplified to directly use the outputs from the release-please action instead of going through unnecessary environment variable indirection.

## Repository Settings Verification

If the workflow still fails with "Resource not accessible by integration", a repository administrator needs to verify the following settings:

1. **Navigate to Repository Settings → Actions → General**

2. **Workflow permissions** section should have one of:
   - "Read and write permissions" (recommended)
   - "Read repository contents and packages permissions" + "Allow GitHub Actions to create and approve pull requests" enabled

3. **Verify the Actions permissions** section:
   - Ensure Actions are allowed to run

## Testing

To test if the fix works:

1. Merge this PR to the main branch
2. The release-please workflow will run automatically on push to main
3. If there's a release-please PR already open (like PR #20), the workflow will attempt to create a release from it

## Alternative Solutions

If `github.token` still doesn't work due to repository settings restrictions:

### Option 1: Personal Access Token (PAT)
1. Create a PAT with `repo` scope
2. Add it as a repository secret (e.g., `RELEASE_TOKEN`)
3. Update the workflow to use `token: ${{ secrets.RELEASE_TOKEN }}`

**Note:** PATs have expiration dates and need to be renewed periodically.

### Option 2: GitHub App
1. Create a GitHub App with appropriate permissions
2. Install it in the repository
3. Use `actions/create-github-app-token` action to generate a token
4. Use that token for release-please

**Benefits:** GitHub Apps have better security model and don't expire.

## References

- [Release Please Action Documentation](https://github.com/googleapis/release-please-action)
- [GitHub Actions Permissions](https://docs.github.com/en/actions/security-for-github-actions/security-guides/automatic-token-authentication)
- [GitHub Releases API](https://docs.github.com/rest/releases/releases#create-a-release)
