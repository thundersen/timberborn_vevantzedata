# Ve Vant ze Data Timberborn!

> Ve vant ze data. All ze data. Ve vant ze graphs, ve vant ze alerts, ve vant a vorld vere ve can eat a sea otter vizout getting sick!

Timberborn doesn't show us any statistics or graphs about our cities' development over time. It's hard to answer basic questions like "How much water did this district have a few days ago?" "Can our farms keep up with our food consumption?"

Eventually the game will surely (hopefully?) improve in that regard, but if you're a numbers nerd and/or you are absolutely determined to min/max the hell out of your beavers, you will always want more data, more graphs than what the game will show you. If you're like me than you have been annoyed by the lack of data and visualization options in other games in the past.

 You may even want to get an alert when certain conditions are met, so that you can run the game in the background without worrying about coming back to a graveyard where everyone has died of thirst and your huge construction project never got finished. 

This mod attempts to solve that problem by gathering all kinds of useful data in-game and exporting it out of the game in a format that allows the use of tools for advanced data analysis, visualization and alerting.


## Gathered Data

Development of this mod has only just started. It's currently collecting data for resource stockpiles and population by district, which already allows a visualization like this:

![example dashboard](example_dashboard.png)

So all the data for creating basic alerts for food or water running out is already there. More will come in the future. Please do [submit your ideas](#feedback)!


## Outputs

### CSV

If enabled, CSV files are written to the directory `BepInEx/vevantzedata`. (You will be able to change the output path in a future release.) There will be a subfolder for each district. The contents of these files should be self-explanatory. They can be loaded into all kinds of programs like Excel or LibreOffice Calc where you can do whatever you want with them.

### InfluxDB

[InfluxDB](https://www.influxdata.com/products/influxdb/) is an open source database which specializes in time series data. Using it allows advanced real-time visualization and alerting. Free download is available [here](https://portal.influxdata.com/downloads/). 

> Unfortunately setting up InfluxDB for yourself requires a certain amount of work and tech-savviness on your end. Nothing I can do about that at the moment. I'm thinking about ways to simplify this, but I'm not sure, if it's worth prioritizing. Please [reach out](#feedback), if you're interested in using the advanced features that InfluxDB allows, but can't (be bothered to ;)) set up InfluxDB. This will at least show me that I should come up with a way to simplify this setup.

Walking you through the process of installing and setting it up is out of the scope of this doc, but it's fairly simple for a database this powerful. [Here](https://docs.influxdata.com/influxdb/v2.0/install/?t=Windows)'s an installation guide for windows.

After installation you will need to set up 3 things in InfluxDB for the mod to be able to use it:
1. A bucket named `vevantzedata`. (You can use a different name and change it in the mod's [config](#config-settings), if you want.)
2. An organization named `thundersen` (You can use a different name and change it in the mod's [config](#config-settings), if you want.)
3. A [token](https://docs.influxdata.com/influxdb/cloud/security/tokens/create-token/) with write access to the `vevantzedata` bucket

**Create an environment variable in your OS with the name `VEVANTZEDATA_INFLUXDB_TOKEN` and the token as its value.** [Here](https://helpdeskgeek.com/how-to/create-custom-environment-variables-in-windows/)'s a guide for creating environment variables in Windows.


## Using Grafana with InfluxDB

[Grafana](https://grafana.com/grafana/) is an open source data visualization tool which can create nice graphs like the ones in the example dasboard above and much more. Free download is available [here](https://grafana.com/grafana/download?pg=get&plcmt=selfmanaged-box1-cta1&edition=oss).

Set up InfluxDB as a data source. It should look like this:
![influx data source](grafana_influx_data_source.png)

Note that "Flux" is set as query language, not InfluxQL. This is required for the example dashboard to work. 
Grafana also needs a [token](https://docs.influxdata.com/influxdb/cloud/security/tokens/create-token/) to be abe to read from InfluxDB. This one needs read access. Paste the token value into the Token field under InfluxDB Details in your Grafana data source settings.

TODO: EXAMPLE DASHBOARD

## <a name="config">Config Settings</a>

The mod creates a config file under `BepInEx/config/`. Make sure to open it after you start with the mod enabled for the first time. The values in there should be self-explanatory.

By default only the CSV writer is enabled. Activate InfluxDB, if you want to use it. I'd recommend to disable the CSV writer, if you don't use the files to save disk space.


## Game Time

The mod converts cycles and days form the game to a custom timestamp. This is required for storage in InfluxDB and greatly simplifies visualization in Grafana.

It does not use your computer's time for the timestamp, because
1. It would create gaps in graphs when you pause the game or load a save
2. It would make comparing different playthroughs much harder. If every playthrough starts at the same time, it's much easier to create panels with graphs from multiple playthroughs.

I have chosen January 1st, 2100 as the start date. So cycle 1, day 1 will be that date, day 10 will be January 10th and so on.


## Playthrough ID

Timberborn currently doesn't support naming playthroughs. The mod creates a unique ID for each playthrough, so that different playthroughs can be distinguished from each other in the data.

I expect the game to eventually support naming playthroughs. At that point the clunky ID will be replaced with that name.


## Planned Features

- Moar data. Power production/consumption, water levels, wind speed, FPS, ... anything goes. Feel free to submit suggestions.
- In-game alerts. Define your own alerts and receive a notification in game.
- Support for IRL alerts. Define alerts and hook up smart home devices, get a text message or what have you. Note that this is already possible for those that know how to set it up with InfluxDB! Examples and guides will follow in the future. 
- Integration with other mods to allow them to use ze data as well, e.g., for showing graphs in-game, using alerts to automate buildings, ...

## <a name="feedback">Feedback</a>

Any feedback is welcome. You can reach me on the Timberborn Discord (thundersen#0586), Reddit (https://www.reddit.com/user/7hund3r53n) or create a GitHub issue [here](https://github.com/thundersen/vevantzedata/issues).