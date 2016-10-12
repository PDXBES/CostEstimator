// <copyright file="CostEstimatorClass.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>CostEstimator class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Forms;

  /// <summary>
  /// Cost Estimator Add-In
  /// </summary>
  public class CostEstimatorClass
  {
    /// <summary>
    /// Stores file info for determining which model directory to perform a cost estimate on
    /// </summary>
    private FileInfo arcFile;

    /// <summary>
    /// Default name for input to PipeXP routines
    /// </summary>
    private string pipeXPInputTableName = "REHABSegments";

    /// <summary>
    /// Prepares a SQLite table for reading
    /// </summary>
    /// <param name="connectionString">Connection string to database</param>
    /// <param name="selectQuery">Select query for reading in table</param>
    /// <returns>DataTable representing query</returns>
    public static DataTable ReadTable(string connectionString, string selectQuery)
    {
      var returnValue = new DataTable();

      var conn = new SQLiteConnection(connectionString);

      try
      {
        conn.Open();
        var command = new SQLiteCommand(selectQuery, conn);

        using (var adapter = new SQLiteDataAdapter(command))
        {
          adapter.Fill(returnValue);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw ex;
      }
      finally
      {
        if (conn.State == ConnectionState.Open)
        {
          conn.Close();
        }
      }

      return returnValue;
    }

    /// <summary>
    /// Writes cost estimate results to file
    /// </summary>
    /// <param name="dataSource">Table to write</param>
    /// <param name="fileOutputPath">Path to file that will contain output</param>
    /// <param name="firstRowIsColumnHeader">Determines whether first row has column headings</param>
    /// <param name="separator">Separator character to use</param>
    public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = true, string separator = ",")
    {
      var sw = new StreamWriter(fileOutputPath, false);

      int icolcount = dataSource.Columns.Count;

      if (firstRowIsColumnHeader)
      {
        for (int i = 0; i < icolcount; i++)
        {
          if ((string)dataSource.Columns[i].ColumnName == "ID")
          {
            sw.Write("id");
          }
          else
          {
            sw.Write(dataSource.Columns[i]);
          }

          if (i < icolcount - 1)
          {
            sw.Write(separator);
          }
        }

        sw.Write(sw.NewLine);
      }

      foreach (DataRow drow in dataSource.Rows)
      {
        for (int i = 0; i < icolcount; i++)
        {
          if (!Convert.IsDBNull(drow[i]))
          {
            sw.Write(drow[i].ToString());
          }

          if (i < icolcount - 1)
          {
            sw.Write(separator);
          }
        }

        sw.Write(sw.NewLine);
      }

      sw.Close();
    }

    /// <summary>
    /// Performs pipe conflict analysis on model directory
    /// </summary>
    /// <param name="selectedFile">Name of file within model directory</param>
    public void PerformPipeXp(FileInfo selectedFile)
    {
      this.arcFile = selectedFile;

      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      conn.Open();

      // Remember we need to enable spatial queries
      SQLiteBasicStrings.EnableSpatial(conn);
      this.NqSqlite(SQLiteBasicStrings.AttachDatabase(@"\\besfile1\ASM_Apps\Apps\CostEstimator\PullTables\PullTables_PipeXP.sqlite", "PullTables"), conn);

      // Now that we have loaded our database, which can be found in inputDatabase as the path and sqliteConnectionString as the connection string,
      // we get to perform our first queries on that data
      // Of course to start, it looks like the first thing we do is transfer that data to a new table, GOLEM_PIPEXP.
      // Lets change the name of that to AMStudio_PIPEXP
      // Prep pipexp table
      this.NqSqlite(AmStudioPipeXpQueries.PrepPipeXp(), conn);

      // Then we do our first insert
      this.NqSqlite(AmStudioPipeXpQueries.TransferBase(250.0), conn);

      // Proximity to hardAreas
      this.NqSqlite(AmStudioPipeXpQueries.ProximityToHardAreas(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ProximityToHydrantsPdx(25.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ProximityToBuildings(10), conn);
      this.NqSqlite(AmStudioPipeXpQueries.SetDepthAndSlopes(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsUIC(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsMS4(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsEMT(50.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsFire(250.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsPolice(250.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsHospital(250.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsSchool(250.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsECSI(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsResidential(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsPZone(12.5), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsCZone(12.5), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsGas(10.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsFiber(10.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsLrt(25.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsRail(25.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsIntersections(30.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsStreetType(30.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsXStreet(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsSewer(12.0), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ProximityToPoverty(), conn);
      this.NqSqlite(AmStudioPipeXpQueries.ResultsWater(12), conn);
      this.NqSqlite(AmStudioPipeXpQueries.CountLaterals(), conn);
      conn.Close();
    }

    /// <summary>
    /// Performs cost estimation
    /// </summary>
    /// <param name="selectedFile">Name of file within model directory</param>
    public void PerformCostEstimates(FileInfo selectedFile)
    {
      this.arcFile = selectedFile;
      double relocationCostPerInchDiameter = 7.9126; // dollars
      double relocationCostBase = 74.093; // dollars
      double utilityCrossingCost = 5000; // dollars
      double hazardousMaterialsCost = 50; // dollars
      double envMitigationCost = 150000; // dollars.  Should this really be 150,000?
      double envMitigationWidth = 25; // feet
      double asphaltRemovalCost = 8.12; // dollars
      double excessAsphaltWidth = 1; // feet
      double asphaltBaseCourseDepth = 0.6666667; // feet
      double asphaltTrenchPatchBaseCourseCost = 29.52; // dollars
      double eightInchPatchCost = 55.77;  // dollars
      double sixInchPatchCost = 44.27; // dollars
      double fourInchPatchCost = 28.62; // dollars
      double pipeZoneDepthAdditionalInches = 18; // inches
      double fillAbovePipeZoneCost = 27.3; // dollars
      double pipeZoneBackfillCost = 70.55; // dollars
      double sawcutPavementLength = 4; // inches?
      double sawcutPavementUnitCost = 4.21; // dollars
      double excavationVolumeFactor = 1.2; // unitless
      double truckHaulSpoilsUnitCost = 4.72; // dollars
      double shoringSquareFeetPerFoot = 2;
      double shoringCostPerSquareFoot = 2.57; // dollars
      double minShoringDepth = 18; // feet
      string material = string.Empty;
      double workingHoursPerDay = 8; // hours
      double excavationDuration = 140; // cubic yards per day
      double paveDuration = 250; // feet per day
      double utilityCrossingDuration = 0.5; // working days
      double smallBoreJackDiameter = 24; // inches
      double largeBoreJackDiameter = 60; // inches
      double fastBoreRate = 100; // feet/day
      double slowBoreRate = 50; // feet/day
      double boreJackCasingAndGroutingDays = 2; // days
      double cippRepairDays = 3; // days
      double shallowSpotDepthCutoff = 20; // feet
      double shallowSpotRepairTime = 4; // hours
      double deepSpotRepairTime = 8; // hours
      double streetTypeStreetTrafficControlMobilization = 1; // hours 
      double streetTypeArterialTrafficControlMobilization = 2; // hours
      double streetTypeMajorArterialTrafficControlMobilization = 3; // hours
      double shallowTrenchDepthCutoff = 20; // feet
      double smallMainlineBypassCutoff = 15; // inches
      double manholeBuildRate = 10; // feet per day
      double lateralTrenchWidth = 4; // feet
      double lateralShoringLength = 10; // feet
      double boreJackArea = 460; // square feet
      double boreJackDepth = 25; // feet
      double fractionalFlow = 0.2; // things per thing
      double kn = 1.486;
      double manningsN = 0.013; // general assumption
      double assumedSlope = 0.005; // slope assumed for negative, null, or 0 slope pipes
      double streetTypeStreetTrafficControlCost = 500; // dollars
      double streetTypeArterialTrafficControlCost = 1000; // dollars 
      double streetTypeMajorArterialTrafficControlCost = 3000; // dollars
      double streetTypeFreewayTrafficCost = 0; // dollars
      double boringJackingCost = 566.95; // dollar bills
      double baseENR = 8090; // unitless
      double jackingENR = 9500; // unitless
      double difficultAreaFactor = 1;
      double currentENR = 9835; // unitless
      double generalConditionsFactor = 0.1; // fraction
      double wasteAllowanceFactor = 0.05; // fraction
      double contingencyFactor = 0.25; // fraction
      double constructionManagementInspectionTestingFactor = 0.15; // fraction
      double designFactor = 0.2; // fraction
      double publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor = 0.03; // fraction
      double startupCloseoutFactor = 0.1; // fraction
      string csvPath = selectedFile.DirectoryName + "\\CostEstimates\\CostEstimates.csv";
      double daysForWholePipeLinerConstruction = 3; // days

      // make sure we can call the Power function
      SQLiteFunction.RegisterFunction(typeof(Power));

      // SQLiteConnection conn = new SQLiteConnection("Data Source = '" + selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      // conn.Open();

      // Create cost estimator database
      File.Delete(selectedFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite");
      SQLiteConnection costEstimateConnection = new SQLiteConnection("Data Source='" + selectedFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite';Version=3;", true);
      costEstimateConnection.Open();

      // transfer PipXP table and REHABSegments to CostEstimates
      this.NqSqlite(SQLiteBasicStrings.AttachDatabase(selectedFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite", "PipeXPResults"), costEstimateConnection);
      this.NqSqlite("CREATE TABLE XPData AS SELECT * FROM PipeXPResults.AMStudio_PIPEXP;", costEstimateConnection);
      this.NqSqlite("CREATE TABLE RehabSegments AS SELECT * FROM PipeXPResults.REHABSegments;", costEstimateConnection);

      // Drop attached database
      this.NqSqlite("DETACH DATABASE 'PipeXPResults';", costEstimateConnection);

      // Prep Costestimator table
      this.NqSqlite(AmStudioCostEstimatorQueries.PrepCostEstimator(), costEstimateConnection);

      // This source needs to change, maybe somewhere on the network
      this.NqSqlite(SQLiteBasicStrings.AttachDatabase(@"\\besfile1\ASM_Apps\Apps\CostEstimator\PullTables\PullTables_CostEstimator.sqlite", "CostEstimator"), costEstimateConnection);

      // Then we do our first insert
      this.NqSqlite(AmStudioCostEstimatorQueries.TransferBase(), costEstimateConnection);

      // Proximity to hardAreas
      this.NqSqlite(AmStudioCostEstimatorQueries.SetOutsideDiameter(), costEstimateConnection);

      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrenchBaseWidth(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetExcavationVolume(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetWaterRelocation(relocationCostPerInchDiameter, relocationCostBase), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetCrossingRelocation(utilityCrossingCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetHazardousMaterials(hazardousMaterialsCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetEnvironmentalMitigation(envMitigationCost, envMitigationWidth), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetAsphaltRemoval(asphaltRemovalCost, excessAsphaltWidth), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetAsphaltBaseCourse(asphaltBaseCourseDepth, excessAsphaltWidth, asphaltTrenchPatchBaseCourseCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrenchPatch(excessAsphaltWidth, eightInchPatchCost, sixInchPatchCost, fourInchPatchCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetFillAbovePipeZone(asphaltBaseCourseDepth, pipeZoneDepthAdditionalInches, fillAbovePipeZoneCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetPipeZoneBackfill(pipeZoneDepthAdditionalInches, pipeZoneBackfillCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetAsphaltSawCutting(sawcutPavementLength, sawcutPavementUnitCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrenchExcavation(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTruckHaul(excavationVolumeFactor, truckHaulSpoilsUnitCost), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrenchShoring(shoringSquareFeetPerFoot, shoringCostPerSquareFoot, minShoringDepth), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetPipeMaterialBaseCostPerFoot(material), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetPipeDepthDifficultyFactor(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetPipeMaterial(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeSize(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeBaseCost(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeCostPerFootBeyondMinimum(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeRimFrameCost(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeDepthFactor(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManhole(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBaseOpenCutRepairTime(workingHoursPerDay, excavationDuration, paveDuration, utilityCrossingDuration), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBasePipeBurstRepairTime(workingHoursPerDay), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBaseBoreJackRepairTime(smallBoreJackDiameter, largeBoreJackDiameter, fastBoreRate, slowBoreRate, workingHoursPerDay, boreJackCasingAndGroutingDays), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBaseCippRepairTime(workingHoursPerDay, cippRepairDays), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBaseSpotRepairTime(shallowSpotDepthCutoff, shallowSpotRepairTime, deepSpotRepairTime), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrafficControlMobilization(streetTypeStreetTrafficControlMobilization, streetTypeArterialTrafficControlMobilization, streetTypeMajorArterialTrafficControlMobilization), costEstimateConnection);

      this.NqSqlite(AmStudioCostEstimatorQueries.SetMainlineBypassMobilization(shallowTrenchDepthCutoff, smallMainlineBypassCutoff, workingHoursPerDay), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetManholeReplacement(manholeBuildRate, workingHoursPerDay), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLateralBypass(lateralTrenchWidth, lateralShoringLength, excavationDuration, paveDuration), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBoreJackPitExcavation(boreJackArea, excavationDuration), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetOpenCutConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBjMicroTConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetCippConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetSpOnlyConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.RemoveOpenCutOptions(boreJackDepth), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.RemoveBoreJackOptions(boreJackDepth), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetNonMobilizationConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetMobilizationConstructionDuration(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBypassPumping(fractionalFlow, kn, manningsN, assumedSlope, workingHoursPerDay), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetTrafficControl(streetTypeStreetTrafficControlCost, streetTypeArterialTrafficControlCost, streetTypeMajorArterialTrafficControlCost, streetTypeFreewayTrafficCost, workingHoursPerDay), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetBoringJacking(boringJackingCost, baseENR, jackingENR), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetDifficultArea(difficultAreaFactor), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLaterals(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.DirectConstructionCost(currentENR, baseENR), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.StandardPipeFactorCosts(generalConditionsFactor, wasteAllowanceFactor), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.ContingencyCost(contingencyFactor), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetCapitalCost(constructionManagementInspectionTestingFactor, designFactor, publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor, startupCloseoutFactor), costEstimateConnection);

      // Liners
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerTrafficControl(streetTypeStreetTrafficControlCost, streetTypeArterialTrafficControlCost, streetTypeMajorArterialTrafficControlCost, streetTypeFreewayTrafficCost, daysForWholePipeLinerConstruction), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerBypassPumping(daysForWholePipeLinerConstruction), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerBuildDuration(daysForWholePipeLinerConstruction), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerLaterals(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerPipeMaterial(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerTvCleaning(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.SetLinerManhole(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.LinerDirectConstructionCost(currentENR, baseENR), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.LinerMobilizationTimes(), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.LinerStandardPipeFactorCost(generalConditionsFactor, wasteAllowanceFactor), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.LinerContingencyCost(contingencyFactor), costEstimateConnection);
      this.NqSqlite(AmStudioCostEstimatorQueries.LinerCapitalCost(constructionManagementInspectionTestingFactor, designFactor, publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor, startupCloseoutFactor), costEstimateConnection);

      this.NqSqlite(AmStudioCostEstimatorQueries.BaseTimesAndMobilizationTimes(workingHoursPerDay, daysForWholePipeLinerConstruction), costEstimateConnection);

      costEstimateConnection.Close();

      var selectQuery = "select * from AMStudio_CapitalCostsMobilizationRatesAndTimes;";

      var table = ReadTable(costEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, selectedFile.DirectoryName + "\\CostEstimates\\CapitalCostsMobilizationRatesAndTimes.csv", true, ",");

      selectQuery = "select * from AMStudio_PIPEDETAILS;";

      table = ReadTable(costEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, selectedFile.DirectoryName + "\\CostEstimates\\COSTEST_PIPEDETAILS.csv", true, ",");

      selectQuery = "SELECT * FROM XPData;"; // "select * from AMStudio_PIPEXP;";

      table = ReadTable(costEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, selectedFile.DirectoryName + "\\CostEstimates\\COSTEST_PIPEXP.csv", true, ",");
    }

    /// <summary>
    /// Non-query SQLite command
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="connection">Connection to use</param>
    public void NqSqlite(string command, SQLiteConnection connection)
    {
      SQLiteCommand cmd = new SQLiteCommand(command, connection);

      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Scalar SQLite command
    /// </summary>
    /// <param name="command">Command string</param>
    /// <param name="connection">Connection to use</param>
    /// <returns>Object representing scalar result</returns>
    public object ScalarSqlite(string command, SQLiteConnection connection)
    {
      try
      {
        SQLiteCommand cmd = new SQLiteCommand(command, connection);
        return cmd.ExecuteScalar();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not execute query:\n" + ex.ToString());
        return null;
      }
    }

    /// <summary>
    /// SQLite: Power function
    /// </summary>
    [SQLiteFunction(Arguments = 2, FuncType = FunctionType.Scalar, Name = "Power")]
    public class Power : SQLiteFunction
    {
      /// <summary>
      /// Invoke method
      /// </summary>
      /// <param name="args">Arguments for function</param>
      /// <returns>Object representing function</returns>
      public override object Invoke(object[] args)
      {
        return Math.Pow(double.Parse(args[0].ToString()), double.Parse(args[1].ToString()));
      }
    }
  }
}
