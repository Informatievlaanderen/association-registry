#!/usr/bin/env bash
set -e

BUILD_DIR="${1:-.build}"

python $BUILD_DIR/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/ci-jiraversion.py ${JIRA_PREFIX}-${JIRA_VERSION} ${JIRA_PROJECT} \
    --user "${CONFLUENCE_USERNAME}" \
    --password "${CONFLUENCE_PASSWORD}" \
    --orgname "vlaamseoverheid" \
    --github "https://github.com/Informatievlaanderen" \
    --repo "${CONFLUENCE_TITLE}"
