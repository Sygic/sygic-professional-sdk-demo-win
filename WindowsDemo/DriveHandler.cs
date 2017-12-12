using System;
using System.Collections.Generic;
using System.Text;
using ApplicationAPI;
using System.IO;
using System.Diagnostics;
using System.Net;


namespace SYGIC_PROFESSINAL_SDK_DEMO
{
    public class DriveHandler
    {
        #region Delegates

        public delegate void NavigationNotificationDelegate(int nEventID, IntPtr strData);
        public delegate void NotificationDelegate(string errorString_in);

        #endregion

        #region Events

        public static event NavigationNotificationDelegate NavigationNotificationEvent;
        public static event NotificationDelegate NotificationEvent;

        #endregion

        #region Private members

        private static string _myDrivePath;
        private static SError _mySError;
        private static bool _runGpsLog=false;
        private static string[] gpsLog;
        private static int gpsLogPosition;

        private static void O(string s)
        {
            if (NotificationEvent != null)
            {
                NotificationEvent(s);
            }
        }

        #endregion

        #region Property

        //Path to the drive.exe
        public static string MyDrivePath
        {
            get { return _myDrivePath; }
            set { _myDrivePath = value; }
        }

        public static SError MySError
        {
            get { return _mySError; }
            set { _mySError = value; }
        }

        public static bool RunGpsLog 
        {
            get { return _runGpsLog; }
            set { _runGpsLog = value; }
        }

        public static string[] GpsLog
        {
            get { return gpsLog; }
            set { gpsLog = value; }
        }

        public static string GpsLogAtPosition(int inPosition)
        {
            if (inPosition < gpsLog.Length)
                return gpsLog[inPosition];
            return "";
        }

        public static int GpsLogPosition
        {
            get { return gpsLogPosition; }
            set { gpsLogPosition = value; }
        }

        #endregion

        #region Public members - Start stop drive

        public static int StartDrive()
        {
            ApplicationAPI.CApplicationAPI.ApplicationHandler AppHnd = new CApplicationAPI.ApplicationHandler(NavHandler);
            return CApplicationAPI.InitApi(_myDrivePath, AppHnd, 100, 100, 320, 200);
        }
        public static int StartDrive(int inLeft, int inTop, int inWidth, int inHeight, bool inRunInForeground, bool inNoCaption)
        {
            ApplicationAPI.CApplicationAPI.ApplicationHandler AppHnd = new CApplicationAPI.ApplicationHandler(NavHandler);
            int ret = CApplicationAPI.InitApi(_myDrivePath, AppHnd, inLeft, inTop, inWidth, inHeight,inRunInForeground,inNoCaption);
            O("InitApi returns: " + ret.ToString());
            return ret;
        }
        public static int StartDrive(int inLeft, int inTop, int inWidth, int inHeight, bool inRunInForeground, bool inNoCaption, IntPtr inParentWnd)
        {
            ApplicationAPI.CApplicationAPI.ApplicationHandler AppHnd = new CApplicationAPI.ApplicationHandler(NavHandler);
            int ret = CApplicationAPI.InitApi(_myDrivePath, AppHnd, inLeft, inTop, inWidth, inHeight, inRunInForeground, inNoCaption, inParentWnd);
            O("InitApi returns: " + ret.ToString());
            return ret;
        }


        public static void StopDrive(int timeout_in)
        {
            int tmpResult1 = CApplicationAPI.EndApplication(out _mySError, timeout_in);
            if (tmpResult1 == 1)
                O("EndApplication was executed sucessfully, result=1");
            else
                O("EndApplication was NOT executed sucessfully, result=" + tmpResult1.ToString());
            int tmpResult2 = CApplicationAPI.CloseApi();
            if (tmpResult2 == 1)
                O("CloseApi was executed sucessfully, result=1");
            else
                O("CloseApi was NOT executed sucessfully, result=" + tmpResult2.ToString());
        }

        public static void NavHandler(int EventID, IntPtr strData)
        {
            if (NavigationNotificationEvent != null)
                NavigationNotificationEvent(EventID, strData);
        }

        #endregion

        #region Public members - Others

