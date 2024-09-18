using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using ApplicationAPI;
using Newtonsoft.Json.Linq;

namespace SYGIC_PROFESSINAL_SDK_DEMO
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
            DriveHandler.NavigationNotificationEvent += new DriveHandler.NavigationNotificationDelegate(NavHandlerLocal);
            DriveHandler.NotificationEvent += new DriveHandler.NotificationDelegate(OnNotification);
            FillComboBoxesFromIniFile();
            EventsChLB.SetItemChecked(13,true);
            EventsChLB.SetItemChecked(18,true);
            EventsChLB.SetItemChecked(19,true);
        }

        IntPtr hwnd;
        bool isRunning = false;
        private void MyThread()
        {
            int tmpResult=0;
            tmpResult = DriveHandler.StartDrive(
                            0, 
                            0, 
                            P.Width, 
                            P.Height, 
                            StartCustom_bRunInForegroundChb.Checked, 
                            StartCustom_bNoCaptionChB.Checked,
                            hwnd);
            if (tmpResult==1)
                isRunning = true;
            else
                isRunning = false;
        }

        private void MyThreadShowMessage() 
        {
            DriveHandler.ShowMessage(
                ShowMessage2_MessageTBox.Text,
                (int)ShowMessage2_ButtonsNum.Value,
                ShowMessage2_BWaitForFeedbackChB.Checked,
                ShowMessage2_BShowApplicationChB.Checked,
                (int)ShowMessage2_MaxtimeNum.Value);
        }
        private void MyThreadDetectWrongWP()
        {
            List<int> ids = DriveHandler.DetectWrongWaypoints(DevideItinerary_PathTBox.Text);
            if (ids != null)
                for (int i = 0; i < ids.Count; i++)
                {
                    O("Wrong waypoint at position: " + ids[i].ToString());
                }
            O("The detection process has finished.");
        }

        private void StartDriveBtn_Click(object sender, EventArgs e)
        {
            if(File.Exists(MyDrivePathCB.Text))
            {
                DriveHandler.MyDrivePath = MyDrivePathCB.Text;
                MyDrivePathCB.Items.Add(MyDrivePathCB.Text);
                WriteComboBoxesIntoIniFile();

                if(isRunning)
                    DriveHandler.StopDrive(0);
                if (ScreenResolutionR1_RBtn.Checked)     //embedded
                {
                    hwnd = P.Handle;
                    Thread oThread = new Thread(new ThreadStart(MyThread));
                    oThread.Start();
                }
                else
                    if (ScreenResolutionR2_RBtn.Checked)     //in separate window
                    {
                        DriveHandler.StartDrive(
                            (int)LeftNum.Value, 
                            (int)TopNum.Value, 
                            (int)WidthNum.Value, 
                            (int)HeightNum.Value, 
                            StartCustom_bRunInForegroundChb.Checked, 
                            StartCustom_bNoCaptionChB.Checked);
                    }
                ApiAI_ToolStripMenuItem.Visible = true;
                ApiJZ_ToolStripMenuItem.Visible = true;
                toolsToolStripMenuItem.Visible = true;
            }
        }

        private void ShowDialogBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.showDialog(Convert.ToInt32(ShowDialog_IdNum.Value), Convert.ToInt32(ShowDialog_MaxTimeNum.Value));
        }
        private void OnNotification(string errorString_in)
        {
            if (this.InvokeRequired)
            {
                DriveHandler.NotificationDelegate del = new DriveHandler.NotificationDelegate(OnNotification);
                Invoke(del, new object[] { errorString_in });
            }
            else
            {
                O(errorString_in);
            }
        } // If there is something to write out, first we make it threadsafe and only after that is the exact method calles

        //parse EVENT_POI_CLICK  strData
        private string[] parse(IntPtr strData) 
        {
            StringBuilder[] sb = new StringBuilder[4];
            unsafe
            {
                char* pStr = (char*)strData.ToPointer();
                if (pStr != null)
                {
                    char ch;
                    while ((ch = *pStr++) != (char)0);
                }
            }

            string s;
            string[] arr = new string[4];

            int l, p;
            s = sb.ToString();

            for (int i = 0; i < arr.Length; i++)
            {
                l = s.IndexOf("{");
                p = s.IndexOf("}");
                arr[i] = s.Substring(l + 1, p - 1);
                s = s.Remove(l, p + 1);
            }
            return arr;
        }

        //parse EVENT_BITMAP_CLICK/EVENT_WAIPOINT_VISITED
        private string parse2(IntPtr strData)
        {
            StringBuilder sb = new StringBuilder();
            unsafe
            {
                char* pStr = (char*)strData.ToPointer();
                if (pStr != null)
                {
                    char ch;
                    while ((ch = *pStr++) != (char)0)
                        sb.Append(ch);
                }
            }

            string s;
            string[] arr = new string[1];
            s = sb.ToString();
            return s;
        }

        //parse EVENT_POI_Warning strData
        private StringBuilder[] parse3(IntPtr strData)
        {
            StringBuilder[] sb = new StringBuilder[4];
            for (int i = 0; i < sb.Length; i++)
                sb[i] = new StringBuilder();

            unsafe
            {
                char* pStr = (char*)strData.ToPointer();
                if (pStr != null)
                {
                    char ch;
                    while ((ch = *pStr++) != (char)0)
                        if (ch != ',') 
                            sb[0].Append(ch);
                        else
                            while ((ch = *pStr++) != (char)0)
                                if (ch != ',')
                                    sb[1].Append(ch);
                                else
                                {
                                    ch = *pStr++;
                                    while ((ch = *pStr++) != (char)0)
                                        if (ch != ',')
                                            sb[2].Append(ch);
                                        else
                                            while ((ch = *pStr++) != (char)0)
                                                if (ch != '}')
                                                    sb[3].Append(ch);
                                }
                }
            }
            return sb;
        }

        public void NavHandlerLocal(int nEventId, IntPtr strData) 
        {
                //cast the eventID integer representation to ApplicationEvents enumeration
                ApplicationEvents anAppEvent = (ApplicationEvents)nEventId;

                //handle event.
                switch (anAppEvent) 
                {
                    case ApplicationEvents.EVENT_ROUTE_USERCANCEL:
                        if (EventsChLB.GetItemChecked(0))
                            O("EVENT_ROUTE_USERCANCEL has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_WAYPOINT_VISITED:
                        if (EventsChLB.GetItemChecked(1))
                        {
                            O("EVENT_WAYPOINT_VISITED has been invoked.");
                            string s = parse2(strData);
                            O("Waypoint Id: " + s);
                        }
                        break;
                    case ApplicationEvents.EVENT_ROUTE_FINISH:
                        if (EventsChLB.GetItemChecked(2))
                            O("EVENT_ROUTE_FINISH has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_ROUTE_COMPUTED:
                        if (EventsChLB.GetItemChecked(3))
                            O("EVENT_ROUTE_COMPUTED has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_OFF_ROUTE:
                        if (EventsChLB.GetItemChecked(4))
                            O("EVENT_OFF_ROUTE has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_APP_EXIT:
                        if (EventsChLB.GetItemChecked(5))
                            O("EVENT_APP_EXIT has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_MAIN_MENU:
                        if (EventsChLB.GetItemChecked(6))
                            O("EVENT_MAIN_MENU has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_CONTEXT_MENU:
                        if (EventsChLB.GetItemChecked(7))
                            O("EVENT_CONTEXT_MENU has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_EXIT_MENU:
                        if (EventsChLB.GetItemChecked(8))
                            O("EVENT_EXIT_MENU has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_CUSTOM_MENU:
                        if (EventsChLB.GetItemChecked(9))
                            O("EVENT_CUSTOM_MENU has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_CHANGE_LANGUAGE:
                        if (EventsChLB.GetItemChecked(10))
                            O("EVENT_CHANGE_LANGUAGE has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_CHANGE_ORIENTATION:
                        if (EventsChLB.GetItemChecked(11))
                            O("EVENT_CHANGE_ORIENTATION has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_RADAR_WARNING:
                        if (EventsChLB.GetItemChecked(12))
                            O("EVENT_RADAR_WARNING has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_POI_WARNING:
                        if (EventsChLB.GetItemChecked(13))
                        {
                            O("EVENT_POI_WARNING has been invoked.");
                            StringBuilder[] arr2 = parse3(strData);
                            O(String.Format("Clicked on poi nr: {0}, name: {1}, coordinates(x,y): {2}, {3} (multiplied by 100000)", arr2));
                        }
                        break;
                    case ApplicationEvents.EVENT_GEOFENCE:
                        if (EventsChLB.GetItemChecked(14))
                        {
                            O("EVENT_GEOFENCE has been invoked.");
                            string s = parse2(strData);
                            O(s);
                        }
                        break;
                    case ApplicationEvents.EVENT_RESTRICTED_ROAD:
                        if (EventsChLB.GetItemChecked(15))
                            O("EVENT_RESTRICTED_ROAD has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_BORDER_CROSSING:
                        if (EventsChLB.GetItemChecked(16))
                            O("EVENT_BORDER_CROSSING has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_SPEED_EXCEEDING:
                        if (EventsChLB.GetItemChecked(17))
                            O("EVENT_SPEED_EXCEEDING has been invoked.");
                        break;
                    case ApplicationEvents.EVENT_SPEED_LIMIT_CHANGED:
                        if (EventsChLB.GetItemChecked(18))
                        {
                            O("EVENT_SPEED_LIMIT_CHANGED has been invoked.");
                            string s = parse2(strData);
                            O("Speed limit changed to: " + s);
                        }
                    break;
                    case ApplicationEvents.EVENT_BITMAP_CLICK:
                        if (EventsChLB.GetItemChecked(19))
                        {
                            O("EVENT_BITMAP_CLICK has been invoked.");
                            string s = parse2(strData);
                            O("EVENT_BITMAP_CLICK has been invoked. Clicked on bitmap: " + s);
                        }
                        break;
                    case ApplicationEvents.EVENT_POI_CLICK:
                        if (EventsChLB.GetItemChecked(20))
                        {
                            O("EVENT_POI_CLICK has been invoked.");
                            StringBuilder[] arr = parse3(strData);
                            O("Clicked on poi: " + arr[3] + ", coordinates(x,y): " + arr[0] + " (multiplied by 100000)");
                        }
                        break;
                    case ApplicationEvents.EVENT_WAYPOINT_CLICKED:
                        if (EventsChLB.GetItemChecked(21))
                        {
                            O("EVENT_WAYPOINT_CLICKED has been invoked.");
                            StringBuilder[] arr = parse3(strData);                            
                            O("Clicked on waypoint: " + arr[0]);
                        }
                        break;
                    case ApplicationEvents.EVENT_SHARE_POSITION:                      
                        if (EventsChLB.GetItemChecked(22))
                        {                            
                            StringBuilder[] arr = parse3(strData);                            
                            O("EVENT_SHARE_POSITION has been invoked: " + arr[0] + ", " + arr[1]);
                        }
                        break;
                    default:
                            break;
                }
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            DriveHandler.StopDrive(0);
            DriveHandler.NotificationEvent -= new DriveHandler.NotificationDelegate(OnNotification);
            base.OnClosing(e);
        }

        //Prints out the messages/logs in the RichTextBox
        public void O(string inOututText) 
        {
            if (OutputRTB.InvokeRequired)
            {
                this.Invoke(new DriveHandler.NotificationDelegate(this.O), inOututText);
            }
            else
            {
                OutputRTB.Text = OutputRTB.Text.Insert(0, string.Format("{0} {1}\n", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), inOututText));
            }
        }


        private void WriteComboBoxesIntoIniFile() 
        {
            StreamWriter sw = new StreamWriter("used_paths.ini");
            List<string> tmpItems = new List<string>();

            // paths to the navigation
            sw.WriteLine("Paths to the Truck.exe:");
            tmpItems.Add(MyDrivePathCB.Text);
            for (int i = 0; i <= MyDrivePathCB.Items.Count-1; i++)
                if (!tmpItems.Contains(MyDrivePathCB.Items[i].ToString()))
                    tmpItems.Add(MyDrivePathCB.Items[i].ToString());
            for (int i = 0; i <= tmpItems.Count - 1; i++)
                sw.WriteLine(tmpItems[i].ToString());

            //paths to the .mlm files
            sw.WriteLine("Paths to the MLMs:");
            tmpItems.Clear();
            tmpItems.Add(SwitchMap_strLoadPathCB.Text);
            for (int i = 0; i <= SwitchMap_strLoadPathCB.Items.Count - 1; i++)
                if (!tmpItems.Contains(SwitchMap_strLoadPathCB.Items[i].ToString()))
                    tmpItems.Add(SwitchMap_strLoadPathCB.Items[i].ToString());
            for (int i = 0; i <= tmpItems.Count - 1; i++)
                sw.WriteLine(tmpItems[i].ToString());


            //paths to the .nmea GPS logs
            sw.WriteLine("Paths to the NMEAs:");
            tmpItems.Clear();
            tmpItems.Add(PlayGpsLogPath.Text);
            for (int i = 0; i <= PlayGpsLogPath.Items.Count - 1; i++)
                if (!tmpItems.Contains(PlayGpsLogPath.Items[i].ToString()))
                    tmpItems.Add(PlayGpsLogPath.Items[i].ToString());
            for (int i = 0; i <= tmpItems.Count - 1; i++)
                sw.WriteLine(tmpItems[i].ToString());            

            sw.Close();
        }

        private void FillComboBoxesFromIniFile()
        {
            if (File.Exists("used_paths.ini"))
            {
                MyDrivePathCB.Items.Clear();
                SwitchMap_strLoadPathCB.Items.Clear();

                StreamReader sw = new StreamReader("used_paths.ini");
                string tmp = "";

                if((tmp = sw.ReadLine()).StartsWith("Paths to the Truck.exe:"))
                    while ((tmp = sw.ReadLine())!=null)
                    {
                        if(tmp.StartsWith("Paths to the MLMs:"))
                            while ((tmp = sw.ReadLine()) != null)
                            {
                                if (tmp.StartsWith("Paths to the NMEAs:"))
                                    while ((tmp = sw.ReadLine()) != null)
                                        PlayGpsLogPath.Items.Add(tmp);
                                else
                                    SwitchMap_strLoadPathCB.Items.Add(tmp);
                            }
                        else
                            MyDrivePathCB.Items.Add(tmp);                            
                    }
                sw.Close();
            }
        }

        //fills the DataGridView with 2 waypoints by AddItinerary
        private void FillAddItineraryDGWWithDefaultValues() 
        {
            AddItineraryDGW.Rows.Add("1712574", "4815034", "3", "0", "0", "Starting point");
            AddItineraryDGW.Rows.Add("1714375", "4814980", "2", "1", "0", "Finish point");
        }

        private bool AddItineraryDGWRowCheck( DataGridViewRowCollection inCells) 
        {
            for (int i = 0; i < inCells.Count-1; i++)
            {
                DataGridViewRow row = inCells[i];
                if (row.Cells[0].Value == null ||
                    row.Cells[1].Value == null ||
                    row.Cells[2].Value == null ||
                    row.Cells[3].Value == null ||
                    row.Cells[4].Value == null)
                    return false;


                //lX column
                int lX;
                if (int.TryParse(row.Cells[0].Value.ToString(), out lX))
                {
                    if ((lX < -18000000) || (lX > 18000000))
                        return false;
                }
                else
                {
                    return false;
                }

                //lY column
                int lY;
                if (int.TryParse(row.Cells[1].Value.ToString(), out lY))
                {
                    if ((lY < -9000000) || (lY > 9000000))
                        return false;
                }
                else
                {
                    return false;
                }

                //nPointType column
                int nPointType;
                if (int.TryParse(row.Cells[2].Value.ToString(), out nPointType))
                {
                    if ((nPointType < 0) || (nPointType > 4))
                        return false;
                }
                else
                {
                    return false;
                }

                //id column
                uint id;
                if (!uint.TryParse(row.Cells[3].Value.ToString(), out id))
                {
                    return false;
                }

                //bVisited column
                int bVisited;
                if (int.TryParse(row.Cells[4].Value.ToString(), out bVisited))
                {
                    if ((bVisited < 0) || (bVisited > 1))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private SStopOffPoint[] CreateSstopofpointsFromAddItineraryDGW(DataGridViewRowCollection inCells)
        {
            SStopOffPoint[] points = new SStopOffPoint[inCells.Count-1];
            for (int i = 0; i < inCells.Count-1; i++)
            {
                DataGridViewRow row = inCells[i];
                points[i] = new SStopOffPoint();
                points[i].Location.lX = Convert.ToInt32(row.Cells[0].Value.ToString());
                points[i].Location.lY = Convert.ToInt32(row.Cells[1].Value.ToString());
                points[i].nPointType = Convert.ToInt32(row.Cells[2].Value.ToString());
                points[i].Id = Convert.ToUInt32(row.Cells[3].Value.ToString());
                points[i].bVisited = Convert.ToInt32(row.Cells[4].Value.ToString());
            }    
            return points;
        }

        private void ShowDialogBtn_Click_1(object sender, EventArgs e)
        {
            DriveHandler.showDialog(
                (int)ShowDialog_IdNum.Value,
                (int)ShowDialog_MaxTimeNum.Value);
        }

        private void LocationFromAddressEx_OkBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.LocationFromAddressEx(
                LocationFromAddressEx_strAddressTBox.Text,
                LocationFromAddressEx_bPostalChB.Checked,
                LocationFromAddressEx_bFuzzySearchChB.Checked,
                Convert.ToInt32(LocationFromAddressEx_MaxTimeNum.Value));
        }

        private void DirectGeocodingBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.DirectGeocoding(
                (int)DirectGeocoding_X_Num.Value,
                (int)DirectGeocoding_Y_Num.Value,
                (int)DirectGeocoding_MaxTime_Num.Value);
        }

        private void AddBitmapToMap_Btn_Click(object sender, EventArgs e)
        {
            DriveHandler.AddBitmapToMap(
                AddBitmapToMap_PathToBitmap_TBox.Text,
                AddBitmapToMap_Address_TBox.Text,
                (int)AddBitmapToMap_Num.Value);
        }

        private void TripStartBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.TripStart(
                TripStart_NameTBox.Text, 
                TripStart_DataModeCB.Text, 
                (int)TripStart_MaxTimeNum.Value);
        }

        private void TripStopBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.TripEnd((int)TripEnd_MaxTimeNum.Value);
        }

        private void TripAddUserEventBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.TripAddUserEvent(
                TripAddUserEvent_EventNameTBox.Text, 
                (uint)TripAddUserEvent_CustomID.Value, 
                (int)TripAddUserEvent_MaxTime.Value);
        }

        private void LocationFromAddressBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.LocationFromAddress(
                LocationFromAddress_StrAddressTBox.Text,
                LocationFromAddress_bPostal_ChB.Checked,
                LocationFromAddress_bValueMatchChB.Checked,
                (int)LocationFromAddress_MaxTimeNum.Value);
                
        }

        private void AddPoiCategoryBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.AddPoiCategory(
                AddPoiCateory_strCategoryTBox.Text, 
                AddPoiCategory_strBitmapPathTBox.Text, 
                AddPoiCategory_strIsoCodeCB.Text, 
                (int)AddPoiCategory_MaxTimeNum.Value);
            Refresh_AddPoi_CategoryCB();
        }

        private void GetPoiCategoryListBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetPoiCategoryList((int)GetPoiCategoryList_MaxTimeNum.Value);
        }

        private void AddPoiBtn_Click(object sender, EventArgs e)
        {   
            DriveHandler.AddPoi(
                AddPoi_StrAddressTBox.Text,
                AddPoi_BPostalChB.Checked,
                AddPoi_BValueMatchChB.Checked,
                (string)AddPoi_CategoryCB.Text,
                AddPoi_NameTBox.Text,
                AddPoi_SearchAddressChB.Checked,
                (int)AddPoi_MaxTimeNum.Value);
        }
        private void Refresh_AddPoi_CategoryCB() 
        {
            AddPoi_CategoryCB.Items.Clear();
            string[] list = DriveHandler.GetPoiCategoryList();
            foreach(string l in list)
                AddPoi_CategoryCB.Items.Add(l);
        }

        private void AddPoi_CategoryCB_MouseClick(object sender, MouseEventArgs e)
        {
            Refresh_AddPoi_CategoryCB();
        }

        private void LoadComputedRouteBtn_Click(object sender, EventArgs e)
        {
            string[] pathParts = LoadComputedRoute_Path.Text.Split('.');
            string fileSufix = pathParts[pathParts.Length - 1];

            if (fileSufix.Equals("ofg"))
            {
                string index = LoadComputedRoute_Index.Text;
                bool showOnly = (bool)LoadComputedRoute_ShowOnly.Checked;
                JObject jsonParams = new JObject(new JProperty("startFromIndex", index), new JProperty("showOnly", showOnly));
                DriveHandler.LoadComputedRoute(
                    LoadComputedRoute_Path.Text, jsonParams.ToString(),
                    (int)LoadComputedRoute_MaxTime.Value);
            }
            else
            {
                DriveHandler.LoadComputedRoute(
                    LoadComputedRoute_Path.Text,
                    (int)LoadComputedRoute_MaxTime.Value);
            }
        }


        private void ShowChangeOptionBtn_Click(object sender, EventArgs e)
        {
            string option;
            DriveHandler.GetChangeOption((int)ShowChangeOption_MaxTimeNum.Value, out option);
            JObject json = JObject.Parse(option);
            ApplicationChangeOptionsTextbox.Text = json.ToString();
            ApplicationChangeOptionsTextbox.Refresh();
        }

        private void GetNextInstructionBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetNextInstruction((int)GetNextInstruction_MaxTimeNum.Value);
        }

        private void GetNextInstructionAutoBtn_Click(object sender, EventArgs e)
        {
            timer.Interval = (int)GetNextInstructionAuto_DelayAndMaxTimeNum.Value;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string[] arr;
            DriveHandler.GetNextInstruction((int)GetNextInstructionAuto_DelayAndMaxTimeNum.Value, out arr);
            if (arr != null)
                if (arr.Length == 6)
                {
                    GetNextInstructionAuto_ExitIndexTBox.Text = arr[0];
                    GetNextInstructionAuto_lDistanceToNextTurnTBox.Text = arr[1];
                    GetNextInstructionAuto_lNextTurnXTBox.Text = arr[2];
                    GetNextInstructionAuto_lNextTurnYTBox.Text = arr[3];
                    GetNextInstructionAuto_nInstructionTBox.Text = arr[4];
                    GetNextInstructionAuto_nRoundaboutExitIndexTBox.Text = arr[5];
                }
            if (!GetNextInstructionAuto_RepeatAutomaticallyChB.Checked)
                timer.Stop();
        }

        private void StartNavigationBtn_Click(object sender, EventArgs e)
        {
            Thread oStartNavigationThread = new Thread(new ThreadStart(StartNAvigation));
            oStartNavigationThread.Start();
        }

        private void StartNAvigation()
        {
            DriveHandler.StartNavigation(
                (int)StartNavigation_lXNum.Value,
                (int)StartNavigation_lYNum.Value,
                (int)StartNavigation_FlagsNum.Value,
                StartNavigation_bShowApplicationChB.Checked,
                StartNavigation_bSearchAddressChB.Checked,
                StartNavigation_CustomAddress.Text,
                (int)StartNavigation_MaxTimeNum.Value);
        }

        private void GetItineraryListBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetItineraryList(
                GetItineraryList_strItineraryNameCB.Text,
                (int)GetItineraryList_MaxTimeNum.Value);
        }

        private void GetMapVersionBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetMapVersion(GetMapVersion_strIsoCB.Text, (int)GetMapVersion_MaxTimeNum.Value);
        }

        private void FindNearbyPoiBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.FindNearbyPoi(
                (int)FindNearbyPoi_ListSizeNum.Value,
                (int)FindNearbyPoi_CategoryNumberNum.Value,
                FindNearbyPoi_strCategoryNameTBox.Text,
                (int)FindNearbyPoi_lXNum.Value,
                (int)FindNearbyPoi_lYNum.Value,
                (int)FindNearbyPoi_MaxTimeNum.Value
                );
        }

        private void NavigateToAddressBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.NavigateToAddress(
                NavigateToAddress_AddressTBox.Text,
                NavigateToAddress_bPostalChB.Checked,
                (int)NavigateToAddress_FlagsNum.Value,
                NavigateToAddress_bShowApplicationChB.Checked,
                (int)NavigateToAddress_MaxTimeNum.Value);
        }

        private void PlayGpsLogStartBtn_Click(object sender, EventArgs e)
        {
                if (!DriveHandler.RunGpsLog)
                {
                    if (DriveHandler.GpsLogLoad(PlayGpsLogPath.Text))
                    {
                        DriveHandler.RunGpsLog = true;
                        DriveHandler.EnableExternalGpsInput();
                        PlayGpsLogTimer.Enabled = true;
                    }
                 }
                PlayGpsLogTimer.Start();
        }

        private void PlayGpsLogTimer_Tick(object sender, EventArgs e)
        {
            string line = "";
            if(DriveHandler.RunGpsLog == true)
            {
                double pos = DriveHandler.GpsLogPosition;
                double lenght = DriveHandler.GpsLog.Length;
                double ratio = 0;
                if (lenght>0)
                    ratio = (pos/lenght)*100;
                GpsLogTrackBar.Value = Convert.ToInt32(ratio);
                bool podm = true;
                while (podm)
                {
                    if (DriveHandler.GpsLogPosition < DriveHandler.GpsLog.Length)
                    {
                        line = DriveHandler.GpsLogAtPosition(DriveHandler.GpsLogPosition);
                        DriveHandler.GpsLogPosition++;
                        DriveHandler.SendGpsData(line);
                        if (line.StartsWith("$GPGGA"))
                            podm = false;
                    }
                    else
                        PlayGpsLogStopBtn_Click(sender, e);
                }
            }
        }

        private void PlaySoundTTSBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.PlaySoundTTS(
                PlaySoundTTS_TextTBox.Text,
                (int)PlaySoundTTS_TimeNum.Value);
        }

        private void IsDriveRunningBtn_Click(object sender, EventArgs e)
        {
        }

        private void GetPoiListBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetPoiList(
                GetPoiList_strCategoryTBox.Text,
                GetPoiList_bSearchAddressChB.Checked,
                (int)GetPoiList_MaxTimeNum.Value);
        }

        private void AddItineraryBtn_Click(object sender, EventArgs e)
        {
            //check if the data from grid are in the correct format and if so, then create waypoints and then itinerary
            if (AddItineraryDGWRowCheck(AddItineraryDGW.Rows))
            {
                DriveHandler.AddItinerary(
                    AddItinerary_strItineraryNameTBox.Text,
                    CreateSstopofpointsFromAddItineraryDGW(AddItineraryDGW.Rows));
            }
            else
                O("The data are not in correct format.");
        }

        private void AddItinerary_Btn_Click(object sender, EventArgs e)
        {
            DriveHandler.AddItinerary(AddItinerary_strItineraryNameTBox.Text);
            AddEntryToItinerary_strItineraryNameCB.Items.Add(AddItinerary_strItineraryNameTBox.Text);
            SetRoute_strItineraryNameCB.Items.Add(AddItinerary_strItineraryNameTBox.Text);
        }

        private void AddEntryToItineraryBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.AddEntryToItinerary(
                AddEntryToItinerary_strItineraryNameCB.Text,
                (int)AddEntryToItinerary_lXNum.Value,
                (int)AddEntryToItinerary_lYNum.Value,
                (int)AddEntryToItinerary_nPointTypeNum.Value,
                AddEntryToItinerary_CaptionTBox.Text,
                (int)AddEntryToItinerary_nIndexNum.Value,
                (uint)AddEntryToItinerary_IdNum.Value
                );
        }

        private void openDriveExeDlg_FileOk(object sender, CancelEventArgs e)
        {
            string tmpPath = openDriveExeDlg.InitialDirectory + openDriveExeDlg.FileName;
            MyDrivePathCB.Text = tmpPath;
        }

        private void GetActualGpsPositionBtn_Click(object sender, EventArgs e)
        {
            if (GetActualGpsPosition_JsonChb.Checked)
            {
                string strJson;
                DriveHandler.GetActualGpsPositionJson(GetActualGpsPosition_SatellitesInfoChb.Checked, out strJson, (int)GetActualGpsPosition_MaxTimeNum.Value);
                JObject json = JObject.Parse(strJson);
                GetActualGpsPosition_JsonTextBox.Text = json.ToString();
                GetActualGpsPosition_JsonTextBox.Refresh();
            }
            else
            {
                DriveHandler.GetActualGpsPosition(GetActualGpsPosition_SatellitesInfoChb.Checked, (int)GetActualGpsPosition_MaxTimeNum.Value);
            }
        }

        private void AutomaticRemoteActivationBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.AutomaticRemoteActivation(
                AutomaticRemoteActivation_ActivationExePathTBox.Text,
                AutomaticRemoteActivation_MlmPathTBox.Text,
                AutomaticRemoteActivation_SdPasswordTBox.Text);
        }

        private void ShowChangeOption_SetChangeOptionBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.SetChangeOption((int)ShowChangeOption_MaxTimeNum.Value, (string)ApplicationChangeOptionsTextbox.Text);
        }

        private void LoadExternalFileBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.LoadExternalFile(
                LoadExternalFile_PathTBox.Text,
                (int)LoadExternalFile_TypeFileNum.Value,
                (int)LoadExternalFile_MaxTimeNum.Value);
        }

        private void SetRouteBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.SetRoute(
                SetRoute_strItineraryNameTBox.Text,
                (int)SetRoute_flagNum.Value,
                SetRoute_bShowApplicationChBox.Checked,
                (int)SetRoute_maximumTimeNum.Value);
            //Thread oThreadItinerary = new Thread(new ThreadStart(MyThreadItinerary));
            //oThreadItinerary.Start();
        }

        private void ChangeAppRectangleBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.ChangeAppRectangle(
                (int)ChangeAppRectangle_nLeftNum.Value,
                (int)ChangeAppRectangle_nTopNum.Value,
                (int)ChangeAppRectangle_nWidthNum.Value,
                (int)ChangeAppRectangle_nHeightNum.Value,
                (int)ChangeAppRectangle_MaxTimeNum.Value);
        }

        private void SwitchMap_strLoadPath_openBtn_Click(object sender, EventArgs e)
        {
            openMlmFileDlg.ShowDialog();
            SwitchMap_strLoadPathCB.Text = openMlmFileDlg.FileName;
        }

        private void SwitchMapThread(object data) 
        {
            DriveHandler.SwithMap(
                (string)data,
                0);
        }

        private void SwitchMapBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(SwitchMap_strLoadPathCB.Text))
            {
                SwitchMap_strLoadPathCB.Items.Add(SwitchMap_strLoadPathCB.Text);
                WriteComboBoxesIntoIniFile();

                Thread oThread = new Thread(this.SwitchMapThread);
                oThread.Start(SwitchMap_strLoadPathCB.Text);
            }
            else
                O(SwitchMap_strLoadPathCB.Text + " doesn't exist.");
        }

        private void P_Click(object sender, EventArgs e)
        {

        }

        private void ScreenResolutionR2_RBtn_CheckedChanged(object sender, EventArgs e)
        {
            if(ScreenResolutionR2_RBtn.Checked)
                ResolutionTlp.Show();
        }

        private void ScreenResolutionR1_RBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (ScreenResolutionR1_RBtn.Checked)
                ResolutionTlp.Hide();
        }

        private void RemoteActivationViaHttpRequest_ActivateBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.RemoteActivationViaHttpRequest();
        }

        private void ShowMessage2Btn_Click(object sender, EventArgs e)
        {
            Thread oThreadShowMessage = new Thread(new ThreadStart(MyThreadShowMessage));
            oThreadShowMessage.Start();
        }

        private void GetCoordinatesFromOffsetBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.GetCoordinatesFromOffset(
                GetCoordinatesFromOffset_IsoCodeTBox.Text,
                (int)GetCoordinatesFromOffset_RoadOffsetNum.Value,
                (int)GetCoordinatesFromOffset_MaxTimeNum.Value);
        }

        private void P_SizeChanged(object sender, EventArgs e)
        {
            if (ScreenResolutionR1_RBtn.Checked)
                DriveHandler.ChangeAppRectangle(
                    0, 
                    0, 
                    P.Width, 
                    P.Height,
                    0);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            for (int j = 10; j < 13; j++)
            {
                Random r = new Random();
                SStopOffPoint[] wp = new SStopOffPoint[70];
                for (int i = 0; i < 70; i++)
                {
                    wp[i] = new SStopOffPoint();
                    wp[i].Id = (uint)i;
                    wp[i].SetCaption(i.ToString());
                    wp[i].nPointType = 1;
                    if (i == 0)
                        wp[i].nPointType = 3;
                    if (i == 69)
                        wp[i].nPointType = 2;
                    wp[i].Location.lX = 500000 + (int)r.Next(1800000);
                    wp[i].Location.lY = 4600000 + (int)r.Next(600000);
                }

                DriveHandler.AddItinerary("70wp" + j.ToString(), wp);
                DriveHandler.SetRoute("70wp" + j.ToString(), 0, true, 0);
                OutputRTB.AppendText(DriveHandler.GetRouteInfoTest("Europe70wp" + j.ToString(), 0));
                //DriveHandler.OptimizeItinerary("Europe70wp" + j.ToString(), 0);
                //DriveHandler.SetRoute("70wp" + j.ToString(), 0, true, 0);
                //OutputRTB.AppendText(DriveHandler.GetRouteInfoTest(";Europe70wp" + j.ToString() + "\n", 0));
            }
            for (int j = 10; j < 13; j++)
            {
                DriveHandler.SetRoute("70wp" + j.ToString(), 0, true, 0);
                DriveHandler.OptimizeItinerary("Default", 0);
                //DriveHandler.SetRoute("70wp" + j.ToString(), 0, true, 0);
                OutputRTB.AppendText(DriveHandler.GetRouteInfoTest("Europe70wp" + j.ToString() + "\n", 0));
            }


            //Random r = new Random();
            //DriveHandler.AddBitmapToMap(
            //    @"c:\C\FLEET\15h\Res\icons\poi\5_favo_3.bmp",
            //    500000 + (int)r.Next(1800000),
            //    4600000 + (int)r.Next( 600000),
            //    0);
        }

        private void OptimizeItineraryBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.OptimizeItinerary(
                OptimizeItinerary_strItineraryNameTBox.Text,
                (int)OptimizeItinerary_MaxTimeNum.Value);
        }

        private void GetCoordinatesFromListOfOffsetIdsBtn_Click(object sender, EventArgs e)
        {
            OutputRTB.AppendText(
                DriveHandler.GetCoordinatesFromListOfOffsetIds(GetCoordinatesFromListOfOffsetIdsPathTBox.Text));
        }

        private void openInEmbeddedWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T1;
        }

        private void closeNavigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.StopDrive(0);
            isRunning = false;
            ApiAI_ToolStripMenuItem.Visible = false;
            ApiJZ_ToolStripMenuItem.Visible = false;
            toolsToolStripMenuItem.Visible = false;
            TC.SelectedTab = T1;
        }

        private void fileToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            fileToolStripMenuItem.ShowDropDown();
        }

        private void ApiAI_ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ApiAI_ToolStripMenuItem.ShowDropDown();
        }

        private void ApiJZ_ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ApiJZ_ToolStripMenuItem.ShowDropDown();
        }

        private void toolsToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ShowDropDown();
        }

        private void addBitmapToMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T24;
        }

        private void addEntryToItineraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T18;
        }

        private void addItineraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T35;
        }

        private void addPoiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T8;
        }

        private void addPoiCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T8;
        }

        private void getSDKversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.GetSdkVersion();
        }

        private void bringAppToBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.BringApplicationToBackground();
        }

        private void bringAppToForegroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.BringApplicationToForeground();
        }

        private void closeDialogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.CloseDialogs();
        }

        private void eventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T20;
        }

        private void findNearbyPoiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T15;
        }

        private void getActualGpsPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T22;
        }

        private void getCoordinatesFromOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T6;
        }

        private void getCurrentSpeedLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.GetCurrentSpeedLimit(0);
        }

        private void getItineraryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T25;
        }

        private void getLocationInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T4;
        }

        private void getMapVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T14;
        }

        private void getNextInstructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T12;
        }

        private void getPoiCategoryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T8;
        }

        private void getPoiListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T17;
        }

        private void getRouteInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.GetRouteInfo(0);
        }

        private void getRouteStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.GetRouteStatus(0);
        }

        private void getUniqueDeviceIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.GetUniqueDeviceId();
        }

        private void changeAppRectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T23;
        }

        private void changeApplicationOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T10;
        }

        private void isDriveRunningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            O("IsDriveRunning returns: " + isRunning.ToString());
        }

        private void loadComputedRouteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T9;
        }

        private void loadComputedRouteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T3;
        }

        private void locationFromAddressExToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T3;
        }

        private void navigateToAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T16;
        }

        private void showDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T5;
        }

        private void playGpsLogToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void playSoundTTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T28;
        }

        private void sendGpsDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T13;
        }

        private void playGPSLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T13;
        }

        private void setRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T25;
        }

        private void showMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T29;
        }

        private void showCoordinatesOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T4;
        }

        private void skipNextWaypointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.SkipNextWaypoint();
        }

        private void startNavigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T30;
        }

        private void switchMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T19;
        }

        private void tripAddUserEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T7;
        }

        private void tripEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T7;
        }

        private void tripStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T7;
        }

        private void loadExternalFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T26;
        }

        private void stopNavigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriveHandler.StopNavigation(0);
        }

        private void remoteActivationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T27;
        }

        private void ItineraryGeneratorBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.ItineraryGenerator(
                (int)ItineraryGenerator_NumberOfItinerariesNum.Value,
                (int)ItineraryGenerator_NumberOfWaypointsNum.Value,
                ItineraryGenerator_prefixTBox.Text,
                (int)ItineraryGenerator_XminNum.Value,
                (int)ItineraryGenerator_XmaxNum.Value,
                (int)ItineraryGenerator_YminNum.Value,
                (int)ItineraryGenerator_YmaxNum.Value);

        }

        private void itineraryGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T2;
        }

        private void initApiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T1;
        }

        private void PlayGpsLogStopBtn_Click(object sender, EventArgs e)
        {
            PlayGpsLogTimer.Stop();
            DriveHandler.RunGpsLog = false;
            DriveHandler.DisableExternalGpsInput();
            DriveHandler.GpsLogPosition = 0;
        }

        private void PlayGpsLogPauseBtn_Click(object sender, EventArgs e)
        {
            PlayGpsLogTimer.Stop();
        }

        private void GpsLogTrackBar_ValueChanged(object sender, EventArgs e)
        {
            DriveHandler.GpsLogPosition = ((GpsLogTrackBar.Value * DriveHandler.GpsLog.Length) / GpsLogTrackBar.Maximum);
        }

        private void DevideItinerary_DevideBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.DevideItinerary(DevideItinerary_PathTBox.Text);
        }

        private void DevideItinerary_DevideComputeBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.DevideItineraryAndCompute(DevideItinerary_PathTBox.Text);
        }

        private void FlashMessageBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.FlashMessage(
                FlashMessage_strMessageTBox.Text,
                FlashMessage_bShowApplicationChB.Checked,
                (int)FlashMessage_MaxTimeNum.Value);
        }

        private void flashMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T31;
        }

        private void showRectangleOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T32;
        }

        private void ShowRectangleOnMapBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.ShowRectangleOnMap(
                (int)ShowRectangleOnMap_lBottomNum.Value,
                (int)ShowRectangleOnMap_lLeftNum.Value,
                (int)ShowRectangleOnMap_lRightNum.Value,
                (int)ShowRectangleOnMap_lTopNum.Value,
                ShowRectangleOnMap_bShowApplicationChB.Checked,
                (int)ShowRectangleOnMap_MaxTimeNum.Value);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            TC.ItemSize = new Size(0,1);
            TC.SizeMode = TabSizeMode.Fixed;
            ApiAI_ToolStripMenuItem.Visible = false;
            ApiJZ_ToolStripMenuItem.Visible = false;
            toolsToolStripMenuItem.Visible = false;
            FillAddItineraryDGWWithDefaultValues();
        }

        private void HighlightPoiBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.HighlightPoi(
                HighlightPoi_PoiCategoryTBox.Text,
                HighlightPoi_PoiNameTBox.Text,
                HighlightPoi_SoundTBox.Text,
                (int)HighlightPoi_lXNum.Value,
                (int)HighlightPoi_lYNum.Value,
                (int)HighlightPoi_MaxTimeNum.Value);
        }

        private void highlightPoiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T33;
        }

        private void OpenGpsLogBtn_Click(object sender, EventArgs e)
        {
            openGpsDataDlg.ShowDialog();
        }

        private void openGpsDataDlg_FileOk(object sender, CancelEventArgs e)
        {
            if (PlayGpsLogPath.Text!="") 
                PlayGpsLogPath.Items.Add(PlayGpsLogPath.Text);
            PlayGpsLogPath.Text = openGpsDataDlg.InitialDirectory + openGpsDataDlg.FileName;
        }

        private void unloadGFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T34;
        }

        private void loadGFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T34;
        }

        private void LoadGFFileBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.LoadGFFile(
                LoadGFFile_PathTBox.Text,
                (int)LoadGFFile_MaxTimeNum.Value);
        }

        private void UnloadGFFileBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.UnloadGFFile(
                UnloadGFFile_PathTBox.Text,
                (int)UnloadGFFile_MaxTimeNum.Value);                
        }

        private void InitApi_chooseAnotherPathBtn_Click(object sender, EventArgs e)
        {
            openDriveExeDlg.ShowDialog();
        }

        private void StopDriveBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.StopDrive(0);
            isRunning = false;
            ApiAI_ToolStripMenuItem.Visible = false;
            ApiJZ_ToolStripMenuItem.Visible = false;
            toolsToolStripMenuItem.Visible = false;
        }

        private void cLEARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OutputRTB.Clear();
        }

        private void deleteItineraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T25;
        }

        private void DeleteItineraryBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.DeleteItinerary(
                DeleteItinerary_strItineraryNameTBox.Text,
                (int)DeleteItinerary_MaxTimeNum.Value);
        }

        private void deviceItineraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T36;
        }

        private void DevideItinerary_DetectWrongBtn_Click(object sender, EventArgs e)
        {
            O("Please wait while the detection is not finish .........");
            Thread oThread = new Thread(new ThreadStart(MyThreadDetectWrongWP));
            oThread.Start();
        }

        private void deleteEntryInItineraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T37;
        }

        private void DeleteEntryInItineraryBtn_Click(object sender, EventArgs e)
        {
            DriveHandler.DeleteEntryInItinerary(
                DeleteEntryInItinerary_strItineraryNameTBox.Text,
                (int)DeleteEntryInItinerary_nIndexNum.Value,
                (int)DeleteEntryInItinerary_MaxTimeNum.Value);
        }
        private void GetMapCorrection_btn_Click(object sender, EventArgs e)
        {
            string status;
            DriveHandler.GetMapCorrectionEvents(out status, -1);
            GetMapCorrection_TB.Text += status;
        }

        private void AddMapCorrectionEvents_Click(object sender, EventArgs e)
        {
            DriveHandler.AddMapCorrectionEvents(AddMapCorrection_TB.Text, -1);
        }

        private void ClearMapCorrecionEvents_btn_Click(object sender, EventArgs e)
        {
            DriveHandler.ClearMapCorrectionEvents(-1);
        }

        private void addMapCorrectionEvensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T11;
        }

        private void getMapCorrectionEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T11;
        }

        private void clearMapCorrectionEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T11;
        }

        private void searchLocationFTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T38;
        }

        private void buttonFts_Click(object sender, EventArgs e)
        {
            WindowsDemo.MyListener ftsListener = new WindowsDemo.MyListener(listBoxFts);

            DriveHandler.SearchLocation(textBoxFts.Text, ftsListener, -1);
        }

        private void EventsChLB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ShowDialogGB_Enter(object sender, EventArgs e)
        {

        }

        private void loadGeofileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T39;
        }

        private void buttonLoadGeofile_Click(object sender, EventArgs e)
        {
            DriveHandler.LoadGeofile(textBoxLoadGeofileName.Text, textBoxLoadGeoJson.Text, 0);
        }

        private void buttonUnloadGeofile_Click(object sender, EventArgs e)
        {
            DriveHandler.UnloadGeofile(textBoxUnloadGeofileName.Text, 0);
        }

        private void buttonUnloadGeofiles_Click(object sender, EventArgs e)
        {
            DriveHandler.UnloadGeofiles(0);
        }

        private void btnFormatJson_Click(object sender, EventArgs e)
        {
            string text = textBoxLoadGeoJson.Text;
            JObject json = JObject.Parse(text);
            textBoxLoadGeoJson.Text = json.ToString();
            textBoxLoadGeoJson.Refresh();
        }

        private void GetPoiOnRoute_btnClick(object sender, EventArgs e)
        {
            int category = (int)GetPoiOnRoute_categoryNum.Value;
            int minDriveTime = (int)GetPoiOnRoute_minDriveTimeNum.Value;
            int maxDriveTime = (int)GetPoiOnRoute_maxDriveTimeNum.Value;
            int maxTime = (int)GetPoiOnRoute_maxDriveTimeNum.Value;
            textBoxGetPoiOnRoute.Text = "";
            string[] poiOnRouteArr = DriveHandler.GetPoiOnRoute(category, minDriveTime, maxDriveTime, maxTime);
            for(int i = 0; i < poiOnRouteArr.Length; i++)
            {
                textBoxGetPoiOnRoute.Text += i+1 + ". " + poiOnRouteArr[i] + "\r\n";
            }
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void TC_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void T40_Click(object sender, EventArgs e)
        {

        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void label48_Click(object sender, EventArgs e)
        {

        }

        private void getPoiOnRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T40;
        }

        private void joinRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T41;
        }

        private void JoinRouteBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(JoinRoutePath_filepath.Text))
            {
                JObject jsonParams = new JObject(new JProperty("startFromIndex", -1), new JProperty("showOnly", false));
                DriveHandler.LoadComputedRoute(JoinRoutePath_filepath.Text, jsonParams.ToString(), 0);
            }
            else
            {
                O("File:" + JoinRoutePath_filepath.Text + " doesn't exist.");
            }            
        }

        private void updatePoisEvensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC.SelectedTab = T42;
        }

        private void UpdatePois_btn_Click(object sender, EventArgs e)
        {
            DriveHandler.UpdatePois(updatePois_TB.Text, 0);
        }
    }
}