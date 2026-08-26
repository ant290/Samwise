## TODO:

## Web App

- Create Dashboard showing readings history
    - ✅ Simple single chart showing single sensor reading history
    - ⬛ Image of garden with last known sensor values shown as an overlay
    - ⬛ Graphs of each sensor, shown when selecting that sensor from the garden image

- Maintenance page for SensorDevices
    - ✅ Names and locations to be editable, Ip address will be updated as devices claiming that ID post to the api
    - ✅ Sensor Id's to be manageable, with ability to label sensors
        - Should display sensor type
        - Should display last sensor reading

- Sensor maintenance
    - ⬛ For soil moisture sensor types:
        - Have a limit to alert for watering required
    - ⬛ For battery level sensors:
        - Have a limit to alert for low battery
    - ⬛ For temperature sensors:
        - Have a limit to alert for overheating
    - ⬛ For humidity sensors:
        - Have a limit to alert for humidity issues 

## Embedded

- D1 Mini
    - ✅ Deep sleep version with battery monitoring 
    - ⬛ Set battery read to be the last sensor read before posting data
    - ⬛ Additional soil moisture sensors to bring the total to 3 per device if possible