        public static void onMenuCommand(int inID, int inSubID) 
        {
            int ret = CApplicationAPI.OnMenuCommand(out _mySError, inID, inSubID, true, 0);
            O("OnMenuCommand returns: " + ret.ToString());
        }
        public static void onMenuCommand(int inID, int inSubID, bool inBShowApplication, int inMaxTime)
        {
            //inID    = (int)CApplicationMenu.ID.IdMenuViewRoute;
            //inSubID = (int)CApplicationMenu.IdMenuViewRoute.OnViewRouteShowOnMap;
            int ret = CApplicationAPI.OnMenuCommand(out _mySError, inID, inSubID, inBShowApplication, inMaxTime);
            O("OnMenuCommand returns: " + ret.ToString());
        }
        public static void LocationFromAddress(string inStrAddress, bool inBPostal, bool inBValueMatch, int inMaxTime)
        {
            LONGPOSITION location;
            int ret = CApplicationAPI.LocationFromAddress(out _mySError, out location, inStrAddress, inBPostal, inBValueMatch, inMaxTime);
            O("LocationFromAddressEx returns: " + ret.ToString());
            if (ret == 1) 
            {
                string tmpAddress;
                CApplicationAPI.GetLocationInfo(out _mySError, location, out tmpAddress, inMaxTime);
                O("x="+location.lX.ToString()+", y="+location.lY.ToString()+", address:"+tmpAddress);
            }
        }
        public static void LocationFromAddressEx(string inStrAddress, bool inBPostal, bool inBFuzzySearch, int inMaxTime) 
        {
            SWayPoint[] wps;
            string tmpString = "";
            int ret = CApplicationAPI.LocationFromAddressEx(out _mySError, inStrAddress, inBPostal, inBFuzzySearch, out wps, inMaxTime);
            O("LocationFromAddressEx returns: "+ret.ToString());
            if(wps!=null)
            for(int i=0; i<wps.Length; i++)
            {
                CApplicationAPI.GetLocationInfo(out _mySError, wps[i].Location, out tmpString, 0);
                O("x= " + wps[i].Location.lX + ", y= " + wps[i].Location.lY + ", address: " + tmpString);
            }
        }
        public static void DirectGeocoding(int inX, int inY, int inMaxTime) 
        {
            LONGPOSITION lp = new LONGPOSITION(inX, inY);
            string strAddress = "";
            SRoadInfo sri=new SRoadInfo();
            int ret = CApplicationAPI.GetLocationInfo(out _mySError, lp, out strAddress,out sri, inMaxTime);
            O("GetLocationInfo returns: "+ret+", errorcode: " + MySError.nCode.ToString());
            if (ret==1) 
            {
                O("lRoadOffset: " + sri.lRoadOffset.ToString());
                O("OnroadPosition: X:" + sri.OnroadPosition.lX.ToString() + " Y: " + sri.OnroadPosition.lY.ToString());
                O("OffroadDistance:" + sri.dwOffroadDistance);
                O("IsCongestionCharge: " + sri.IsCongestionCharge.ToString());
                O("IsFerry: " + sri.IsFerry.ToString());
                O("IsoCode: " + sri.IsoCode.ToString());
                O("IsPaved: " + sri.IsPaved.ToString());
                O("IsProhibited: " + sri.IsProhibited.ToString());
                O("IsTollRoad: " + sri.IsTollRoad.ToString());
                O("IsTunnel: " + sri.IsTunnel.ToString());
                O("IsUrban: " + sri.IsUrban.ToString());
                O("RoadClass: " + sri.RoadClass.ToString());
                O("SpeedCategory: " + sri.SpeedCategory.ToString());
                O("SpeedRestriction: " + sri.SpeedRestriction.ToString());
                O("strAddress: " + strAddress);
            }

            int ret2 = CApplicationAPI.ShowCoordinatesOnMap(out _mySError, lp, 2, true, 0);
            O("ShowCoordinatesOnMap returns: " + ret2.ToString());
        }
        public static void AddBitmapToMap(string inPathToBitmap, string inAddress, int inMaxTime) 
        {
            SError error;
            int lX, lY;
            int nBitmapID;
            LONGPOSITION Position;

            CApplicationAPI.LocationFromAddress(out error, out Position, inAddress, false, inMaxTime);
            lX = Position.lX;
            lY = Position.lY;

            int ret = CApplicationAPI.AddBitmapToMap(out error, inPathToBitmap, lX, lY, out nBitmapID, inMaxTime);
            O("AddBitmapToMap returns: " + ret.ToString());
        }

        public static void AddBitmapToMap(string inPathToBitmap, int inX, int inY, int inMaxTime)
        {
            SError error;
            int nBitmapID;

            int ret = CApplicationAPI.AddBitmapToMap(out error, inPathToBitmap, inX, inY, out nBitmapID, inMaxTime);
            O("AddBitmapToMap returns: " + ret.ToString());
        }

        public static void TripStart(string inTripName, string inDataMode, int inMaxTime) 
        {
            TripBookRecordDataMode tmpDataMode = TripBookRecordDataMode.tbdmNoData;
            switch(inDataMode)
            {
                case "tbdmNoData":
                    tmpDataMode = TripBookRecordDataMode.tbdmNoData;
                    break;
                case "tbdmFullData":
                    tmpDataMode = TripBookRecordDataMode.tbdmFullData;
                    break;
                case "tbdmFullNMEA":
                    tmpDataMode = TripBookRecordDataMode.tbdmFullNMEA;
                    break;
                default:
                    break;
            }

            int ret = CApplicationAPI.TripStart(out _mySError, inTripName, (int)tmpDataMode, inMaxTime);
            O("TripStart returns: "+ret.ToString());
        }
        public static void TripEnd(int inMaxTime) 
        {
            int ret = CApplicationAPI.TripEnd(out _mySError, inMaxTime);
            O("TripEnd returns: " + ret.ToString());
        }
        public static void TripAddUserEvent(string inEventName, uint inCustomID, int inMaxTime) 
        {
            int ret = CApplicationAPI.TripAddUserEvent(out _mySError, inEventName, inCustomID, inMaxTime);
            O("TripAddUserEvent returns: "+ret.ToString());
        }
        public static void BringApplicationToBackground() 
        {
            int ret = CApplicationAPI.BringApplicationToBackground(out _mySError, 0);
            O("BringApplicationToBackground returns: " + ret.ToString());
        }
        public static void BringApplicationToForeground() 
        {
            int ret = CApplicationAPI.BringApplicationToForeground(out _mySError, 0);
            O("BringApplicationToForeground returns: " + ret.ToString());
        }

