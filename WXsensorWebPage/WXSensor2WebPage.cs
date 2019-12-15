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

//this program reads data from a sql database and fills the arrays below with that data.  A JS script page is
// written to the web server area for display on a web page.  This hopefully is stolen from previous work



namespace WXsensorWebPage
{
    public partial class WXSensor2WebPage : Form
    {
        //globals here
        //array to hold graphing data
        // hour is the index, the data is from a sensor 
        double[] tIn = new double[24]; //{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tInYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tOut = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tOutYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tBOM = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tBOMYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tRover = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tRoverYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tPool = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] tPoolYesterday = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] tHr = new int[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] BestMax = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] BestMin = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        struct CurrentReadings
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

        struct BOMReadings
        { //this is to store current sensor reading for display on web page
            public double BestMax;
            public double BestMin;
            public double BcurrentTemp;
            public double BcurrentHumid;
            public double BcurrentPress;
            public double BWindSpeed;
            public string BWindDir;
            public double Brainfall;
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

        struct houseConditions
        {//to store the cutoff conditions
            public double ThighTemp;
            public double TlowTemp;
            public double WhighWind;
        }
        houseConditions hc = new houseConditions();  //store the house cutoffs


        struct trendingData
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

        struct SWSreadings
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



        static double barMax = 0.0;
        string connectionString = @"Data Source=192.168.1.109\DAWES_SQL2008; Database = WeatherStation; User Id = WeatherStation; Password = Esp32a.b.;";
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
        }

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
            aTimer.Interval = 5000;// for testing only
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
                writeToHTML();  //write the index.html page
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
                //status report to log every 30 counts
                readSWSfromDatabase();
#if VERBOSELOGGING
                writeToLog($"Reading SWS data fromdatabase recCnt: {recCnt}");
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
            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
     //     SqlCommand getReadings = new SqlCommand($"select TIME, ROUND(AvgValue, 1) as AvgTemp from(select TIME = dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)), AvgValue = AVG(TEMP)from {table} group by dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)))a order by TIME DESC", conn);

