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
        double[] tIn = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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

        struct CurrentReadings        { //this is to store current sensor reading for display on web page
            public double cTEMPIn;
            public double cTEMPOut;
            public double cTEMPPool;
            public double cTEMPRover;
            public double cTEMPBom;        }
        CurrentReadings cr = new CurrentReadings();
        static double barMax = 0.0;
        string connectionString = @"Data Source=192.168.1.109\DAWES_SQL2008; Database = WeatherStation; User Id = WeatherStation; Password = Esp32a.b.;";

//        static string WXOUTconnString;
//        static string WXROVERconnString;
//        static string WXPOOLconnString;
//        static string WXINconnString;
        static double tInAdjust, tOutAdjust, tRoverAdjust, tBOMadjust; //these are to calibrate the thermometers

        static uint recCnt = 0;

        static string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");


        public WXSensor2WebPage()
        {
            InitializeComponent();
 
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }


        //Declaring timer - a FORMS timer
        public System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
        void SetTimer()
        {
            aTimer.Tick += OnTimedEvent;
            aTimer.Interval = 30000; //miliseconds - fixed at once every minute for the basic tick
            aTimer.Enabled = true;
        }

        void OnTimedEvent(Object sender, EventArgs e)
        {
            //creating a new static instance of our form so the next line does not yell at me
            WXSensor2WebPage sd = new WXSensor2WebPage();

            recCnt += 1;  //the first time through recCnt=1 (0 based)

            lblRecCnt.Text = recCnt.ToString();
            if (recCnt % 1 == 0)  //every tick this would be
            {
                //fill the today arrays
                readSensorDataFromDatabase("WXIN", tIn, tInYesterday);
                readSensorDataFromDatabase("WXOUT", tOut, tOutYesterday);
                readSensorDataFromDatabase("WXROVER", tRover, tRoverYesterday);
                readSensorDataFromDatabase("WXPOOL", tPool, tPoolYesterday);
                readSensorDataFromDatabase("WXBOMGHILL", tBOM, tBOMYesterday);

                //get the current sensor reading from the database
                CurrentReadingFromDatabase("WXIN");
                CurrentReadingFromDatabase("WXOUT");
                CurrentReadingFromDatabase("WXROVER");
                CurrentReadingFromDatabase("WXPOOL");
                CurrentReadingFromDatabase("WXBOMGHILL");
                writeToHTML();
            }
        }


            private void btnStart_Click(object sender, EventArgs e)
        {

            lblStartTime.Text = now.ToString();
            SetTimer(); //Start the loop

        }


        private void readSensorDataFromDatabase(string table, double[] tArray, double[] tArrayYesterday )
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngTemp;
            DateTime rdngTime;
            DateTime  rightNow = DateTime.Now;
            int intDay, intHour;
            

            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
            SqlCommand getReadings = new SqlCommand($"select TIME, ROUND(AvgValue, 1) as AvgTemp from(select TIME = dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)), AvgValue = AVG(TEMP)from {table} group by dateadd(hh, datepart(hh, TIME), cast(CAST(TIME as date) as datetime)))a order by TIME DESC",conn);
            
           // SqlCommand getReadings = new SqlCommand($"select * from dbo.{table} where SENSOR='{sqlSensorName}'", conn);
           //SqlCommand writeToTable = new SqlCommand($"insert into {table} (TIME,TEMP,HUMID,PRESS,COMMENT) values ('{now}',{temp},{humid},0,'{cmnt}');", conn);
            conn.Open();
            rdr = getReadings.ExecuteReader();
           

            while (rdr.Read())
            {
                // get the results of each column
                rdngTime = (DateTime)rdr["TIME"];
                rdngTemp = (double)rdr["AvgTemp"];

                if (rightNow.Day == rdngTime.Day) {
                    intHour = (int)(rdngTime.Hour);
                    tArray[intHour] = rdngTemp;

                    if (rdngTemp > barMax)
                    {
                        barMax = rdngTemp;
                        Array.Clear(tHr, 0, 24);
                        tHr[rightNow.Hour] = (int)barMax;
                    }


                }
            }
        }

        private void CurrentReadingFromDatabase(string table)
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            double rdngTemp;
            DateTime rdngTime;
            DateTime rightNow = DateTime.Now;
            int intDay, intHour;
            //            CurrentReadings cr = new CurrentReadings();
            
            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
            SqlCommand getReadings = new SqlCommand($"select  top 1 TEMP from {table} order by TIME DESC", conn);

            // SqlCommand getReadings = new SqlCommand($"select * from dbo.{table} where SENSOR='{sqlSensorName}'", conn);
            //SqlCommand writeToTable = new SqlCommand($"insert into {table} (TIME,TEMP,HUMID,PRESS,COMMENT) values ('{now}',{temp},{humid},0,'{cmnt}');", conn);
            conn.Open();
            rdr = getReadings.ExecuteReader();
            while (rdr.Read())
            {
                // get the results of each column
//                rdngTime = (DateTime)rdr["TIME"];
                rdngTemp = (double)rdr["TEMP"];

                if (table == "WXIN")
                {
                    cr.cTEMPIn = rdngTemp;    
                }
                else if (table == "WXOUT")
                {
                    cr.cTEMPOut = rdngTemp;
                }
                else if (table == "WXROVER")
                {
                    cr.cTEMPRover = rdngTemp;
                }
                else if (table == "WXPOOL")
                {
                    cr.cTEMPPool = rdngTemp;
                }
                else if (table == "WXBOMGHILL")
                {
                    cr.cTEMPBom = rdngTemp;
                }


            }
        }










        void writeToHTML()
        {

            // var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
            var pathWithEnv = @"c:\inetpub\wwwroot\t.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                string now = DateTime.Now.ToString("h:mm:ss tt");
                string fullDate = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt");

                sw.WriteLine("<HTML><HEAD>");
                sw.WriteLine("<title>SCARP Weather</title>");
                sw.WriteLine("<meta http-equiv = \"refresh\" content = \"60\" >");// 1 is 1 second  60 is 60 seconds

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

                sw.WriteLine(".button_blueInfo{background-color: blue; color: white;font-size: 20px; padding: 15px 32px;}");
                sw.WriteLine(".button_redInfo{background-color: red; color: white;font-size: 20px; padding: 15px 32px;}");
                sw.WriteLine(".button_greenInfo{background-color: green; color: white;font-size: 20px; padding: 15px 32px;}");

                sw.WriteLine(".button_info{background-color: lightgray; color: blue;font-size: 16px;}");

                sw.WriteLine(".btn-group button{background-color: #4CAF50; border: 1px solid green; color: white; padding: 10px 24px; cursor:pointer; float:left;}");
                //        sw.WriteLine("");
                //        sw.WriteLine("");


                sw.WriteLine("</style>");
                //this is the library for charts.js
                sw.WriteLine(@"<script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js""></script>");
                sw.WriteLine(@"</head><body>");
                //sw.WriteLine("<center><h1>Scarp Weather Weather Station</H1>");

                sw.WriteLine("</center>");

                 sw.WriteLine(@"<iframe src = ""http://free.timeanddate.com/clock/i70o7n5a/n196/tlau/fn3/fs28/tct/pct/ftb/th2"" frameborder = ""0"" width = ""157"" height = ""34"" allowTransparency = ""true"" ></iframe>");


                sw.WriteLine(@"<span style=""font-family:Arial;font-size:45px;>""");
                sw.WriteLine($" &nbsp; </span>In <span style=font-size:65px;>{cr.cTEMPIn}</span><span style=font-size:40px;><sup>o</sup>C &nbsp;</span> <span style=font-size:45px;>Out </span> <span style=font-size:65px;>{cr.cTEMPOut}<span style=font-size:40px;><sup>o</sup>C </span></font></center>&nbsp;");
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
              //  sw.WriteLine($"<td class=button_green>estMax: {bomForecastTempMax} </td><td class=button_white>&nbsp</td>");
              //  sw.WriteLine($"<td class=button_green>estMin: {bomForecastTempMin}</td><td class=button_white></td>");
              //  sw.WriteLine($"<td class=button_green>Current Temp: {BOM_temp}</td><td class=button_white></td>");
              //  sw.WriteLine($"<td class=button_green>Wind km/h {BOM_windspeed}</td><td class=button_white></td>");
              //  sw.WriteLine($"<td class=button_green> Direction {BOM_winddir} </td><td class=button_white></td>");
                //sw.WriteLine("</tr></table>");

                //sw.WriteLine($"<table><tr>");
              //  sw.WriteLine($"<td class=button_blue>Rover Temp: {strOutTemp2}</td><td class=button_white></td>");
              //  sw.WriteLine($"<td class=button_blue>Pool Temp: {strOutTemp2} </td><td class=button_white></td>");
                //sw.WriteLine($"<td class=button_blue><button onclick = ""location.href = 'http://www.bom.gov.au/products/IDR703.loop.shtml'; "" target = ""_blank""  value = ""BOM Serpentine Radar"" /></td>");
                sw.WriteLine("</tr></table>");

                //sw.WriteLine("<div class=btn-group><button>Perth Forecast</button>");
                //sw.WriteLine("<button>Radar</button>");
                //sw.WriteLine("<button>Windy</button>");
                //sw.WriteLine("</div>");

                //                sw.WriteLine("<center>");
                //the rover is strOutTemp2
//                float tempIn = curTemp;
  //              float tempOut = (float)Math.Round(float.Parse(strOutTemp1), 1);
             //   float tempBomMax = float.Parse(bomForecastTempMax);
                List<int> daylight = new List<int> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };



                //sw.WriteLine("<center>");

                //if ((tempBomMax >= int.Parse(txtCloseHseTempHot.Text)) && (daylight.Contains(globalHour)))
                //{
                //    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House HOT: {Math.Round(tempOut - tempIn, 0)}\" />");

                //}
                //else if ((tempBomMax >= int.Parse(txtCloseHseTempHot.Text)) && (daylight.Contains(globalHour)) && tempIn > int.Parse(txtCloseHseTempHot.Text))
                //{
                //    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"House HOT AirCon On: {Math.Round(tempOut - tempIn, 0)}\" />");
                //}
                //else if (tempBomMax <= int.Parse(txtCloseHseTempCold.Text))
                //{
                //    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"Close House COLD: {Math.Round(tempOut - tempIn, 0)}\" />");
                //}
                //else if ((tempIn > int.Parse(txtCloseHseTempCold.Text)) && (tempIn < int.Parse(txtCloseHseTempHot.Text)) && (tempBomMax <= int.Parse(txtBOMhot.Text)))
                //{
                //    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Open Up House: {Math.Round(tempOut - tempIn, 0)}\" />");
                //}
                //else if (!daylight.Contains(globalHour) && (tempIn >= int.Parse(txtCloseHseTempCold.Text) || (tempIn) <= int.Parse(txtCloseHseTempHot.Text)))
                //{
                //    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Its Night and balmy.  Open Up House: {Math.Round(tempOut - tempIn, 0)}\" />");

                //} //its night time
                //else if (!daylight.Contains(globalHour) && (tempIn < int.Parse(txtCloseHseTempCold.Text)))
                //{
                //    sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Its Too Cold, Light the fire.: {Math.Round(tempOut - tempIn, 0)}\" />");

                //} //its night time
                //else sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"Unknown: {tempIn},{tempOut},{tempBomMax}\" />");

                ////sw.WriteLine($"<input type=button class=button_white onclick=location.href = '';  target=_blank value=\"Out Min: {dailyMin}, OutMax: {dailyMax}, In Min: {dailyInMin} In Max: {dailyInMax} \" />");


                sw.WriteLine("</center>");
                //sw.WriteLine($"<input type=button onclick=location.href = '';  target=_blank value=tMinOut={tMinOut} />");
                //sw.WriteLine($"<input type=button onclick=location.href = '';  target=_blank value=tMaxOut={tMaxOut} />");
                //sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=BomTemp={BOM_temp} />");
                //sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=WindSpeed={BOM_windspeed} />");
                //sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=WindDirectn={BOM_winddir} />");
                //sw.WriteLine($"<input type=button class=button_info  onclick=location.href = '';  target=_blank value=RoverTemp={strOutTemp2} />");
                //sw.WriteLine("<br>");
                //sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=ForecastMin={bomForecastTempMin} />");

                //sw.WriteLine($"<input type=button class=button_info  onclick=location.href = '';  target=_blank value=ForecastMax={bomForecastTempMax} />");
                // sw.WriteLine(@"<input type=""button"" onclick=""location.href = 'http://www.bom.gov.au/wa/forecasts/perth.shtml'; "" target=""_blank"" value=""Perth Forecast"" />");
                // sw.WriteLine(@"<input type=""button"" onclick=""location.href = 'http://www.bom.gov.au/products/IDR703.loop.shtml'; "" target=""_blank""  value=""BOM Serpentine Radar""  />");
                //sw.WriteLine(@"<input type=""button"" onclick=""location.href = 'http://www.bom.gov.au/climate/averages/tables/cw_009240.shtml'; "" target=""_blank""  value=""BOM Averages""  />");
                // sw.WriteLine(@"<input type=""button"" onclick=""location.href = 'https://www.windy.com/-Satellite-satellite?satellite,-31.875,115.778,6'; "" target=""_blank""  value=""Windy""  />");
                //sw.WriteLine(@"<input type=""button"" onclick=""location.href = 'http://www.members.iinet.net.au/~richard_g/NOAA/wxtoimg/web/noaa.html'; "" target=""_blank""  value=""NOAA""  />");

                sw.WriteLine("<p>");
                //sw.WriteLine("<br>");
                //sw.WriteLine("</div>");

                //the weather forecast   
                // sw.WriteLine("</center><ul>");
                // sw.WriteLine($"<li><b>Synopsis:</b> {strBOMsynopsis} </li><p>");
                // sw.WriteLine($"<li><b>Today:</b>   {strBOMtoday} </li><p>");
                // sw.WriteLine($"<li><b>Tomorrow:</b>  {strBOMtomorrow} </li><p>");
                sw.WriteLine("</ul><center>");

                //the graph
                sw.WriteLine("<canvas id=\"line-chart\" width=\"300\" height = \"130\" ></canvas>");
                //sw.WriteLine("<canvas id=\"mixed-chart\" width=\"300\" height = \"110\" ></canvas>");

                sw.WriteLine("<script>");



                sw.WriteLine(@"new Chart(document.getElementById(""line-chart""), {");
                //sw.WriteLine("type: 'line',");
                sw.WriteLine("type: 'bar',");
                sw.WriteLine("data: {");
                sw.WriteLine(@"labels: [""9am"",""10am"",""11am"",""Noon"",""1pm"",""2pm"",""3pm"",""4pm"",""5pm"",""6pm"",""7pm"",""8pm"",""9pm"",""10pm"",""11pm"",""Midnight"",""1am"",""2am"",""3am"",""4am"",""5am"",""6am"",""7am"",""8am""],");
                sw.WriteLine("datasets: [{");

                sw.WriteLine("label: \"Outside\",");
                sw.WriteLine("backgroundColor: [\"red\"],");
                sw.WriteLine($"data: [ {tOut[9]}, {tOut[10]}, {tOut[11]}, {tOut[12]}, {tOut[13]}, {tOut[14]}, {tOut[15]}, {tOut[16]}, {tOut[17]}, {tOut[18]}, {tOut[19]}, {tOut[20]}, {tOut[21]}, {tOut[22]}, {tOut[23]}, {tOut[0]}, {tOut[1]}, {tOut[2]}, {tOut[3]}, {tOut[4]}, {tOut[5]}, {tOut[6]}, {tOut[7]},{tOut[8]}  ],");
                sw.WriteLine(@"borderColor: ""#FF3355"",");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");


                sw.WriteLine("},{");
                sw.WriteLine("label: \"Inside\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"blue\"],");
                sw.WriteLine($"data: [ {tIn[9]}, {tIn[10]}, {tIn[11]}, {tIn[12]}, {tIn[13]}, {tIn[14]}, {tIn[15]}, {tIn[16]}, {tIn[17]}, {tIn[18]}, {tIn[19]}, {tIn[20]}, {tIn[21]}, {tIn[22]}, {tIn[23]}, {tIn[0]}, {tIn[1]}, {tIn[2]}, {tIn[3]}, {tIn[4]}, {tIn[5]}, {tIn[6]}, {tIn[7]},{tIn[8]}],");
                sw.WriteLine("borderColor: \"#3339FF\",");
                sw.WriteLine("fill: false");


                sw.WriteLine("},{");
                sw.WriteLine("label: \"BOM\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"green\"],");
                sw.WriteLine($"data: [{tBOM[9]}, {tBOM[10]}, {tBOM[11]}, {tBOM[12]}, {tBOM[13]}, {tBOM[14]}, {tBOM[15]}, {tBOM[16]}, {tBOM[17]}, {tBOM[18]}, {tBOM[19]}, {tBOM[20]}, {tBOM[21]}, {tBOM[22]}, {tBOM[23]}, {tBOM[0]}, {tBOM[1]}, {tBOM[2]}, {tBOM[3]}, {tBOM[4]}, {tBOM[5]}, {tBOM[6]}, {tBOM[7]},{tBOM[8]}],");
                sw.WriteLine("borderColor: \"#42FF33\",");
                sw.WriteLine("fill: false");

                sw.WriteLine("},{");
                sw.WriteLine("label: \"ROVER\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"yellow\"],");
                sw.WriteLine($"data: [{tRover[9]}, {tRover[10]}, {tRover[11]}, {tRover[12]}, {tRover[13]}, {tRover[14]}, {tRover[15]}, {tRover[16]}, {tRover[17]}, {tRover[18]}, {tRover[19]}, {tRover[20]}, {tRover[21]}, {tRover[22]}, {tRover[23]}, {tRover[0]}, {tRover[1]}, {tRover[2]}, {tRover[3]}, {tRover[4]}, {tRover[5]}, {tRover[6]}, {tRover[7]},{tRover[8]}],");
                sw.WriteLine("borderColor: \"#CCCC00\",");
                sw.WriteLine("fill: false");


                sw.WriteLine("},{");
                sw.WriteLine("label: \"CurrentHour\",");
                sw.WriteLine("type: 'bar',");
                sw.WriteLine("backgroundColor: [\"pink\"],");
                sw.WriteLine($"data: [{tHr[9]}, {tHr[10]}, {tHr[11]}, {tHr[12]}, {tHr[13]}, {tHr[14]}, {tHr[15]}, {tHr[16]}, {tHr[17]}, {tHr[18]}, {tHr[19]}, {tHr[20]}, {tHr[21]}, {tHr[22]}, {tHr[23]}, {tHr[0]}, {tHr[1]}, {tHr[2]}, {tHr[3]}, {tHr[4]}, {tHr[5]}, {tHr[6]}, {tHr[7]},{tHr[8]}   ],");
                sw.WriteLine("borderColor: \"#C00C00\",");
                sw.WriteLine("fill: false");

                sw.WriteLine("}");
                sw.WriteLine("]");
                sw.WriteLine(" },");

                sw.WriteLine("options: {");
                sw.WriteLine("legend: { display: true },");
                sw.WriteLine("title: {");
                sw.WriteLine("display: true,");
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
                sw.WriteLine($"<p>Readings from Lesmurdie {fullDate}");
                sw.WriteLine("</center>");

                sw.WriteLine("</span></body></html>");
                sw.Close();
            }
        }

















    }
}
