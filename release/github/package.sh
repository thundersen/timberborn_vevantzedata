#!/bin/bash

NET_VERSION=`grep -oPm1 "(?<=<TargetFramework>)[^<]+" collector/Collector.csproj`
PROJECT_DIR=`dirname $0`/../../collector
DLL_DIR=${PROJECT_DIR}/bin/Release/${NET_VERSION}
ZIP_FILE=`dirname $0`/vevantzedata_github.zip

rm -f ${ZIP_FILE}

zip -v -j ${ZIP_FILE} \
  ${DLL_DIR}/VeVantZeData.dll \
  ${DLL_DIR}/InfluxDB.Client*.dll \
  ${DLL_DIR}/Microsoft.Extensions.ObjectPool.dll \
  ${DLL_DIR}/System.Reactive.dll \
  ${DLL_DIR}/RestSharp.dll

echo "\n--- packaged github release at ${ZIP_FILE} ---\n"