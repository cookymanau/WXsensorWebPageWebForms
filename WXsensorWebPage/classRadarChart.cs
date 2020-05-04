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



        public static double[] windSpdMonth = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static double[] windSpdToday = new double[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
       // public static string[] windDirMonth = new string[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

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
                //double rdngTemp;
                //double tempToUse = 0;
                DateTime rdngTime;
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

                SqlCommand getReadings = new SqlCommand(fred, conn);


                //Array.Clear(tArray, 0, 24);
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
                //double rdngTemp;
                //double tempToUse = 0;
                DateTime rdngTime;
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

                SqlCommand getReadings = new SqlCommand(sqlQuery, conn);


                //Array.Clear(tArray, 0, 24);
                conn.Open();
                rdr = getReadings.ExecuteReader();
                while (rdr.Read())
                {
                    windSpdToday[cnt] = (double)rdr["WINDSPEED"];
                    // windDirMonth[cnt] = (string)rdr["WINDDIR"];
                    cnt += 1;
                }
                conn.Close();
            }
            plotRadar();  //plot the graph
        }





        public void plotRadar()
        {
            var pathWithEnv = @"c:\inetpub\wwwroot\windRadar.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            //string updateCycle;  //= txtWebUpdateCycle.Text;
            //string twuc = ((WXSensor2WebPage)f).txtWebUpdateCycle.Text;

            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                sw.WriteLine(@"<HTML><HEAD>
                            <title>SCARP Weather</title>");
                sw.WriteLine($"<meta http-equiv = \"refresh\" content = \"120\" >");// 1 is 1 second  60 is 60 seconds
                sw.WriteLine(@"<script src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js""></script>");

                sw.WriteLine("</head><body>");
                sw.WriteLine(@"<center><h2>Scarp Weather - Wind Chart Avg Over 10 minutes data</h2></center>");
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
sw.WriteLine(@"         }
      ]
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
