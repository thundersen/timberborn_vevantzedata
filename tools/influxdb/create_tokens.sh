#!/bin/sh

ID=$(influx bucket list --name ${INFLUX_BUCKET_NAME} | cut -f1 | tail -n1)

GRAFANA_TOKEN=$(influx auth create --read-bucket ${ID} --description "Read Token for Grafana" | cut -f3 | tail -n1)

MOD_TOKEN=$(influx auth create --write-bucket ${ID} --description "Write Token for VeVantZeData Mod" | cut -f3 | tail -n1)

echo "${GRAFANA_TOKEN}\n${MOD_TOKEN}"