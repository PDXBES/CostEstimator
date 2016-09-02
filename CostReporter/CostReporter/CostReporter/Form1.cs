using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;

namespace CostReporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            Excel.Range oRng;

            DataTable CapitalCosts = new DataTable();
            DataTable Details = new DataTable();
            DataTable PipeXP = new DataTable();

            double contingency = 0.25;
            double CIT = 0.15;
            double Design = 0.20;
            double PIEE = 0.03;
            double StartUpCloseout = 0.01;


            FolderBrowserDialog fbd = new FolderBrowserDialog();
            try
            {
                DialogResult dr = fbd.ShowDialog();

                CapitalCosts = GetDataTableFromCsv(fbd.SelectedPath + "\\CapitalCostsMobilizationRatesAndTimes.csv", true);
                Details = GetDataTableFromCsv(fbd.SelectedPath + "\\COSTEST_PIPEDETAILS.csv", true);
                PipeXP = GetDataTableFromCsv(fbd.SelectedPath + "\\COSTEST_PIPEXP.csv", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read files!\n"+ex.ToString());
                return;
            }

            var results = from tableDetails in Details.AsEnumerable()
                          join tablePipeXP in PipeXP.AsEnumerable()
                          on (int)tableDetails["ID"] equals (int)tablePipeXP["ID"]
                          select new
                          {
                              ID = (int)tableDetails["ID"],
                              us_node_id = (string)tableDetails["USNode"],
                              ds_node_id = (string)tableDetails["DSNode"],
                              pipeSectionNumber = ((int)tableDetails["ID"]).ToString() + " " + (string)tableDetails["USNode"] + "-" + (string)tableDetails["DSNode"],
                              material = (string)"CSP",
                              pipeDiam = (double)tableDetails["DiamWidth"],
                              pipeDepth = ((double)tablePipeXP["uDepth"])+((double)tablePipeXP["dDepth"])/2.0,
                              pipeRun = (double)tablePipeXP["Length"], 
                              manholeCost = (double)tableDetails["Manhole"],
                              OCDirectCostPerLinearFoot = ((double)tableDetails["DirectConstructionCost"] - (double)tableDetails["Manhole"]) / (double)tablePipeXP["Length"],
                              LinerDirectCostPerLinearFoot = (double)tableDetails["LinerDirectConstructionCost"] / (double)tablePipeXP["Length"],
                              OCDirectCostPerLinearFootPerInchDiameter = ((double)tableDetails["DirectConstructionCost"]) / ((double)tablePipeXP["Length"]*(double)tableDetails["DiamWidth"]),
                              LinerDirectCostPerLinearFootPerInchDiameter = (double)tableDetails["LinerDirectConstructionCost"] / ((double)tablePipeXP["Length"] * (double)tableDetails["DiamWidth"]),
                              OCTotalCost = (double)tableDetails["DirectConstructionCost"],
                              LinerTotalCost = (double)tableDetails["LinerDirectConstructionCost"] 
                          };

            var resultsSummary = from tableDetails in Details.AsEnumerable()
                          join tablePipeXP in PipeXP.AsEnumerable()
                          on (int)tableDetails["ID"] equals (int)tablePipeXP["ID"]
                          select new
                          {
                              /*ID = (int)tableDetails["ID"],
                              us_node_id = (string)tableDetails["USNode"],
                              ds_node_id = (string)tableDetails["DSNode"],
                              pipeSectionNumber = ((int)tableDetails["ID"]).ToString() + " " + (string)tableDetails["USNode"] + "-" + (string)tableDetails["DSNode"],
                              material = (string)"CSP",
                              pipeDiam = (double)tableDetails["DiamWidth"],
                              pipeDepth = ((double)tablePipeXP["uDepth"]) + ((double)tablePipeXP["dDepth"]) / 2.0,
                              pipeRun = (double)tablePipeXP["Length"],
                              manholeCost = (double)tableDetails["Manhole"],*/
                              OCDirectCost = (double)tableDetails["DirectConstructionCost"],
                              LinerDirectCost = (double)tableDetails["LinerDirectConstructionCost"]

                              /*LinerDirectCostPerLinearFoot = (double)tableDetails["LinerDirectConstructionCost"] / (double)tablePipeXP["Length"],
                              OCDirectCostPerLinearFootPerInchDiameter = ((double)tableDetails["DirectConstructionCost"]) / ((double)tablePipeXP["Length"] * (double)tableDetails["DiamWidth"]),
                              LinerDirectCostPerLinearFootPerInchDiameter = (double)tableDetails["LinerDirectConstructionCost"] / ((double)tablePipeXP["Length"] * (double)tableDetails["DiamWidth"]),
                              OCTotalCost = (double)tableDetails["DirectConstructionCost"],
                              LinerTotalCost = (double)tableDetails["LinerDirectConstructionCost"]*/
                          };
            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                //widen all of the cells
                oSheet.Cells.ColumnWidth = 23;
                oSheet.Cells.NumberFormat = "$#,##0";

                //Add report headers going cell by cell
                oSheet.Cells[2, 1].Font.Bold = true;
                oSheet.Cells[2, 1].Font.Size = 14;
                oSheet.Range[oSheet.Cells[2, 1], oSheet.Cells[2, 6]].Merge();
                oSheet.Cells[2, 1] = "Order of Magnitude Level Project \nCost Estimate Development Summary";
                oSheet.Cells[2, 1].RowHeight = 36.0;
                oSheet.Cells[2, 1].WrapText = true;

                oSheet.Cells[3, 1].Font.Bold = true;
                oSheet.Cells[3, 1].Font.Size = 24;
                oSheet.Range[oSheet.Cells[3, 1], oSheet.Cells[3, 4]].Merge();
                oSheet.Cells[3, 1] = "ModelNameHere";

                oSheet.Cells[5, 1].Font.Bold = true;
                oSheet.Cells[5, 1].Font.Size = 10;
                oSheet.Range[oSheet.Cells[5, 1], oSheet.Cells[5, 4]].Merge();
                oSheet.Cells[5, 1] = "Project Information";


                oSheet.Range[oSheet.Cells[6, 1], oSheet.Cells[14, 1]].Font.Size = 8;
                oSheet.Range[oSheet.Cells[6, 1], oSheet.Cells[13, 1]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                oSheet.Cells[6, 1] = "Project Title";
                oSheet.Cells[7, 1] = "Project Number";
                oSheet.Cells[8, 1] = "Project Description";
                oSheet.Cells[9, 1] = "Project Manager";
                oSheet.Cells[10, 1] = "Cost Estimator";
                oSheet.Cells[11, 1] = "Date Prepared";
                oSheet.Cells[12, 1] = "Data Source";
                oSheet.Cells[12, 1] = "ENR";
                oSheet.Cells[13, 1] = "Pipe Table ENR";

                
                oSheet.Range[oSheet.Cells[16, 1], oSheet.Cells[16, 4]].Merge();
                oSheet.Cells[16, 1].Font.Size = 14;
                oSheet.Cells[16, 1] = "Summary";
                oSheet.Cells[16, 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Purple);
                oSheet.Cells[16, 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                oSheet.Cells[17, 1] = "Direct construction pipe cost";
                oSheet.Cells[17, 4] = resultsSummary.Sum(od => od.OCDirectCost);

                oSheet.Cells[18, 1] = "Direct consrtuction inflow control cost";

                oSheet.Range[oSheet.Cells[19, 1], oSheet.Cells[19, 3]].Merge();
                oSheet.Cells[19, 1].Font.Bold = true;
                oSheet.Cells[19, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                oSheet.Cells[19, 1] = "Total direct construction cost";
                oSheet.Cells[19, 4] = resultsSummary.Sum(od => od.OCDirectCost);

                oSheet.Cells[20, 1] = "Contingency";
                oSheet.Cells[20, 4] = resultsSummary.Sum(od => od.OCDirectCost) * contingency;

                oSheet.Range[oSheet.Cells[21, 1], oSheet.Cells[21, 3]].Merge();
                oSheet.Cells[21, 1].Font.Bold = true;
                oSheet.Cells[21, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                oSheet.Cells[21, 1] = "Total direct construction & contingency cost";
                oSheet.Cells[21, 4] = resultsSummary.Sum(od => od.OCDirectCost) * (1 + contingency);

                oSheet.Cells[22, 1] = "Const mgt, Insp, Test";
                oSheet.Cells[22, 4] = resultsSummary.Sum(od => od.OCDirectCost) * (1 + contingency) * CIT;
                oSheet.Cells[23, 1] = "Design";
                oSheet.Cells[23, 4] = resultsSummary.Sum(od => od.OCDirectCost) * (1 + contingency) * Design;
                oSheet.Cells[24, 1] = "PI, I&C, Easements, Environmental";
                oSheet.Cells[24, 4] = resultsSummary.Sum(od => od.OCDirectCost) * (1 + contingency) * PIEE;
                oSheet.Cells[25, 1] = "Startup/closeout";
                oSheet.Cells[25, 4] = resultsSummary.Sum(od => od.OCDirectCost) * (1 + contingency) * StartUpCloseout;

                oSheet.Range[oSheet.Cells[26, 1], oSheet.Cells[26, 3]].Merge();
                oSheet.Cells[26, 1].Font.Bold = true;
                oSheet.Cells[26, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                oSheet.Cells[26, 1] = "Total indirect project cost";
                oSheet.Cells[26, 4] = "=SUM(D22:D25)";

                oSheet.Range[oSheet.Cells[28, 1], oSheet.Cells[28, 3]].Merge();
                oSheet.Cells[28, 1].Font.Bold = true;
                oSheet.Cells[28, 1].Font.Size = 11;
                oSheet.Cells[28, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                oSheet.Cells[28, 1] = "Total Estimated Project Cost";
                oSheet.Cells[28, 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Purple);
                oSheet.Cells[28, 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                oSheet.Cells[28, 4] = "=D21+D26";

                //Add another sheet and make that the active sheet
                oSheet = oWB.Sheets.Add(Type.Missing, Type.Missing, Type.Missing);

                //Add report headers going cell by cell.
                oSheet.Cells[2, 1].Font.Bold = true;
                oSheet.Cells[2, 1].Font.Size = 14;
                oSheet.Range[oSheet.Cells[2, 1], oSheet.Cells[2, 6]].Merge();
                oSheet.Cells[2, 1] = "Order of Magnitude Level Project \nCost Estimate Development Summary";
                oSheet.Cells[2, 1].RowHeight = 36.0;
                oSheet.Cells[2, 1].WrapText = true;

                oSheet.Cells[4, 1].Font.Size = 14;
                oSheet.Range[oSheet.Cells[4, 1], oSheet.Cells[4, 11]].Merge();
                oSheet.Cells[4, 1] = "Direct Construction Pipe Cost";
                oSheet.Cells[4, 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);



                //Add table headers going cell by cell.
                oSheet.Range[oSheet.Cells[5, 1], oSheet.Cells[5, 11]].Font.Size = 9;
                oSheet.Range[oSheet.Cells[5, 1], oSheet.Cells[5, 11]].Font.Bold = true;
                oSheet.Range[oSheet.Cells[5, 1], oSheet.Cells[5, 11]].WrapText = true;


                oSheet.Cells[5, 1] = "Pipe Selection Number";
                oSheet.Cells[5, 2] = "Pipe Material Type";
                oSheet.Cells[5, 3] = "Pipe Diam (in)";
                oSheet.Cells[5, 4] = "Pipe Depth (ft)";
                oSheet.Cells[5, 5] = "Total Direct Construction Cost/lf";
                oSheet.Cells[5, 6] = "Pipe Run (ft)";
                oSheet.Cells[5, 7] = "Manhole Cost";
                oSheet.Cells[5, 8] = "Total Cost";
                oSheet.Cells[5, 9] = "Cost/lf-in diam";

                //Format A1:D1 as bold, vertical alignment = center.
                /*oSheet.get_Range("A1", "D1").Font.Bold = true;
                oSheet.get_Range("A1", "D1").VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;*/

                

                //Fill A2:B6 with an array of values (First and Last Names).
                int row = 6;
                foreach (var item in results.ToList())
                {
                    oSheet.Cells[row, 1] = item.pipeSectionNumber; // get_Range("A" + row.ToString(), "A" + row.ToString()).Value2 = item.pipeSectionNumber;
                    oSheet.Cells[row, 2] = item.material;
                    oSheet.Cells[row, 3] = item.pipeDiam;
                    oSheet.Cells[row, 4] = item.pipeDepth;
                    oSheet.Cells[row, 5] = item.OCDirectCostPerLinearFoot;
                    oSheet.Cells[row, 6] = item.pipeRun;
                    oSheet.Cells[row, 7] = item.manholeCost;
                    oSheet.Cells[row, 8] = item.OCTotalCost;
                    oSheet.Cells[row, 9] = item.OCDirectCostPerLinearFootPerInchDiameter;
                    row++;
                }

                //Fill C2:C6 with a relative formula (=A2 & " " & B2).
                /*oRng = oSheet.get_Range("C2", "C6");
                oRng.Formula = "=A2 & \" \" & B2";

                //Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                oRng = oSheet.get_Range("D2", "D6");
                oRng.Formula = "=RAND()*100000";
                oRng.NumberFormat = "$0.00";

                //AutoFit columns A:D.
                oRng = oSheet.get_Range("A1", "D1");
                oRng.EntireColumn.AutoFit();

                //Manipulate a variable number of columns for Quarterly Sales Data.
                DisplayQuarterlySales(oSheet);*/

                //Make sure Excel is visible and give the user control
                //of Microsoft Excel's lifetime.
                oXL.Visible = true;
                oXL.UserControl = true;
            }
            catch (Exception theException)
            {
                /*String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                MessageBox.Show(errorMessage, "Error");*/
                theException.ToString();
            }
        }

        private void DisplayQuarterlySales(Excel._Worksheet oWS)
        {
            Excel._Workbook oWB;
            Excel.Series oSeries;
            Excel.Range oResizeRange;
            Excel._Chart oChart;
            String sMsg;
            int iNumQtrs;

            //Determine how many quarters to display data for.
            for (iNumQtrs = 4; iNumQtrs >= 2; iNumQtrs--)
            {
                sMsg = "Enter sales data for ";
                sMsg = String.Concat(sMsg, iNumQtrs);
                sMsg = String.Concat(sMsg, " quarter(s)?");

                DialogResult iRet = MessageBox.Show(sMsg, "Quarterly Sales?",
                    MessageBoxButtons.YesNo);
                if (iRet == DialogResult.Yes)
                    break;
            }

            sMsg = "Displaying data for ";
            sMsg = String.Concat(sMsg, iNumQtrs);
            sMsg = String.Concat(sMsg, " quarter(s).");

            MessageBox.Show(sMsg, "Quarterly Sales");

            //Starting at E1, fill headers for the number of columns selected.
            oResizeRange = oWS.get_Range("E1", "E1").get_Resize(Missing.Value, iNumQtrs);
            oResizeRange.Formula = "=\"Q\" & COLUMN()-4 & CHAR(10) & \"Sales\"";

            //Change the Orientation and WrapText properties for the headers.
            oResizeRange.Orientation = 38;
            oResizeRange.WrapText = true;

            //Fill the interior color of the headers.
            oResizeRange.Interior.ColorIndex = 36;

            //Fill the columns with a formula and apply a number format.
            oResizeRange = oWS.get_Range("E2", "E6").get_Resize(Missing.Value, iNumQtrs);
            oResizeRange.Formula = "=RAND()*100";
            oResizeRange.NumberFormat = "$0.00";

            //Apply borders to the Sales data and headers.
            oResizeRange = oWS.get_Range("E1", "E6").get_Resize(Missing.Value, iNumQtrs);
            oResizeRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

            //Add a Totals formula for the sales data and apply a border.
            oResizeRange = oWS.get_Range("E8", "E8").get_Resize(Missing.Value, iNumQtrs);
            oResizeRange.Formula = "=SUM(E2:E6)";
            oResizeRange.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle
                = Excel.XlLineStyle.xlDouble;
            oResizeRange.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).Weight
                = Excel.XlBorderWeight.xlThick;

            //Add a Chart for the selected data.
            oWB = (Excel._Workbook)oWS.Parent;
            oChart = (Excel._Chart)oWB.Charts.Add(Missing.Value, Missing.Value,
                Missing.Value, Missing.Value);

            //Use the ChartWizard to create a new chart from the selected data.
            oResizeRange = oWS.get_Range("E2:E6", Missing.Value).get_Resize(
                Missing.Value, iNumQtrs);
            oChart.ChartWizard(oResizeRange, Excel.XlChartType.xl3DColumn, Missing.Value,
                Excel.XlRowCol.xlColumns, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            oSeries = (Excel.Series)oChart.SeriesCollection(1);
            oSeries.XValues = oWS.get_Range("A2", "A6");
            for (int iRet = 1; iRet <= iNumQtrs; iRet++)
            {
                oSeries = (Excel.Series)oChart.SeriesCollection(iRet);
                String seriesName;
                seriesName = "=\"Q";
                seriesName = String.Concat(seriesName, iRet);
                seriesName = String.Concat(seriesName, "\"");
                oSeries.Name = seriesName;
            }

            oChart.Location(Excel.XlChartLocation.xlLocationAsObject, oWS.Name);

            //Move the chart so as not to cover your data.
            oResizeRange = (Excel.Range)oWS.Rows.get_Item(10, Missing.Value);
            oWS.Shapes.Item("Chart 1").Top = (float)(double)oResizeRange.Top;
            oResizeRange = (Excel.Range)oWS.Columns.get_Item(2, Missing.Value);
            oWS.Shapes.Item("Chart 1").Left = (float)(double)oResizeRange.Left;
        }

        static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