        public static void AddPoiCategory(string inStrCategory,string inStrBitmapPath,string inStrIsoCode,int inMaxTime) 
        {
            int ret = CApplicationAPI.AddPoiCategory(out _mySError, inStrCategory, inStrBitmapPath, inStrIsoCode, inMaxTime);
            O("AddPooiCategory returns: " + ret.ToString());
        }
        public static void GetPoiCategoryList(int inMaxTime) 
        {
            SPoiCategory[] pc;
            int ret = CApplicationAPI.GetPoiCategoryList(out _mySError, out pc, inMaxTime);
            O("GetPoiCategoryList returns: " + ret.ToString()+", found categories: "+pc.Length);
            for (int i = 0; i < pc.Length; i++) 
                O("Category["+i.ToString() + "]: " + pc[i].Name);
        }
        public static string[] GetPoiCategoryList()
        {
            SPoiCategory[] pc;
            string[] list;
            int ret = CApplicationAPI.GetPoiCategoryList(out _mySError, out pc, 0);
            list = new string[pc.Length];
            for (int i = 0; i < pc.Length; i++)
                list[i] = pc[i].Name;
            return list;
        }
        public static void AddPoi(int inX, int inY, string inCategory, string inName, bool inSearch_address, int inMaxTime) 
        {
            LONGPOSITION lp;
            lp.lX = inX;
            lp.lY = inY;
            SPoi p = new SPoi(lp, inCategory, inName, inSearch_address ? 1 : 0);
            int ret = CApplicationAPI.AddPoi(out _mySError, ref p, inMaxTime);
            O("AddPoi returns: " + ret.ToString());
        }
        //AddPoi with LocationFromAddress
        public static void AddPoi(string inStrAddress, bool inBPostal, bool inBValueMatch, string inCategory, string inName, bool inSearch_address, int inMaxTime)
        {
            LONGPOSITION lp;
            int ret = CApplicationAPI.LocationFromAddress(out _mySError, out lp, inStrAddress, inBPostal, inBValueMatch, inMaxTime);
            O("LocationFromAddress returns: " + ret.ToString());
            SPoi p = new SPoi(lp, inCategory, inName, inSearch_address ? 1 : 0);
            ret = CApplicationAPI.AddPoi(out _mySError, ref p, inMaxTime);
            O("AddPoi returns: " + ret.ToString());
        }
        public static void GetRouteInfo(int inMaxTime) 
        {
            SRouteInfo RouteInfo = new SRouteInfo();
            int ret = CApplicationAPI.GetRouteInfo(out _mySError, out RouteInfo, inMaxTime);

            ushort years;
            byte months, days, hours, minutes, seconds;
            RouteInfo.GetEstimatedTimeArival(out years, out months, out days, out hours, out minutes, out seconds);

            O("GetRouteInfo returns: " + ret.ToString());
            O("RouteInfo.BoundaryRectangle(lBottom,lLeft,lRight,lTop): " + RouteInfo.BoundaryRectangle.lBottom.ToString()
                +","+RouteInfo.BoundaryRectangle.lLeft.ToString()
                +","+RouteInfo.BoundaryRectangle.lRight.ToString()
                +","+RouteInfo.BoundaryRectangle.lTop.ToString());
            O("RouteInfo.EstimatedTimeArival: " + RouteInfo.EstimatedTimeArival.ToString() 
                + ", " + years.ToString()
                + "-" + months.ToString()
                + "-" + days.ToString()
                + ", " + hours.ToString()
                + ":" + minutes.ToString()
                + ":" + seconds.ToString());
            O("RouteInfo.FerriesLength: " + RouteInfo.FerriesLength.ToString());
            O("RouteInfo.MotorwaysLength: " + RouteInfo.MotorwaysLength.ToString());
            O("RouteInfo.RemainingDistance: " + RouteInfo.RemainingDistance.ToString());
            O("RouteInfo.RemaningTime: " + RouteInfo.RemaningTime.ToString());
            O("RouteInfo.Status: " + RouteInfo.Status.ToString());
            O("RouteInfo.TollRoadsLength: " + RouteInfo.TollRoadsLength.ToString());
            O("RouteInfo.TotalDistance: " + RouteInfo.TotalDistance.ToString());
            O("RouteInfo.TotalTime: " + RouteInfo.TotalTime.ToString());
        }
        public static string GetRouteInfoTest(string inItineraryName, int inMaxTime)
        {
            SRouteInfo RouteInfo = new SRouteInfo();
            int ret = CApplicationAPI.GetRouteInfo(out _mySError, out RouteInfo, inMaxTime);

            ushort years;
            byte months, days, hours, minutes, seconds;
            RouteInfo.GetEstimatedTimeArival(out years, out months, out days, out hours, out minutes, out seconds);

            string output = inItineraryName + ";";
            output = output + RouteInfo.TotalDistance.ToString() + ";";
            output = output + RouteInfo.TotalTime.ToString();
            return output;
        }
        public static void GetItineraryList(string inStrItineraryName, int inMaxTime) 
        {
            SStopOffPoint[] points;
            CApplicationAPI.GetItineraryList(out _mySError, inStrItineraryName, out points, inMaxTime);
            if(points!=null)
                for (int i = 0; i < points.Length; i++)
                    O(" " + i.ToString()
                         + ". point type:" + points[i].nPointType
                         + ", visited:" + points[i].bVisited
                         + ", x=" + points[i].Location.lX
                         + ", y=" + points[i].Location.lY
                         + ", caption:" + points[i].GetCaption()
                         + ", Address:" + points[i].GetAddress()
                         );
        }
        public static void LoadComputedRoute(string inRoutePath, int inMaxValue)
        {
            int ret = CApplicationAPI.LoadComputedRoute(out _mySError, inRoutePath, inMaxValue);
            O("LoadComputedRoute returns: " + ret);
        }

