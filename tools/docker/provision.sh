#!/bin/bash

set -euo pipefail

source influx2.env

echo "==> start influxdb"
docker-compose up -d influxdb

echo "==> waiting for influxdb..."

HEALTH_STATUS=-1
for i in {1..10}
do
  curl -s localhost:8086/health && HEALTH_STATUS=$? || HEALTH_STATUS=$?
  if [ ${HEALTH_STATUS} -eq 0 ]; then
      echo "==> ...yay! it's healthy."
      break
  fi
  sleep 5s
done

if [ ${HEALTH_STATUS} -ne 0 ]; then
    echo "==> ... :( it failed to start in time. maybe increase the timeout?"
    exit 1
fi

echo "==> create influxdb tokens"
TOKENS=$(docker exec -i \
  -e INFLUX_ORG="${DOCKER_INFLUXDB_INIT_ORG}" \
  -e INFLUX_BUCKET_NAME="${DOCKER_INFLUXDB_INIT_BUCKET}" \
  -e INFLUX_TOKEN="${DOCKER_INFLUXDB_INIT_ADMIN_TOKEN}" \
  influxdb sh < ../influxdb/create_tokens.sh)

GRAFANA_TOKEN=$(echo "${TOKENS}" | head -n1)
MOD_TOKEN=$(echo "${TOKENS}" | tail -n1)

echo "==> prepare data source config"
sed -e 's/%%INFLUXDB_ORG%%/'${DOCKER_INFLUXDB_INIT_ORG}'/g' \
    -e 's/%%INFLUXDB_BUCKET%%/'${DOCKER_INFLUXDB_INIT_BUCKET}'/g' \
    -e 's/%%INFLUXDB_TOKEN%%/'${GRAFANA_TOKEN}'/g' \
    grafana/etc/provisioning/datasources/datasource.yaml.template \
  > grafana/etc/provisioning/datasources/datasource.yaml

echo "==> start grafana"
docker-compose up -d grafana

echo ">>> done. now you need to create this environment variable for the mod:"
echo "VEVANTZEDATA_INFLUXDB_TOKEN=${MOD_TOKEN}"