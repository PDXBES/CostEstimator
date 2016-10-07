using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using System.Reflection;

namespace CostEstimatorAddIn
{
  public class TranslateArcToSQLite
  {
    FileInfo ArcFile;
    string emgaatsLinksTableName = "Links";
    string emgaatsNodesTableName = "Nodes";
    string pipeXPInputTableName = "REHABSegments";

    public void TranslateEmgaatsModel(FileInfo sFile)
    {
      ArcFile = sFile;

      //create a cost estimates folder in the directory of sFile
      try
      {
        DirectoryInfo dir = new DirectoryInfo(sFile.DirectoryName + "\\CostEstimates");
        dir.Create();

        createSpatialSQLiteFile(sFile.DirectoryName + "\\CostEstimates");
        string[] parameters = new string[3];
        parameters[0] = sFile.DirectoryName + "\\EmgaatsModel.gdb\\Network\\Links";
        parameters[1] = sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite";
        parameters[2] = "Links";
        CopyFeatureClass(parameters[0], parameters[1], parameters[2]);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        parameters[0] = sFile.DirectoryName + "\\EmgaatsModel.gdb\\Network\\Nodes";
        parameters[1] = sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite";
        parameters[2] = "Nodes";
        CopyFeatureClass(parameters[0], parameters[1], parameters[2]);
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not create cost estimate database.\n" + ex.ToString());
      }

    }

    public void createDMEPipesFromEmgaatsTables(FileInfo sFile)
    {
      ArcFile = sFile;

      //using the information from the 'translateEmgaatsModel' function, combine links and nodes to 
      //form a table called 'REHABSegments'.  This will be the input table to the cost estimator process.
      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      conn.Open();
      //Remember we need to enable spatial queries
      SQLiteBasicStrings.enableSpatial(conn);

      string shapeType = "";
      int srid = 2913;

      //Create the new REHABSegments table 
      try
      {
        nqsqlite(SQLiteBasicStrings.createREHABSegmentsTable(), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem creating new REHABSegments table: " + ex.ToString());
        return;
      }
      //get the Shape type (integer for linestirng, point, polygon, multipolygon, etc)
      try
      {
        shapeType = (string)iqsqlite(SQLiteBasicStrings.getShapeType(emgaatsLinksTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem reading shape type: " + ex.ToString());
        return;
      }
      //add the new shape field to the new table
      try
      {
        nqsqlite(SQLiteBasicStrings.addGeometryColumn(pipeXPInputTableName, shapeType, srid), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem creating shape type: " + ex.ToString());
        return;
      }
      //now of course it is time to populate the REHABSegments table.
      //First we will add the info we know from the links table
      try
      {
        nqsqlite(SQLiteBasicStrings.populateREHABSegmentsFromLinksTable(emgaatsLinksTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments: " + ex.ToString());
        return;
      }

      //First we will add the info we know from the nodes table
      try
      {
        nqsqlite(SQLiteBasicStrings.populateREHABSegmentsNodes(emgaatsNodesTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments: " + ex.ToString());
        return;
      }
      //next update the depths fields using a join on links and nodes
      try
      {
        nqsqlite(SQLiteBasicStrings.populateREHABSegmentsFromLinksNodesJoin(emgaatsLinksTableName, emgaatsNodesTableName), conn);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Problem completing population of REHABSegments using join on links and nodes: " + ex.ToString());
        return;
      }
      //Do i hav a procedure for creating tables?  I'm pretty sure I do, and you can find that in the ArcToSQLite program.

      conn.Close();

      //dont forget to add the spatial index!
      string[] parameters = new string[1];
      parameters[0] = sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite\\main.REHABSegments";
      //callPythonScript("PythonAddSpatialIndex", parameters);
    }

    public void createSpatialSQLiteFile(string directory)
    {
      SQLiteConnection conn = new SQLiteConnection("Data Source ='" + directory + "\\EmgaatsTranslation.sqlite';Version=3;", true);
      conn.Open();
      SQLiteBasicStrings.enableSpatial(conn);
      nqsqlite(SQLiteBasicStrings.createArcSpatialEnvironment(), conn);
      conn.Close();
    }

    public void nqsqlite(string command, SQLiteConnection m_dbConnection)
    {
      SQLiteCommand cmd = new SQLiteCommand(command, m_dbConnection);

      cmd.ExecuteNonQuery();
    }

    public object iqsqlite(string command, SQLiteConnection m_dbConnection)
    {
      try
      {
        SQLiteCommand cmd = new SQLiteCommand(command, m_dbConnection);
        return cmd.ExecuteScalar();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute query:\n" + ex.ToString());
        return null;
      }
    }

    public void callPythonScript(string toolName, string[] allParameters)
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

    public void CopyFeatureClass(string in_features, string out_path, string out_name, string where_clause = "")
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
        GP.Execute(copyTool, null);

      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute CopyFeatureClass:\n" + in_features + "\n" + out_path + "\n" + out_name + "\n" + where_clause + "\n" + ex.ToString());
      }
    }
  }
}
