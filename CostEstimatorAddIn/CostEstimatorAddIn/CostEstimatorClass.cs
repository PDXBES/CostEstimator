using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

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
                return Math.Pow(Double.Parse(args[0].ToString()), Double.Parse(args[0].ToString()));
            }
        }

        public void PerformPipeXP(FileInfo sFile)
        {
            ArcFile = sFile;

            SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
            conn.Open();
            //Remember we need to enable spatial queries
            nqsqlite(SQLiteBasicStrings.enableSpatial(), conn);
            nqsqlite(SQLiteBasicStrings.attachDatabase("C:\\SQLite\\Arc\\PullTables_PipeXP.sqlite", "PullTables"), conn);


            //Now that we have loaded our database, which can be found in inputDatabase as the path and sqliteConnectionString as the connection string,
            //we get to perform our first queries on that data
            //Of course to start, it looks like the first thing we do is transfer that data to a new table, GOLEM_PIPEXP.
            //Lets change the name of that to AMStudio_PIPEXP
            //Prep pipexp table
            nqsqlite(AMStudio_PIPXP_Queries.prepPIPEXP(), conn);

            //Then we do our first insert
            nqsqlite(AMStudio_PIPXP_Queries.transferBase(), conn);

            //Proximity to hardAreas
            nqsqlite(AMStudio_PIPXP_Queries.ProximityToHardAreas(), conn);
            nqsqlite(AMStudio_PIPXP_Queries.ProximityToHydrants_pdx(25.0), conn);
            //nqsqlite(AMStudio_PIPXP_Queries.ProximityToBuildings(10));
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
            double shallowTrenchDepthCutoff = 1;
            double smallMainlineBypassCutoff = 15; //inches
            double manholeBuildRate = 10; //feet per day
            double lateralTrenchWidth = 4; //feet
            double lateralShoringLength = 10; //feet
            double boreJackArea = 460; //square feet
            double boreJackDepth = 1;
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

            //make sure we can call the power function
            SQLiteFunction.RegisterFunction(typeof(power));

            //SQLiteConnection conn = new SQLiteConnection("Data Source = '" + sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite';Version=3", true);
            //conn.Open();

            //Create cost estimator database
            File.Delete(sFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite");
            SQLiteConnection CostEstimateConnection = new SQLiteConnection("Data Source='" + sFile.DirectoryName + "\\CostEstimates\\CostEstimates.sqlite';Version=3;");
            CostEstimateConnection.Open();

            //transfer PipXP table and REHABSegments to CostEstimates
            nqsqlite(SQLiteBasicStrings.attachDatabase(sFile.DirectoryName + "\\CostEstimates\\EmgaatsTranslation.sqlite", "PipeXPResults"), CostEstimateConnection);
            nqsqlite("CREATE TABLE XPData AS SELECT * FROM PipeXPResults.AMStudio_PIPEXP;", CostEstimateConnection);
            nqsqlite("CREATE TABLE RehabSegments AS SELECT * FROM PipeXPResults.REHABSegments;", CostEstimateConnection);
            //Drop attached database
            nqsqlite("DETACH DATABASE 'PipeXPResults';", CostEstimateConnection);

            //Prep Costestimator table
            nqsqlite(AMStudio_CostEstimator_Queries.prepCostEstimator(), CostEstimateConnection);

            //This source needs to change, maybe somewhere on the network
            nqsqlite(SQLiteBasicStrings.attachDatabase("C:\\SQLite\\Arc\\PullTables_CostEstimator.sqlite", "CostEstimator"), CostEstimateConnection);

            //Then we do our first insert
            nqsqlite(AMStudio_CostEstimator_Queries.transferBase(), CostEstimateConnection);

            //Proximity to hardAreas
            nqsqlite(AMStudio_CostEstimator_Queries.SetOutsideDiameter(), CostEstimateConnection);

            nqsqlite(AMStudio_CostEstimator_Queries.setTrenchBaseWidth(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setExcavationVolume(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setWaterRelocation(relocationCostPerInchDiameter, relocationCostBase), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setCrossingRelocation(utilityCrossingCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setHazardousMaterials(hazardousMaterialsCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setEnvironmentalMitigation(envMitigationCost, envMitigationWidth), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setAsphaltRemoval(asphaltRemovalCost, excessAsphaltWidth), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setAsphaltBaseCourse(asphaltBaseCourseDepth, excessAsphaltWidth, asphaltTrenchPatchBaseCourseCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTrenchPatch(excessAsphaltWidth, eightInchPatchCost, sixInchPatchCost, fourInchPatchCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setFillAbovePipeZone(asphaltBaseCourseDepth, pipeZoneDepthAdditionalInches, fillAbovePipeZoneCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setPipeZoneBackfill(pipeZoneDepthAdditionalInches, pipeZoneBackfillCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setAsphaltSawCutting(sawcutPavementLength, sawcutPavementUnitCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTrenchExcavation(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTruckHaul(excavationVolumeFactor, truckHaulSpoilsUnitCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTrenchShoring(shoringSquareFeetPerFoot, ShoringCostPerSquareFoot, minShoringDepth), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setPipeMaterialBaseCostPerFoot(material), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setPipeDepthDifficultyFactor(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setPipeMaterial(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeSize(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeBaseCost(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeCostPerFootBeyondMinimum(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeRimFrameCost(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeDepthFactor(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManhole(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBaseOpenCutRepairTime(workingHoursPerDay, excavationDuration, paveDuration, utilityCrossingDuration), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBasePipeBurstRepairTime(workingHoursPerDay), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBaseBoreJackRepairTime(smallBoreJackDiameter, largeBoreJackDiameter, fastBoreRate, slowBoreRate, workingHoursPerDay, boreJackCasingAndGroutingDays), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBaseCIPPRepairTime(workingHoursPerDay, CIPPRepairDays), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBaseSPRepairTime(shallowSpotDepthCutoff, shallowSpotRepairTime, deepSpotRepairTime), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTrafficControlMobilization(streetTypeStreetTrafficControlMobilization, streetTypeArterialTrafficControlMobilization, streetTypeMajorArterialTrafficControlMobilization), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setMainlineBypassMobilization(shallowTrenchDepthCutoff, smallMainlineBypassCutoff, workingHoursPerDay), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setManholeReplacement(manholeBuildRate, workingHoursPerDay), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setLateralBypass(lateralTrenchWidth, lateralShoringLength, excavationDuration, paveDuration), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBoreJackPitExcavation(boreJackArea, excavationDuration), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setocConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBJMicroTConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setcippConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setspOnlyConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.removeOpenCutOptions(boreJackDepth), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.removeBoreJackOptions(boreJackDepth), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setnonMobilizationConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setMobilizationConstructionDuration(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBypassPumping(fractionalFlow, Kn, manningsN, assumedSlope), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setTrafficControl(streetTypeStreetTrafficControlCost, streetTypeArterialTrafficControlCost, streetTypeMajorArterialTrafficControlCost, streetTypeFreewayTrafficCost), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setBoringJacking(boringJackingCost, baseENR, jackingENR), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setDifficultArea(difficultAreaFactor), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setLaterals(), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.DirectConstructionCost(currentENR, baseENR), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.standardPipeFactorCosts(generalConditionsFactor, wasteAllowanceFactor), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.contingencyCost(contingencyFactor), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.setCapitalCost(ConstructionManagementInspectionTestingFactor, designFactor, PublicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor, StartupCloseoutFactor), CostEstimateConnection);
            nqsqlite(AMStudio_CostEstimator_Queries.saveResultsAsCSV(csvPath), CostEstimateConnection);

            CostEstimateConnection.Close();
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
