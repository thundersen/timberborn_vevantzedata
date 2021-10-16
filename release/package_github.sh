#!/bin/bash

NET_VERSION=`grep -oPm1 "(?<=<TargetFramework>)[^<]+" collector/Collector.csproj`
PROJECT_DIR=`dirname $0`/../collector
DLL_DIR=${PROJECT_DIR}/bin/Release/${NET_VERSION}

zip -v -j `dirname $0`/vevantzedata_github.zip \
  ${DLL_DIR}/VeVantZeData.dll \
  ${DLL_DIR}/InfluxDB.Client*.dll \
  ${DLL_DIR}/Microsoft.Extensions.ObjectPool.dll \
  ${DLL_DIR}/System.Reactive.dll \
  ${DLL_DIR}/RestSharp.dll