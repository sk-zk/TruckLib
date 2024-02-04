# SCS Route creater

## create google maps route, copy URL
* [googleMaps](https://www.google.de/maps/dir/48.7058515,9.1666909/48.2560476,8.8394007/@48.6717281,9.1982895,14300m/data=!3m1!1e3!4m6!4m5!2m3!6e0!7e2!8j1705992180!3e0!5m1!1e4?entry=ttu)

## save route as .gpx
* [mapstogpx](https://mapstogpx.com/)

## add elevation data to route
* [gpsvisualizer](https://www.gpsvisualizer.com/elevation)

## convert route from .gpx to .csv
* [gpxeditor](https://sourceforge.net/projects/gpxeditor/)

## convert "Decimal Degree - Lat,Long" to UTM and copy values to new .csv
* [/Samples/02-Route/coordiniate_converter.xlsx](https://github.com/gsus24/TruckLib/blob/route/Samples/02-Route/coordinate_converter.xlsx)

    recalculate waypoints to relativ path
        = 1st -1st = 0
        = 2nd - 1st = ...
        .
        .
        .
    
    reduce number of waypoints, filter every 10th, reverse direction in y

    * [/Samples/02-Route/route.csv](https://github.com/gsus24/TruckLib/blob/route/Samples/02-Route/route.csv)

## run Route
* [/Samples/02-Route/Program.cs](https://github.com/gsus24/TruckLib/blob/route/Samples/02-Route/Program.cs)

## open map with ETS2 Editor, recalculate map, save

## run map
