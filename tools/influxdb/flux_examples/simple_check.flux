import "experimental"
import "influxdata/influxdb/monitor"
import "influxdata/influxdb/v1"

option task = {
    name: "Berries Test",
    every: 1m,
}

settlement = "Berries Alert Test"
check_duration = 2d

lastTime = (tables=<-) => {
    extract = tables
        |> last()
        |> findColumn(fn: (key) => true, column: "_time")

    return extract[0]
}

stop = from(bucket: "vevantzedata")
    |> range(start: 2100-01-01T00:00:00Z, stop: 2200-01-01T00:00:00Z)
    |> filter(fn: (r) => r["_measurement"] == "stocks")
    |> filter(fn: (r) => r["settlement"] == settlement)
    |> filter(fn: (r) => r["_field"] == "Berries")
    |> lastTime()

start = experimental.subDuration(d: check_duration, from: stop)

berries = from(bucket: "vevantzedata")
    |> range(start: start, stop: stop)
    |> filter(fn: (r) => r["_measurement"] == "stocks")
    |> filter(fn: (r) => r["settlement"] == settlement)
    |> filter(fn: (r) => r["_field"] == "Berries")
    |> aggregateWindow(every: check_duration, fn: mean, createEmpty: false)

berries
    |> v1["fieldsAsCols"]()
    |> monitor.check(
        data: {
            _check_id: "0000000000000001",
            _check_name: "Berries Test",
            _type: "threshold",
            tags: {"settlement": settlement},
        },
        messageFn: (r) => "Check: ${r._check_name} is: ${r._level} (${r.Berries}) for ${r.district} in ${r.settlement}",
        crit: (r) => r["Berries"] <= 20.0,
        warn: (r) => r["Berries"] <= 100.0,
        ok: (r) => r["Berries"] > 100.0,
    )