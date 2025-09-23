#!/usr/bin/env bash
set -e

cd "$(dirname "$0")"

echo "Current directory: $(pwd)"

# ./Applications/candb.appがなければエラーにする
if [ ! -d "./Applications/candb.app" ]; then
  echo "Error: ./Applications/candb.app not found!"
  exit 1
fi

pkgbuild \
  --root ./Applications/ \
  --component-plist ./candb.plist \
  --identifier com.htwb.candb \
  --install-location /Applications \
  ./build/candb.pkg