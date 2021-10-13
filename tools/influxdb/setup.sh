#!/bin/bash

set -e

USER=`pass show vevantzedata/influxdb/localdev | sed '3q;d' | tr -d 'user: '`
PASSWORD=`pass show vevantzedata/influxdb/localdev | sed '1q;d'`

influx setup \
  --org thundersen \
  --bucket vevantzedata \
  --username ${USER} \
  --password ${PASSWORD} \
  --force