        public static void GetChangeOption(int inMaxTime, out string outOption) 
        {
            int ret = CApplicationAPI.GetApplicationOptions(out _mySError, out outOption, inMaxTime);
            O("GetApplicationOptions returns: " + ret.ToString());
        }

        public static void SetChangeOption(int inMaxTime, string inOption)
        {
            int ret = CApplicationAPI.SetApplicationOptions(out _mySError, inOption, inMaxTime);
            O("SetApplicationOptions returns: " + ret.ToString());
        }

        public static void AddTMCEvent(
            int inBUserAvoid, 
            int inLX, 
            int inLY,
            int inNEventCode,
            byte inNValidityDay,
            byte inNValidityHour,
            byte inNValidityMinute,
            byte inNValidityMonth,
            uint inNValidityYear,
            byte inWID,
            int inMaxTime) 
        {
            STmcEvent tmc = new STmcEvent();
            tmc.bUserAvoid = inBUserAvoid;
            tmc.lX = inLX;
            tmc.lY = inLY;
            tmc.nEventCode = inNEventCode;
            tmc.nValidityDay = inNValidityDay;
            tmc.nValidityHour = inNValidityHour;
            tmc.nValidityMinute = inNValidityMinute;
            tmc.nValidityMonth = inNValidityMonth;
            tmc.nValidityYear = inNValidityYear;
            tmc.wID = inWID;

            int ret = CApplicationAPI.AddTMCEvent(out _mySError, ref tmc, inMaxTime);
            O("AddTMCEvent returns: " + ret.ToString());
        }

        public static void GetNextInstruction(int inMaxTime) 
        {
            SRouteInstruction instr = new SRouteInstruction();
            int ret = CApplicationAPI.GetNextInstruction(out _mySError, out instr, inMaxTime);
            O("GetNextInstruction returns: " + ret);
            O("ExitIndex: " + instr.ExitIndex.ToString());
            O("lDistanceToNextTurn: " + instr.lDistanceToNextTurn.ToString());
            O("lNextTurnX: " + instr.lNextTurnX.ToString());
            O("lNextTurnY: " + instr.lNextTurnY.ToString());
            O("nInstruction: " + instr.nInstruction.ToString());
            O("nRoundaboutExitIndex: " + instr.nRoundaboutExitIndex.ToString());
        }

        public static void GetNextInstruction(int inMaxTime, out string[] outArr)
        {
            SRouteInstruction outInstr = new SRouteInstruction();
            int ret = CApplicationAPI.GetNextInstruction(out _mySError, out outInstr, inMaxTime);

            string[] arr = new string[6];
            arr[0] = outInstr.ExitIndex.ToString();
            arr[1] = outInstr.lDistanceToNextTurn.ToString();
            arr[2] = outInstr.lNextTurnX.ToString();
            arr[3] = outInstr.lNextTurnY.ToString();
            arr[4] = outInstr.nInstruction.ToString();
            arr[5] = outInstr.nRoundaboutExitIndex.ToString();
            outArr = arr;
        }

        public static void StartNavigation(int inlX, int inlY, int inFlags, bool inbShowApplication, bool inbSearchAddress, int inMaxTime) 
        {
            LONGPOSITION lp = new LONGPOSITION(inlX, inlY);
            SWayPoint wp = new SWayPoint();
            wp.Location = lp;
            int ret = CApplicationAPI.StartNavigation(out _mySError, ref wp, inFlags, inbShowApplication, inbSearchAddress, inMaxTime);
            O("StartNavigation returns: " + ret.ToString() + ", errorcode: " + _mySError.nCode.ToString());
        }

        public static void StopNavigation(int inMaxTime)
        {
            int ret = CApplicationAPI.StopNavigation(out _mySError, inMaxTime);
            O("StopNavigation returns: " + ret.ToString() + ", errorcode: " + _mySError.nCode.ToString());
        }

