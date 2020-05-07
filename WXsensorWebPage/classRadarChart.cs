using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace WXsensorWebPage
{
    class classRadarChart
    {
        //create a radar chart of data from a sql database showing wind direction and windspeeds
        string connectionString = @"Data Source=192.168.1.15\DAWES_SQL2008; Database = WeatherStation; User Id = WeatherStation; Password = Esp32a.b.;";


        //ok I have made the control holding the windRadarChart public
        //no we have to instantiate a object from the form
        System.Windows.Forms.Form f = System.Windows.Forms.Application.OpenForms["WXSensor2WebPage"];
        //now we should be able to see any Public things from the form  - you cant do this here - you have to do it down in the function
        //ie I dont seem to be able to make this global across the class... see line 37
        //string radarInterval = ((WXSensor2WebPage)f).txtWindChart.Text;
        // but maybe I can create a global here and set it from the function
        int gInt = 0;

        string wGstColor = "";

        public static double[] windSpdMonth = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] windSpdToday = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] wGst = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };




        public  classRadarChart() //constructors cant return anything
        {

        }


        public void getData()
        {
            string radarInterval = ((WXSensor2WebPage)f).txtWindChart.Text;
            int intRadarInterval = int.Parse(radarInterval);
            gInt = intRadarInterval;

            {
                SqlConnection conn;
                SqlDataReader rdr = null;
                DateTime rightNow = DateTime.Now;
                int cnt = 0;
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM

                  string fred = $@"
-- this one calcs the last N days counting back from today
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'N' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'N' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NNE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'NNE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'NE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'ENE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'ENE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'E' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'E' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'ESE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'ESE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'SE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SSE' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'SSE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'S' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'S' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SSW' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'SSW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SW' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'SW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'WSW' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'WSW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'W' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'W' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'WNW' and convert(date, TIME)  >= convert(date, getdate()-{gInt})  group by WINDDIR),0) AS WINDSPEED,'WNW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NW' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'NW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NNW' and convert(date, TIME)  >= convert(date, getdate()-{gInt}) group by WINDDIR),0) AS WINDSPEED,'NNW' as WINDDIR
";
                Array.Clear(windSpdMonth, 0, 32);
                SqlCommand getReadings = new SqlCommand(fred, conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    windSpdMonth[cnt] = (double)rdr["WINDSPEED"];
                   // windDirMonth[cnt] = (string)rdr["WINDDIR"];
                    cnt += 1;

                }
                conn.Close();
            }
            //plotRadar();
            getTodaysData();
        }

        public void getTodaysData()
        {
            {
                SqlConnection conn;
                SqlDataReader rdr = null;
                DateTime rightNow = DateTime.Now;
                int cnt = 0;
                conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                //takes care of null values, this query
                string sqlQuery = $@"
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'N' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'N' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NNE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'NNE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'NE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'ENE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'ENE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'E' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'E' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'ESE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'ESE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'SE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SSE' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'SSE' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'S' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'S' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SSW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'SSW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'SW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'SW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'WSW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'WSW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'W' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'W' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'WNW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'WNW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'NW' as WINDDIR
union
SELECT isnull((select max(WINDSPEED)  as WINDSPEED from WXBOMGHILL  where  WINDDIR = 'NNW' and convert(date, TIME) = convert(date, getdate()) group by WINDDIR),0) AS WINDSPEED,'NNW' as WINDDIR
";

                Array.Clear(windSpdToday, 0, 32);
                SqlCommand getReadings = new SqlCommand(sqlQuery, conn);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    windSpdToday[cnt] = (double)rdr["WINDSPEED"];
                    cnt += 1;
                }
                conn.Close();
            }
            getWindGust();  //plot the graph
        }

        public void getWindGust()
        {
            SqlConnection conn;
            SqlDataReader rdr = null;
            DateTime rightNow = DateTime.Now;
            int cnt = 0;
            string gustDir="";
            double wGustkmh = 0;

            Dictionary<string, int> dirs = new Dictionary<string, int>()
            {
                {"N",0},
                {"NNE",1},
                {"NE",2},
                {"ENE",3},
                {"E",4},
                {"ESE",5},
                {"SE",6},
                {"SSE",7},
                {"S",8},
                {"SSW",9},
                {"SW",10},
                {"WSW",11},
                {"W",12},
                {"WNW",13},
                {"NW",14},
                {"NNW",15},
            };


            Array.Clear(wGst, 0, 32);


            conn = new SqlConnection(connectionString);  //connectionString is a global ATM
                                                         //takes care of null values, this query
            string sqlQuery = $@"  select  top 1 WINDGUST  , WINDDIR,TIME from WXBOMGHILL   order by TIME DESC";
            SqlCommand getReadings = new SqlCommand(sqlQuery, conn);
            conn.Open();
            rdr = getReadings.ExecuteReader();
            while (rdr.Read())
            {
                wGustkmh = (double)rdr["WINDGUST"];
                gustDir = rdr["WINDDIR"].ToString();
            }

            if (wGustkmh > 50)
            {
                wGstColor = "rgba(255,0,0,1)";//red
            }
            else
            {
                wGstColor = "rgba(0,0,255,1)";//blue
            }

            //now we have to put the wGustkmh intot he correct slot in the grid

            foreach (var dir in dirs)
            {
                if (dir.Key == gustDir)
                    wGst[dir.Value] = wGustkmh;
            }
            plotRadar();
        }//end


        public void plotRadar()
        {
            var pathWithEnv = @"c:\inetpub\wwwroot\windRadar.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

            


            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                sw.WriteLine(@"<HTML><HEAD>
                            <title>SCARP Weather</title>");
                sw.WriteLine($"<meta http-equiv = \"refresh\" content = \"120\" >");// 1 is 1 second  60 is 60 seconds
                sw.WriteLine(@"<script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js""></script>");

                sw.WriteLine("</head><body>");
                sw.WriteLine(@"<center><h2>Scarp Weather - Wind Chart Avg over 10 minutes , WindGust Avg over 3 minutes</h2></center>");
                //sw.WriteLine(@"<center><h3>Avg Over 10 Minutes Data</h3></center>");

                sw.WriteLine(@"<canvas id=""radar-chart"" width=""400"" height=""200""></canvas> ");
                sw.WriteLine("<script>");
                sw.WriteLine(@"new Chart(document.getElementById(""radar-chart""), { ");

                sw.WriteLine(@"type: 'radar', ");
                sw.WriteLine(@"data: {
      labels: [""North"", ""NNE"", ""NE"", ""ENE"", ""East"",""ESE"",""SE"",""SSE"",""South"",""SSW"",""SW"",""WSW"",""West"",""WNW"",""NW"",""NNW""],
      datasets: [
     { ");
                sw.WriteLine($@" label: ""{gInt} Days"",  ");
                sw.WriteLine(@" fill: true,
          backgroundColor: ""rgba(179,181,198,0.2)"",
          borderColor: ""rgba(179,181,198,1)"",
          pointBorderColor: ""#fff"",
          pointBackgroundColor: ""rgba(179,181,198,1)"",  ");
                sw.WriteLine($@"          data: [{windSpdMonth[0]}, {windSpdMonth[1]}, {windSpdMonth[2]}, {windSpdMonth[3]}, {windSpdMonth[4]},{windSpdMonth[5]}
                                                 ,{windSpdMonth[6]},{windSpdMonth[7]},{windSpdMonth[8]},{windSpdMonth[9]},{windSpdMonth[10]},{windSpdMonth[11]}
                                                 ,{windSpdMonth[12]},{windSpdMonth[13]},{windSpdMonth[14]},{windSpdMonth[15]}]");
                sw.WriteLine(@"        }, {
            label: ""Today"",
          fill: true,
          backgroundColor: ""rgba(255,99,132,0.2)"",
          borderColor: ""rgba(255,99,132,1)"",
          pointBorderColor: ""#fff"",
          pointBackgroundColor: ""rgba(255,99,132,1)"",
          pointBorderColor: ""#fff"", ");
          
sw.WriteLine($@"data: [{ windSpdToday[0]}, { windSpdToday[1]}, { windSpdToday[2]}, { windSpdToday[3]}, { windSpdToday[4]},{ windSpdToday[5]},{ windSpdToday[6]},{ windSpdToday[7]}
                      ,{ windSpdToday[8]}, { windSpdToday[9]}, { windSpdToday[10]},{ windSpdToday[11]},{ windSpdToday[12]},{ windSpdToday[13]},{ windSpdToday[14]},{ windSpdToday[15]}]");
                sw.WriteLine(@"         } ");
                sw.WriteLine(@", { ");
                sw.WriteLine(@"label: ""WindGust"", ");
                sw.WriteLine(@"fill: false,");
                sw.WriteLine(@"pointRadius: 7,");
                sw.WriteLine(@"pointStyle: ""circle"",");
                sw.WriteLine(@"backgroundColor: ""rgba(255,99,132,0.2)"",");
                sw.WriteLine($@"borderColor: ""{wGstColor}"",");
                sw.WriteLine(@"pointBorderColor: ""#fff"",");
                sw.WriteLine($@"pointBackgroundColor: ""{wGstColor}"",");
                sw.WriteLine(@"pointBorderColor: ""#fff"", ");
               sw.WriteLine($@"data: [{wGst[0]}, {wGst[1]}, {wGst[2]}, {wGst[3]}, {wGst[4]}, {wGst[5]}, {wGst[6]}, {wGst[7]}, {wGst[8]}, {wGst[9]}, {wGst[10]}, {wGst[11]}, {wGst[12]}, {wGst[13]}, {wGst[14]}, {wGst[15]}]");
sw.WriteLine(@"      }]
    },
    options: {
      title: {
        display: true,
        text: 'Wind at Lesmurdie'
      }
}
});
</script>");




            }

        }//end of plot radar

        }//end of class
}//end namespace
