using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using System.Windows.Forms;
using System.Reflection;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace CostEstimatorAddIn
{
    class PipeXP_Arc
    {
        public string sourceDatabase = @"Database Connections\\egh_Public.sde\\EGH_PUBLIC.ARCMAP_ADMIN.";
        public string sourceDatabaseWater = @"Database Connections\\egh_Water.sde\\PWBWATER.ARCMAP_ADMIN.";
        //public string modelLinksLayer = "Links";
        //public string modelNodesLayer = "Nodes";
        public string pipeXPSQLiteDBName = "PipeXP";
        public string modelPath;

        public DataTable listOfXPFiles = new DataTable("listOfXPFiles");
        public DataColumn FileName = new DataColumn("FileName", System.Type.GetType("System.String"));
        public DataColumn DBLocation = new DataColumn("DBLocation", System.Type.GetType("System.String"));
        public DataColumn FileType = new DataColumn("FileType", System.Type.GetType("System.String"));
        public DataColumn Alias = new DataColumn("Alias", System.Type.GetType("System.String"));


        public PipeXP_Arc(
                     string theModelPath,
                     string _sourceSchema,
                     string _sourceSchemaWater,
                     string _sourceDatabase = @"Database Connections\\egh_Public.sde\\",
                     string _sourceDatabaseWater = @"Database Connections\\egh_Water.sde\\"
                  )
        {
            if (_sourceSchema != "")
            {
                _sourceSchema = _sourceSchema + ".";
            }

            if (_sourceSchemaWater != "")
            {
                _sourceSchemaWater = _sourceSchemaWater + ".";
            }
            sourceDatabase = _sourceDatabase + _sourceSchema;
            sourceDatabaseWater = _sourceDatabaseWater + _sourceSchemaWater;
            modelPath = theModelPath;
        }

        public void callPythonScript(string toolName, string[] allParameters)
        {
            /*
            foreach (string s in allParameters)
            {
                MessageBox.Show(s);
            }*/
            string errorParameters = "";
            try
            {
                IGeoProcessor2 gp = new GeoProcessorClass();
                ESRI.ArcGIS.esriSystem.IVariantArray parameters = new ESRI.ArcGIS.esriSystem.VarArrayClass();

                foreach (string s in allParameters)
                {
                    parameters.Add(s);
                    errorParameters = errorParameters + s + "\n";
                }

                gp.SetEnvironmentValue("workspace", modelPath);
                gp.AddOutputsToMap = false;
                gp.OverwriteOutput = true;
                gp.Execute(toolName, parameters, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not execute script: " + toolName + "\n" + "Using parameters:\n" + errorParameters + "\n" + ex.ToString());
            }
        }

        public void selectLayerByLocation(string baseLayer, string overlap_type, string selectLayer, string distance)
        {
            string[] parameters = new string[4];
            parameters[0] = baseLayer;
            parameters[1] = overlap_type;
            parameters[2] = selectLayer;
            parameters[3] = distance;

            callPythonScript("SelectLayerByLocation_management", parameters);
        }

        public void makeFeatureLayer(string layerPath, string layerAlias, string whereclause)
        {
            string[] parameters = new string[3];
            parameters[0] = layerPath;
            parameters[1] = layerAlias;
            parameters[2] = whereclause;

            callPythonScript("MakeFeatureLayer_management", parameters);
        }

        public void copyFeatures(string layerAlias, string outputLocation)
        {
            string[] parameters = new string[2];
            parameters[0] = layerAlias;
            parameters[1] = outputLocation;

            callPythonScript("CopyFeatures_management", parameters);
        }

        public void intersectionsPoint(FileInfo sFile, string layer1, string layer2, string outputName)
        {
            string[] parameters = new string[5];
            parameters[0] = layer1 + " ; " + layer2;
            parameters[1] = outputName;
            parameters[2] = "";
            parameters[3] = "";
            parameters[4] = "POINT";

            callPythonScript("Intersect_analysis", parameters);
        }

        public void copyTable(string layerAlias, string outputLocation)
        {
            string[] parameters = new string[2];
            parameters[0] = layerAlias;
            parameters[1] = outputLocation;

            callPythonScript("Copy_management", parameters);
        }

        public void proximityProcedure(string distance, string modelTableName, string serverTableName, string workingTableName, string clause = "")
        {
            makeFeatureLayer(sourceDatabase + serverTableName, workingTableName, clause);
            selectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
            copyFeatures(workingTableName, modelPath + "\\" + workingTableName);
        }

        public void saveSelection(string distance, string modelTableName, string serverTableName, string workingTableName, string clause = "")
        {
            makeFeatureLayer(sourceDatabase + serverTableName, workingTableName, clause);
            selectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
            copyFeatures(workingTableName, modelPath + "\\" + workingTableName);
        }

        public void proximityProcedureWater(string distance, string modelTableName, string serverTableName, string workingTableName, string clause = "")
        {
            makeFeatureLayer(sourceDatabaseWater + serverTableName, workingTableName, clause);
            selectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
            copyFeatures(workingTableName, modelPath + "\\" + workingTableName);
        }

        public void createBufferTable(string distance, string tableName, string bufferTableName = "", string line_side = "FULL", string line_end_type = "ROUND", string dissolve_option = "NONE")
        {
            string[] parameters = new string[3];
            if (bufferTableName == "")
            {
                bufferTableName = tableName + "Buffer";
            }

            parameters[0] = tableName;
            parameters[1] = bufferTableName;
            parameters[2] = distance;

            callPythonScript("Buffer_analysis", parameters);
        }

        public void createNearTable(string distance, string modelTableName, string tableName, string alternateOutputName = "")
        {
            //I'm sticking this here because I don't want to rewrite all of this code right now just to account fo the buffers
            //if there is no distance to buffer, we cannot call the buffer procedure because it doesnt work in that case.
            //you must just call a copy proc

            if (distance == "0" || distance == "")
            {
                copyFeatures(tableName, tableName + "Buffer");
            }
            else
            {
                createBufferTable(distance, tableName);
            }

            if (alternateOutputName == "")
            {
                alternateOutputName = tableName + "Near";
            }

            string[] parameters = new string[7];
            parameters[0] = modelTableName;
            parameters[1] = tableName;
            parameters[2] = alternateOutputName;
            parameters[3] = distance;
            parameters[4] = "NO_LOCATION";
            parameters[5] = "ANGLE";
            parameters[6] = "ALL";

            callPythonScript("GenerateNearTable_analysis", parameters);
        }

        public void proximity(FileInfo sFile, string modelTableName, string distance, string sourceTableName, string outputTableName, string clause)
        {
            prepTables(sFile, modelTableName, distance, sourceTableName, outputTableName, clause);
        }

        public void translateCensus(FileInfo sFile)
        {
            string serverTableName = "ACS_2010_5YR_TABLE_S1701_POVERTY_TRACTS";
            string workingTableName = "XP_POVERTY";

            copyTable(sourceDatabase + serverTableName, modelPath + "\\" + workingTableName);
        }

        public void proximityToPressurizedWater(FileInfo sFile, string modelTableName, string distance)
        {
            proximityProcedureWater(distance, modelTableName, "pressurizedMain", "XP_PressurizedWater", "[Status] NOT IN ('Abandoned', 'Proposed')");
            createNearTable(distance, modelTableName, "XP_PressurizedWater");
        }

        public void prepTables(FileInfo sFile, string modelTableName, string distance, string tableName, string xpName, string clause)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\PipeXP\\" + pipeXPSQLiteDBName + ".sqlite';Version=3", true);
            proximityProcedure(distance, modelTableName, tableName, xpName, clause);
            createNearTable(distance, modelTableName, xpName);

            conn.Open();
            CopyFeatureClass(sFile, xpName, sFile.DirectoryName + "\\PipeXP\\" + pipeXPSQLiteDBName + ".sqlite", xpName, clause);
            //CopyTable(sFile, xpName + "Near", sFile.DirectoryName + "\\PipeXP\\"+pipeXPSQLiteDBName+".sqlite", xpName + "Near", "");
            copyTable(xpName + "Near", sFile.DirectoryName + "\\PipeXP\\" + pipeXPSQLiteDBName + ".sqlite\\" + xpName + "Near");
            conn.Close();
        }

        public void CopyFeatureClass(FileInfo sFile, string in_features, string out_path, string out_name, string where_clause = "")
        {
            try
            {
                Geoprocessor GP = new Geoprocessor();
                ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass copyTool = new ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass();
                GP.SetEnvironmentValue("OutputMFlag", "FALSE");
                GP.SetEnvironmentValue("OutputZFlag", "FALSE");
                copyTool.in_features = in_features;
                copyTool.out_path = out_path;
                copyTool.out_name = out_name;
                copyTool.where_clause = where_clause;
                GP.OverwriteOutput = true;
                GP.Execute(copyTool, null);
                copyTool = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not execute CopyFeatureClass on file:\n" + in_features + "\nTo:\n" + out_path + "\n" + out_name + "\n" + ex.ToString());
            }
        }

        public void CopyTable(FileInfo sFile, string in_features, string out_path, string out_name, string where_clause = "")
        {
            try
            {
                Geoprocessor GP = new Geoprocessor();
                ESRI.ArcGIS.ConversionTools.TableToTable copyTool = new ESRI.ArcGIS.ConversionTools.TableToTable();
                GP.SetEnvironmentValue("OutputMFlag", "FALSE");
                GP.SetEnvironmentValue("OutputZFlag", "FALSE");
                copyTool.in_rows = in_features;
                copyTool.out_path = out_path;
                copyTool.out_name = out_name;
                copyTool.where_clause = where_clause;
                GP.AddOutputsToMap = false;
                GP.OverwriteOutput = true;
                GP.Execute(copyTool, null);
                copyTool = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not execute CopyTable on file:\n" + in_features + "\nTo:\n" + out_path + "\n" + out_name + "\n" + ex.ToString());
            }
        }

        public void nqsqlite(string command, SQLiteConnection m_dbConnection)
        {
            SQLiteCommand cmd = new SQLiteCommand(command, m_dbConnection);

            cmd.ExecuteNonQuery();
        }
    }
}