        public static void GetMapVersion(string inStrIso, int inMaxTime)
        {
            string strMapVersion = "";
            int ret = CApplicationAPI.GetMapVersion(out _mySError, inStrIso, out strMapVersion, inMaxTime);
            O("GetMapVersion returns: " + ret.ToString());
            O("StrMapVersion: " + strMapVersion);
        }

        public static void FindNearbyPoi(int inListSize, int inCategoryNumber, string inStrCategoryName, int inLX, int inLY, int inMaxTime) 
        {
            SPoi[] pois = new SPoi[0];
            int ret = CApplicationAPI.FindNearbyPoi(out _mySError, out pois, ref inListSize, inCategoryNumber, inStrCategoryName, inLX, inLY, inMaxTime);
            O("FindNearbyPoi returns: "+ret.ToString());
            if(pois!=null)
                for (int i = 0; i < pois.Length; i++)
                    O(" " + i.ToString() + ". " + pois[i].GetName() + ", " + pois[i].GetAddress());
        }

        public static void CloseDialogs() 
        {
            int ret = CApplicationAPI.CloseDialogs(out _mySError, 0);
            O("CloseDialogs returns: " + ret.ToString());
        }
        public static void NavigateToAddress(string inAddress, bool inBPostal, int inFlags, bool inBShowpplication, int inMaxTime) 
        {
            int ret = CApplicationAPI.NavigateToAddress(out _mySError, inAddress, inBPostal, inFlags, inBShowpplication, inMaxTime);
            O("NavigateToAddress returns: " + ret.ToString());
        }

        public static void EnableExternalGpsInput() 
        {
            CApplicationAPI.EnableExternalGpsInput(out _mySError, 0);
        }

        public static void SendGpsData(string inLine) 
        {
            CApplicationAPI.SendGpsData(inLine);
        }

        public static void DisableExternalGpsInput() 
        {
            CApplicationAPI.DisableExternalGpsInput(out _mySError, 0);
        }

        public static void PlaySoundTTS(string inText, int inTime) 
        {
            int ret = CApplicationAPI.PlaySoundTTS(out _mySError, inText, inTime);
            O("PlaySoundTTS returns: " + ret.ToString());
        }

        public static void GetPoiList(string inStrCategory, bool inBSearchAddress, int inMaxTime) 
        {
            SPoi[] pois;
            int ret = CApplicationAPI.GetPoiList(out _mySError, out pois, inStrCategory, inBSearchAddress, inMaxTime);
            O("GetPoiList returns: " + ret.ToString() + "error: "+_mySError.nCode.ToString());
            if(pois!=null)
                for (int i = 0; i < pois.Length; i++) 
                {
                    O(i.ToString() + ". poi: " + pois[i].GetName() + ", " + pois[i].GetAddress());
                }
        }

        public static void GetUniqueDeviceId() 
        {
            string strDeviceId;
            CApplicationAPI.GetUniqueDeviceId(out _mySError, out strDeviceId, 0);
            O("GetUniqueDeviceId returns: " + strDeviceId);
        }

        public static void RemoteActivationViaHttpRequest()
        {
            string strDeviceId;
            CApplicationAPI.GetUniqueDeviceId(out _mySError, out strDeviceId, 0);
            O("GetUniqueDeviceId returns: " + strDeviceId);


            //string productId = "2324";
            //string sdPassword = "6E193DBF";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.sygic.com/partners/activate.php");
            string postData = "type=2&SD_password=6E193DBF&product_ID=2324&device_code="+strDeviceId+"&description=Skuska";
            byte[] data = Encoding.ASCII.GetBytes(postData.ToCharArray());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            O(responseString);
        }

        public static void AddItineraryTest() 
        {
            SStopOffPoint[] points = new SStopOffPoint[3];
            points[0] = new SStopOffPoint();
            points[1] = new SStopOffPoint();
            points[2] = new SStopOffPoint();

            points[0].SetCaption("1. waypoint: start");
            points[0].nPointType = 3; //3=start point
            points[1].SetCaption("2. waypoint: via point");
            points[1].nPointType = 1; //1=waypoint            
            points[2].SetCaption("3. waypoint: finish");
            points[2].nPointType = 2; //2=finish pint
            int ret = CApplicationAPI.AddItinerary(out _mySError, points, "testing", 0);
            O("AddItinerary returns: " + ret.ToString());
        }

        public static void AddItinerary(string inStrName)
        {
            SStopOffPoint[] points = new SStopOffPoint[2];

            points[0] = new SStopOffPoint();
            points[0].Location.lX = 1712574;
            points[0].Location.lY = 4815034;
            points[0]. nPointType = 3;
            points[0].SetCaption("Starting point");

            points[1] = new SStopOffPoint();
            points[1].Location.lX = 1714375;
            points[1].Location.lY = 4814980;
            points[1].nPointType = 2;
            points[1].SetCaption("Finish point");

            int ret = CApplicationAPI.AddItinerary(out _mySError, points, inStrName, 0);
            O("AddItinerary returns: " + ret.ToString());
            if (ret == 1)
                O("The itinerary containing two waypoints has been added into the navigation sucessfully.");
        }

