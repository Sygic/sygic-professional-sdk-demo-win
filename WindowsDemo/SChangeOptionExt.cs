using ApplicationAPI;

namespace SYGIC_PROFESSINAL_SDK_DEMO
{
    class SChangeOptionExt: SChangeOption
    {
        //BOOL			bSoundEnabled;
        //BOOL			bOperateRightHanded;

        //// 0 .. 10
        ////int				nVolumeMin;
        ////int				nVolumeMax;

        ////MILES(YARDS) = 0, KILOMETERS = 1, MILES(FEETS) = 2
        ////int				nDistanceUnit;

        ////EUROPE = 0, US = 1, UK = 2
        ////int				nClockFormat;

        ////DEGREES = 0, MINUTES = 1, SECONDS = 2
        ////int				nGPSUnits;

        ////ABCD = 1, QWERTY = 2, AZERTY = 3, DVORAK = 4
        ////int				nKeyboardType;

        //// ASK = 1, ALWAYS = 2, NEVER = 3
        ////int				nAvoidTollRoads;

        //BOOL			bAvoidUTurns;

        //// ASK=1, FASTEST=2, SHORTEST=3, AVOIDING_MOTORWAYS=4, WALKS=5, BIKES=6, LIMITED=7
        ////int				nPlanningSettings;

        //int				nPlanningSettingsLimitedSpeed;

        //BOOL			bAvoidFerries;

        //BOOL			bDisableMainMenu;
        //BOOL			bDisableRecompute;

        //int				nETAMaximumSpeed;
        //int				nETAPercentageChange;

        //BOOL			bRadarsWarnOn;
        //BOOL			bRadarsVisible;
        //int				nRadarDistance;
        //int				nRadarDistanceInCity;

        ////int				nSkin; // DAY = 0, NIGHT = 1, AUTO = 2

        //int				nTimeZone;

        //int				nSpeedExceedInCity;
        //int				nSpeedExceed;
        //int				nView;
        //int				nSignpostDirection;
        //int				nSignpostSize;
        //int				bSnapToEveryRoad;
        //int				bMaxSpeedWarn;
        //BOOL			bTTSEnabled;
        //int				nVisiblePointReachDistance;
        //int				nInvisiblePointReachDistance;
        //BOOL			bAllowClosedRoads;

        //// truck attributes
        //BOOL	bTruckInMap;		//readonly
        //BOOL	bUseTruckAtt;
        //int		nTruckMaxSpeed;		//kmh

        //int		nTruckWeightTotal;	//kg
        //int		nTruckWeightAxle;	//kg
        //int		nTruckTandemWeight;		//kg
        //int		nTruckTridemWeight;		//kg
        //int		nTruckOtherWeight;		//kg
        //int		nTruckUnladenWeight;		//kg
        //int		nTruckLenght;		//mm
        //int		nTruckAxleLength;		//mm
        //int		nTrailerLength;		//mm
        //int		nTractorLength;		//mm
        //int		nKingpinLastAxle;	//mm
        //int		nKingpinLastTandem;	//mm
        //int		nKingpinEndTrailer;	//mm
        //int		nTruckOtherLength;		//mm
        //int		nTruckWidth;		//mm
        //int		nTruckHeight;		//mm

        #region Property

        public int BSoundEnabled
        {
            get { return bSoundEnabled; }
            set { bSoundEnabled = value; }
        }

        public int BOperateRightHanded
        {
            get { return bOperateRightHanded; }
            set { bOperateRightHanded = value; }
        }

        public int BAvoidUTurns
        {
            get { return bAvoidUTurns; }
            set { bAvoidUTurns = value; }
        }

        public int NplanningSettings 
        {
            get { return nPlanningSettings; }
            set { nPlanningSettings = value; }
        }

        public int NPlanningSettingsLimitedSpeed
        {
            get { return nPlanningSettingsLimitedSpeed; }
            set { nPlanningSettingsLimitedSpeed = value; }
        }

        public int BAvoidFerries
        {
            get { return bAvoidFerries; }
            set { bAvoidFerries = value; }
        }

        public int BDisableMainMenu
        {
            get { return bDisableMainMenu; }
            set { bDisableMainMenu = value; }
        }

        public int BDisableRecompute
        {
            get { return bDisableRecompute; }
            set { bDisableRecompute = value; }
        }

        public int NETAMaximumSpeed
        {
            get { return nETAMaximumSpeed; }
            set { nETAMaximumSpeed = value; }
        }

        public int NETAPercentageChange
        {
            get { return nETAPercentageChange; }
            set { nETAPercentageChange = value; }
        }

        public int BRadarsWarnOn
        {
            get { return bRadarsWarnOn; }
            set { bRadarsWarnOn = value; }
        }

