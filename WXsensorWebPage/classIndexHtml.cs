using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WXsensorWebPage
{
    public class classIndexHtml
    {
        //this class is to create the index.html web page
        // I want to be able to see todays data andays data

        //public WXSensor2WebPage.BOMReadings WXSbr;
        private DateTime rightNow = DateTime.Now;   //we use this a lot to get current time

        // getting the arrays declared in the form WXSensor2WebPage class available from this class.  These have all been made public and static
        double[] tBOM = WXSensor2WebPage.tBOM;
        double[] tOut = WXSensor2WebPage.tOut;
        double[] tIn = WXSensor2WebPage.tIn;
        double[] tPool = WXSensor2WebPage.tPool;
        double[] tRover = WXSensor2WebPage.tRover;
        int[] tHr = WXSensor2WebPage.tHr;
        double[] tInYesterday = WXSensor2WebPage.tInYesterday;
        double[] tOutYesterday = WXSensor2WebPage.tOutYesterday;


        // we want to get the value from the WXSensor2WebPage.txtWebUpdateCycle value.  You need make the control public on the 
        //WXSensor2WebPage designer and then make an instance of the form so we can access its controls
        System.Windows.Forms.Form f = System.Windows.Forms.Application.OpenForms["WXSensor2WebPage"];    //so we have now made an object from WXSensor2WebPage

        //private static readonly string twuc = ((WXSensor2WebPage)f).txtWebUpdateCycle.Text;  //YOU CANT DO THIS HERE.  Goes into the method


        public classIndexHtml(){} //constructor

        public void test1()
        { 
        }



            public void test(WXSensor2WebPage.CurrentReadings cr)
        {
            Console.WriteLine(cr.cTEMPOut.ToString());
        }

        /// <summary>
        /// Writes the html for - the webpage.  This is the main web page - index.html
        /// </summary>
        public void createHTMLpage(WXSensor2WebPage.BOMReadings br,
            WXSensor2WebPage.CurrentReadings cr,
            WXSensor2WebPage.MaxMinTemps mm,
            WXSensor2WebPage.sensorStatus ss,
            WXSensor2WebPage.SWSreadings sws,
            WXSensor2WebPage.houseConditions hc,
            WXSensor2WebPage.trendingData td )
        {
        
        // var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
        var pathWithEnv = @"c:\inetpub\wwwroot\index.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            string updateCycle;  //= txtWebUpdateCycle.Text;
            string twuc = ((WXSensor2WebPage)f).txtWebUpdateCycle.Text;

            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                string now = DateTime.Now.ToString("h:mm:ss tt");
                string fullDate = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt");
                //updateCycle = txtWebUpdateCycle.Text;
                updateCycle = twuc;

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

                sw.WriteLine(".button_info{background-color: gray; color: blue;font-size: 14px; padding: 4px 10px;}");
                sw.WriteLine(".button_infoIn{background-color: white; color: blue;font-size: 14px; padding: 4px 10px;}");
                sw.WriteLine(".button_infoOut{background-color: white; color: red;font-size: 14px; padding: 4px 10px;}");

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
                sw.WriteLine(@"<input type=""button"" style=""float: right;""  onclick=""location.href = 'http://www.bom.gov.au/wa/forecasts/kalamunda.shtml'; "" target=""_blank"" value=""Kalamunda Forecast"" />");
                sw.WriteLine(@"<input type=""button"" style=""float: right;"" onclick=""location.href = 'http://www.bom.gov.au/products/IDR703.loop.shtml'; "" target=""_blank""  value=""BOM Radar""  />");
                sw.WriteLine(@"<input type=""button"" style=""float: right;""  onclick=""location.href = 'https://www.windy.com/-Satellite-satellite?satellite,-31.875,115.778,6'; "" target=""_blank""  value=""Windy""  />");


                // this is the logic that controls the colour of the Status leds
                if (ss.Ib)
                    sw.WriteLine($"<span style=font-size:20px;>Sensor Status: In<img src = \"greenled.jpg\" alt =\" sensed \" style=\"width: 20px; height: 20px; \">  ");
                else
                    sw.WriteLine($"<span style=font-size:20px;>Sensor Status: In<img src = \"redled.jpg\" alt =\"not sensed \" style=\"width: 20px; height: 20px; \"> ");

                if (ss.Ob)
                    sw.WriteLine("<span style=font-size:20px;> Out <img src = \"greenled.jpg\" alt =\" sensed \" style=\"width: 20px; height: 20px; \"> ");
                else
                    sw.WriteLine("<span style=font-size:20px;> Out <img src = \"redled.jpg\" alt =\"not sensed \" style=\"width: 20px; height: 20px; \"> ");
                if (ss.Rb)
                    sw.WriteLine("<span style=font-size:20px;> Rover <img src = \"greenled.jpg\" alt =\" sensed \" style=\"width: 20px; height: 20px; \"> ");
                else
                    sw.WriteLine("<span style=font-size:20px;> Rover <img src = \"redled.jpg\" alt =\"not sensed \" style=\"width: 20px; height: 20px; \"> ");
                if (ss.Pb)
                    sw.WriteLine("<span style=font-size:20px;> Pool <img src = \"greenled.jpg\" alt =\" sensed \" style=\"width: 20px; height: 20px; \"> ");
                else
                    sw.WriteLine("<span style=font-size:20px;> Pool <img src = \"redled.jpg\" alt =\"not sensed \" style=\"width: 20px; height: 20px; \"> ");


                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine();

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
                else if (br.BestMax >= hc.ThighTemp && daylight.Contains(rightNow.Hour) && cr.cTEMPOut > hc.ThighTemp - 3 && td.trend1 > 1)
                {//air con on its hot in the house, its daylight
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"CloseUp, AirCon On: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (cr.cTEMPOut < cr.cTEMPIn && td.trend2 < 1)
                { //trending cool
                    sw.WriteLine($"<input type=button class=button_greenInfo onclick=location.href = '';  target=_blank value=\"OpenUp:Its cooler outside: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
                }
                else if (td.trend1 < 1 && cr.cTEMPIn < hc.ThighTemp)
                {
                    sw.WriteLine($"<input type=button class=button_greenInfo onclick=location.href = '';  target=_blank value=\"OpenUp: {Math.Round(cr.cTEMPOut - cr.cTEMPIn, 0)}\" />");
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

                sw.WriteLine($"<input type=button class=button_blueInfo onclick=location.href = '';  target=_blank value=\"OutTempTrend:{td.trend1}\" />");

                if (br.BWindSpeed > hc.WhighWind)
                {
                    sw.WriteLine($"<input type=button class=button_redInfo onclick=location.href = '';  target=_blank value=\"High Wind - Lower Yagi:\" />");
                }

                sw.WriteLine($"<input type=button class=button_info onclick=location.href = '';  target=_blank value=\"SWS SFI:{sws.SFI} SSN:{sws.SSN} Ap:{sws.Ap} Kp:{sws.Kp} Xray:{sws.xRay} T:{sws.tIndex}\" />");
                sw.WriteLine($"<input type=button class=button_infoIn onclick=\"location.href='minmax.html';\"  target=\"_blank\" value=\"InMin:{mm.wxInMin} @ {mm.wxInMinHour} InMax:{mm.wxInMax} @ {mm.wxInMaxHour}\" />");
                //sw.WriteLine($"<input type=button class=button_infoIn onclick=\"location.href='c:\\inetpub\\wwwroot\\minmax.html';\"  target=\"_blank\" value=\"InMin:{mm.wxInMin} @ {mm.wxInMinHour} InMax:{mm.wxInMax} @ {mm.wxInMaxHour}\" />");
                sw.WriteLine($"<input type=button class=button_infoOut onclick=\"location.href='minmaxLastmonth.html';\"  target=_blank value=\"OutMin:{mm.wxOutMin} @ {mm.wxOutMinHour} OutMax:{mm.wxOutMax} @ {mm.wxOutMaxHour}\" />");
                sw.WriteLine($"<input type=button class=button_infoOut onclick=\"location.href='minmaxLastmonth.html';\"  target=_blank value=\"Fcast:{br.BestRainfall}\" />");


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
                for (int i = 0; i < 24; i++)
                {
                    if (tOut[i] > 0)
                    {
                        sw.Write($"{tOut[i]},");
                    }
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
                for (int i = 0; i < 24; i++)
                {
                    if (tIn[i] > 0)
                    {
                        sw.Write($"{tIn[i]},");
                    }
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
                for (int i = 0; i < 24; i++)
                {
                    if (tBOM[i] > 0)
                    {
                        sw.Write($"{tBOM[i]},");
                    }
                    else if (tBOM[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#42FF33\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");


                //Pool
                sw.WriteLine("},{");
                sw.WriteLine("label: \"Pool\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"cyan\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++)
                {
                    if (tPool[i] > 0)
                    {
                        sw.Write($"{tPool[i]},");
                    }
                    else if (tPool[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderDash: [30,10],");
                sw.WriteLine("borderColor: \"#00ffff\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");

                sw.WriteLine("},{");
                sw.WriteLine("label: \"ROVER\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"yellow\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 24; i++)
                {
                    if (tRover[i] > 0)
                    {
                        sw.Write($"{tRover[i]},");
                    }
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
                sw.WriteLine("borderColor: \"rgba(255,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 3,");
                sw.WriteLine("pointRadius: 0,");
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
                sw.WriteLine("borderColor: \"rgba(255,0,0,0.4)\",");
                sw.WriteLine("borderWidth: 3,"); //sets how thick the line is
                sw.WriteLine("pointRadius: 0,"); //sets whether there is a smooth line or a line with points along it
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
                //sw.WriteLine("tooltips: {enabled: true, mode: 'index', intersect: false} ,");
                //sw.WriteLine("hover: {mode: '', intersect: true } , ");
                sw.WriteLine("scales: {");

                if (DateTime.Now.Hour > 12)  //swap the y Axes ticks and scale to the rhs of the graph after midday
                {
                    sw.WriteLine("     yAxes: [{ id: 'y-axis-0', type: 'linear',position: 'right' , ");
                }
                else
                {
                    sw.WriteLine("     yAxes: [{ id: 'y-axis-0', type: 'linear',position: 'left' , ");
                }
                sw.WriteLine("        ticks: { ");
                sw.WriteLine($"           min: {br.BestMin - 5}, ");
                sw.WriteLine("           stepSize: 1, ");
                sw.WriteLine("           beginAtZero:false},");
                sw.WriteLine("        gridLines:{color: \"#c0c0c0\",lineWidth:1} ");
                sw.WriteLine("      }],"); //end of yAxes
                sw.WriteLine("     xAxes: [{ ");
                sw.WriteLine("         barThickness: 10,lineWidth:5}]");
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





    }//end class
}//namespace