        public static void AddItinerary(string inStrName, SStopOffPoint[] points)
        {
            int ret = CApplicationAPI.AddItinerary(out _mySError, points, inStrName, 0);
            O("AddItinerary returns: " + ret.ToString());
        }

        public static void AddEntryToItinerary(string inStrItineraryName, int inLX, int inLY, int inNPointType, string inCaption, int inNIndex, uint inId)
        {
            SStopOffPoint point= new SStopOffPoint();
            point.Location.lX = inLX;
            point.Location.lY = inLY;
            point.nPointType = inNPointType;
            point.SetCaption(inCaption);
            point.Id = inId;

            int ret = CApplicationAPI.AddEntryToItinerary(out _mySError, inStrItineraryName, ref point, inNIndex, 0);
            O("AddEntryToItinerary returns: " + ret.ToString());
        }

        public static void DeleteItinerary(string inStrItineraryName, int inMaxTime)
        {
            int ret = CApplicationAPI.DeleteItinerary(out _mySError, inStrItineraryName, inMaxTime);
            O("DeleteItinerary returns: " + ret.ToString());
        }

        public static void SkipNextWaypoint() 
        {
            int ret = CApplicationAPI.SkipNextWaypoint(out _mySError, 0);
            O("SkipNextWaypoint returns: " + ret.ToString());
        }

        public static void SetRoute(string inStrItineraryName, int inFlags, bool inBShowApplication, int inMaxTime) 
        {
            int ret = CApplicationAPI.SetRoute(out _mySError, inStrItineraryName, inFlags, inBShowApplication, inMaxTime);
            O("SetRoute returns: " + ret.ToString());
        }

        public static void OptimizeItinerary(string inStrItineraryName, int inMaxTime)
        {
            int ret = CApplicationAPI.OptimizeItinerary(out _mySError, inStrItineraryName, inMaxTime);
            O("OptimizeItinerary returns: " + ret.ToString());
        }

        public static void GetActualGpsPosition(bool inSatellitesInfo, int inMaxTime) 
        {
            SGpsPosition gpos = new SGpsPosition();
            int ret = CApplicationAPI.GetActualGpsPosition(out _mySError, out gpos, inSatellitesInfo, inMaxTime);
            O("GetActualGpsPosition returns: " + ret.ToString());
            //O("Altitude: " + gpos.Altitude.ToString());
            O("Course: " + gpos.Course.ToString());
            //O("Date: " + gpos.Date.ToString());
            //O("FromPointOffset: " + gpos.FromPointOffset.ToString());
            O("HDoP: " + gpos.HDoP.ToString());
            O("Latitude: " + gpos.Latitude.ToString());
            O("Longitude: " + gpos.Longitude.ToString());
            //O("MapIso: " + gpos.MapIso.ToString());
            O("RealCourse: " + gpos.RealCourse.ToString());
            O("RoadOffset: " + gpos.RoadOffset.ToString());
            O("Satellites:" + gpos.Satellites.ToString());
            //for (int i = 0; i < gpos.Satellites; i++ )
            //    O(i.ToString() + ". sat: " 
            //        + gpos.satellitesInfo[i].Azimuth.ToString() + ", " 
            //        + gpos.satellitesInfo[i].Elevation.ToString() + ", " 
            //        + gpos.satellitesInfo[i].Quality.ToString() + ", "
            //        + gpos.satellitesInfo[i].SateliteId.ToString() + ", " 
            //        + gpos.satellitesInfo[i].UsedForFix.ToString());
            O("Speed: " + gpos.Speed.ToString());
            //O("Time: " + gpos.Time.ToString());
            //O("ToPointOffset: " + gpos.ToPointOffset.ToString());
        }

        public static void GetCurrentSpeedLimit(int inMaxTime)
        {
            int nSpeedLimit=0;
            int ret = CApplicationAPI.GetCurrentSpeedLimit(out _mySError, out nSpeedLimit, inMaxTime);
            O("GetCurrentSpeedLimit returns: " + ret.ToString() + ", nSpeedLimit= " + nSpeedLimit.ToString());
        }

        public static void AutomaticRemoteActivation(string inActivateExePath, string inMlmPath, string inSdPassword) 
        {
            string strDeviceID = "";
            int ret = CApplicationAPI.GetUniqueDeviceId(out _mySError, out strDeviceID, 0);

            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = inActivateExePath;
            process.Arguments = "\"" + inMlmPath + "\" " + inSdPassword;
            Process.Start(process);
        }

        public static void LoadExternalFile(string inPath, int inFileType, int inMaxTime)
        {
            int ret = CApplicationAPI.LoadExternalFile(out _mySError, inPath, inFileType, inMaxTime);
            O("LoadExternalFile: " + ret.ToString());
        }

        public static void ChangeAppRectangle(int inNLeft, int inNTop, int inNWidth, int inNHeight, int inMaxTime) 
        {
            int ret = CApplicationAPI.ChangeAppRectangle(out _mySError, inNLeft, inNTop, inNWidth, inNHeight, inMaxTime);
            O("ChangeAppRectangle returns: " + ret.ToString());
        }