        public int BRadarsVisible
        {
            get { return bRadarsVisible; }
            set { bRadarsVisible = value; }
        }

        public int NRadarDistance
        {
            get { return nRadarDistance; }
            set { nRadarDistance = value; }
        }

        public int NRadarDistanceInCity
        {
            get { return nRadarDistanceInCity; }
            set { nRadarDistanceInCity = value; }
        }

        public int NTimeZone
        {
            get { return nTimeZone; }
            set { nTimeZone = value; }
        }

        public int NSkin
        {
            get { return nSkin; }
            set { nSkin = value; }
        }

        public int NSpeedExceedInCity
        {
            get { return nSpeedExceedInCity; }
            set { nSpeedExceedInCity = value; }
        }

        public int NSpeedExceed
        {
            get { return nSpeedExceed; }
            set { nSpeedExceed = value; }
        }

        public int NView
        {
            get { return nView; }
            set { nView = value; }
        }

        public int NSignpostDirection
        {
            get { return nSignpostDirection; }
            set { nSignpostDirection = value; }
        }

        public int NSignpostSize
        {
            get { return nSignpostSize; }
            set { nSignpostSize = value; }
        }

        public int BSnapToEveryRoad
        {
            get { return bSnapToEveryRoad; }
            set { bSnapToEveryRoad = value; }
        }

        public int BMaxSpeedWarn
        {
            get { return bMaxSpeedWarn; }
            set { bMaxSpeedWarn = value; }
        }

        public int BTTSEnabled
        {
            get { return bTTSEnabled; }
            set { bTTSEnabled = value; }
        }

        public int NVisiblePointReachDistance
        {
            get { return nVisiblePointReachDistance; }
            set { nVisiblePointReachDistance = value; }
        }

        public int NInvisiblePointReachDistance
        {
            get { return nInvisiblePointReachDistance; }
            set { nInvisiblePointReachDistance = value; }
        }

        public int BAllowClosedRoads
        {
            get { return bAllowClosedRoads; }
            set { bAllowClosedRoads = value; }
        }

        public int BTruckInMap
        {
            get { return bTruckInMap; }
            set { bTruckInMap = value; }
        }

        public int BUseTruckAtt
        {
            get { return bUseTruckAtt; }
            set { bUseTruckAtt = value; }
        }

        public int NTruckMaxSpeed
        {
            get { return nTruckMaxSpeed; }
            set { nTruckMaxSpeed = value; }
        }

        public int NTruckWeightTotal
        {
            get { return nTruckWeightTotal; }
            set { nTruckWeightTotal = value; }
        }

        public int NTruckWeightAxle
        {
            get { return nTruckWeightAxle; }
            set { nTruckWeightAxle = value; }
        }

        public int NTruckTandemWeight
        {
            get { return nTruckTandemWeight; }
            set { nTruckTandemWeight = value; }
        }

        public int NTruckTridemWeight
        {
            get { return nTruckTridemWeight; }
            set { nTruckTridemWeight = value; }
        }

        public int NTruckOtherWeight
        {
            get { return nTruckOtherWeight; }
            set { nTruckOtherWeight = value; }
        }

        public int NTruckUnladenWeight
        {
            get { return nTruckUnladenWeight; }
            set { nTruckUnladenWeight = value; }
        }

        public int NTruckLenght
        {
            get { return nTruckLenght; }
            set { nTruckLenght = value; }
        }

        public int NTruckAxleLength
        {
            get { return nTruckAxleLength; }
            set { nTruckAxleLength = value; }
        }

        public int NTrailerLength
        {
            get { return nTrailerLength; }
            set { nTrailerLength = value; }
        }

        public int NTractorLength
        {
            get { return nTractorLength; }
            set { nTractorLength = value; }
        }

        public int NKingpinLastAxle
        {
            get { return nKingpinLastAxle; }
            set { nKingpinLastAxle = value; }
        }

        public int NKingpinLastTandem
        {
            get { return nKingpinLastTandem; }
            set { nKingpinLastTandem = value; }
        }

        public int NKingpinEndTrailer
        {
            get { return nKingpinEndTrailer; }
            set { nKingpinEndTrailer = value; }
        }

        public int NTruckOtherLength
        {
            get { return nTruckOtherLength; }
            set { nTruckOtherLength = value; }
        }

        public int NTruckWidth
        {
            get { return nTruckWidth; }
            set { nTruckWidth = value; }
        }

        public int NTruckHeight
        {
            get { return nTruckHeight; }
            set { nTruckHeight = value; }
        }

        #endregion

