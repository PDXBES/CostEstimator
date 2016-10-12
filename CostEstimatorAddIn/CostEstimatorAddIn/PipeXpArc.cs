// <copyright file="PipeXpArc.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>PipeXpArc class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using ESRI.ArcGIS.Geoprocessing;
  using ESRI.ArcGIS.Geoprocessor;

  /// <summary>
  /// PipeXPArc class
  /// </summary>
  public class PipeXpArc
  {
    private string sourceDatabase = @"Database Connections\\egh_Public.sde\\EGH_PUBLIC.ARCMAP_ADMIN.";
    private string sourceDatabaseWater = @"Database Connections\\egh_Water.sde\\PWBWATER.ARCMAP_ADMIN.";

    // public string modelLinksLayer = "Links";
    // public string modelNodesLayer = "Nodes";
    private string pipeXPSQLiteDBName = "PipeXP";
    private string modelPath;

    private DataTable listOfXpFiles = new DataTable("listOfXpFiles");
    private DataColumn fileName = new DataColumn("fileName", System.Type.GetType("System.String"));
    private DataColumn dbLocation = new DataColumn("dbLocation", System.Type.GetType("System.String"));
    private DataColumn fileType = new DataColumn("fileType", System.Type.GetType("System.String"));
    private DataColumn alias = new DataColumn("alias", System.Type.GetType("System.String"));

    /// <summary>
    /// Initializes a new instance of the <see cref="PipeXpArc" /> class
    /// </summary>
    /// <param name="theModelPath">Path to the Emgaats model</param>
    /// <param name="sourceSchema">Name of pipe database</param>
    /// <param name="sourceSchemaWater">Name of water table</param>
    /// <param name="sourceDatabase">Collection system database</param>
    /// <param name="sourceDatabaseWater">water pipes database</param>
    public PipeXpArc(
      string theModelPath,
      string sourceSchema,
      string sourceSchemaWater,
      string sourceDatabase = @"Database Connections\\egh_Public.sde\\",
      string sourceDatabaseWater = @"Database Connections\\egh_Water.sde\\")
    {
      if (sourceSchema != string.Empty)
      {
        sourceSchema = sourceSchema + ".";
      }

      if (sourceSchemaWater != string.Empty)
      {
        sourceSchemaWater = sourceSchemaWater + ".";
      }

      this.sourceDatabase = sourceDatabase + sourceSchema;
      this.sourceDatabaseWater = sourceDatabaseWater + sourceSchemaWater;
      this.modelPath = theModelPath;
    }

    /// <summary>
    /// Calls a python script
    /// </summary>
    /// <param name="toolName">Name of the script</param>
    /// <param name="allParameters">A list of the parameters the script accepts</param>
    public void CallPythonScript(string toolName, string[] allParameters)
    {
      /*
      foreach (string s in allParameters)
      {
          MessageBox.Show(s);
      }*/
      string errorParameters = string.Empty;
      try
      {
        IGeoProcessor2 gp = new GeoProcessorClass();
        ESRI.ArcGIS.esriSystem.IVariantArray parameters = new ESRI.ArcGIS.esriSystem.VarArrayClass();

        foreach (string s in allParameters)
        {
          parameters.Add(s);
          errorParameters = errorParameters + s + "\n";
        }

        gp.SetEnvironmentValue("workspace", this.modelPath);
        gp.AddOutputsToMap = false;
        gp.OverwriteOutput = true;
        gp.Execute(toolName, parameters, null);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute script: " + toolName + "\n" + "Using parameters:\n" + errorParameters + "\n" + ex.ToString());
      }
    }

    /// <summary>
    /// Selects objects in a layer by location
    /// </summary>
    /// <param name="baseLayer">The base layer to use for comparison</param>
    /// <param name="overlap_type">The type of overlap</param>
    /// <param name="selectLayer">The layer from which to make the selection</param>
    /// <param name="distance">The distance to use for detection</param>
    public void SelectLayerByLocation(
      string baseLayer, 
      string overlap_type, 
      string selectLayer, 
      string distance)
    {
      string[] parameters = new string[4];
      parameters[0] = baseLayer;
      parameters[1] = overlap_type;
      parameters[2] = selectLayer;
      parameters[3] = distance;

      this.CallPythonScript("SelectLayerByLocation_management", parameters);
    }

    /// <summary>
    /// Makes a feature layer
    /// </summary>
    /// <param name="layerPath">Path of layer</param>
    /// <param name="layerAlias">Alias of layer</param>
    /// <param name="whereclause">Clause to limit what goes into the layer</param>
    public void MakeFeatureLayer(string layerPath, string layerAlias, string whereclause)
    {
      string[] parameters = new string[3];
      parameters[0] = layerPath;
      parameters[1] = layerAlias;
      parameters[2] = whereclause;

      this.CallPythonScript("MakeFeatureLayer_management", parameters);
    }

    /// <summary>
    /// Copies features
    /// </summary>
    /// <param name="layerAlias">Alias of layer</param>
    /// <param name="outputLocation">Where to send copies</param>
    public void CopyFeatures(string layerAlias, string outputLocation)
    {
      string[] parameters = new string[2];
      parameters[0] = layerAlias;
      parameters[1] = outputLocation;

      this.CallPythonScript("CopyFeatures_management", parameters);
    }

    /// <summary>
    /// Creates set of points of intersection between two layers
    /// </summary>
    /// <param name="selectedFile">Unknown parameter</param>
    /// <param name="layer1">First layer</param>
    /// <param name="layer2">Second layer</param>
    /// <param name="outputName">Name of output</param>
    public void IntersectionsPoint(
      FileInfo selectedFile, 
      string layer1, 
      string layer2, 
      string outputName)
    {
      string[] parameters = new string[5];
      parameters[0] = layer1 + " ; " + layer2;
      parameters[1] = outputName;
      parameters[2] = string.Empty;
      parameters[3] = string.Empty;
      parameters[4] = "POINT";

      this.CallPythonScript("Intersect_analysis", parameters);
    }

    /// <summary>
    /// Copies a table
    /// </summary>
    /// <param name="layerAlias">Alias of layer</param>
    /// <param name="outputLocation">Location of copies</param>
    public void CopyTable(string layerAlias, string outputLocation)
    {
      string[] parameters = new string[2];
      parameters[0] = layerAlias;
      parameters[1] = outputLocation;

      this.CallPythonScript("Copy_management", parameters);
    }

    /// <summary>
    /// Makes a Proximity comparison
    /// </summary>
    /// <param name="distance">Distance to use for checking Proximity</param>
    /// <param name="modelTableName">The model table name</param>
    /// <param name="serverTableName">The server table name</param>
    /// <param name="workingTableName">The working table name</param>
    /// <param name="clause">Filter clause</param>
    public void ProximityProcedure(
      string distance, 
      string modelTableName, 
      string serverTableName, 
      string workingTableName, 
      string clause = "")
    {
      this.MakeFeatureLayer(this.sourceDatabase + serverTableName, workingTableName, clause);
      this.SelectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
      this.CopyFeatures(workingTableName, this.modelPath + "\\" + workingTableName);
    }

    /// <summary>
    /// Saves a selection
    /// </summary>
    /// <param name="distance">Distance to use for checking Proximity</param>
    /// <param name="modelTableName">The model table name</param>
    /// <param name="serverTableName">The server table name</param>
    /// <param name="workingTableName">The working table name</param>
    /// <param name="clause">Filter clause</param>
    public void SaveSelection(
      string distance, 
      string modelTableName, 
      string serverTableName, 
      string workingTableName, 
      string clause = "")
    {
      this.MakeFeatureLayer(this.sourceDatabase + serverTableName, workingTableName, clause);
      this.SelectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
      this.CopyFeatures(workingTableName, this.modelPath + "\\" + workingTableName);
    }

    /// <summary>
    /// Makes a Proximity comparison with water
    /// </summary>
    /// <param name="distance">Distance to use for checking Proximity</param>
    /// <param name="modelTableName">The model table name</param>
    /// <param name="serverTableName">The server table name</param>
    /// <param name="workingTableName">The working table name</param>
    /// <param name="clause">Filter clause</param>
    public void ProximityProcedureWater(
      string distance, 
      string modelTableName, 
      string serverTableName, 
      string workingTableName, 
      string clause = "")
    {
      this.MakeFeatureLayer(this.sourceDatabaseWater + serverTableName, workingTableName, clause);
      this.SelectLayerByLocation(workingTableName, "WITHIN_A_DISTANCE", modelTableName, distance);
      this.CopyFeatures(workingTableName, this.modelPath + "\\" + workingTableName);
    }

    /// <summary>
    /// Creates a buffer table
    /// </summary>
    /// <param name="distance">Buffer distance</param>
    /// <param name="tableName">Name of table</param>
    /// <param name="bufferTableName">Name of resulting buffer</param>
    /// <param name="line_side">Line side parameter</param>
    /// <param name="line_end_type">Buffer shape at line end</param>
    /// <param name="dissolve_option">Whether to dissolve all buffers into one object</param>
    public void CreateBufferTable(
      string distance, 
      string tableName, 
      string bufferTableName = "", 
      string line_side = "FULL", 
      string line_end_type = "ROUND", 
      string dissolve_option = "NONE")
    {
      string[] parameters = new string[3];
      if (bufferTableName == string.Empty)
      {
        bufferTableName = tableName + "Buffer";
      }

      parameters[0] = tableName;
      parameters[1] = bufferTableName;
      parameters[2] = distance;

      this.CallPythonScript("Buffer_analysis", parameters);
    }

    /// <summary>
    /// Create near table
    /// </summary>
    /// <param name="distance">Distance to use</param>
    /// <param name="modelTableName">Model table name</param>
    /// <param name="tableName">Table name</param>
    /// <param name="alternateOutputName">Alternate output name</param>
    public void CreateNearTable(
      string distance, 
      string modelTableName, 
      string tableName, 
      string alternateOutputName = "")
    {
      // I'm sticking this here because I don't want to rewrite all of this code right now just to account fo the buffers
      // if there is no distance to buffer, we cannot call the buffer procedure because it doesnt work in that case.
      // you must just call a copy proc
      if (distance == "0" || distance == string.Empty)
      {
        this.CopyFeatures(tableName, tableName + "Buffer");
      }
      else
      {
        this.CreateBufferTable(distance, tableName);
      }

      if (alternateOutputName == string.Empty)
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

      this.CallPythonScript("GenerateNearTable_analysis", parameters);
    }

    /// <summary>
    /// Runs proximities
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    /// <param name="modelTableName">Model table name</param>
    /// <param name="distance">Distance to use for Proximity check</param>
    /// <param name="sourceTableName">Source table name</param>
    /// <param name="outputTableName">Table name of result</param>
    /// <param name="clause">Filter clause</param>
    public void Proximity(
      FileInfo selectedFile, 
      string modelTableName, 
      string distance, 
      string sourceTableName, 
      string outputTableName, 
      string clause)
    {
      this.PrepTables(selectedFile, modelTableName, distance, sourceTableName, outputTableName, clause);
    }

    /// <summary>
    /// Translates census data
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    public void TranslateCensus(FileInfo selectedFile)
    {
      string serverTableName = "ACS_2010_5YR_TABLE_S1701_POVERTY_TRACTS";
      string workingTableName = "XP_POVERTY";

      this.CopyTable(this.sourceDatabase + serverTableName, this.modelPath + "\\" + workingTableName);
    }

    /// <summary>
    /// Detects proximity to pressurized water pipe
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    /// <param name="modelTableName">Model table name</param>
    /// <param name="distance">Distance to detect water pipe</param>
    public void ProximityToPressurizedWater(
      FileInfo selectedFile, 
      string modelTableName, 
      string distance)
    {
      this.ProximityProcedureWater(distance, modelTableName, "pressurizedMain", "XP_PressurizedWater", "[Status] NOT IN ('Abandoned', 'Proposed')");
      this.CreateNearTable(distance, modelTableName, "XP_PressurizedWater");
    }

    /// <summary>
    /// Prepare tables
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    /// <param name="modelTableName">Model table name</param>
    /// <param name="distance">Distance to use for proximity</param>
    /// <param name="tableName">Name of table</param>
    /// <param name="xpName">Unknown parameter</param>
    /// <param name="clause">Filter clause</param>
    public void PrepTables(
      FileInfo selectedFile, 
      string modelTableName, 
      string distance, 
      string tableName, 
      string xpName, 
      string clause)
    {
      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + selectedFile.DirectoryName + "\\PipeXP\\" + this.pipeXPSQLiteDBName + ".sqlite';Version=3", true);
      this.ProximityProcedure(distance, modelTableName, tableName, xpName, clause);
      this.CreateNearTable(distance, modelTableName, xpName);

      conn.Open();
      this.CopyFeatureClass(selectedFile, xpName, selectedFile.DirectoryName + "\\PipeXP\\" + this.pipeXPSQLiteDBName + ".sqlite", xpName, clause);

      // CopyTable(selectedFile, xpName + "Near", selectedFile.DirectoryName + "\\PipeXP\\"+pipeXPSQLiteDBName+".sqlite", xpName + "Near", "");
      this.CopyTable(xpName + "Near", selectedFile.DirectoryName + "\\PipeXP\\" + this.pipeXPSQLiteDBName + ".sqlite\\" + xpName + "Near");
      conn.Close();
    }

    /// <summary>
    /// Copy feature class
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    /// <param name="inFeatures">Features to copy</param>
    /// <param name="outPath">Location of database to copy into</param>
    /// <param name="outName">Name of table to copy into</param>
    /// <param name="whereClause">Filter clause</param>
    public void CopyFeatureClass(FileInfo selectedFile, string inFeatures, string outPath, string outName, string whereClause = "")
    {
      try
      {
        Geoprocessor gp = new Geoprocessor();
        ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass copyTool = new ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass();
        gp.SetEnvironmentValue("OutputMFlag", "FALSE");
        gp.SetEnvironmentValue("OutputZFlag", "FALSE");
        copyTool.in_features = inFeatures;
        copyTool.out_path = outPath;
        copyTool.out_name = outName;
        copyTool.where_clause = whereClause;
        gp.OverwriteOutput = true;
        gp.Execute(copyTool, null);
        copyTool = null;
        GC.Collect();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute CopyFeatureClass on file:\n" + inFeatures + "\nTo:\n" + outPath + "\n" + outName + "\n" + ex.ToString());
      }
    }

    /// <summary>
    /// Copy table
    /// </summary>
    /// <param name="selectedFile">Selected file</param>
    /// <param name="inFeatures">Features to copy</param>
    /// <param name="outPath">Location of database to copy into</param>
    /// <param name="outName">Name of table to copy into</param>
    /// <param name="whereClause">Filter clause</param>
    public void CopyTable(
      FileInfo selectedFile, 
      string inFeatures, 
      string outPath, 
      string outName, 
      string whereClause = "")
    {
      try
      {
        Geoprocessor gp = new Geoprocessor();
        ESRI.ArcGIS.ConversionTools.TableToTable copyTool = new ESRI.ArcGIS.ConversionTools.TableToTable();
        gp.SetEnvironmentValue("OutputMFlag", "FALSE");
        gp.SetEnvironmentValue("OutputZFlag", "FALSE");
        copyTool.in_rows = inFeatures;
        copyTool.out_path = outPath;
        copyTool.out_name = outName;
        copyTool.where_clause = whereClause;
        gp.AddOutputsToMap = false;
        gp.OverwriteOutput = true;
        gp.Execute(copyTool, null);
        copyTool = null;
        GC.Collect();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute CopyTable on file:\n" + inFeatures + "\nTo:\n" + outPath + "\n" + outName + "\n" + ex.ToString());
      }
    }

    /// <summary>
    /// No query command
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="dbConnection">SQLite connection object</param>
    public void NqSqlite(string command, SQLiteConnection dbConnection)
    {
      SQLiteCommand cmd = new SQLiteCommand(command, dbConnection);

      cmd.ExecuteNonQuery();
    }
  }
}