        public static void SwithMap(string inStrLoadPath, int inMaxTime) 
        {
            int ret = CApplicationAPI.SwitchMap(out _mySError, inStrLoadPath, inMaxTime);
            O("SwitchMap returns: " + ret.ToString());
        }

        public static void ShowMessage(string inMessage, int inButtons, bool inWaitForFeedback,bool inBShowApplication,  int inMaxTime)
        {
            int inNUserFeedback = 0;
            int ret = CApplicationAPI.ShowMessage(out _mySError, inMessage, inButtons, inWaitForFeedback, inBShowApplication,ref inNUserFeedback, inMaxTime);
            O("ShowMessage: " + ret.ToString() + ", nUserFeedback: " + inNUserFeedback);
        }

        public static void GetCoordinatesFromOffset(string inIsoCode, int inRoadOffset, int inMaxTime)
        {
            LONGPOSITION lp = new LONGPOSITION();
            int ret = CApplicationAPI.GetCoordinatesFromOffset(out _mySError, inIsoCode, inRoadOffset, out lp, inMaxTime);
            O("GetCoordinatesFromOffset returns: " + ret.ToString());
            O("x="+lp.lX.ToString() + ", y=" + lp.lY.ToString());
        }

        public static string GetCoordinatesFromListOfOffsetIds(string inPath)
        {
            StringBuilder s = new StringBuilder("");
            if (File.Exists(inPath))
            {
                LONGPOSITION lp = new LONGPOSITION();
                string tmp = "";
                string[] parameters = new string[2];

                StreamReader sr = new StreamReader(inPath);
                while (!sr.EndOfStream)
                {
                    tmp = sr.ReadLine();
                    parameters = tmp.Split(';');
                    CApplicationAPI.GetCoordinatesFromOffset(
                        out _mySError,
                        parameters[0],
                        int.Parse(parameters[1]),
                        out lp,
                        0);
                    s.AppendLine(lp.lX.ToString() + ";" + lp.lY.ToString());
                }
                sr.Close();
            }
            return s.ToString();
        }

        public static void ItineraryGenerator(int inNumberOfItineraries, int inNumberOfWaypointsInItinerary, string inPrefix, int inXmin, int inXmax, int inYmin, int inYmax)
        {
            Random r = new Random();
            for (int j = 0; j < inNumberOfItineraries; j++)
            {
                SStopOffPoint[] wp = new SStopOffPoint[inNumberOfWaypointsInItinerary];
                for (int i = 0; i < inNumberOfWaypointsInItinerary; i++)
                {
                    wp[i] = new SStopOffPoint();
                    wp[i].Id = (uint)i;
                    wp[i].SetCaption(i.ToString());
                    wp[i].nPointType = 1;
                    if (i == 0)
                        wp[i].nPointType = 3;
                    if (i == (inNumberOfWaypointsInItinerary - 1))
                        wp[i].nPointType = 2;
                    wp[i].Location.lX = inXmin + (int)r.Next(inXmax-inXmin);
                    wp[i].Location.lY = inYmin + (int)r.Next(inYmax-inYmin);
                }

                DriveHandler.AddItinerary(inPrefix + j.ToString(), wp);                
            }
        }

        public static bool GpsLogLoad(string inPath)
        {
            if (File.Exists(inPath))
            {
                gpsLogPosition = 0;
                StreamReader sr = new System.IO.StreamReader(inPath);
                while (!sr.EndOfStream)
                {
                    sr.ReadLine();
                    gpsLogPosition++;
                }
                sr.Close();

                gpsLog = new string[gpsLogPosition];
                sr = new System.IO.StreamReader(inPath);
                for (int i = 0; i < gpsLogPosition; i++)
                {
                    gpsLog[i] = sr.ReadLine();
                }
                sr.Close();
                gpsLogPosition = 0;

                return true;
            }
            return false;         
        }

        //public static void DevideItinerary(string inPath)
        //{
        //    SStopOffPoint[] wp;
        //    CApplicationAPI.GetItineraryList(out _mySError, inPath, out wp, 0);

        //    if (wp != null)
        //    {
        //        for (int i = 0; i < wp.Length - 2; i++)
        //        {
        //            SStopOffPoint[] tmpWp = new SStopOffPoint[3];
        //            tmpWp[0] = wp[i];
        //            tmpWp[0].nPointType = 3;

        //            tmpWp[1] = wp[i + 1];
        //            tmpWp[1].nPointType = 1;

        //            tmpWp[2] = wp[i + 2];
        //            tmpWp[2].nPointType = 2;
        //            O("Itinerary name: " + inPath + i.ToString());
        //            DriveHandler.AddItinerary(inPath + i.ToString(), tmpWp);
        //        }
        //    }
        //}

        public static void DevideItinerary(string inPath)
        {
            SStopOffPoint[] wp;
            CApplicationAPI.GetItineraryList(out _mySError, inPath, out wp, 0);

            if (wp != null)
            {
                for (int i = 0; i < wp.Length - 1; i++)
                {
                    SStopOffPoint[] tmpWp = new SStopOffPoint[2];
                    tmpWp[0] = wp[i];
                    tmpWp[0].nPointType = 3;

                    tmpWp[1] = wp[i + 1];
                    tmpWp[1].nPointType = 2;
                    O("Itinerary name: " + inPath + i.ToString() + ", " + tmpWp[1].GetAddress());
                    DriveHandler.AddItinerary(inPath + i.ToString(), tmpWp);
                }
            }
        }

