#!/bin/bash

set -e

GAME_STEAM_ID=1062090
PROTON_VERSION=6.3
HOME=~

# define env vars with `strings /proc/[pid of timberborn]/environ > .env`
set -a; source `dirname $0`/.env; set +a

${HOME}/.steam/ubuntu12_32/reaper SteamLaunch AppId=${GAME_STEAM_ID} -- \
  ${HOME}/.steam/steam/steamapps/common/SteamLinuxRuntime_soldier/_v2-entry-point --verb=waitforexitandrun -- \
  "${HOME}/.steam/steam/steamapps/common/Proton ${PROTON_VERSION}"/proton waitforexitandrun \
  "${HOME}/.steam/steam/steamapps/common/Timberborn/Timberborn.exe " "$@"