        public void CopyFrom(SChangeOption inOption) 
        {
            bAllowClosedRoads = inOption.bAllowClosedRoads;
            bAvoidFerries = inOption.bAvoidFerries;
            bAvoidUTurns = inOption.bAvoidUTurns;
            bDisableMainMenu = inOption.bDisableMainMenu;
            bDisableRecompute = inOption.bDisableRecompute;
            bMaxSpeedWarn = inOption.bMaxSpeedWarn;
            bOperateRightHanded = inOption.bOperateRightHanded;
            bRadarsVisible = inOption.bRadarsVisible;
            bRadarsWarnOn = inOption.bRadarsWarnOn;
            bSnapToEveryRoad = inOption.bSnapToEveryRoad;
            bSoundEnabled = inOption.bSoundEnabled;
            bTruckInMap = inOption.bTruckInMap;
            bTTSEnabled = inOption.bTTSEnabled;
            bUseTruckAtt = inOption.bUseTruckAtt;
            ClockFormat = inOption.ClockFormat;
            DistanceUnit = inOption.DistanceUnit;
            GPSUnits = inOption.GPSUnits;
            HomePosition.lX = inOption.HomePosition.lX;
            HomePosition.lY = inOption.HomePosition.lY;
            KeyboardType = inOption.KeyboardType;
            Language = inOption.Language;
            MaxSpeedSound = inOption.MaxSpeedSound;
            nAvoidTollRoads = inOption.nAvoidTollRoads;
            nETAMaximumSpeed = inOption.nETAMaximumSpeed;
            nETAPercentageChange = inOption.nETAPercentageChange;
            nInvisiblePointReachDistance = inOption.nInvisiblePointReachDistance;
            nKingpinEndTrailer = inOption.nKingpinEndTrailer;
            nKingpinLastAxle = inOption.nKingpinLastAxle;
            nKingpinLastTandem = inOption.nKingpinLastTandem;
            nLoadRestrictions = inOption.nLoadRestrictions;
            nPlanningSettings = inOption.nPlanningSettings;
            nPlanningSettingsLimitedSpeed = inOption.nPlanningSettingsLimitedSpeed;
            nRadarDistance = inOption.nRadarDistance;
            nRadarDistanceInCity = inOption.nRadarDistanceInCity;
            nSignpostDirection = inOption.nSignpostDirection;
            nSignpostSize = inOption.nSignpostSize;
            nSkin = inOption.nSkin;
            nSpeedExceed = inOption.nSpeedExceed;
            nSpeedExceedInCity = inOption.nSpeedExceedInCity;
            nTimeZone = inOption.nTimeZone;
            nTractorLength = inOption.nTractorLength;
            nTrailerLength = inOption.nTrailerLength;
            nTruckAxleLength = inOption.nTruckAxleLength;
            nTruckHeight = inOption.nTruckHeight;
            nTruckLenght = inOption.nTruckLenght;
            nTruckMaxSpeed = inOption.nTruckMaxSpeed;
            nTruckOtherLength = inOption.nTruckOtherLength;
            nTruckOtherWeight = inOption.nTruckOtherWeight;
            nTruckTandemWeight = inOption.nTruckTandemWeight;
            nTruckTridemWeight = inOption.nTruckTridemWeight;
            nTruckUnladenWeight = inOption.nTruckUnladenWeight;
            nTruckWeightAxle = inOption.nTruckWeightAxle;
            nTruckWeightTotal = inOption.nTruckWeightTotal;
            nTruckWidth = inOption.nTruckWidth;
            nView = inOption.nView;
            nVisiblePointReachDistance = inOption.nVisiblePointReachDistance;
            nVolumeMax = inOption.nVolumeMax;
            nVolumeMin = inOption.nVolumeMin;
            Voice = inOption.Voice;
            VoicePerson = inOption.VoicePerson;
        }
        public void CopyTo(ref SChangeOption outOption) 
        {
            outOption.bAllowClosedRoads = bAllowClosedRoads;
            outOption.bAvoidFerries = bAvoidFerries;
            outOption.bAvoidUTurns = bAvoidUTurns;
            outOption.bDisableMainMenu = bDisableMainMenu;
            outOption.bDisableRecompute = bDisableRecompute;
            outOption.bMaxSpeedWarn = bMaxSpeedWarn;
            outOption.bOperateRightHanded = bOperateRightHanded;
            outOption.bRadarsVisible = bRadarsVisible;
            outOption.bRadarsWarnOn = bRadarsWarnOn;
            outOption.bSnapToEveryRoad = bSnapToEveryRoad;
            outOption.bSoundEnabled = bSoundEnabled;
            outOption.bTruckInMap = bTruckInMap;
            outOption.bTTSEnabled = bTTSEnabled;
            outOption.bUseTruckAtt = bUseTruckAtt;
            outOption.ClockFormat = ClockFormat;
            outOption.DistanceUnit = DistanceUnit;
            outOption.GPSUnits = GPSUnits;
            outOption.HomePosition.lX = HomePosition.lX; 
            outOption.HomePosition.lY = HomePosition.lY;
            outOption.KeyboardType = KeyboardType;
            outOption.Language = Language;
            outOption.MaxSpeedSound = MaxSpeedSound;
            outOption.nAvoidTollRoads = nAvoidTollRoads;
            outOption.nETAMaximumSpeed = nETAMaximumSpeed;
            outOption.nETAPercentageChange = nETAPercentageChange;
            outOption.nInvisiblePointReachDistance = nInvisiblePointReachDistance;
            outOption.nKingpinEndTrailer = nKingpinEndTrailer;
            outOption.nKingpinLastAxle = nKingpinLastAxle;
            outOption.nKingpinLastTandem = nKingpinLastTandem;
            outOption.nLoadRestrictions = nLoadRestrictions;
            outOption.nPlanningSettings = nPlanningSettings;
            outOption.nPlanningSettingsLimitedSpeed = nPlanningSettingsLimitedSpeed;
            outOption.nRadarDistance = nRadarDistance;
            outOption.nRadarDistanceInCity = nRadarDistanceInCity;
            outOption.nSignpostDirection = nSignpostDirection;
            outOption.nSignpostSize = nSignpostSize;
            outOption.nSkin = nSkin;
            outOption.nSpeedExceed = nSpeedExceed;
            outOption.nSpeedExceedInCity = nSpeedExceedInCity;
            outOption.nTimeZone = nTimeZone;
            outOption.nTractorLength = nTractorLength;
            outOption.nTrailerLength = nTrailerLength;
            outOption.nTruckAxleLength = nTruckAxleLength;
            outOption.nTruckHeight = nTruckHeight;
            outOption.nTruckLenght = nTruckLenght;
            outOption.nTruckMaxSpeed = nTruckMaxSpeed;
            outOption.nTruckOtherLength = nTruckOtherLength;
            outOption.nTruckOtherWeight = nTruckOtherWeight;
            outOption.nTruckTandemWeight = nTruckTandemWeight;
            outOption.nTruckTridemWeight = nTruckTridemWeight;
            outOption.nTruckUnladenWeight = nTruckUnladenWeight;
            outOption.nTruckWeightAxle = nTruckWeightAxle;
            outOption.nTruckWeightTotal = nTruckWeightTotal;
            outOption.nTruckWidth = nTruckWidth;
            outOption.nView = nView;
            outOption.nVisiblePointReachDistance = nVisiblePointReachDistance;
            outOption.nVolumeMax = nVolumeMax;
            outOption.nVolumeMin = nVolumeMin;
            outOption.Voice = Voice;
            outOption.VoicePerson = VoicePerson;
        }

