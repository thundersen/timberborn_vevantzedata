#!/bin/bash

set -e

set -a; source .env; set +a

promtail-linux-amd64 -config.file=config.yaml -config.expand-env=true $@