            //this bit courtesy of stackoverflow - how to do these sort of long queries in C# formatting
            // get the reading from the database as closes as possible to the top of hour - a few seconds either side
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"select * from {table} yt join( ");
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
                while (rdr.Read())
                {
                    // get the results of each column
                    rdngTime = (DateTime)rdr["TIME"]; //this is the ime of thereading from the database
                    
                    //rdngTemp = (double)rdr["AvgTemp"];
                    rdngTemp = (double)rdr["TEMP"];


                    //if (rightNow.Day == rdngTime.Day)
                    if (DateTime.Now.Day == rdngTime.Day) //this ensures we get TODAYS readings  DateTime.Now.Day is todays date
                    {
                        intHour = (int)(rdngTime.Hour); //this is the hour of the reading returned by the SQL above

                        if (intHour <= DateTime.Now.Hour ) // for some reason the end of the arrays were filling up. This stops that
                        {
                            tArray[intHour] = Math.Round(rdngTemp + tAdjust, 1);    //get todays readings of temperature and put in the array and add the correction
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


        private void BOMReadingFromDatabase(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngTemp;
            double rdngHumid;
            double rdngPressure;
            double rdngWindspeed;
            double rdngRainfall;
            double rdngEstbommin;
            double rdngEstbommax;
            string rdngWinddir;
            DateTime rdngTime;
            DateTime rightNow = DateTime.Now;
            int intDay, intHour;

            try
            {

                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                SqlCommand getReadings = new SqlCommand($"select  top 1 TIME, TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from {table}   order by TIME DESC", conn);
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


     


        /// <summary>
        /// Writes the html fo the webpage
        /// </summary>
        void writeToHTML()
        {

            // var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
            var pathWithEnv = @"c:\inetpub\wwwroot\index.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            string updateCycle;  //= txtWebUpdateCycle.Text;
            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                string now = DateTime.Now.ToString("h:mm:ss tt");
                string fullDate = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt");
                updateCycle = txtWebUpdateCycle.Text;

                sw.WriteLine("<HTML><HEAD>");
                sw.WriteLine("<title>SCARP Weather</title>");
                sw.WriteLine($"<meta http-equiv = \"refresh\" content = \"{updateCycle}\" >");// 1 is 1 second  60 is 60 seconds

                sw.WriteLine(@"<meta name=""viewport"" content=""width = device - width, initial - scale = 1"">");
                sw.WriteLine("<style>");
                sw.WriteLine("* {box-sizing: border-box; } ");
                sw.WriteLine(".column {");
                sw.WriteLine("float: left;");
                sw.WriteLine("width: 50%;");
                sw.WriteLine("padding: 10px; }");

                sw.WriteLine(".row:after {");
                sw.WriteLine("content: \"\";");
                sw.WriteLine("display: table;");
                sw.WriteLine("clear: both;}");

                sw.WriteLine("table.t1 { border-collapse: collapse; width: 100%;}");
                sw.WriteLine("table.t1 th{ font-size:24px;}");
                sw.WriteLine("table.t1 td{ font-size:16px; height: 20px; padding: 5px; border-bottom:1px solid #ddd;}");
                sw.WriteLine("table.t2 { border-collapse: collapse; width: 50%;}");
                sw.WriteLine("table.t2 th{ font-size:24px;}");
                sw.WriteLine("table.t2 td{ font-size:16px; height: 20px; padding: 5px; border-bottom:1px solid #ddd;}");

                sw.WriteLine("");
                sw.WriteLine(".button_green{background-color: green; color: white; font-size: 14px;}");  //a bit of css to use to colour the buttons as required
                sw.WriteLine(".button_red{background-color: red; color: black; font-size: 14x;}");
                sw.WriteLine(".button_white{background-color: white; color: black; font-size: 14px;}");
                sw.WriteLine(".button_yellow{background-color: yellow; color: black; font-size: 14px;}");
                sw.WriteLine(".button_blue{background-color: blue; color: white; font-size: 14px;}");

                sw.WriteLine(".button_blueInfo{background-color: blue; color: white;font-size: 14px; padding: 4px 10px;}");
                sw.WriteLine(".button_redInfo{background-color: red; color: white;font-size: 14px; padding: 4px 10px;}");
                sw.WriteLine(".button_greenInfo{background-color: green; color: white;f1ont-size: 14px; padding: 4px 10px;}");

                sw.WriteLine(".button_info{background-color: lghtgray; color: blue;font-size: 14px; padding: 4px 10px;}");

                sw.WriteLine(".btn-group button{background-color: #4CAF50; border: 1px solid green; color: white; padding: 10px 24px; cursor:pointer; float:left;}");
                sw.WriteLine("</style>");
                //this is the library for charts.js
                sw.WriteLine(@"<script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js""></script>");
                sw.WriteLine(@"</head><body>");
                //sw.WriteLine("<center><h1>Scarp Weather Weather Station</H1>");

                sw.WriteLine("</center>");

                 sw.WriteLine(@"<iframe src = ""http://free.timeanddate.com/clock/i70o7n5a/n196/tlau/fn3/fs28/tct/pct/ftb/th2"" frameborder = ""0"" width = ""157"" height = ""34"" allowTransparency = ""true"" ></iframe>");


                sw.WriteLine(@"<span style=""font-family:Arial;font-size:45px;>""");
                sw.WriteLine($" &nbsp; </span>In <span style=font-size:55px;>{cr.cTEMPIn}</span><span style=font-size:30px;><sup>o</sup>C &nbsp;</span> <span style=font-size:35px;>Out </span> <span style=font-size:55px;>{cr.cTEMPOut}<span style=font-size:30px;><sup>o</sup>C </span></font></center>&nbsp;");
                sw.WriteLine(@"</span>");
                //sw.WriteLine($"<input type=button  style=float: right; class=button_white onclick=location.href = '';  target=_blank value=\"OutMin: {dailyMin} OutMax: {dailyMax} InMin: {dailyInMin} InMax: {dailyInMax} \" />");
                sw.WriteLine(@"<input type=""button"" style=""float: right;""  onclick=""location.href = 'http://www.bom.gov.au/wa/forecasts/perth.shtml'; "" target=""_blank"" value=""Perth Forecast"" />");
                sw.WriteLine(@"<input type=""button"" style=""float: right;"" onclick=""location.href = 'http://www.bom.gov.au/products/IDR703.loop.shtml'; "" target=""_blank""  value=""BOM Radar""  />");
                sw.WriteLine(@"<input type=""button"" style=""float: right;""  onclick=""location.href = 'https://www.windy.com/-Satellite-satellite?satellite,-31.875,115.778,6'; "" target=""_blank""  value=""Windy""  />");
                sw.WriteLine("<br>");
                sw.WriteLine("</center>");
                sw.WriteLine($"<table class=t1><tr>");
                sw.WriteLine($"<td class=button_green>estMax: {br.BestMax} </td><td class=button_white>&nbsp</td>");
                sw.WriteLine($"<td class=button_green>estMin: {br.BestMin}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Temp: {br.BcurrentTemp}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Press: {br.BcurrentPress}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Humid: {br.BcurrentHumid}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Wind km/h {br.BWindSpeed}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green> Direction {br.BWindDir} </td><td class=button_white></td>");

                sw.WriteLine($"<td class=button_blue>Rover Temp: {cr.cTEMPRover}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_blue>Pool Temp: {cr.cTEMPPool} </td><td class=button_white></td>");
                sw.WriteLine("</tr></table>");

                List<int> daylight = new List<int> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }; //list of the hours of daylight

                sw.WriteLine("<center>");

                if (cr.cTEMPOut >= hc.ThighTemp && daylight.Contains(rightNow.Hour))
                {//close house hot and its daylight
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House: Its HOT: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                // add a control for airconditioning on or off - say 27 degrees
                else if (br.BestMax >= hc.ThighTemp && daylight.Contains(rightNow.Hour) && cr.cTEMPOut > hc.ThighTemp -3 && td.trend1 > 1) 
                {//air con on its hot in the house, its daylight
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"CloseUp, AirCon On: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (cr.cTEMPOut < cr.cTEMPIn && td.trend2 < 1)
                { //trending cool
                    sw.WriteLine($"<input type=button class=button_greenInfo onclick=location.href = '';  target=_blank value=\"OpenUp: Trending cool, Its cooler outside: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (br.BestMax <= hc.TlowTemp)
                { //close the house - its cold
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House COLD: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (!daylight.Contains(rightNow.Hour) && cr.cTEMPIn < hc.TlowTemp)
                {//cold light the fire
                    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Its Too Cold, Light the fire.: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Unknown:\" />");

                ////sw.WriteLine($"<input type=button class=button_white onclick=location.href = '';  target=_blank value=\"Out Min: {dailyMin}, OutMax: {dailyMax}, In Min: {dailyInMin} In Max: {dailyInMax} \" />");
                //sw.WriteLine("<div id = \"rigref-solar-widget\"><a href=\"https://rigreference.com/solar\" target = \"_blank\" ><img src=\"https://rigreference.com/solar/img/wide\" border=\"0\"></a></div>");

                sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Trend Now:Previous {td.trend1} : {td.trend2}\" />");

                if (br.BWindSpeed > hc.WhighWind) {
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"High Wind - Lower Yagi:\" />");
                }

                sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=\"SWS SFI:{sws.SFI} SSN:{sws.SSN} Ap:{sws.Ap} Kp:{sws.Kp} Xray:{sws.xRay} T:{sws.tIndex}\" />");


                sw.WriteLine("</center>");
                sw.WriteLine("<p>");
                sw.WriteLine("</ul><center>");

                //the graph
                sw.WriteLine("<canvas id=\"line-chart\" width=\"300\" height = \"130\" ></canvas>");
                //sw.WriteLine("<canvas id=\"mixed-chart\" width=\"300\" height = \"110\" ></canvas>");
                sw.WriteLine("<script>");
                sw.WriteLine(@"new Chart(document.getElementById(""line-chart""), {");
                sw.WriteLine("type: 'bar',");
                sw.WriteLine("data: {");
                sw.WriteLine(@"labels: [""MidNight"",""1am"",""2am"",""3am"",""4am"",""5am"",""6am"",""7am"",""8am"",""9am"",""10am"",""11am"",""MidDay"",""1pm"",""2pm"",""3pm"",""4pm"",""5pm"",""6pm"",""7pm"",""8pm"",""9pm"",""10pm"",""11pm""],");
                sw.WriteLine("datasets: [{");

                //OUT
                sw.WriteLine("label: 'Out',");
                sw.WriteLine("id: \"y-axis-0\",");
                sw.WriteLine("backgroundColor: [\"red\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++){
                    if (tOut[i] > 0){
                        sw.Write($"{tOut[i]},");}
                    else if (tOut[i] == 0)
                    {
                        sw.Write($" ,");
                    }

                }
                sw.WriteLine(" ],");
                sw.WriteLine(@"borderColor: ""#FF3355"",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");
                


                //OutYesterday
                sw.WriteLine("},{");
                sw.WriteLine("label: \"Yesterday\",");
                sw.WriteLine("id: \"y-axis-1\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(247,70,74,0.2)\"],");
                sw.WriteLine($"data: [ {tOutYesterday[0]}, {tOutYesterday[1]}, {tOutYesterday[2]}, {tOutYesterday[3]}, {tOutYesterday[4]}, {tOutYesterday[5]}, {tOutYesterday[6]}, {tOutYesterday[7]}, {tOutYesterday[8]}, {tOutYesterday[9]}, {tOutYesterday[10]}, {tOutYesterday[11]}, {tOutYesterday[12]}, {tOutYesterday[13]}, {tOutYesterday[14]}, {tOutYesterday[15]}, {tOutYesterday[16]}, {tOutYesterday[17]}, {tOutYesterday[18]}, {tOutYesterday[19]}, {tOutYesterday[20]}, {tOutYesterday[21]}, {tOutYesterday[22]},{tOutYesterday[23]}  ], ");
                //sw.WriteLine("yAxisID: \"y-axis-1\",");
                sw.WriteLine("borderDash: [10,4],");
                sw.WriteLine("borderColor: \"rgba(247,70,74,0.4)\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");

                //IN
                sw.WriteLine("},{");
                sw.WriteLine("label: \"In\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"blue\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++){
                    if (tIn[i] > 0){
                        sw.Write($"{tIn[i]},");}
                    else if (tIn[i] == 0)
                    {
                        sw.Write($" ,");
                    }

                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#3339FF\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");

                //INYesterday
                sw.WriteLine("},{");
                sw.WriteLine("label: \"Yesterday\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(151,187,205,0.2)\"],");
                sw.WriteLine($"data: [ {tInYesterday[0]}, {tInYesterday[1]}, {tInYesterday[2]}, {tInYesterday[3]}, {tInYesterday[4]}, {tInYesterday[5]}, {tInYesterday[6]}, {tInYesterday[7]}, {tInYesterday[8]}, {tInYesterday[9]}, {tInYesterday[10]}, {tInYesterday[11]}, {tInYesterday[12]}, {tInYesterday[13]}, {tInYesterday[14]}, {tInYesterday[15]}, {tInYesterday[16]}, {tInYesterday[17]}, {tInYesterday[18]}, {tInYesterday[19]}, {tInYesterday[20]}, {tInYesterday[21]}, {tInYesterday[22]},{tInYesterday[23]}  ],");
                sw.WriteLine("borderDash: [10,4],");
                sw.WriteLine("borderColor: \"rgba(151,187,205,0.4)\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");


                //BOM
                sw.WriteLine("},{");
                sw.WriteLine("label: \"BOM\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"green\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++) {
                    if (tBOM[i] > 0) {
                        sw.Write($"{tBOM[i]},"); }
                    else if (tBOM[i] == 0 ){
                       sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#42FF33\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");

                sw.WriteLine("},{");
                sw.WriteLine("label: \"ROVER\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"yellow\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++) {
                    if (tRover[i] > 0) {
                        sw.Write($"{tRover[i]},");}
                    else if (tRover[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#CCCC00\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                //------------------------------------------------vvvvvvvvvvvvvvvvvvvvvvvv
                //BOM Maximum as a straight horizontal line
                sw.WriteLine("},{");
                 sw.WriteLine("label: \"eMax\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(0,0,0,0.4)\"],");
                sw.WriteLine($"data: [ {br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax}   ],");
                sw.WriteLine("borderColor: \"rgba(0,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 1,");
                sw.WriteLine("pointRadius: 1,");
                sw.WriteLine("lineTension: 0,");
                sw.WriteLine("fill: false");
                //--------------------------------------------------------------^^^^^^^^^^^^^^^^^^^

                //---vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv-----------------------------------------
                //BOM Minimum as a straight horizontal line
                sw.WriteLine("},{");
                sw.WriteLine("label: \"eMin\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(0,0,0,0.4)\"],");
                sw.WriteLine($"data: [ {br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin}   ],");
                sw.WriteLine("borderColor: \"rgba(0,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 1,");
                sw.WriteLine("pointRadius: 1,");
                sw.WriteLine("fill: false");
                //----------^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^-----------------------
                sw.WriteLine("},{");
                sw.WriteLine("label: \"CurrentHour\",");
                //sw.WriteLine("yAxisID: \"y-axis-1\","); 
                sw.WriteLine("type: 'bar',");
                sw.WriteLine("backgroundColor: [\"pink\"],");
                sw.WriteLine($"data: [ {tHr[0]}, {tHr[1]}, {tHr[2]}, {tHr[3]}, {tHr[4]}, {tHr[5]}, {tHr[6]}, {tHr[7]}, {tHr[8]}, {tHr[9]}, {tHr[10]}, {tHr[11]}, {tHr[12]}, {tHr[13]}, {tHr[14]}, {tHr[15]}, {tHr[16]}, {tHr[17]}, {tHr[18]}, {tHr[19]}, {tHr[20]}, {tHr[21]}, {tHr[22]},{tHr[23]}  ],");
                sw.WriteLine("borderColor: \"#C00C00\",");
                sw.WriteLine("fill: false");

                sw.WriteLine("}");
                sw.WriteLine("]");
                sw.WriteLine(" },");

                // little bit lets you touch a legend item and turn it off
                sw.WriteLine("options: {");
                sw.WriteLine("responsive: true, ");
                sw.WriteLine("legend: { display: true },");
                sw.WriteLine("labels: {");
                sw.WriteLine("      filter: function(legendItem, chartData) {");
                sw.WriteLine("      if (legendItem.datasetIndex === 0) {");
                sw.WriteLine("      return false;");
                sw.WriteLine("             }");
                sw.WriteLine("   return true;");
                sw.WriteLine("       }},");

                sw.WriteLine("title: {");
                sw.WriteLine("    display: false,");
                sw.WriteLine("    text: 'Scarp Weather V2'");
                sw.WriteLine("    }, ");
             //   sw.WriteLine("tooltips: {enabled: true, mode: 'label'} ,");
           //     sw.WriteLine("hover: {mode: 'nearest', intersect: true } , ");
                sw.WriteLine("scales: {");



                //if (rightNow.Hour > 12)  //swap the y Axes ticks and scale to the rhs of the graph after midday
                if (DateTime.Now.Hour > 12)  //swap the y Axes ticks and scale to the rhs of the graph after midday
                {
                    sw.WriteLine("     yAxes: [{ id: 'y-axis-0', type: 'linear',position: 'right' , ");
                }
                else
                {
                    sw.WriteLine("     yAxes: [{ id: 'y-axis-0', type: 'linear',position: 'left' , ");
                }

                sw.WriteLine("        ticks: { ");
                sw.WriteLine($"       min: {br.BestMin - 5}, ");
                sw.WriteLine("        stepSize: 1, ");
                sw.WriteLine("        beginAtZero:false}");
                sw.WriteLine("      }],"); //end of yAxes
                sw.WriteLine("     xAxes: [{ ");
                sw.WriteLine("         barThickness: 10,}]");
                sw.WriteLine("");
                sw.WriteLine("         }");//end of xAxes
                sw.WriteLine("      }"); //end of scales
                sw.WriteLine(" });"); //end of options

                sw.WriteLine("canvas.onclick = function (evt) {");
                sw.WriteLine("var points = line-chart.getPointsAtEvent(evt);");
                sw.WriteLine("alert(line-chart.datasets[0].points.indexOf(points[0])); };");
                sw.WriteLine("");


                sw.WriteLine("  </script>");
                sw.WriteLine(@"<span style=""font-family:Arial;font-size:12px;>""");
                sw.WriteLine($"<p>Scarp Weather - Readings from Lesmurdie {fullDate}");
                sw.WriteLine("</center>");

                sw.WriteLine("</span></body></html>");
                sw.Close();
            }
        }


    } //end of program
}//end of namespace
