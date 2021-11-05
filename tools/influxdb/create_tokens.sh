#!/bin/bash

set -eu

function determine_token {
    set +e
    MATCHES=$(influx auth list | grep -c "$1")
    set -e

    if [ ${MATCHES} -ne 0 ]; then
        # using a regex for the jwt, because sometimes the output of "influx auth list"
        # seemed to contain tabs between columns which messed up "cut"
        influx auth list | grep "$1" | grep -o '[a-zA-Z0-9\=\_\-]*==' | head -n1
        echo "reusing existing $1" >&2
    else
        influx auth create $2 --description "$1" | cut -f3 | tail -n1
        echo "created new $1" >&2
    fi
}

ID=$(influx bucket list --name ${INFLUX_BUCKET_NAME} | cut -f1 | tail -n1)

determine_token "Read Token for Grafana" "--read-bucket ${ID}"
determine_token "Write Token for VeVantZeData Mod" "--write-bucket ${ID} --read-orgs --read-tasks --write-tasks --read-checks --write-checks"
