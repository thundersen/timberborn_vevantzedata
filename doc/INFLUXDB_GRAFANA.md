# Setting up Grafana and InfluxDB

[InfluxDB](https://www.influxdata.com/products/influxdb/) is an open source database which specializes in time series data. Using it allows advanced real-time visualization and alerting. 

[Grafana](https://grafana.com/grafana/) is an open source data visualization tool which can create nice graphs like the ones in the example dasboard above and much more. 

## Docker

The easiest way to set these up on your PC is to use the scripts available [here](https://github.com/thundersen/timberborn_vevantzedata/tree/main/tools/docker)

If you're familiar with Docker, you can just clone the repo and run `provision.sh` to set up everything. That's it. Open http://localhost:3000 in your browser (default user and password are admin/admin) and find the empty default dashboard. Data will appear in it after a little while of playing the game with the mod enabled. Remember to set up the environment variable, as the output of `provision.sh` will have told you.

If you don't know what Docker is, you might want to check out the [Wikipedia article](https://en.wikipedia.org/wiki/Docker_(software)) before continuing to read.

### Docker Desktop

Before installing Docker Desktop for Windows you need to set up the "Windows Subsystem for Linux" aka WSL. Instructions are available [here](https://docs.microsoft.com/en-us/windows/wsl/install).
After that you can install Docker for Desktop as explained [here](https://docs.docker.com/desktop/windows/install/#install-docker-desktop-on-windows)

From within your WSL Linux environment run the following commands:
```
git clone https://github.com/thundersen/timberborn_vevantzedata.git
cd timberborn_vevantzedata/tools/docker
bash provision.sh
```
This will copy the complete GitHub repository of the mod onto your system, go to subfolder `tools/docker` and run a script to start InfluxDB and Grafana in their own Docker containers and connect them with each other. 

At the end of the text output of the script you will find the name and value of an [environment variable](https://helpdeskgeek.com/how-to/create-custom-environment-variables-in-windows/) which you need to set up for the mod to be able to write data to InfluxDB.

That's it. You can now open Grafana in the browser and check out the default dashboard as described above.


## Manual Installation

> Unfortunately setting up InfluxDB for yourself requires a certain amount of work and tech-savviness on your end. Nothing I can do about that at the moment. 

### InfluxDB

A free download of InfluxDB is available [here](https://portal.influxdata.com/downloads/). 

Walking you through the process of installing and setting it up is out of the scope of this doc, but it's fairly simple for a database this powerful. [Here](https://docs.influxdata.com/influxdb/v2.0/install/?t=Windows)'s an installation guide for windows.

After installation you will need to set up 3 things in InfluxDB for the mod to be able to use it:
1. A bucket named `vevantzedata`. (You can use a different name and change it in the mod's [config](#config-settings), if you want.)
2. An organization named `thundersen` (You can use a different name and change it in the mod's [config](#config-settings), if you want.)
3. A [token](https://docs.influxdata.com/influxdb/cloud/security/tokens/create-token/) with write access to the `vevantzedata` bucket

**Create an environment variable in your OS with the name `VEVANTZEDATA_INFLUXDB_TOKEN` and the token as its value.** [Here](https://helpdeskgeek.com/how-to/create-custom-environment-variables-in-windows/)'s a guide for creating environment variables in Windows.


### Grafana with InfluxDB

See installation instructions for Windows [here](https://grafana.com/docs/grafana/latest/installation/windows/)

Set up InfluxDB as a data source. It should look like this:

![influx data source](https://raw.githubusercontent.com/thundersen/timberborn_vevantzedata/main/doc/grafana_influx_data_source.png)

Note that "Flux" is set as query language, not InfluxQL. This is required for the example dashboard to work. 
Grafana also needs a [token](https://docs.influxdata.com/influxdb/cloud/security/tokens/create-token/) to be abe to read from InfluxDB. This one needs read access. Paste the token value into the Token field under InfluxDB Details in your Grafana data source settings.

You can import the [example dashboard](https://raw.githubusercontent.com/thundersen/timberborn_vevantzedata/main/tools/grafana/dashboards/example_dashboard.json) included in the GitHub repo to get started. Download the JSON file and [import](https://grafana.com/docs/grafana/latest/dashboards/export-import/) it into your Grafana instance.

