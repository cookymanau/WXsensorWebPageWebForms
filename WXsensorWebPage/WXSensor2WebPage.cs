//#define MYTEST
//the MYTEST sets the ticker to 5000mS line 155

#define VERBOSELOGGING
// optional status stuff to the log


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Timers;

//using WXSensorWebPage.classIndexHtml;

//this program reads data from a sql database and fills the arrays below with that data.  A JS script page is
// written to the web server area for display on a web page.  This hopefully is stolen from previous work



namespace WXsensorWebPage
{
    public partial class WXSensor2WebPage : Form
    {
        //globals here
        //array to hold graphing data
        // hour is the index, the data is from a sensor 
        //these have all been made public and static so we can see them in the classIndexHtml etc

        public static double[] tIn = new double[24]; //{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tInYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tOut = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tOutYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static double[] tBOM = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //public double[] gtBOM() { return tBOM; }  // to access the array in the classIndexHtml class
        //public IEnumerable<double> GetValues() { return tBOM; }



        public static double[] tBOMYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRover = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRoverYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPool = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPoolYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] tHr = new int[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] BestMax = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] BestMin = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static double[] tInMin = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0 ,0,0,0,0,0,0,0};
        public static double[] tInMax = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0};
        public static double[] tOutMin = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tOutMax = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPoolMin = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPoolMax = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRoverMin = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRoverMax = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static double[] tInMinLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tInMaxLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tOutMinLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tOutMaxLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPoolMinLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tPoolMaxLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRoverMinLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] tRoverMaxLM = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public struct CurrentReadings
        { //this is to store current sensor reading for display on web page
            public double cTEMPIn;
            public double cTEMPOut;
            public double cTEMPPool;
            public double cTEMPRover;
            public double cTEMPBom;
            public double cHumidOut;
            public double cPressureOut;
        }
        CurrentReadings cr = new CurrentReadings();

        public struct MaxMinTemps
        {
            public double wxOutMax;
            public double wxOutMin;
            public string wxOutMaxHour;
            public string wxOutMinHour;
            public double wxInMax;
            public double wxInMin;
            public string wxInMaxHour;
            public string wxInMinHour;
            public double wxRoverMax;
            public double wxRoverMin;
            public double wxPoolMax;
            public double wxPoolMin;
        }
        MaxMinTemps mm = new MaxMinTemps();

        public struct BOMReadings
        { //this is to store current sensor reading for display on web page
            public double BestMax;  //this comes from another table now
            public double BestMin; //this comes from another table  - WXKALAMUNDAFC (forecast)
            public string BestRainfall;
            public double BcurrentTemp; 
            public double BcurrentHumid;
            public double BcurrentPress;
            public double BWindSpeed;
            public string BWindDir;
            public double Brainfall;
            public double BWindGust;
        }
        BOMReadings br = new BOMReadings();

        struct sensorCorrections
        {//store the sensor correction factors
            public double TinAdjust;
            public double ToutAdjust;
            public double TroverAdjust;
            public double TpoolAdjust;
            public double TbomAdjust;
            public double HinAdjust;
            public double HoutAdjust;
            public double HroverAdjust;
            public double HpoolAdjust;
            public double HbomAdjust;
        }
        sensorCorrections sc = new sensorCorrections();

        public struct houseConditions
        {//to store the cutoff conditions
            public double ThighTemp;
            public double TlowTemp;
            public double WhighWind;
        }
        houseConditions hc = new houseConditions();  //store the house cutoffs


        public struct trendingData
        {//to store the average data brought back from the database
            public double wxOut1; //avg temp this hour
            public double wxOut2; //avg temp previous hour
            public double wxOut3; //avg temp the 2hrs back
            public double trend1; //difference between wxOut1 - wxOut0
            public double trend2; //diffnce between wxOut1 - wxOut2
            public double wxIn1;
            public double wxIn2;
            public double wxIn3;
        }
        trendingData td = new trendingData(); //WX averaging data

        public struct SWSreadings
        {
            public DateTime readingDate;
            public double SFI;
            public double SSN;
            public double Ap;
            public double Kp;
            public string xRay;
            public double tIndex;
            public string Comment;
        }
        SWSreadings sws = new SWSreadings();

        public struct sensorStatus
        {
            public bool Ib, Ob, Pb, Rb;
        }
        sensorStatus ss = new sensorStatus();


        static double barMax = 0.0;
        string connectionString = @"Data Source=192.168.1.15\DAWES_SQL2008; Database = WeatherStation; User Id = WeatherStation; Password = Esp32a.b.;";
        static double tInAdjust, tOutAdjust, tRoverAdjust, tBOMadjust; //these are to calibrate the thermometers
        static uint recCnt = 0;
        static uint readBOMCount = 0;  //used to try and control how often we hit the database for information that only changes once per hour.
        static string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        DateTime rightNow = DateTime.Now;   //we use this a lot to get current time

        //this is the constructor
        public WXSensor2WebPage()
        {
            InitializeComponent();
            writeToLog("***** Program start *************");
            br.BcurrentPress = 999.9; //just in case
            hc.ThighTemp = double.Parse(txtHighTempcondition.Text);
            hc.TlowTemp = double.Parse(txtLowTempCondition.Text);
            hc.WhighWind = double.Parse(txtHighWindCondition.Text);

            double tempAvgThisHour = getTempTrendHourly("WXOUT", 1);
            double tempAvgLastHour = getTempTrendHourly("WXOUT", 2);

            //read the estimated mins and maxs from BOM into arrays for plotting
            // the name of the TABLE, the name of the array declared above and the naem of the field in the table
            //readBOMMinMaxDataFromDatabase("WXBOMGHILL", BestMax,"ESTBOMMAX");
            //     readBOMMinMaxDataFromDatabase("WXBOMGHILL", BestMin, "ESTBOMMIN");


            StartProgram();

           

        } //end of constructor

        private void btnExit_Click(object sender, EventArgs e)
        {
#if VERBOSELOGGING
            writeToLog($"********Exit the program**********");
            writeToLog("---------------------------------------------------");
#endif
            Application.Exit();
        }


        //Declaring timer - a FORMS timer Everything runs from this timer.
        public System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
        void SetTimer()
        {
            aTimer.Tick += OnTimedEvent;
#if MYTEST
            aTimer.Interval = 10000;// for testing only
            chkTweet.Checked = false;
            txtWebUpdateCycle.Text = "30";
#else
            aTimer.Interval = 60000; //miliseconds - fixed at once every minute for the basic tick
            chkTweet.Checked = true;
            txtWebUpdateCycle.Text = "120";
#endif
            aTimer.Enabled = true;
        } //end of function

        /// <summary>
        /// Called when [timed event].  This is where the hard work is done.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void OnTimedEvent(Object sender, EventArgs e)  //everything happens here
        {
            DateTime tickerTime = DateTime.Now;
            recCnt += 1;  //the first time through recCnt=1 (0 based)
            lblRecCnt.Text = recCnt.ToString();  //display on the service page what record we are at

            // all about getting the estimated BOM mins and maxes for the day
            if (readBOMCount < 30 && (tickerTime.Hour % 6 == 0 || recCnt == 1))  // read it for 30 minutes just in case from midnight to :30am
            {//do this at the beginning of the cycle and every time the Hour  is 1
                readBOMMinMaxDataFromDatabase("WXBOMGHILL", BestMax, "ESTBOMMAX");
                readBOMCount += 1;
            }

            if (tickerTime.Hour == 2 )  // set it back to 0 for thje next day
            {//do this at the beginning of the cycle and every time the Hour  is 1
                
                readBOMCount = 0;
                #if VERBOSELOGGING
  //                  writeToLog("tickerTime.Hour ==2 and putting readBOMCount to 0");
                #endif
            }

            if (recCnt == 1) //first thing to do is read these, then the data and fill the arrays
            {
                ReadCorrections();
#if VERBOSELOGGING
                writeToLog($"Reading the corrections At Record Number: {recCnt}");
#endif
            }


            if (recCnt % 1 == 0)  //every tick, ie once every tick - every 60 seconds
            {
                //fill the BOM br.BestMax and br.BestMin so we can use them everywhere
                //readBOMMinMaxDataFromDatabase("WXBOMGHILL", BestMax, "ESTBOMMAX");

                //fill the today arrays pass the table, arrays to fill and any adjustment
                // the TinAdjust, ToutAdjust ARE NOT collected from here, these just fill the arrays
                readSensorDataFromDatabase("WXIN", tIn, tInYesterday, sc.TinAdjust);
                System.Threading.Thread.Sleep(500);
                readSensorDataFromDatabase("WXOUT", tOut, tOutYesterday, sc.ToutAdjust);
                System.Threading.Thread.Sleep(500);
                readSensorDataFromDatabase("WXROVER", tRover, tRoverYesterday, sc.TroverAdjust);
                System.Threading.Thread.Sleep(500);
                readSensorDataFromDatabase("WXPOOL", tPool, tPoolYesterday, sc.TpoolAdjust);
                System.Threading.Thread.Sleep(500);
                readSensorDataFromDatabase("WXBOMGHILL", tBOM, tBOMYesterday, sc.TbomAdjust);
                System.Threading.Thread.Sleep(500);

                //get the current sensor reading from the database
                CurrentReadingFromDatabase("WXIN");
                System.Threading.Thread.Sleep(100);
                CurrentReadingFromDatabase("WXOUT");
                System.Threading.Thread.Sleep(100);
                CurrentReadingFromDatabase("WXROVER");
                System.Threading.Thread.Sleep(100);
                CurrentReadingFromDatabase("WXPOOL");
                System.Threading.Thread.Sleep(100);
                CurrentReadingFromDatabase("WXBOMGHILL");
                System.Threading.Thread.Sleep(100);
                BOMReadingFromDatabase("WXBOMGHILL");
                System.Threading.Thread.Sleep(100);
                BOMReadingMinMaxFromDatabase("WXKALAMUNDAFC");


                //update the House Conditions
                hc.ThighTemp = double.Parse(txtHighTempcondition.Text);
                hc.TlowTemp = double.Parse(txtLowTempCondition.Text);
                hc.WhighWind = double.Parse(txtHighWindCondition.Text);

                // this is the averaging / trending data
                //double tempAvgThisHour = getTempTrendHourly("WXOUT", 1);
                td.wxOut1 = getTempTrendHourly("WXOUT", 1);
                td.wxOut2 = getTempTrendHourly("WXOUT", 2);
                td.wxOut3 = getTempTrendHourly("WXOUT", 3);

                td.trend1 = td.wxOut1 - td.wxOut2;
                td.trend2 = td.wxOut2 - td.wxOut3;
                td.trend1 = Math.Round(td.trend1, 1);
                td.trend2 = Math.Round(td.trend2, 1);

                // these 4 calls to the GetMaxMin... populate the minmax arrays from the database
                //and display the data on the buttons on index.html
                GetMaxMinTempFromDatabase("WXOUT", "MAX");
                GetMaxMinTempFromDatabase("WXOUT", "MIN");
                GetMaxMinTempFromDatabase("WXIN", "MIN");
                GetMaxMinTempFromDatabase("WXIN", "MAX");

                //update the sensor readings are they alive
                TimeLastReading();


                //this my test of classIndexHtml()
                //object reference is below.  You have to do this
                classIndexHtml chtm = new classIndexHtml();
                chtm.createHTMLpage(br, cr, mm, ss, sws, hc, td);

                classIndexHtml cih = new classIndexHtml();
                cih.createHTMLpage(br, cr, mm, ss, sws, hc, td );

                 //this is the radar plot of wind and wind direction , we should make it so that it only gets todays data here
                classRadarChart crc = new classRadarChart();
                crc.getData();  //so we instantiate the class as an object and call the getdata method t get the data we want and call the html from there





                // this is the main page index.html
                // writeToHTML();  //write the index.html page
            }
            // this is the Twitter announcement.
            if ((recCnt % 120 == 0 || recCnt == 2) && chkTweet.Checked) //every 90 mins and just after startup  IFFT have changed their rules to 26 tweets per day
            {
                webPOSTtoScarpWeather(cr.cTEMPIn, cr.cHumidOut, cr.cPressureOut, cr.cTEMPOut);  //send it to Scarpweather
#if VERBOSELOGGING
                writeToLog($"Twitter WebPost : Record Number: {recCnt}");

#endif

            }

            if (recCnt == 1 || recCnt % 60 == 0)
            {
                //status report to log every 60 counts
                readSWSfromDatabase();
                //lets fill the min max arrays as we start and every hour
                // and also write the html pages
                prepareMinMaxArrays("WXOUT");
                prepareMinMaxArrays("WXIN");
                prepareMinMaxArrays("WXPOOL");
                prepareMinMaxArrays("WXROVER");


#if VERBOSELOGGING
                writeToLog($"Reading SWS data fromdatabase recCnt: {recCnt}");
                writeToLog($"   also updating the MinMax web pages: {recCnt}");
#endif
            }

            if (recCnt % 60 == 0)  // evey 60 minutes
            {
                //status report to log every 30 counts
                writeToLog($" -----Status Report...OK At Record Number: {recCnt} ");
            }



        }//end of the timed function

        //private void btnStart_Click(object sender, EventArgs e) { }

        private void StartProgram()
        {
            lblStartTime.Text = now.ToString();
            btnStart.Text = "Running";
            SetTimer(); //Start the timer loop and go forever

        }

        /// <summary>
        /// Reads the sensor data from database and fills the arrays that hold the average reading for the hour.
        /// </summary>
        /// <param name="table">The table in the SQL database to read from</param>
        /// <param name="tArray">The t array for the current 24 hours (midnight to midnight</param>
        /// <param name="tArrayYesterday">The t array yesterday.</param>
        private void readSensorDataFromDatabase(string table, double[] tArray, double[] tArrayYesterday, double tAdjust)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngTemp;
            DateTime rdngTime;
            DateTime rightNow = DateTime.Now;
            int intHour;
            int counter = 0;
            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
     //     SqlCommand getReadings = new SqlCommand($"select TIME, ROUND(AvgValue, 1) as AvgTemp from(select TIME = dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)), AvgValue = AVG(TEMP)from {table} group by dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)))a order by TIME DESC", conn);

            //this bit courtesy of stackoverflow - how to do these sort of long queries in C# formatting
            // get the reading from the database as closes as possible to the top of hour - a few seconds either side
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"select top 72 * from {table} yt join( ");
            sb.AppendLine(@"  select  min(TIME) as dt ");
            sb.AppendLine($" from {table} ");
            sb.AppendLine(@" where datediff(day, TIME, getdate()) <= 60  ");
            sb.AppendLine(@" group by cast(TIME as date), datepart(hh, TIME)  ");
            sb.AppendLine(@" ) filter  ");
            sb.AppendLine(@" on  filter.dt = yt.TIME ");
            sb.AppendLine(@" order by TIME DESC ");

            string topOfTheHour = sb.ToString();  //make a big single string of the query...and execute it
            SqlCommand getReadings = new SqlCommand(topOfTheHour,conn);

            try
            {
                conn.Open();
                rdr = getReadings.ExecuteReader();
                lblNowTime.Text = rightNow.TimeOfDay.ToString();
                Array.Clear(tArray, 0, 24);
                Array.Clear(tArrayYesterday, 0, 24);

                while (rdr.Read())
                {
                    // get the results of each column
                    rdngTime = (DateTime)rdr["TIME"]; //this is the ime of thereading from the database
                    
                    //rdngTemp = (double)rdr["AvgTemp"];
                    rdngTemp = (double)rdr["TEMP"];


                    if (rightNow.Day == rdngTime.Day)
//                    if (DateTime.Now.Day == rdngTime.Day) //this ensures we get TODAYS readings  DateTime.Now.Day is todays date
                    {
                        intHour = (int)(rdngTime.Hour); //this is the hour of the reading returned by the SQL above

                        if (intHour <= DateTime.Now.Hour ) // for some reason the end of the arrays were filling up. This stops that
                        {
                            tArray[intHour] = Math.Round(rdngTemp + tAdjust, 1);    //get todays readings of temperature and put in the array and add the correction
                            counter += 1;
                        }
                    }
                    else if (rightNow.Day - 1 == rdngTime.Day)  //fills yesterdays arrays
                    {
                        intHour = (int)(rdngTime.Hour);
                        tArrayYesterday[intHour] = Math.Round(rdngTemp + tAdjust ,1) ; //yesterdays reading of temp into the array
                    }
                    // this is the attempt to fill the tHr array for the vertical bar to the highest numerical value
                    if (rdngTemp > barMax && rightNow.Day == rdngTime.Day)
                    {
                        barMax = rdngTemp;
                    }
                    Array.Clear(tHr, 0, 24);  //empty the array for the new hour???
                    tHr[rightNow.Hour] = (int)barMax;
                }
                conn.Close();
            } catch(Exception ex)
            {
                writeToLog($"ERROR - readSensorDataFromDatabase: {ex}");
            }
        }

        /// <summary>
        /// Reads the bom minimum maximum data from database.  The SQL query gets the last reading at 9am and uses that for the 
        /// rest of the run   The time is currently set at 9 but it might be better to be 1
        /// </summary>
        /// <param name="table">BOM readings table usuially WXBOMGHILL</param>
        /// <param name="tArray">The array to write the estMax and Min data to.</param>
        /// <param name="field">Not used</param>
        private void readBOMMinMaxDataFromDatabase(string table, double[] tArray,string field)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngTemp;
            double tempToUse = 0;
            DateTime rdngTime;
            DateTime rightNow = DateTime.Now;
            int intHour;
            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
            SqlCommand getReadings = new SqlCommand($"select top 1 TIME, TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from {table}  where  datepart(hh, TIME) = 0 order by TIME DESC", conn);
            //                                        select top 1 TIME, TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from WXBOMGHILL where  datepart(hh, TIME) = '9'   order by TIME DESC

                Array.Clear(tArray, 0, 24);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read()) { 
                    // get the results of each column
                    rdngTime = (DateTime)rdr["TIME"];
                    br.BestMax = (double)rdr["ESTBOMMAX"];
                    br.BestMin = (double)rdr["ESTBOMMIN"];
                }
                conn.Close();
        }

        /// <summary>
        /// This gets the very latest readings from the database for display on the web page.
        /// </summary>
        /// <param name="table">The table.</param>
        private void CurrentReadingFromDatabase(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            //double rdngTemp;
            //DateTime rdngTime;
            DateTime rightNow = DateTime.Now;
            //int intDay, intHour;
            try
            {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  top 1 TEMP,HUMID,PRESS from {table} order by TIME DESC", conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    if (table == "WXIN")
                    {
                        cr.cTEMPIn = (double)rdr["TEMP"];
                        cr.cTEMPIn += sc.TinAdjust;
                        cr.cTEMPIn = Math.Round(cr.cTEMPIn, 1);
                    }
                    else if (table == "WXOUT")
                    {
                        cr.cTEMPOut = (double)rdr["TEMP"];
                        cr.cTEMPOut += sc.ToutAdjust;  //add the adjustment
                        cr.cTEMPOut = Math.Round(cr.cTEMPOut, 1);

                        cr.cHumidOut = (double)rdr["HUMID"];
                        cr.cPressureOut = br.BcurrentPress;  //this is because we dont have a working pressure sensor
                    }
                    else if (table == "WXROVER")
                    {
                        cr.cTEMPRover = (double)rdr["TEMP"];
                        cr.cTEMPRover += sc.TroverAdjust;
                        cr.cTEMPRover = Math.Round(cr.cTEMPRover, 1);
                    }
                    else if (table == "WXPOOL")
                    {
                        cr.cTEMPPool = (double)rdr["TEMP"];
                        cr.cTEMPPool += sc.TpoolAdjust;
                        cr.cTEMPPool = Math.Round(cr.cTEMPPool, 1);
                    }
                    else if (table == "WXBOMGHILL")
                    {
                        cr.cTEMPBom = (double)rdr["TEMP"];
                    }
                }
                conn.Close();
            } catch (Exception ex)
            {
                writeToLog($"ERROR-CurrentReadingFromDatabase: {ex}");
            }
        }



        // -------------vvvvvvvvvvvvvv  Max and Min from database vvvvvvvvvvvvvv------------
        private void GetMaxMinTempFromDatabase(string table,string minmax )
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
           System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            { // get the min and max temps and their respective times
                sb.AppendLine($"select TOP 1 {minmax}(TEMP) as [Temp] ");
                sb.AppendLine($", TIME as [Time] ");
                sb.AppendLine($",RIGHT('0' + CAST(DATEPART(hour, TIME) as varchar(2)), 2) + ':' + RIGHT('0' + CAST(DATEPART(minute, TIME) as varchar(2)), 2) as [Hour] ");
                sb.AppendLine($" from {table} where convert(date, TIME) = convert(date, getdate()) ");

                if (minmax == "MAX")
                {
                    sb.AppendLine($" group by TEMP,TIME order by TEMP DESC "); } //getting a min and max requires slightly different sorting
                else if (minmax == "MIN")
                {
                    sb.AppendLine($" group by TEMP,TIME order by TEMP ASC "); }

                string getMaxTempTime = sb.ToString();  //make a big single string of the query...and execute it

                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand(getMaxTempTime, conn); // get todays Max
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    if (table == "WXIN" && minmax == "MAX")
                    {
                        mm.wxInMax = (double)rdr["Temp"];
                        mm.wxInMax += sc.TinAdjust;
                        mm.wxInMax = Math.Round(mm.wxInMax, 1);
                        mm.wxInMaxHour = rdr["Hour"].ToString();
                    }
                    if (table == "WXIN" && minmax == "MIN")
                    {
                        mm.wxInMin = (double)rdr["Temp"];
                        mm.wxInMin += sc.TinAdjust;
                        mm.wxInMin = Math.Round(mm.wxInMin, 1);
                        mm.wxInMinHour = rdr["Hour"].ToString();
                    }
                    else if (table == "WXOUT" && minmax == "MAX")
                    {
                        mm.wxOutMax = (double)rdr["Temp"];
                        mm.wxOutMax += sc.ToutAdjust;  //add the adjustment
                        mm.wxOutMax = Math.Round(mm.wxOutMax, 1);
                        mm.wxOutMaxHour = rdr["Hour"].ToString();
                    }
                    else if (table == "WXOUT" && minmax == "MIN")
                    {
                        mm.wxOutMin = (double)rdr["Temp"];
                        mm.wxOutMin += sc.ToutAdjust;  //add the adjustment
                        mm.wxOutMin = Math.Round(mm.wxOutMin, 1);
                        mm.wxOutMinHour = rdr["Hour"].ToString();
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                writeToLog($"ERROR-MaxReadingFromDatabase: {ex}");
            }
        }

        //---------------------^^^^^^^^^^^^^^^^^ end of function ^^^^^^^^^^^^^^-----------


        // --------------- Get Max Temps
        private void GetMaxMinReadingFromDatabase(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
            try
            {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select max(TEMP)as [MaxTemp],min(TEMP)as [MinTemp] from {table} where convert(date,TIME) = convert(date,getdate())", conn); // get todays Max
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    if (table == "WXIN")
                    {
                        mm.wxInMax = (double)rdr["MaxTemp"];
                        mm.wxInMax += sc.TinAdjust;
                        mm.wxInMax = Math.Round(mm.wxInMax, 1);
                        mm.wxInMin = (double)rdr["MinTemp"];
                        mm.wxInMin += sc.TinAdjust;
                        mm.wxInMin = Math.Round(mm.wxInMin, 1);
                    }
                    else if (table == "WXOUT")
                    {
                        mm.wxOutMax = (double)rdr["MaxTemp"];
                        mm.wxOutMax += sc.ToutAdjust;  //add the adjustment
                        mm.wxOutMax = Math.Round(mm.wxOutMax, 1);
                        mm.wxOutMin = (double)rdr["MinTemp"];
                        mm.wxOutMin += sc.ToutAdjust;  //add the adjustment
                        mm.wxOutMin = Math.Round(mm.wxOutMin, 1);
                    }
                    else if (table == "WXROVER")
                    {
                        mm.wxRoverMax = (double)rdr["MaxTemp"];
                        mm.wxRoverMax += sc.TroverAdjust;
                        mm.wxRoverMax = Math.Round(mm.wxRoverMax, 1);
                        mm.wxRoverMin = (double)rdr["MinTemp"];
                        mm.wxRoverMin += sc.TroverAdjust;
                        mm.wxRoverMin = Math.Round(mm.wxRoverMin, 1);
                    }
                    else if (table == "WXPOOL")
                    {
                        mm.wxPoolMax = (double)rdr["MaxTemp"];
                        mm.wxPoolMax += sc.TpoolAdjust;
                        mm.wxPoolMax = Math.Round(mm.wxPoolMax, 1);
                        mm.wxPoolMin = (double)rdr["MinTemp"];
                        mm.wxPoolMin += sc.TpoolAdjust;
                        mm.wxPoolMin = Math.Round(mm.wxPoolMin, 1);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                writeToLog($"ERROR-MaxReadingFromDatabase: {ex}");
            }
        }

//---------------------^^^^^^^^^^^^^^^^^ end of function ^^^^^^^^^^^^^^-----------



        private void BOMReadingFromDatabase(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
            try
            {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  top 1 TIME, TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX,WINDGUST from {table}   order by TIME DESC", conn);
                //select top 1 TIME,TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from WXBOMGHILL where  datepart(hh, TIME) = '9'   order by TIME DESC
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    // get the results of each column
                    br.BcurrentTemp = (double)rdr["TEMP"];
                    br.BcurrentHumid = (double)rdr["HUMID"];
                    br.BcurrentPress = (double)rdr["PRESS"];
                    br.BWindSpeed = (double)rdr["WINDSPEED"];
                    br.BWindDir = (string)rdr["WINDDIR"];
                    br.Brainfall = (double)rdr["RAINFALL"];
                    br.BWindGust = (double)rdr["WINDGUST"];
                    //br.BestMax = (double)rdr["ESTBOMMAX"];  //dont want to do this all the time -once per day by readBOMMInMaxFromdatabase() higher up
                    //br.BestMin = (double)rdr["ESTBOMMIN"];
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                writeToLog($"ERROR-BOMReadingFromDatabase {ex}");
            }
        }
        private void BOMReadingMinMaxFromDatabase(string table)  //this is new 20200429 to replace other tries at getting this data
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
   //         try
   //         {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  top 1 * from {table} where [ISOYEAR] = convert(char(10), getdate(), 112) order by [TIME] DESC", conn);
                //select top 1 TIME,TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from WXBOMGHILL where  datepart(hh, TIME) = '9'   order by TIME DESC
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    // get the results of each column
                    br.BestMax = double.Parse(rdr["MAXTEMP"].ToString());  //dont want to do this all the time -once per day by readBOMMInMaxFromdatabase() higher up
                    br.BestMin = double.Parse(rdr["MINTEMP"].ToString());
                    br.BestRainfall = rdr["FORECAST"].ToString();

                }
                conn.Close();
 //          }
 //          catch (Exception ex)
 //           {
 //               writeToLog($"ERROR-BOMReadingFromDatabase Min and Max Data {ex}");
 //           }
        }


        private void readSWSfromDatabase()
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
            try
            {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  top 1 * from SPACEWEATHER order by TIME DESC", conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    // get the results of each column
                    sws.SFI = (double)rdr["SFI"];
                    sws.SSN = (double)rdr["SSN"];
                    sws.Ap = (double)rdr["Ap"];
                    sws.Kp = (double)rdr["Kp"];
                    sws.xRay = (string)rdr["XRAY"];
                    sws.tIndex = (double)rdr["TINDEX"];
                }
                conn.Close();
            }

            catch (Exception ex)
            {
                writeToLog($"ERROR- BOM SWS data fromDatabase {ex}");
            }
        }//end of function




        /// <summary>
        /// Reads the corrections from the SENSORURL table for correcting the readings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //private void btnReadCorrections_Click(object sender, EventArgs e)
        void ReadCorrections()
        {
            CorrectionsFromDatabase("SENSORURL", "WXIN"); //get the corrections
            CorrectionsFromDatabase("SENSORURL", "WXOUT"); //get the corrections
            CorrectionsFromDatabase("SENSORURL", "WXROVER"); //get the corrections
            CorrectionsFromDatabase("SENSORURL", "WXPOOL"); //get the corrections
            CorrectionsFromDatabase("SENSORURL", "WXBOM"); //get the corrections
#if VERBOSELOGGING
            writeToLog($"Reading Temperature corrections ");
#endif

        }



        private double getTempTrendHourly(string table,double rownum) {

            //string getTEMPHour = $"select TOP 1 * from (select row_number() over (order by TIME DESC) as ROWNUMBER,TIME, ROUND(AvgValue,1) as AvgTemp from (select TIME=dateadd(hh,datepart(hh,TIME), cast(CAST(TIME as date) as datetime)), AvgValue=AVG(TEMP)from  {table} group by dateadd(hh,datepart(hh,TIME), cast(CAST(TIME as date) as datetime)))a)b where ROWNUMBER = {rownum}";

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine($"SELECT * FROM( ");
                sb.AppendLine($" select top 5 *, ROWNUMBER = row_number() over(order by TIME desc) from {table} yt join ( ");
                sb.AppendLine($" select min(TIME) as dt ");
                sb.AppendLine($"from {table} ");
                sb.AppendLine($" where datediff(day, TIME, getdate()) <= 60 ");
                sb.AppendLine($"group by cast(TIME as date),datepart(hh, TIME) ");
                sb.AppendLine($" ) filter ");
                sb.AppendLine($" on  filter.dt = yt.TIME ");
                sb.AppendLine($" )b where ROWNUMBER = {rownum} ");
                sb.AppendLine($" order by TIME desc ");

                string getTEMPHour = sb.ToString();


                SqlConnection conn;
                SqlDataReader rdr = null;
                double data;
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getHour = new SqlCommand(getTEMPHour, conn);
                conn.Open();
                rdr = getHour.ExecuteReader();

                rdr.Read();
                data = (double)rdr["TEMP"];
                conn.Close();
                return data;
            }
            catch
            {
                return 0;
            }


            //return 
        }



        /// <summary>
        /// Read the sensor corrections from database.
        /// </summary>
        /// <param name="table">The table.</param>
        private void CorrectionsFromDatabase(string table, string sens)
        {// this only happens once - at the beginning of the program after the START button is pressed
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;

            try {
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  SENSOR,TEMP_CORRECTION,HUMID_CORRECTION from {table} where SENSOR ='{sens}' ", conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    // get the results of each column
                    if (sens == "WXIN"){
                        sc.TinAdjust = (float)rdr["TEMP_CORRECTION"];
                    }
                    else if (sens == "WXOUT") {
                        sc.ToutAdjust = (float)rdr["TEMP_CORRECTION"];
                    }
                    else if (sens == "WXROVER"){
                        sc.TroverAdjust = (float)rdr["TEMP_CORRECTION"];
                    }
                    else if (sens == "WXBOM"){
                        sc.TbomAdjust = (float)rdr["TEMP_CORRECTION"];
                    }
                    else if (sens == "WXPOOL")
                    {
                        sc.TpoolAdjust = (float)rdr["TEMP_CORRECTION"];
                    }

                }
                conn.Close();
            }
            catch (Exception ex){
                writeToLog($"ERROR - reading corrections from the database {ex}");}

        }// end of CorrectionsFromDatabase

        private void btnReadCorrections_Click(object sender, EventArgs e)
        {
            ReadCorrections();
            MessageBox.Show("Re Read Corrections. Wait one minute.");
#if VERBOSELOGGING
            writeToLog($"Manual read of correction data from database - button pressed");
#endif

        }

 



        /// <summary>
        /// Posts the data to TWITTER scarp weather.
        /// </summary>
        /// <param name="temp">The temporary.</param>
        /// <param name="humid">The humid.</param>
        /// <param name="press">The press.</param>
        private void webPOSTtoScarpWeather(double temp, double humid, double press, double strOutTemp1)
        {
            try
            {
                double outTemp = strOutTemp1;  //this is straight from MS example code - more or less.
                string URL = "https://maker.ifttt.com/trigger/scarpweather_Data_Push/with/key/bvtUzCTPgTSrphRutbFIBh/";
                //string postData = "{ value1 : T:23f, value2 : H: 74s, value3 : P: 995s }";
                //string postData = "value1=T:23f&value2=H:74s&value3=P:995s";

                string postData = "value1=\nTemp In:" + temp + " Temp Out:" + outTemp + "&value2=\nHumidity:" + humid + "&value3=\nPressure:" + press + ".";
                //writeToLog("Twitter Post Data " + postData);

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                WebRequest request = WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";// Set the ContentType property of the WebRequest.  
                request.ContentLength = byteArray.Length;// Set the ContentLength property of the WebRequest.  
                Stream dataStream = request.GetRequestStream();// Get the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);// Write the data to the request stream.  
                dataStream.Close();// Close the Stream object.  

                WebResponse response = request.GetResponse(); // Get the response.  
                                                              // Display the status.  
                                                              // Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                                                              // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);// Open the stream using a StreamReader for easy access.  

                string responseFromServer = reader.ReadToEnd();// Read the content.  
                                                               // Display the content.  
                                                               //Console.WriteLine(responseFromServer);
                                                               // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                writeToLog($"ERROR- in function webPOSTtoScarpWeather {ex}");
            }
        }

        /// <summary>
        /// Writes to the logfile, typically where I have a Catch situation
        /// </summary>
        /// <param name="mesg">The mesg.</param>
        void writeToLog(string mesg)
            {
                // var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
                var pathWithEnv = @"%USERPROFILE%\Documents\WXWebPage.log";
                var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

                using (StreamWriter sw = new StreamWriter(filePath, append: true))  //using controls low-level resource useage
                {
                string nowDate = DateTime.Now.ToString("d");
                string now = DateTime.Now.ToString("h:mm:ss tt");
                    sw.WriteLine($"Log Time: {nowDate} : {now} : {mesg}"); //thats String Interpolation -the $ and curly brackets make it easy!
                    sw.Close();
                }
        }


        //-------------vvvvvvvvvvvvvv time of last reading from database vvvvvvvvvvvvvv------------   
        /// <summary>
        /// To get the most recent time of the last reading from the database and use these readings
        /// compared to RightNow to alert to a malfunctioning sensor.  Use to the nearest minute or two
        /// </summary>
        private void TimeLastReading()
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string wxtable;
            DateTime rdngTime;
            try
            { // get the min and max temps and their respective times
                sb.AppendLine($" SELECT * FROM(select TOP 1 'WXPOOL' as [Table], TIME as [MostRecent] from WXPOOL order by TIME DESC)a ");
                sb.AppendLine($" union ");
                sb.AppendLine($" SELECT * FROM(select TOP 1 'WXROVER' as [Table], TIME from WXROVER order by TIME DESC)b ");
                sb.AppendLine($"  union ");
                sb.AppendLine($" SELECT * FROM(select TOP 1 'WXOUT' as [Table], TIME from WXOUT order by TIME DESC)c ");
                sb.AppendLine($"  union ");
                sb.AppendLine($" select * from(select TOP 1 'WXIN' as [Table], TIME from WXIN order by TIME DESC)d ");
                string getLastReadingTime = sb.ToString();  //make a big single string of the query...and execute it

                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand(getLastReadingTime, conn); // get todays Max
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    rdngTime = (DateTime)rdr["MostRecent"]; //this is the time of thereading from the database
                    wxtable = (string)rdr["Table"];

                    //attempting give around a couple of minutes leeway before reporting an red led
                    //and laying it out this way is better than the convoluted long logic
                    if(rdngTime.Hour == rightNow.Hour && (rdngTime.Minute > rightNow.Minute - 2 || rdngTime.Minute < rightNow.Minute + 2))
                    {
                        switch (wxtable)
                        {
                            case "WXPOOL":
                                ss.Pb = true;
                                break;
                            case "WXROVER":
                                ss.Rb = true;
                                break;
                            case "WXOUT":
                                ss.Ob = true;
                                break;
                            case "WXIN":
                                ss.Ib = true;
                                break;
                        }//end case
                    }//end if
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                writeToLog($"ERROR-Calculating sensor status from the Database: {ex}");
            }
        }

        //---------------------^^^^^^^^^^^^^^^^^ end of function ^^^^^^^^^^^^^^-----------

        void prepareMinMaxArrays(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngMaxTemp;
            double rdngMinTemp;
            DateTime rdngDate;
            DateTime rightNow = DateTime.Now;
            conn = new SqlConnection(connectionString);  //connectionString is a global ATM

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"select cast(TIME as date) as date, min(TEMP) as mintemp, max(TEMP) as maxtemp ");
            sb.AppendLine($" from {table} pvh ");
            sb.AppendLine($" group by cast(TIME as date) ");
            sb.AppendLine($" order by 1 DESC ");
            sb.AppendLine($" ");
            string sqlQuery = sb.ToString();
            SqlCommand getReadings = new SqlCommand(sqlQuery, conn);
            conn.Open();
            rdr = getReadings.ExecuteReader();

            while (rdr.Read())
            {
                // get the results of each column
                rdngDate = (DateTime)rdr["date"]; //this is the ime of thereading from the database
                rdngMaxTemp = (double)rdr["maxtemp"];
                rdngMinTemp = (double)rdr["mintemp"];

                //---------------------------
                if (table == "WXIN" && rdngDate.Month == rightNow.Month)
                {
                    tInMin[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tInMax[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }
                else if (table == "WXIN" && (rdngDate.Month == rightNow.Month - 1))
                {
                    tInMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tInMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }

                else if (table == "WXIN" && (rdngDate.Year == rightNow.Year - 1 && rdngDate.Month == 12)) //this is for last year if we are in January
                {
                    tInMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tInMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }

                //------------------------------
                if (table == "WXOUT" && rdngDate.Month == rightNow.Month)
                {
                    tOutMin[rdngDate.Day] = rdngMinTemp + sc.ToutAdjust;
                    tOutMax[rdngDate.Day] = rdngMaxTemp + sc.ToutAdjust;
                }
                else if (table == "WXOUT" && (rdngDate.Month == rightNow.Month - 1))
                {
                    tOutMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tOutMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }
                else if (table == "WXOUT" && (rdngDate.Year == rightNow.Year - 1 && rdngDate.Month == 12)) //this is for last year if we are in January
                {
                    tOutMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tOutMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }

                //-----------------------------------------------
                if (table == "WXPOOL" && rdngDate.Month == rightNow.Month)
                {
                    tPoolMin[rdngDate.Day] = rdngMinTemp + sc.TpoolAdjust;
                    tPoolMax[rdngDate.Day] = rdngMaxTemp + sc.TpoolAdjust;
                }
                else if (table == "WXPOOL" && (rdngDate.Month == rightNow.Month - 1))
                {
                    tPoolMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tPoolMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }
                else if (table == "WXPOOL" && (rdngDate.Year == rightNow.Year - 1 && rdngDate.Month == 12)) //this is for last year if we are in January
                {
                    tPoolMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tPoolMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }

                //----------------------------------------------------------
                if (table == "WXROVER" && rdngDate.Month == rightNow.Month)
                {
                    tRoverMin[rdngDate.Day] = rdngMinTemp + sc.TroverAdjust;
                    tRoverMax[rdngDate.Day] = rdngMaxTemp + sc.TroverAdjust;
                }
                else if (table == "WXROVER" && (rdngDate.Month == rightNow.Month - 1))
                {
                    tRoverMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tRoverMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }
                else if (table == "WXROVER" && (rdngDate.Year == rightNow.Year - 1 && rdngDate.Month == 12)) //this is for last year if we are in January
                {
                    tRoverMinLM[rdngDate.Day] = rdngMinTemp + sc.TinAdjust;
                    tRoverMaxLM[rdngDate.Day] = rdngMaxTemp + sc.TinAdjust;
                }
            }
            conn.Close(); //close the connection to the database

            classMinMaxHtml cmm = new classMinMaxHtml(1);
            cmm.MinMaxHTML();  //create the webpage
                               

            classMinMaxHtml cmmLM = new classMinMaxHtml(2);
            cmmLM.MinMaxHTML();  //create the webpage

            classRadarChart crc = new classRadarChart();
            crc.getData();  //so we instantiate the class as an object and call the getdata method t get the data we want and call the html from there

        }// end of the function

      
        


    } //end of program
}//end of namespace
