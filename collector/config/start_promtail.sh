#!/bin/bash

set -e

set -a; source .promtailenv; set +a

echo ${DATA_DIR_PARENT}vevantzedata/

promtail-linux-amd64 -config.file=vevantzedata_promtail.yaml -config.expand-env=true --dry-run
