#!/usr/bin/env bash
set -euo pipefail

RID="$1"        # linux-x64 or linux-arm64
OUTDIR="$2"     # out/linux-x64 etc.

dotnet --info

dotnet publish -c Release -r "$RID" \
  -p:PublishAot=true \
  -p:SelfContained=true \
  -p:PublishSingleFile=true \
  -p:StripSymbols=true \
  -o "$OUTDIR"

# rename to stable name for packaging
chmod +x "$OUTDIR/mbdc"
