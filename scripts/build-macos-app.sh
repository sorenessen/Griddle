#!/bin/zsh

set -e

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

APP_PROJECT="$ROOT_DIR/src/Griddle.App/Griddle.App.csproj"
PUBLISH_DIR="$ROOT_DIR/src/Griddle.App/bin/Release/net10.0/osx-arm64/publish"
MACOS_DIR="$ROOT_DIR/src/Griddle.App/MacOS"

APP_BUNDLE="$ROOT_DIR/artifacts/Griddle.app"
APP_CONTENTS="$APP_BUNDLE/Contents"
APP_MACOS="$APP_CONTENTS/MacOS"

SIGNING_IDENTITY="Developer ID Application: Soren Essen (282VJ37HVD)"

echo "Publishing Griddle..."

dotnet publish "$APP_PROJECT" \
  -c Release \
  -r osx-arm64 \
  --self-contained true

echo "Creating app bundle..."

rm -rf "$APP_BUNDLE"

mkdir -p "$APP_MACOS"
mkdir -p "$APP_CONTENTS/Resources"

cp -R "$PUBLISH_DIR/." "$APP_MACOS/"

echo "Copying Griddle native bridges..."

cp \
  "$ROOT_DIR/src/Griddle.Platform/bin/Release/net10.0/libGriddleCaptureBridge.dylib" \
  "$APP_MACOS/libGriddleCaptureBridge.dylib"

cp \
  "$ROOT_DIR/src/Griddle.Platform/bin/Release/net10.0/libGriddleRecordingBridge.dylib" \
  "$APP_MACOS/libGriddleRecordingBridge.dylib"

echo "Copying bundle resources..."

cp \
  "$MACOS_DIR/Info.plist" \
  "$APP_CONTENTS/Info.plist"

cp \
  "$MACOS_DIR/Resources/Griddle.icns" \
  "$APP_CONTENTS/Resources/Griddle.icns"

echo "Signing embedded Mach-O binaries..."

find "$APP_MACOS" \
  -type f \
  -print0 |
while IFS= read -r -d '' file_path
do
    if ! file "$file_path" | grep -q "Mach-O"; then
        continue
    fi

    if [ "$file_path" = "$APP_MACOS/Griddle.App" ]; then
        continue
    fi

    echo "  Signing: ${file_path#$ROOT_DIR/}"

    codesign \
      --force \
      --options runtime \
      --timestamp \
      --sign "$SIGNING_IDENTITY" \
      "$file_path"
done

echo "Signing Griddle executable..."

codesign \
  --force \
  --options runtime \
  --timestamp \
  --entitlements "$MACOS_DIR/Griddle.entitlements" \
  --sign "$SIGNING_IDENTITY" \
  "$APP_MACOS/Griddle.App"

echo "Signing Griddle app bundle..."

rm -rf "$APP_CONTENTS/_CodeSignature"

codesign \
  --force \
  --deep \
  --options runtime \
  --timestamp \
  --entitlements "$MACOS_DIR/Griddle.entitlements" \
  --sign "$SIGNING_IDENTITY" \
  "$APP_BUNDLE"

echo "Verifying Griddle executable signature..."

codesign \
  --verify \
  --strict \
  --verbose=2 \
  "$APP_MACOS/Griddle.App"

echo "Verifying Griddle app bundle..."

codesign \
  --verify \
  --deep \
  --strict \
  --verbose=2 \
  "$APP_BUNDLE"

echo
echo "Built:"
echo "$APP_BUNDLE"