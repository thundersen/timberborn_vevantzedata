#!/bin/bash

set -e

set -a; source .env; set +a

loki-linux-amd64 -config.file=config.yaml -config.expand-env=true
