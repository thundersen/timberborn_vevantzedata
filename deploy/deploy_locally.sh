#!/bin/bash

NET_VERSION=`grep -oPm1 "(?<=<TargetFramework>)[^<]+" collector/Collector.csproj`
PROJECT_DIR=`dirname $0`/../collector
DLL_DIR=${PROJECT_DIR}/bin/Debug/${NET_VERSION}
TARGET_DIR=~/.steam/steam/steamapps/common/Timberborn/BepInEx/plugins/vevantzedata

mkdir -p ${TARGET_DIR}

cp -v ${DLL_DIR}/VeVantZeData.dll ${TARGET_DIR}
cp -v ${DLL_DIR}/InfluxDB.Client*.dll ${TARGET_DIR}
cp -v ${DLL_DIR}/Microsoft.Extensions.ObjectPool.dll ${TARGET_DIR}
cp -v ${DLL_DIR}/System.Reactive.dll ${TARGET_DIR}
cp -v ${DLL_DIR}/RestSharp.dll ${TARGET_DIR}