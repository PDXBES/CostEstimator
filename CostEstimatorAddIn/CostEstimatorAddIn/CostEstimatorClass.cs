using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data;

namespace CostEstimatorAddIn
{
  class CostEstimatorClass
  {

    FileInfo ArcFile;
    string pipeXPInputTableName = "REHABSegments";

    [SQLiteFunction(Arguments = 2, FuncType = FunctionType.Scalar, Name = "power")]
    class power : SQLiteFunction
    {
      public override object Invoke(object[] args)
      {
        return Math.Pow(Double.Parse(args[0].ToString()), Double.Parse(args[1].ToString()));
      }
    }

    public void PerformPipeXP(FileInfo sFile)
    {
      ArcFile = sFile;

      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      conn.Open();
      //Remember we need to enable spatial queries
      SQLiteBasicStrings.enableSpatial(conn);
      nqsqlite(SQLiteBasicStrings.attachDatabase(@"\\besfile1\ASM_Apps\Apps\CostEstimator\PullTables\PullTables_PipeXP.sqlite", "PullTables"), conn);


      //Now that we have loaded our database, which can be found in inputDatabase as the path and sqliteConnectionString as the connection string,
      //we get to perform our first queries on that data
      //Of course to start, it looks like the first thing we do is transfer that data to a new table, GOLEM_PIPEXP.
      //Lets change the name of that to AMStudio_PIPEXP
      //Prep pipexp table
      nqsqlite(AMStudio_PIPXP_Queries.prepPIPEXP(), conn);

      //Then we do our first insert
      nqsqlite(AMStudio_PIPXP_Queries.transferBase(250.0), conn);

      //Proximity to hardAreas
      nqsqlite(AMStudio_PIPXP_Queries.ProximityToHardAreas(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ProximityToHydrants_pdx(25.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ProximityToBuildings(10), conn);
      nqsqlite(AMStudio_PIPXP_Queries.RandomStats(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsUIC(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsMS4(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsEMT(50.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsFire(250.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsPolice(250.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsHospital(250.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsSchool(250.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsECSI(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsResidential(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsPZone(12.5), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsCZone(12.5), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsGas(10.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsFiber(10.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsLRT(25.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsRail(25.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsIntersections(30.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsStreetType(30.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsxStreet(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsSewer(12.0), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ProximityToPoverty(), conn);
      nqsqlite(AMStudio_PIPXP_Queries.ResultsWater(12), conn);
      nqsqlite(AMStudio_PIPXP_Queries.CountLaterals(), conn);
      conn.Close();
    }

    public void PerformCostEstimates(FileInfo sFile)
    {
      ArcFile = sFile;
      double relocationCostPerInchDiameter = 7.9126; //dollars
      double relocationCostBase = 74.093;//dollars
      double utilityCrossingCost = 5000;//dollars
      double hazardousMaterialsCost = 50;//dollars
      double envMitigationCost = 150000; //dollars.  Should this really be 150,000?
      double envMitigationWidth = 25; //feet
      double asphaltRemovalCost = 8.12; //dollars
      double excessAsphaltWidth = 1; //feet
      double asphaltBaseCourseDepth = 0.6666667; //feet
      double asphaltTrenchPatchBaseCourseCost = 29.52; //dollars
      double eightInchPatchCost = 55.77;  //dollars
      double sixInchPatchCost = 44.27; //dollars
      double fourInchPatchCost = 28.62; //dollars
      double pipeZoneDepthAdditionalInches = 18; //inches
      double fillAbovePipeZoneCost = 27.3; //dollars
      double pipeZoneBackfillCost = 70.55; //dollars
      double sawcutPavementLength = 4; //inches?
      double sawcutPavementUnitCost = 4.21; //dollars
      double excavationVolumeFactor = 1.2; //unitless
      double truckHaulSpoilsUnitCost = 4.72; //dollars
      double shoringSquareFeetPerFoot = 2; //
      double ShoringCostPerSquareFoot = 2.57; //dollars
      double minShoringDepth = 18;//feet
      string material = "";
      double workingHoursPerDay = 8; //hours
      double excavationDuration = 140; //cubic yards per day
      double paveDuration = 250; //feet per day
      double utilityCrossingDuration = 0.5;// working days
      double smallBoreJackDiameter = 24; //inches
      double largeBoreJackDiameter = 60; //inches
      double fastBoreRate = 100; //feet/day
      double slowBoreRate = 50;//feet/day
      double boreJackCasingAndGroutingDays = 2; //days
      double CIPPRepairDays = 3;//days
      double shallowSpotDepthCutoff = 20; //feet
      double shallowSpotRepairTime = 4;//hours
      double deepSpotRepairTime = 8;//hours
      double streetTypeStreetTrafficControlMobilization = 1;//hours 
      double streetTypeArterialTrafficControlMobilization = 2; //hours
      double streetTypeMajorArterialTrafficControlMobilization = 3;//hours
      double shallowTrenchDepthCutoff = 20; //feet
      double smallMainlineBypassCutoff = 15; //inches
      double manholeBuildRate = 10; //feet per day
      double lateralTrenchWidth = 4; //feet
      double lateralShoringLength = 10; //feet
      double boreJackArea = 460; //square feet
      double boreJackDepth = 25;//feet
      double fractionalFlow = 0.2; //things per thing
      double Kn = 1.486; //
      double manningsN = 0.013; //general assumption
      double assumedSlope = 0.005; //slope assumed for negative, null, or 0 slope pipes
      double streetTypeStreetTrafficControlCost = 500; //dollars
      double streetTypeArterialTrafficControlCost = 1000; // dollars 
      double streetTypeMajorArterialTrafficControlCost = 3000; //dollars
      double streetTypeFreewayTrafficCost = 0; //dollars
      double boringJackingCost = 566.95; //dollar bills
      double baseENR = 8090; //unitless
      double jackingENR = 9500;//unitless
      double difficultAreaFactor = 1;
      double currentENR = 9835; // unitless
      double generalConditionsFactor = 0.1; //fraction
      double wasteAllowanceFactor = 0.05;//fraction
      double contingencyFactor = 0.25; //fraction
      double ConstructionManagementInspectionTestingFactor = 0.15; //fraction
      double designFactor = 0.2;//fraction
      double PublicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor = 0.03; //fraction
      double StartupCloseoutFactor = 0.1;//fraction
      string csvPath = sFile.DirectoryName + "\\CostEstimates\\CostEstimates.csv";
      double daysForWholePipeLinerConstruction = 3;//days

      //make sure we can call the power function
      SQLiteFunction.RegisterFunction(typeof(power));

      //SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
      //conn.Open();

      //Create cost estimator database
      File.Delete(sFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite");
      SQLiteConnection CostEstimateConnection = new SQLiteConnection("Data Source='" + sFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite';Version=3;", true);
      CostEstimateConnection.Open();

      //transfer PipXP table and REHABSegments to CostEstimates
      nqsqlite(SQLiteBasicStrings.attachDatabase(sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite", "PipeXPResults"), CostEstimateConnection);
      nqsqlite("CREATE TABLE XPData AS SELECT * FROM PipeXPResults.AMStudio_PIPEXP;", CostEstimateConnection);
      nqsqlite("CREATE TABLE RehabSegments AS SELECT * FROM PipeXPResults.REHABSegments;", CostEstimateConnection);
      //Drop attached database
      nqsqlite("DETACH DATABASE 'PipeXPResults';", CostEstimateConnection);

      //Prep Costestimator table
      nqsqlite(AmStudioCostEstimatorQueries.PrepCostEstimator(), CostEstimateConnection);

      //This source needs to change, maybe somewhere on the network
      nqsqlite(SQLiteBasicStrings.attachDatabase(@"\\besfile1\ASM_Apps\Apps\CostEstimator\PullTables\PullTables_CostEstimator.sqlite", "CostEstimator"), CostEstimateConnection);

      //Then we do our first insert
      nqsqlite(AmStudioCostEstimatorQueries.TransferBase(), CostEstimateConnection);

      //Proximity to hardAreas
      nqsqlite(AmStudioCostEstimatorQueries.SetOutsideDiameter(), CostEstimateConnection);

      nqsqlite(AmStudioCostEstimatorQueries.SetTrenchBaseWidth(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetExcavationVolume(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetWaterRelocation(relocationCostPerInchDiameter, relocationCostBase), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetCrossingRelocation(utilityCrossingCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetHazardousMaterials(hazardousMaterialsCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetEnvironmentalMitigation(envMitigationCost, envMitigationWidth), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetAsphaltRemoval(asphaltRemovalCost, excessAsphaltWidth), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetAsphaltBaseCourse(asphaltBaseCourseDepth, excessAsphaltWidth, asphaltTrenchPatchBaseCourseCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetTrenchPatch(excessAsphaltWidth, eightInchPatchCost, sixInchPatchCost, fourInchPatchCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetFillAbovePipeZone(asphaltBaseCourseDepth, pipeZoneDepthAdditionalInches, fillAbovePipeZoneCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetPipeZoneBackfill(pipeZoneDepthAdditionalInches, pipeZoneBackfillCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetAsphaltSawCutting(sawcutPavementLength, sawcutPavementUnitCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetTrenchExcavation(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetTruckHaul(excavationVolumeFactor, truckHaulSpoilsUnitCost), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetTrenchShoring(shoringSquareFeetPerFoot, ShoringCostPerSquareFoot, minShoringDepth), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetPipeMaterialBaseCostPerFoot(material), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setPipeDepthDifficultyFactor(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setPipeMaterial(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeSize(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeBaseCost(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeCostPerFootBeyondMinimum(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeRimFrameCost(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeDepthFactor(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManhole(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBaseOpenCutRepairTime(workingHoursPerDay, excavationDuration, paveDuration, utilityCrossingDuration), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBasePipeBurstRepairTime(workingHoursPerDay), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBaseBoreJackRepairTime(smallBoreJackDiameter, largeBoreJackDiameter, fastBoreRate, slowBoreRate, workingHoursPerDay, boreJackCasingAndGroutingDays), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBaseCIPPRepairTime(workingHoursPerDay, CIPPRepairDays), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBaseSPRepairTime(shallowSpotDepthCutoff, shallowSpotRepairTime, deepSpotRepairTime), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setTrafficControlMobilization(streetTypeStreetTrafficControlMobilization, streetTypeArterialTrafficControlMobilization, streetTypeMajorArterialTrafficControlMobilization), CostEstimateConnection);


      nqsqlite(AmStudioCostEstimatorQueries.setMainlineBypassMobilization(shallowTrenchDepthCutoff, smallMainlineBypassCutoff, workingHoursPerDay), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setManholeReplacement(manholeBuildRate, workingHoursPerDay), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setLateralBypass(lateralTrenchWidth, lateralShoringLength, excavationDuration, paveDuration), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBoreJackPitExcavation(boreJackArea, excavationDuration), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setocConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBJMicroTConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setcippConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setspOnlyConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.removeOpenCutOptions(boreJackDepth), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.removeBoreJackOptions(boreJackDepth), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setnonMobilizationConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setMobilizationConstructionDuration(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBypassPumping(fractionalFlow, Kn, manningsN, assumedSlope, workingHoursPerDay), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setTrafficControl(streetTypeStreetTrafficControlCost, streetTypeArterialTrafficControlCost, streetTypeMajorArterialTrafficControlCost, streetTypeFreewayTrafficCost, workingHoursPerDay), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setBoringJacking(boringJackingCost, baseENR, jackingENR), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setDifficultArea(difficultAreaFactor), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setLaterals(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.DirectConstructionCost(currentENR, baseENR), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.standardPipeFactorCosts(generalConditionsFactor, wasteAllowanceFactor), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.contingencyCost(contingencyFactor), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.setCapitalCost(ConstructionManagementInspectionTestingFactor, designFactor, PublicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor, StartupCloseoutFactor), CostEstimateConnection);
      //Liners
      nqsqlite(AmStudioCostEstimatorQueries.setLinerTrafficControl(streetTypeStreetTrafficControlCost, streetTypeArterialTrafficControlCost, streetTypeMajorArterialTrafficControlCost, streetTypeFreewayTrafficCost, daysForWholePipeLinerConstruction), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerBypassPumping(daysForWholePipeLinerConstruction), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerBuildDuration(daysForWholePipeLinerConstruction), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerLaterals(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerPipeMaterial(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerTvCleaning(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.SetLinerManhole(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.LinerDirectConstructionCost(currentENR, baseENR), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.LinerMobilizationTimes(), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.LinerStandardPipeFactorCost(generalConditionsFactor, wasteAllowanceFactor), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.LinerContingencyCost(contingencyFactor), CostEstimateConnection);
      nqsqlite(AmStudioCostEstimatorQueries.LinerCapitalCost(ConstructionManagementInspectionTestingFactor, designFactor, PublicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor, StartupCloseoutFactor), CostEstimateConnection);

      nqsqlite(AmStudioCostEstimatorQueries.BaseTimesAndMobilizationTimes(workingHoursPerDay, daysForWholePipeLinerConstruction), CostEstimateConnection);


      CostEstimateConnection.Close();

      var selectQuery = "select * from AMStudio_CapitalCostsMobilizationRatesAndTimes;";

      var table = ReadTable(CostEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, sFile.DirectoryName + "\\CostEstimates\\CapitalCostsMobilizationRatesAndTimes.csv", true, ",");

      selectQuery = "select * from AMStudio_PIPEDETAILS;";

      table = ReadTable(CostEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, sFile.DirectoryName + "\\CostEstimates\\COSTEST_PIPEDETAILS.csv", true, ",");

      selectQuery = "SELECT * FROM XPData;";//"select * from AMStudio_PIPEXP;";

      table = ReadTable(CostEstimateConnection.ConnectionString, selectQuery);
      WriteToFile(table, sFile.DirectoryName + "\\CostEstimates\\COSTEST_PIPEXP.csv", true, ",");
    }

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
          conn.Close();
      }

      return returnValue;
    }

    public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = true, string seperator = ",")
    {
      var sw = new StreamWriter(fileOutputPath, false);

      int icolcount = dataSource.Columns.Count;

      if (firstRowIsColumnHeader)
      {
        for (int i = 0; i < icolcount; i++)
        {
          if ((string)(dataSource.Columns[i].ColumnName) == "ID")
          {
            sw.Write("id");
          }
          else
          {
            sw.Write(dataSource.Columns[i]);
          }
          if (i < icolcount - 1)
            sw.Write(seperator);
        }

        sw.Write(sw.NewLine);
      }

      foreach (DataRow drow in dataSource.Rows)
      {
        for (int i = 0; i < icolcount; i++)
        {
          if (!Convert.IsDBNull(drow[i]))
            sw.Write(drow[i].ToString());
          if (i < icolcount - 1)
            sw.Write(seperator);
        }
        sw.Write(sw.NewLine);
      }
      sw.Close();
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
  }
}
