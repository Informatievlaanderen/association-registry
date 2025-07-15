#!/usr/bin/env bash
set -e

BUILD_DIR="${1:-.build}"

python $BUILD_DIR/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/ci-md2conf.py CHANGELOG.md VBR \
    --user "${CONFLUENCE_USERNAME}" \
    --password "${CONFLUENCE_PASSWORD}" \
    --orgname "vlaamseoverheid" \
    --title "${CONFLUENCE_TITLE}" \
    --ancestor "Changelog" \
    --nogo
