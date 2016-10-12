// <copyright file="TranslateArcToSQLite.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>TranslateArcToSQLite class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using ESRI.ArcGIS.Geoprocessing;
  using ESRI.ArcGIS.Geoprocessor;

  /// <summary>
  /// Class for translating Arc data to SQLite
  /// </summary>
  public class TranslateArcToSQLite
  {
    private FileInfo arcFile;
    private string emgaatsLinksTableName = "Links";
    private string emgaatsNodesTableName = "Nodes";
    private string pipeXPInputTableName = "REHABSegments";

    /// <summary>
    /// Translates an EMGAATS model
    /// </summary>
    /// <param name="selectedFile">Model directory</param>
    public void TranslateEmgaatsModel(FileInfo selectedFile)
    {
      this.arcFile = selectedFile;

      // create a cost estimates folder in the directory of selectedFile
      try
      {
        DirectoryInfo dir = new DirectoryInfo(selectedFile.DirectoryName + "\\CostEstimates");
        dir.Create();

        this.CreateSpatialSQLiteFileForEmgaatsTranslation(selectedFile.DirectoryName + "\\CostEstimates");
        string[] parameters = new string[3];
        parameters[0] = selectedFile.DirectoryName + "\\EmgaatsModel.gdb\\Network\\Links";
        parameters[1] = selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite";
        parameters[2] = "Links";
        this.CopyFeatureClass(parameters[0], parameters[1], parameters[2]);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        parameters[0] = selectedFile.DirectoryName + "\\EmgaatsModel.gdb\\Network\\Nodes";
        parameters[1] = selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite";
        parameters[2] = "Nodes";
        this.CopyFeatureClass(parameters[0], parameters[1], parameters[2]);
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not create cost estimate database.\n" + ex.ToString());
      }
    }

    /// <summary>
    /// Creates tables with DME structure and EMGAATS data
    /// </summary>
    /// <param name="selectedFile">Model directory</param>
    public void CreateDmePipesFromEmgaatsTables(FileInfo selectedFile)
    {
      this.arcFile = selectedFile;

      // using the information from the 'translateEmgaatsModel' function, combine links and nodes to 
      // form a table called 'REHABSegments'.  This will be the input table to the cost estimator process.
      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      conn.Open();

      // Remember we need to enable spatial queries
      SQLiteBasicStrings.EnableSpatial(conn);

      string shapeType = string.Empty;
      int srid = 2913;

      // Create the new REHABSegments table 
      try
      {
        this.NqSqlite(SQLiteBasicStrings.CreateRehabSegmentsTable(), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem creating new REHABSegments table: " + ex.ToString());
        return;
      }

      // get the Shape type (integer for linestirng, point, polygon, multipolygon, etc)
      try
      {
        shapeType = (string)this.IqSqlite(SQLiteBasicStrings.GetShapeType(this.emgaatsLinksTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem reading shape type: " + ex.ToString());
        return;
      }

      // add the new shape field to the new table
      try
      {
        this.NqSqlite(SQLiteBasicStrings.AddGeometryColumn(this.pipeXPInputTableName, shapeType, srid), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem creating shape type: " + ex.ToString());
        return;
      }

      // now of course it is time to populate the REHABSegments table.
      // First we will add the info we know from the links table
      try
      {
        this.NqSqlite(SQLiteBasicStrings.PopulateRehabSegmentsFromLinksTable(this.emgaatsLinksTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments: " + ex.ToString());
        return;
      }

      // First we will add the info we know from the nodes table
      try
      {
        this.NqSqlite(SQLiteBasicStrings.PopulateRehabSegmentsNodes(this.emgaatsNodesTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments: " + ex.ToString());
        return;
      }

      // next update the depths fields using a join on links and nodes
      try
      {
        this.NqSqlite(SQLiteBasicStrings.PopulateRehabSegmentsFromLinksNodesJoin(this.emgaatsLinksTableName, this.emgaatsNodesTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments using join on links and nodes: " + ex.ToString());
        return;
      }

      // Do I have a procedure for creating tables?  I'm pretty sure I do, and you can find that in the ArcToSQLite program.
      conn.Close();

      // don't forget to add the spatial index!
      string[] parameters = new string[1];
      parameters[0] = selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite\\main.REHABSegments";

      // CallPythonScript("PythonAddSpatialIndex", parameters);
    }

    /// <summary>
    /// Creates a spatial SQLite database for EMGAATS translation
    /// </summary>
    /// <param name="directory">Directory to place database</param>
    public void CreateSpatialSQLiteFileForEmgaatsTranslation(string directory)
    {
      SQLiteConnection conn = new SQLiteConnection("Data Source ='" + directory + "\\EmgaatsTranslation.sqlite';Version=3;", true);
      conn.Open();
      SQLiteBasicStrings.EnableSpatial(conn);
      this.NqSqlite(SQLiteBasicStrings.CreateArcSpatialEnvironment(), conn);
      conn.Close();
    }

    /// <summary>
    /// Runs a no-query command
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="dbConnection">SQLite connection</param>
    public void NqSqlite(string command, SQLiteConnection dbConnection)
    {
      SQLiteCommand cmd = new SQLiteCommand(command, dbConnection);

      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Runs a scalar command
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="dbConnection">SQLite connection</param>
    /// <returns>An object representing the return value of the command</returns>
    public object IqSqlite(string command, SQLiteConnection dbConnection)
    {
      try
      {
        SQLiteCommand cmd = new SQLiteCommand(command, dbConnection);
        return cmd.ExecuteScalar();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute query:\n" + ex.ToString());
        return null;
      }
    }

    /// <summary>
    /// Calls a python script
    /// </summary>
    /// <param name="toolName">Path or name of the tool</param>
    /// <param name="allParameters">List of parameters accepted by the tool</param>
    public void CallPythonScript(string toolName, string[] allParameters)
    {
      try
      {
        IGeoProcessor2 gp = new GeoProcessorClass();
        ESRI.ArcGIS.esriSystem.IVariantArray parameters = new ESRI.ArcGIS.esriSystem.VarArrayClass();
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        foreach (string s in allParameters)
        {
          parameters.Add(s);
        }

        gp.Execute(toolName, parameters, null);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute script:\n" + toolName + "\n" + ex.ToString());
      }
    }

    /// <summary>
    /// Copies the feature classes from one database to another
    /// </summary>
    /// <param name="inFeatures">Feature class to copy</param>
    /// <param name="outPath">Where the feature class should be copied to</param>
    /// <param name="outName">The name of the destination feature class</param>
    /// <param name="whereClause">Clause to limit which features to copy</param>
    public void CopyFeatureClass(string inFeatures, string outPath, string outName, string whereClause = "")
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
        gp.Execute(copyTool, null);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute CopyFeatureClass:\n" + inFeatures + "\n" + outPath + "\n" + outName + "\n" + whereClause + "\n" + ex.ToString());
      }
    }
  }
}
