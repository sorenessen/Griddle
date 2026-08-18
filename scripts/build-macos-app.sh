#!/bin/zsh

set -e

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

APP_PROJECT="$ROOT_DIR/src/Griddle.App/Griddle.App.csproj"
BUILD_DIR="$ROOT_DIR/src/Griddle.App/bin/Debug/net10.0"
MACOS_DIR="$ROOT_DIR/src/Griddle.App/MacOS"

APP_BUNDLE="$ROOT_DIR/artifacts/Griddle.app"
APP_CONTENTS="$APP_BUNDLE/Contents"
APP_MACOS="$APP_CONTENTS/MacOS"

SIGNING_IDENTITY="Apple Development: Soren Essen (6N4D63A5D2)"

echo "Building Griddle..."
dotnet build "$ROOT_DIR/Griddle.slnx"

echo "Creating app bundle..."
rm -rf "$APP_BUNDLE"

mkdir -p "$APP_MACOS"
mkdir -p "$APP_CONTENTS/Resources"

cp -R "$BUILD_DIR/." "$APP_MACOS/"
cp "$MACOS_DIR/Info.plist" "$APP_CONTENTS/Info.plist"

echo "Signing Griddle..."
codesign \
  --force \
  --deep \
  --entitlements "$MACOS_DIR/Griddle.entitlements" \
  --sign "$SIGNING_IDENTITY" \
  "$APP_BUNDLE"

echo "Verifying signature..."
codesign \
  --verify \
  --deep \
  --strict \
  "$APP_BUNDLE"

echo
echo "Built:"
echo "$APP_BUNDLE"