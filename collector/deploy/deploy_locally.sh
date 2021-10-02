#!/bin/bash

NET_VERSION=`grep '<TargetFramework>' Collector.csproj | tr -dc '0-9'`

PROJECT_DIR=`dirname $0`/..

cp -v ${PROJECT_DIR}/bin/Debug/net${NET_VERSION}/VeVantZeData.dll ~/.steam/steam/steamapps/common/Timberborn/BepInEx/plugins