            //bAllowClosedRoads
            //int bAvoidFerries
            //int bAvoidUTurns
            //int bDisableMainMenu
            //bDisableRecompute
            //bMaxSpeedWarn
            //bOperateRightHanded
            //bRadarsVisible
            //bRadarsWarnOn
            //bSnapToEveryRoad
            //bSoundEnabled
            //bTruckInMap
            //bTTSEnabled
            //bUseTruckAtt
            //ClockFormat
            //DistanceUnit
            //GPSUnits
            //HomePosition X,Y
            //KeyboardType
            //Language
            //MaxSpeedSound
            //nAvoidTollRoads
            //nETAMaximumSpeed
            //nETAPercentageChange
            //nInvisiblePointReachDistance
            //nKingpinEndTrailer
            //nKingpinLastAxle
            //nKingpinLastTandem
            //nLoadRestrictions
            //nPlanningSettings
            //nPlanningSettingsLimitedSpeed
            //nRadarDistance
            //nRadarDistanceInCity
            //nSignpostDirection
            //nSignpostSize
            //nSkin
            //nSpeedExceed
            //nSpeedExceedInCity
            //nTimeZone
            //nTractorLength
            //nTrailerLength
            //nTruckAxleLength
            //nTruckHeight
            //nTruckLenght
            //nTruckMaxSpeed
            //nTruckOtherLength
            //nTruckOtherWeight
            //nTruckTandemWeight
            //nTruckTridemWeight
            //nTruckUnladenWeight
            //nTruckWeightAxle
            //nTruckWeightTotal
            //nTruckWidth
            //nView
            //nVisiblePointReachDistance
            //nVolumeMax
            //nVolumeMin
            //Voice
            //VoicePerson

    }
}
