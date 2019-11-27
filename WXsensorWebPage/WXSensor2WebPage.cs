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

        static double barMax = 0.0;
        string connectionString = @"Data Source=192.168.1.109\DAWES_SQL2008; Database = WeatherStation; User Id = WeatherStation; Password = Esp32a.b.;";
        static double tInAdjust, tOutAdjust, tRoverAdjust, tBOMadjust; //these are to calibrate the thermometers
        static uint recCnt = 0;
        static string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        DateTime rightNow = DateTime.Now;

        //this is the constructor
        public WXSensor2WebPage()
        {
            InitializeComponent();
            writeToLog("Program start");
            br.BcurrentPress = 999.9; //just in case
            hc.ThighTemp = double.Parse(txtHighTempcondition.Text);
            hc.TlowTemp = double.Parse(txtLowTempCondition.Text);
            hc.WhighWind = double.Parse(txtHighWindCondition.Text);

            double tempAvgThisHour = getTempTrendHourly("WXOUT", 1);
               txtTempNow.Text = tempAvgThisHour.ToString();

            double tempAvgLastHour = getTempTrendHourly("WXOUT", 2);
               txtTempLast.Text = tempAvgLastHour.ToString();
            txtTrend.Text = (tempAvgThisHour - tempAvgLastHour).ToString();

            StartProgram();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        //Declaring timer - a FORMS timer Everything runs from this timer.
        public System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
        void SetTimer()
        {
            aTimer.Tick += OnTimedEvent;
            aTimer.Interval = 60000; //miliseconds - fixed at once every minute for the basic tick
            aTimer.Enabled = true;
        }

        void OnTimedEvent(Object sender, EventArgs e)
        {
            //creating a new static instance of our form so the next line does not yell at me
           // WXSensor2WebPage sd = new WXSensor2WebPage();

            recCnt += 1;  //the first time through recCnt=1 (0 based)

            lblRecCnt.Text = recCnt.ToString();
            if (recCnt % 1 == 0)  //every tick this would be
            {
                //fill the today arrays
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
                System.Threading.Thread.Sleep(500);
                CurrentReadingFromDatabase("WXOUT");
                System.Threading.Thread.Sleep(500);
                CurrentReadingFromDatabase("WXROVER");
                System.Threading.Thread.Sleep(500);
                CurrentReadingFromDatabase("WXPOOL");
                System.Threading.Thread.Sleep(500);
                CurrentReadingFromDatabase("WXBOMGHILL");
                System.Threading.Thread.Sleep(500);
                BOMReadingFromDatabase("WXBOMGHILL");
                //update the House Conditions
                hc.ThighTemp = double.Parse(txtHighTempcondition.Text);
                hc.TlowTemp = double.Parse(txtLowTempCondition.Text);
                hc.WhighWind = double.Parse(txtHighWindCondition.Text);

                double tempAvgThisHour = getTempTrendHourly("WXOUT", 1);
                txtTempNow.Text = tempAvgThisHour.ToString();

                double tempAvgLastHour = getTempTrendHourly("WXOUT", 2);
                txtTempLast.Text = tempAvgLastHour.ToString();
                txtTrend.Text = (tempAvgThisHour - tempAvgLastHour).ToString();



                writeToHTML();

            }
            // this is the Twitter announcement.
            if (recCnt % 60 == 0 || recCnt == 2)  //every 1 hour and just after startup
            {
                webPOSTtoScarpWeather(cr.cTEMPIn, cr.cHumidOut, cr.cPressureOut, cr.cTEMPOut);  //send it to Scarpweather
                writeToLog($"WebPost To Twitter(every 60 and once at 2), Record Number: {recCnt}");
            }

            if ( recCnt == 1)
            {
                ReadCorrections();
                writeToLog($"Reading the corrections At Record Number: {recCnt}");
            }

            if (recCnt %30 == 0)
            {
                //status report to log every 30 counts
                writeToLog($" ***Status Report...OK At Record Number: {recCnt} ");
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
            SqlCommand getReadings = new SqlCommand($"select TIME, ROUND(AvgValue, 1) as AvgTemp from(select TIME = dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)), AvgValue = AVG(TEMP)from {table} group by dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)))a order by TIME DESC", conn);
            try
            {
                conn.Open();
                rdr = getReadings.ExecuteReader();
                lblNowTime.Text = rightNow.TimeOfDay.ToString();
                Array.Clear(tArray, 0, 24);
                while (rdr.Read())
                {
                    // get the results of each column
                    rdngTime = (DateTime)rdr["TIME"];
                    rdngTemp = (double)rdr["AvgTemp"];

                    if (rightNow.Day == rdngTime.Day)
                    {
                        intHour = (int)(rdngTime.Hour);
                        tArray[intHour] = rdngTemp + tAdjust;    //get todays readings of temperature and put in the array and add the correction
                    }
                    else if (rightNow.Day - 1 == rdngTime.Day)
                    {
                        intHour = (int)(rdngTime.Hour);
                        tArrayYesterday[intHour] = rdngTemp; //yesterdays reading of temp into the array
                    }
                    // this is the attempt to fill the tHr array for the vertical bar
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
                writeToLog($"readSensorDataFromDatabase: {ex}");
            }
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
                    }
                    else if (table == "WXOUT")
                    {
                        cr.cTEMPOut = (double)rdr["TEMP"];
                        cr.cHumidOut = (double)rdr["HUMID"];
                        cr.cPressureOut = br.BcurrentPress;  //this is because we dont have a working pressure sensor
                    }
                    else if (table == "WXROVER")
                    {
                        cr.cTEMPRover = (double)rdr["TEMP"];
                    }
                    else if (table == "WXPOOL")
                    {
                        cr.cTEMPPool = (double)rdr["TEMP"];
                    }
                    else if (table == "WXBOMGHILL")
                    {
                        cr.cTEMPBom = (double)rdr["TEMP"];
                    }
                }
                conn.Close();
            } catch (Exception ex)
            {
                writeToLog($" CurrentReadingFromDatabase: {ex}");
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
                SqlCommand getReadings = new SqlCommand($"select  top 1 TEMP,HUMID,PRESS,WINDSPEED,WINDDIR,RAINFALL,ESTBOMMIN,ESTBOMMAX from {table} order by TIME DESC", conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    // get the results of each column
                    br.BcurrentTemp = (double)rdr["TEMP"];
                    br.BcurrentHumid = (double)rdr["HUMID"];
                    br.BcurrentPress = (double)rdr["PRESS"];
                    br.BestMax = (double)rdr["ESTBOMMAX"];
                    br.BestMin = (double)rdr["ESTBOMMIN"];
                    br.BWindSpeed = (double)rdr["WINDSPEED"];
                    br.BWindDir = (string)rdr["WINDDIR"];
                    br.Brainfall = (double)rdr["RAINFALL"];
                }
                conn.Close();
            }

            catch (Exception ex)
            {
                writeToLog($"Error BOMReadingFromDatabase {ex}");
            }
        }


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
            
        }



        private double getTempTrendHourly(string table,double rownum) {

            string getTEMPHour = $"select TOP 1 * from (select row_number() over (order by TIME DESC) as ROWNUMBER,TIME, ROUND(AvgValue,1) as AvgTemp from (select TIME=dateadd(hh,datepart(hh,TIME), cast(CAST(TIME as date) as datetime)), AvgValue=AVG(TEMP)from  {table} group by dateadd(hh,datepart(hh,TIME), cast(CAST(TIME as date) as datetime)))a)b where ROWNUMBER = {rownum}";
            
            SqlConnection conn;
            SqlDataReader rdr = null;

            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
            SqlCommand getHour = new SqlCommand(getTEMPHour, conn);
            conn.Open();
            rdr = getHour.ExecuteReader();

            rdr.Read();
            return (double)rdr["AvgTemp"];
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
                }
                conn.Close();
            }
            catch (Exception ex){
                writeToLog($"Error reading corrections from the database {ex}");}

        }// end of CorrectionsFromDatabase

        private void btnReadCorrections_Click(object sender, EventArgs e)
        {
            ReadCorrections();
            MessageBox.Show("Re Read Corrections. Wait one minute.");
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
                    string now = DateTime.Now.ToString("h:mm:ss tt");
                    sw.WriteLine($"Log Time: {now} : {mesg}"); //thats String Interpolation -the $ and curly brackets make it easy!
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

                sw.WriteLine(".button_blueInfo{background-color: blue; color: white;font-size: 18px; padding: 10px 32px;}");
                sw.WriteLine(".button_redInfo{background-color: red; color: white;font-size: 18px; padding: 10px 32px;}");
                sw.WriteLine(".button_greenInfo{background-color: green; color: white;font-size: 18px; padding: 10px 32px;}");

                sw.WriteLine(".button_info{background-color: lightgray; color: blue;font-size: 16px;}");

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

                // sw.WriteLine("<div class=\"row\">");
                // sw.WriteLine("<div class=\"column\" style=\"background - color:#aaa;\">");
                sw.WriteLine("<br>");

                sw.WriteLine("</center>");
                sw.WriteLine($"<table class=t1><tr>");
                sw.WriteLine($"<td class=button_green>estMax: {br.BestMax} </td><td class=button_white>&nbsp</td>");
                sw.WriteLine($"<td class=button_green>estMin: {br.BestMin}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Temp: {br.BcurrentTemp}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Press: {br.BcurrentPress}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green>Wind km/h {br.BWindSpeed}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_green> Direction {br.BWindDir} </td><td class=button_white></td>");
               // sw.WriteLine("</tr></table>");

               // sw.WriteLine($"<table><tr>");
                sw.WriteLine($"<td class=button_blue>Rover Temp: {cr.cTEMPRover}</td><td class=button_white></td>");
                sw.WriteLine($"<td class=button_blue>Pool Temp: {cr.cTEMPPool} </td><td class=button_white></td>");
                //sw.WriteLine($"<td class=button_blue><button onclick = ""location.href = 'http://www.bom.gov.au/products/IDR703.loop.shtml'; "" target = ""_blank""  value = ""BOM Serpentine Radar"" /></td>");
                sw.WriteLine("</tr></table>");

                List<int> daylight = new List<int> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }; //list of the hours of daylight

                sw.WriteLine("<center>");

                if (cr.cTEMPBom >= hc.ThighTemp && daylight.Contains(rightNow.Hour))
                {//close house hot and its daylight
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House HOT: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (br.BestMax >= hc.ThighTemp && daylight.Contains(rightNow.Hour) && cr.cTEMPIn > hc.ThighTemp)
                {//air con on its hot in the house, its daylight
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"House HOT AirCon On: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (cr.cTEMPIn < br.BestMax && cr.cTEMPIn > br.BestMin)
                {
                    sw.WriteLine($"<input type=button class=button_greenInfo onclick=location.href = '';  target=_blank value=\"Nice day - air out the house: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (br.BestMax <= hc.TlowTemp)
                { //close the house - its cold
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House COLD: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (!daylight.Contains(rightNow.Hour) && (cr.cTEMPIn >= hc.TlowTemp) || (cr.cTEMPIn <= hc.ThighTemp) )
                {//its night time
                    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Its Night and balmy.  Open Up House: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                 } 
                else if (!daylight.Contains(rightNow.Hour) && cr.cTEMPIn < hc.TlowTemp)
                {//cold light the fire
                    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Its Too Cold, Light the fire.: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Unknown:\" />");

                ////sw.WriteLine($"<input type=button class=button_white onclick=location.href = '';  target=_blank value=\"Out Min: {dailyMin}, OutMax: {dailyMax}, In Min: {dailyInMin} In Max: {dailyInMax} \" />");


                //sw.WriteLine("<div id = \"rigref-solar-widget\"><a href=\"https://rigreference.com/solar\" target = \"_blank\" ><img src=\"https://rigreference.com/solar/img/wide\" border=\"0\"></a></div>");








                if (br.BWindSpeed > hc.WhighWind) {
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"High Wind - Lower Yagi:\" />");
                }


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
                sw.WriteLine("label: \"Out\",");
                sw.WriteLine("backgroundColor: [\"red\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++){
                    if (tOut[i] > 0){
                        sw.Write($"{tOut[i]},");}
                }
                sw.WriteLine( " ],");
                sw.WriteLine(@"borderColor: ""#FF3355"",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");
                
                
                //OutYesterday
                sw.WriteLine("},{");
                sw.WriteLine("label: \"Yesterday\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(247,70,74,0.2)\"],");
                sw.WriteLine($"data: [ {tOutYesterday[0]}, {tOutYesterday[1]}, {tOutYesterday[2]}, {tOutYesterday[3]}, {tOutYesterday[4]}, {tOutYesterday[5]}, {tOutYesterday[6]}, {tOutYesterday[7]}, {tOutYesterday[8]}, {tOutYesterday[9]}, {tOutYesterday[10]}, {tOutYesterday[11]}, {tOutYesterday[12]}, {tOutYesterday[13]}, {tOutYesterday[14]}, {tOutYesterday[15]}, {tOutYesterday[16]}, {tOutYesterday[17]}, {tOutYesterday[18]}, {tOutYesterday[19]}, {tOutYesterday[20]}, {tOutYesterday[21]}, {tOutYesterday[22]},{tOutYesterday[23]}  ],");
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
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#CCCC00\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");

                //BOM Maximum as a straight horizontal line
                sw.WriteLine("},{");
                 sw.WriteLine("label: \"eMax\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(0,0,0,0.4)\"],");
                sw.WriteLine($"data: [ {br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax},{br.BestMax}   ],");
                //sw.WriteLine("borderDash: [10,4],");
                sw.WriteLine("borderColor: \"rgba(0,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 1,");
                sw.WriteLine("pointRadius: 1,");
                sw.WriteLine("fill: false");


                //BOM Minimum as a straight horizontal line
                sw.WriteLine("},{");
                sw.WriteLine("label: \"eMin\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"rgba(0,0,0,0.4)\"],");
                sw.WriteLine($"data: [ {br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin},{br.BestMin}   ],");
                //sw.WriteLine("borderDash: [10,4],");
                sw.WriteLine("borderColor: \"rgba(0,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 1,");
                sw.WriteLine("pointRadius: 1,");
                sw.WriteLine("fill: false");

                sw.WriteLine("},{");
                sw.WriteLine("label: \"CurrentHour\",");
                sw.WriteLine("type: 'bar',");
                sw.WriteLine("backgroundColor: [\"pink\"],");
                //                sw.WriteLine($"data: [{tHr[9]}, {tHr[10]}, {tHr[11]}, {tHr[12]}, {tHr[13]}, {tHr[14]}, {tHr[15]}, {tHr[16]}, {tHr[17]}, {tHr[18]}, {tHr[19]}, {tHr[20]}, {tHr[21]}, {tHr[22]}, {tHr[23]}, {tHr[0]}, {tHr[1]}, {tHr[2]}, {tHr[3]}, {tHr[4]}, {tHr[5]}, {tHr[6]}, {tHr[7]},{tHr[8]}   ],");
                sw.WriteLine($"data: [ {tHr[0]}, {tHr[1]}, {tHr[2]}, {tHr[3]}, {tHr[4]}, {tHr[5]}, {tHr[6]}, {tHr[7]}, {tHr[8]}, {tHr[9]}, {tHr[10]}, {tHr[11]}, {tHr[12]}, {tHr[13]}, {tHr[14]}, {tHr[15]}, {tHr[16]}, {tHr[17]}, {tHr[18]}, {tHr[19]}, {tHr[20]}, {tHr[21]}, {tHr[22]},{tHr[23]}  ],");
                sw.WriteLine("borderColor: \"#C00C00\",");
                sw.WriteLine("fill: false");

                sw.WriteLine("}");
                sw.WriteLine("]");
                sw.WriteLine(" },");

                sw.WriteLine("options: {");
                sw.WriteLine("legend: { display: true },");
                sw.WriteLine("labels: {");
                sw.WriteLine("filter: function(legendItem, chartData) {");
                sw.WriteLine("if (legendItem.datasetIndex === 0) {");
                sw.WriteLine("return false;");
                sw.WriteLine("}");
                sw.WriteLine("return true;");
                sw.WriteLine("}},");
              //  sw.WriteLine("");
             //   sw.WriteLine("");




                sw.WriteLine("title: {");
                sw.WriteLine("display: false,");
                sw.WriteLine("text: 'Scarp Weather V2'");
                sw.WriteLine(" },");
                sw.WriteLine(" scales: {");
                sw.WriteLine("yAxes: [{");
                sw.WriteLine("ticks: {");
                // sw.WriteLine($"max: {xAxisMax}, ");
                  sw.WriteLine($"min: 10, ");  
                //sw.WriteLine($"min: {float.Parse(textBox6.Text)}, ");
                //sw.WriteLine($"min: {Math.Round(tMinIn,0)-5}, ");
                //sw.WriteLine($"min: {Math.Round(float.Parse(bomForecastTempMin), 0) - float.Parse(textBox6.Text) }, ");
                sw.WriteLine("stepSize: 2, ");
                sw.WriteLine("beginAtZero:false");
                sw.WriteLine(" }");
                sw.WriteLine(" }],");
                sw.WriteLine("xAxes: [{ barThickness: 10,}]");
                sw.WriteLine("");
                sw.WriteLine("}");
                sw.WriteLine("}");
                sw.WriteLine("});");

                sw.WriteLine("canvas.onclick = function (evt) {");
                sw.WriteLine("var points = line-chart.getPointsAtEvent(evt);");
                sw.WriteLine("alert(line-chart.datasets[0].points.indexOf(points[0])); };");
                sw.WriteLine("");


                sw.WriteLine("  </script>");
                //sw.WriteLine("  </div>");
                //sw.WriteLine("</div>");

                // sw.WriteLine("<center>");
                sw.WriteLine(@"<span style=""font-family:Arial;font-size:12px;>""");
                sw.WriteLine($"<p>Scarp Weather - Readings from Lesmurdie {fullDate}");
                sw.WriteLine("</center>");

                sw.WriteLine("</span></body></html>");
                sw.Close();
            }
        }


    } //end of program
}//end of namespace
