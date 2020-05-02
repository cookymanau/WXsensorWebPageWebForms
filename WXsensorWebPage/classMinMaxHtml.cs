using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WXsensorWebPage
{
    class classMinMaxHtml
    {

        //instead of being two functions, we are now one function for this month and last month
        // the arrays to use are set by the constructor

        double[] tRoverMax;//= WXSensor2WebPage.tRoverMax;
        double[] tRoverMin;// = WXSensor2WebPage.tRoverMin;
        double[] tPoolMin;// = WXSensor2WebPage.tPoolMin;
        double[] tPoolMax;// = WXSensor2WebPage.tPoolMax;
        double[] tOutMin;// = WXSensor2WebPage.tOutMin;
        double[] tOutMax;// = WXSensor2WebPage.tOutMax;
        double[] tInMax;// = WXSensor2WebPage.tInMax;
        double[] tInMin;// = WXSensor2WebPage.tInMin;

       string pathWithEnv; //where to put the file and what to call it

        public classMinMaxHtml()
        {

        }

        public classMinMaxHtml(int month)
        {

            if (month == 2)
            {
                tRoverMax = WXSensor2WebPage.tRoverMaxLM;
                tRoverMin = WXSensor2WebPage.tRoverMinLM;
                tPoolMin = WXSensor2WebPage.tPoolMinLM;
                tPoolMax = WXSensor2WebPage.tPoolMaxLM;
                tOutMin = WXSensor2WebPage.tOutMinLM;
                tOutMax = WXSensor2WebPage.tOutMaxLM;
                tInMax = WXSensor2WebPage.tInMaxLM;
                tInMin = WXSensor2WebPage.tInMinLM;

                pathWithEnv = @"c:\inetpub\wwwroot\minmaxLastmonth.html";
            }
            else if (month ==1)
            {
                tRoverMax = WXSensor2WebPage.tRoverMax;
                tRoverMin = WXSensor2WebPage.tRoverMin;
                tPoolMin = WXSensor2WebPage.tPoolMin;
                tPoolMax = WXSensor2WebPage.tPoolMax;
                tOutMin = WXSensor2WebPage.tOutMin;
                tOutMax = WXSensor2WebPage.tOutMax;
                tInMax = WXSensor2WebPage.tInMax;
                tInMin = WXSensor2WebPage.tInMin;

                pathWithEnv = @"c:\inetpub\wwwroot\minmax.html";
            }

        }//end 




        System.Windows.Forms.Form f = System.Windows.Forms.Application.OpenForms["WXSensor2WebPage"];    //so we have now made an object from WXSensor2WebPage
        //double[] tRoverMax = WXSensor2WebPage.tRoverMax;
        //double[] tRoverMin = WXSensor2WebPage.tRoverMin;
        //double[] tPoolMin = WXSensor2WebPage.tPoolMin;
        //double[] tPoolMax = WXSensor2WebPage.tPoolMax;
        //double[] tOutMin = WXSensor2WebPage.tOutMin;
        //double[] tOutMax = WXSensor2WebPage.tOutMax;
        //double[] tInMax = WXSensor2WebPage.tInMax;
        //double[] tInMin = WXSensor2WebPage.tInMin;




        public void MinMaxHTML()
        {

            // var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
            //var pathWithEnv = @"c:\inetpub\wwwroot\minmax.html";
            //var pathWithEnv = @"c:\inetpub\wwwroot\minmaxLastmonth.html";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            string updateCycle;  //= txtWebUpdateCycle.Text;
            using (StreamWriter sw = new StreamWriter(filePath, append: false))  //using controls low-level resource useage
            {
                string now = DateTime.Now.ToString("h:mm:ss tt");
                string fullDate = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt");
                
                string twuc = ((WXSensor2WebPage)f).txtWebUpdateCycle.Text;
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
                sw.WriteLine("<center><h2>Scarp Weather - Minimums and Maximums for the month</H2>");

                sw.WriteLine("</center>");

                sw.WriteLine(@"<span style=""font-family:Arial;font-size:45px;>""");
                sw.WriteLine(@"</span>");
                //sw.WriteLine($"<input type=button  style=float: right; class=button_white onclick=location.href = '';  target=_blank value=\"OutMin: {dailyMin} OutMax: {dailyMax} InMin: {dailyInMin} InMax: {dailyInMax} \" />");
                sw.WriteLine("<br>");
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
                sw.WriteLine(@"labels: [""1st"",""2nd"",""3rd"",""4th"",""5th"",""6th"",""7th"",""8th"",""9th"",""10th"",""11th"",""12th"",""13th"",""14th"",""15"",""16"",""17"",""18th"",""19"",""20"",""21"",""22"",""23"",""24"",""25"",""26"",""27"",""28"",""29"",""30"",""31""],");
                sw.WriteLine("datasets: [{");

                //IN Minimums this month
                sw.WriteLine("label: 'In Minimum',");
                sw.WriteLine("id: \"y-axis-0\",");
                sw.WriteLine("backgroundColor: [\"blue\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tInMin[i] > 0)
                    {
                        sw.Write($"{tInMin[i]},");
                    }
                    else if (tInMin[i] == 0)
                    {
                        sw.Write($" ,");
                    }

                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: [\"blue\"],");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");
                sw.WriteLine("},{");

                //In Maximums this month
                sw.WriteLine("label: 'In Max',");
                sw.WriteLine("id: \"y-axis-0\",");
                sw.WriteLine("backgroundColor: [\"blue\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tInMax[i] > 0)
                    {
                        sw.Write($"{tInMax[i]},");
                    }
                    else if (tInMax[i] == 0)
                    {
                        sw.Write($" ,");
                    }

                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: [\"blue\"],");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");
                sw.WriteLine("},{");
                //OUT MINimums this month
                sw.WriteLine("label: 'Out Minimum',");
                sw.WriteLine("id: \"y-axis-0\",");
                sw.WriteLine("backgroundColor: [\"red\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tOutMin[i] > 0)
                    {
                        sw.Write($"{tOutMin[i]},");
                    }
                    else if (tOutMin[i] == 0)
                    {
                        sw.Write($" ,");
                    }

                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: [\"red\"],");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill:false,");
                sw.WriteLine("type: 'line' ");
                sw.WriteLine("},{");
                // OUT Max 
                sw.WriteLine("label: \"Out Maximum\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"red\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tOutMax[i] > 0)
                    {
                        sw.Write($"{tOutMax[i]},");
                    }
                    else if (tOutMax[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: [\"red\"],");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                sw.WriteLine(" },{");

                sw.WriteLine("label: \"Rover Minimum\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: \"#CCCC00\",");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tRoverMin[i] > 0)
                    {
                        sw.Write($"{tRoverMin[i]},");
                    }
                    else if (tRoverMin[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#CCCC00\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                sw.WriteLine(" },{");//-----------------------------
                sw.WriteLine("label: \"Rover Maximum\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: \"#CCCC00\",");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tRoverMax[i] > 0)
                    {
                        sw.Write($"{tRoverMax[i]},");
                    }
                    else if (tRoverMax[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"#CCCC00\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                //sw.WriteLine("}");
                //sw.WriteLine("]");
                sw.WriteLine(" },{"); //-----------------------------------
                sw.WriteLine("label: \"Pool Minimum\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: \"cyan\",");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tPoolMin[i] > 0)
                    {
                        sw.Write($"{tPoolMin[i]},");
                    }
                    else if (tPoolMin[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: [\"cyan\"],");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                sw.WriteLine(" },{"); //------------------------------------
                sw.WriteLine("label: \"Pool Maximum\",");
                sw.WriteLine("type: \"line\",");
                sw.WriteLine("backgroundColor: [\"cyan\"],");
                sw.Write($"data:[");
                for (int i = 0; i < 32; i++)
                {
                    if (tPoolMax[i] > 0)
                    {
                        sw.Write($"{tPoolMax[i]},");
                    }
                    else if (tPoolMax[i] == 0)
                    {
                        sw.Write($" ,");
                    }
                }
                sw.WriteLine(" ],");
                sw.WriteLine("borderColor: \"cyan\",");
                sw.WriteLine("pointRadius: 2,");
                sw.WriteLine("fill: false");
                sw.WriteLine("}"); //only here
                sw.WriteLine("]");// only here
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
                sw.WriteLine("       min: 0, ");
                sw.WriteLine("       max: 50, ");
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





    }
}