        //public static void DevideItineraryAndCompute(string inPath)
        //{
        //    SStopOffPoint[] wp;
        //    CApplicationAPI.GetItineraryList(out _mySError, inPath, out wp, 0);

        //    if (wp != null)
        //    {
        //        for (int i = 0; i < wp.Length - 2; i++)
        //        {
        //            SStopOffPoint[] tmpWp = new SStopOffPoint[3];
        //            tmpWp[0] = wp[i];
        //            tmpWp[0].nPointType = 3;

        //            tmpWp[1] = wp[i + 1];
        //            tmpWp[1].nPointType = 1;

        //            tmpWp[2] = wp[i + 2];
        //            tmpWp[2].nPointType = 2;
        //            O("Itinerary name: " + inPath + i.ToString());
        //            DriveHandler.AddItinerary(inPath + i.ToString(), tmpWp);
        //            DriveHandler.SetRoute(inPath + i.ToString(), 0, true, 0);
        //        }
        //    }
        //}

        public static void DevideItineraryAndCompute(string inPath)
        {
            SStopOffPoint[] wp;
            CApplicationAPI.GetItineraryList(out _mySError, inPath, out wp, 0);

            if (wp != null)
            {
                for (int i = 0; i < wp.Length - 2; i++)
                {
                    SStopOffPoint[] tmpWp = new SStopOffPoint[2];
                    tmpWp[0] = wp[i];
                    tmpWp[0].nPointType = 3;

                    tmpWp[1] = wp[i + 1];
                    tmpWp[1].nPointType = 2;
                    O("Itinerary name: " + inPath + i.ToString() + ", " + tmpWp[1].GetCaption());
                    DriveHandler.AddItinerary(inPath + i.ToString(), tmpWp);
                    DriveHandler.SetRoute(inPath + i.ToString(), 0, true, 0);
                }
            }
        }

        public static List<int> DetectWrongWaypoints(string inPath) //inPath = name of itinerary (i.e. "default")
        {
            SStopOffPoint[] wp;
            List<int> ids = new List<int>();
            CApplicationAPI.GetItineraryList(out _mySError, inPath, out wp, 0);

            if (wp != null)
            {
                for (int i = 0; i < wp.Length - 1; i++)
                {
                    SStopOffPoint[] tmpWp = new SStopOffPoint[2];
                    tmpWp[0] = wp[i];
                    tmpWp[0].nPointType = 3;

                    tmpWp[1] = wp[i + 1];
                    tmpWp[1].nPointType = 2;

                    CApplicationAPI.AddItinerary(out _mySError, tmpWp, inPath + "-" + i.ToString(), 0);
                    int result = CApplicationAPI.SetRoute(out _mySError, inPath + "-" + i.ToString(), 0, true, 0);
                    if (result != 1)
                        ids.Add(i + 1);
                }
            }
            return ids;
        }

        public static void FlashMessage(string inStrMessage, bool inBShowApplication, int inMaxTime)
        {
            int ret = CApplicationAPI. FlashMessage(out _mySError, inStrMessage, inBShowApplication, inMaxTime);
            O("FlashMessage returns: " + ret.ToString());
        }

        public static void ShowRectangleOnMap(int inLBottom, int inLLeft, int inLRight, int inLTop, bool inBShowApplication, int inMaxTime)
        {
            LONGRECT r;
            r.lBottom = inLBottom;
            r.lLeft = inLLeft;
            r.lRight = inLRight;
            r.lTop = inLTop;
            int ret = CApplicationAPI.ShowRectangleOnMap(out _mySError, r, inBShowApplication, inMaxTime);
            O("ShowRectangleOnMap returns: " + ret.ToString());
        }

        public static void HighlightPoi(string inPoiCategory, string inPoiName, string inSound, int inX, int inY, int inMaxTime)
        {
            LONGPOSITION lp = new LONGPOSITION(inX, inY);
            int ret = CApplicationAPI.HighlightPoi(out _mySError, inPoiCategory, inPoiName, inSound, lp, inMaxTime);
            O("HighlightPoi returns: " + ret.ToString());
        }

        public static void LoadGFFile(string inPath, int inMaxTime)
        {
            int ret = CApplicationAPI.LoadGFFile(out _mySError, inPath, inMaxTime);
            O("LoadGFFile returns: " + ret.ToString());
        }

        public static void UnloadGFFile(string inPath, int inMaxTime)
        {
            int ret = CApplicationAPI.UnloadGFFile(out _mySError, inPath, inMaxTime);
            O("UnloadGFFile returns: " + ret.ToString());
        }

        public static void DeleteEntryInItinerary(string inStrItineraryName, int inNIndex, int inMaxTime)
        {
            int ret = CApplicationAPI.DeleteEntryInItinerary(out _mySError, inStrItineraryName, inNIndex, inMaxTime);
            O("DeleteEntryInItinerary returns: " + ret.ToString());
        }

        #endregion

    }

}
