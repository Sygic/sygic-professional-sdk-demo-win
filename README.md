# sygic-win-professional-sdk-demo
This demo is a good tool for testing purposes on platform Windows XP/Vista/7/8/8.1/10. You can find there:
*	Launching Fleet application as an embedded component onto the application window or as a new windows. You can set there the position and range of this window as well. 
*	More than 40 API functions. You can easily change the parameters and all results are logged at left bottom of the application window.
*	The way how to parse the data getting from events.
*	Shows the way how to push nmea sentences into the navigation and playing the GPS log.
*	Possibility of generating itineraries inside specific area on map automatically.
*	Device itinerary into smaller ones.
*	Detect not correct waypoints – the ones when the navigation fails.

## Installation
1. Please make sure that you have installed the corresponding navigation on your computer.
1. Please make sure that you have copied the “ApplicationApi.dll” and “sdkdriver.dll” into this folder on your computer: C:\Windows\
1. Please use MS Visual Studio 2015 and run the project under X86 platform.
1. More information can be found https://www.sygic.com/developers/professional-navigation-sdk/windows

## Quick tutorial
1. After building and launching the “WindowsDemo” application, please set the path to the *.exe navigation.
1. Click on the green “Start” button on the middle left of the screen.
1. After starting the navigation successfully you can select specific API function or feature via main menu. 

## List of API functions & features
* AddBitmapToMap
* AddEntryToItinerary
* AddItinerary
* AddPoi
* AddPoiCategory
* AddTMCEvent
* BringAppToBackground
* BringAppToForeground
* CloseDialogs
* FindNearbyPoi
* GetCoordinatesFromOffset
* GetItineraryList
* GetLocationInfo
* GetMapVersion
* GetNextInstruction
* GetPoiCategoryList
* GetPoiList
* GetRouteInfo
* GetUniqueDeviceId
* ChangeApplicationOptions (show options only)
* IsDriveRunning
* LoadComputedRoute
* LocationFromAddress
* LocationFromAddressEx
* NavigateToAddress
* OnMenuCommand
* PlayGpsLog
* PlaySoundTTS
* ShowCoordinatesOnMap
* SkipNextWaypoint
* Start/Stop/Quit
* StartNavigation
* TripAddUserEvent
* TripEnd
* TripStart

… and many more
