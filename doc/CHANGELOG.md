# Changes

## 0.1.5

- Use InfluxDB token from config file instead of environment variable
- Calculate days of water and food left in game. This improves dashboard performance and allows publishing these values in an event for use in other mods.
- Use precalculated values in dashboard
- Tweak dashboard variable definitions to avoid errors for certain values with special characters

## 0.1.4

- Use settlement name instead of playthrough ID where possible
- Add days of food/water stored to example dashboard
- Publish captured data as event for use in other mods