// <copyright file="AMStudioCostEstimatorQueries.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>AmStudioCostEstimatorQueries class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading.Tasks;

  /// <summary>
  /// Query strings for performing cost estimates
  /// </summary>
  public class AmStudioCostEstimatorQueries
  {
    /// <summary>
    /// Prepares the cost estimator tables
    /// </summary>
    /// <returns>Query string for preparing the cost estimator tables</returns>
    public static string PrepCostEstimator()
    {
      return
        "DROP TABLE IF EXISTS AMStudio_PipeDetails; " +
        "CREATE TABLE AMStudio_PipeDetails( " +
        "ID integer NULL, " +
        "Compkey integer NULL, " +
        "GlobalID integer NULL, " +
        "Type text NULL, " +
        "USNode text NULL, " +
        "DSNode text NULL, " +
        "Cost real NULL, " +
        "Factor real NULL, " +
        "Pipe real NULL, " +
        "Lateral real NULL, " +
        "SawcuttingAC real NULL, " +
        "AsphaltRemoval real NULL, " +
        "TruckHaul real NULL, " +
        "TrenchShoring real NULL, " +
        "AsphaltBaseCourse real NULL, " +
        "AsphaltTrenchPatch real NULL, " +
        "PipeZoneBackfill real NULL, " +
        "FillAbovePipeZone real NULL, " +
        "PipeMaterial real NULL, " +
        "PipeDepthDifficultyFactor real NULL, " +
        "PipeMaterialBaseCostPerFoot real NULL, " +
        "TrenchExcavation real NULL, " +
        "LinerTVCleaning real NULL, " +
        "Manhole real NULL, " +
        "ManholeSize real NULL, " +
        "ManholeBaseCost real NULL, " +
        "ManholeCostPerFootBeyondMinimum real NULL, " +
        "ManholeDepthFactor real NULL, " +
        "ManholeRimFrameCost real NULL, " +
        "BoringJacking real NULL, " +
        "Microtunneling real NULL, " +
        "TrafficControl real NULL, " +
        "ParallelWaterRelocation real NULL, " +
        "CrossingRelocation real NULL, " +
        "EnvironmentalMitigation real NULL, " +
        "HazardousMaterials real NULL, " +
        "BypassPumping real NULL, " +
        "BypassFlow real NULL, " +
        "ExcavationVolume real NULL, " +
        "TrenchBaseWidth real NULL, " +
        "length real NULL, " +
        "outsideDiameter real NULL, " +
        "diamWidth real NULL, " +
        "height real NULL, " +
        "cutno integer NULL, " +
        "DifficultArea real NULL, " +
        "DirectConstructionCost real NULL, " +
        "CapitalCost real NULL, " +
        "StandardPipeFactorCost real NULL, " +
        "ContingencyCost real NULL, " +
        "LinerTrafficControl real NULL, " +
        "LinerBypassPumping real NULL, " +
        "LinerLaterals real NULL, " +
        "LinerPipeMaterial real NULL, " +
        "LinerManhole real NULL, " +
        "LinerDirectConstructionCost real NULL, " +
        "LinerStandardPipeFactorCost real NULL, " +
        "LinerContingencyCost real NULL, " +
        "LinerCapitalCost real NULL, " +
        "OpenCutBuildDuration real NULL, " +
        "SegmentBuildDuration real NULL, " +
        "SpotLineBuildDuration real NULL, " +
        "ManholeBuildDuration real NULL, " +
        "fm real NULL, " +
        "[to] real NULL, " +
        "mobilizationTimeCost real NULL, " +
        "nonMobilizationTimeCost real NULL, " +
        "mobilizationTimeCostCapital real NULL, " +
        "nonMobilizationTimeCostCapital real NULL, " +
        "mobilizationLinerTimeCost real NULL, " +
        "nonMobilizationLinerTimeCost real NULL, " +
        "mobilizationLinerTimeCostCapital real NULL, " +
        "nonMobilizationLinerTimeCostCapital real NULL, " +
        "mobilizationConstructionDuration real NULL, " +
        "nonMobilizationConstructionDuration real NULL, " +
        "mobilizationLinerDuration real NULL, " +
        "nonMobilizationLinerDuration real NULL, " +
        "prejudiceSpot real NULL, " +
        "prejudiceLine real NULL, " +
        "prejudiceDig real NULL); " +
        " CREATE  INDEX IDX_PipeDetails_ID ON AMStudio_PipeDetails (ID); " +
        " CREATE INDEX IDX_PipeDetails_COMPKEY ON AMStudio_PipeDetails (compkey); " +
        "CREATE INDEX IDX_PipeDetails_GLOBALID ON AMStudio_PipeDetails (GLOBALID); " +
        "DROP TABLE IF EXISTS AMStudio_ConstructionDurations; " +
        "CREATE TABLE AMStudio_ConstructionDurations( " +
        "ID integer NULL, " +
        "globalID integer NULL, " +
        "compkey integer NULL, " +
        "cutno integer NULL, " +
        "[fm] real NULL, " +
        "[to] real NULL, " +
        "trafficControl real NULL, " +
        "mainlineBypass real NULL, " +
        "manholeReplacement real NULL, " +
        "lateralBypass real NULL, " +
        "pipeBurstPitExcavation real NULL, " +
        "boreJackPitExcavation real NULL, " +
        "baseOpenCutRepairTime real NULL, " +
        "basePipeBurstRepairTime real NULL, " +
        "baseBoreJackRepairTime real NULL, " +
        "baseCIPPRepairTime real NULL, " +
        "baseSPRepairTime real NULL, " +
        "ocConstructionDuration real NULL, " +
        "pbConstructionDuration real NULL, " +
        "BJMicroTConstructionDuration real NULL, " +
        "cippConstructionDuration real NULL, " +
        "spOnlyConstructionDuration real NULL);" +
        " CREATE  INDEX IDX_ConstructionDurations_ID ON AMStudio_ConstructionDurations (ID); " +
        " CREATE INDEX IDX_ConstructionDurations_COMPKEY ON AMStudio_ConstructionDurations (compkey); " +
        "CREATE INDEX IDX_ConstructionDurations_GLOBALID ON AMStudio_ConstructionDurations (GLOBALID); " +

        "DROP TABLE IF EXISTS AMStudio_CapitalCostsMobilizationRatesAndTimes; " +
        "CREATE TABLE AMStudio_CapitalCostsMobilizationRatesAndTimes( " +
        "ID integer NULL, " +
        "compkey integer NULL, " +
        "globalid integer NULL, " +
        "[type] text NULL, " +
        "[CapitalNonMobilization] real NULL, " +
        "[CapitalMobilizationRate] real NULL, " +
        "[BaseTime] real NULL, " +
        "[MobilizationTime] real NULL, " +
        "[Prejudice] real NULL);" +
        " CREATE  INDEX IDX_CapitalCostsMobilizationRatesAndTimes_ID ON AMStudio_CapitalCostsMobilizationRatesAndTimes (ID); " +
        " CREATE INDEX IDX_CapitalCostsMobilizationRatesAndTimes_COMPKEY ON AMStudio_CapitalCostsMobilizationRatesAndTimes (compkey); " +
        " CREATE INDEX IDX_CapitalCostsMobilizationRatesAndTimes_GLOBALID ON AMStudio_CapitalCostsMobilizationRatesAndTimes (GLOBALID); ";
    }

    /// <summary>
    /// Transfers the base data to the working tables
    /// </summary>
    /// <returns>Query string for transferring base data</returns>
    public static string TransferBase()
    {
      return
        "INSERT INTO AMStudio_PipeDetails (ID, GLOBALID, USnode, DSNode, [length], diamWidth, height, cutno, compkey) " +
        "SELECT OBJECTID, GLOBALID, us_node_id, ds_node_id, [length], pipesize, pipeheight, cutno, hansen_compkey FROM REHABSegments; " +
        "INSERT INTO AMStudio_ConstructionDurations(ID, globalID, compkey, cutno, [fm], [to]) " +
        "SELECT OBJECTID, globalID, hansen_compkey, cutno, [fm], [to_] FROM RehabSegments; " +
        "INSERT INTO AMStudio_CapitalCostsMobilizationRatesAndTimes(ID, Compkey, GlobalID, [Type], [CapitalNonMobilization], [CapitalMobilizationRate], BaseTime, [MobilizationTime]) " +
        "SELECT OBJECTID, hansen_compkey, GlobalID, 'Spot', NULL, NULL, NULL, NULL FROM RehabSegments WHERE OBJECTID >= 40000000; " +
        "INSERT INTO AMStudio_CapitalCostsMobilizationRatesAndTimes(ID, Compkey, GlobalID, [Type], [CapitalNonMobilization], [CapitalMobilizationRate], BaseTime, [MobilizationTime]) " +
        "SELECT OBJECTID, hansen_compkey, GlobalID, 'Dig', NULL, NULL, NULL, NULL FROM RehabSegments WHERE OBJECTID < 40000000; " +
        "INSERT INTO AMStudio_CapitalCostsMobilizationRatesAndTimes(ID, Compkey, GlobalID, [Type], [CapitalNonMobilization], [CapitalMobilizationRate], BaseTime, [MobilizationTime]) " +
        "SELECT OBJECTID, hansen_compkey, GlobalID, 'Line', NULL, NULL, NULL, NULL FROM RehabSegments WHERE OBJECTID < 40000000; ";
    }

    /// <summary>
    /// Sets the outside diameter of the pipes
    /// </summary>
    /// <returns>Query string for setting the outside diameter</returns>
    public static string SetOutsideDiameter()
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    OutsideDiameter = ( " +
          "SELECT MIN(outsideDiameterInches) " +
          "FROM   InsideOutsideDiameterRosetta " +
          "WHERE insideDiameterInches >= AMStudio_PipeDetails.diamWidth); ";
    }

    /// <summary>
    /// Sets the trench base width of the pipes
    /// </summary>
    /// <returns>Query string for setting the trench base width</returns>
    public static string SetTrenchBaseWidth()
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    [TrenchBaseWidth] = ( " +
          "SELECT MIN(widthFeet) " +
          "FROM   TrenchWidthByPipeDiameter " +
          "WHERE  outsideDiameterInches >= AMStudio_PipeDetails.OutsideDiameter); ";
    }

    /// <summary>
    /// Sets the excavation volume for the pipes, where
    /// ExcavationVolume (yd^3/ft)= Average pipe depth (ft) * Trench Base width (ft) * (1yd^3/27ft^3)
    /// </summary>
    /// <returns>Query string for setting the excavation volume</returns>
    public static string SetExcavationVolume()
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    [ExcavationVolume] = ( " +
          "SELECT ((uDepth + dDepth)/2.0)*[TrenchBaseWidth]/27.0 " + // 27 cubic feet per cubic yard
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the water relocation cost
    /// </summary>
    /// <param name="relocationCostPerInchDiameter">Cost per inch of diameter</param>
    /// <param name="relocationCostBase">Base cost</param>
    /// <returns>Query string for setting the water relocation cost</returns>
    public static string SetWaterRelocation(double relocationCostPerInchDiameter, double relocationCostBase)
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    [ParallelWaterRelocation] = ( " +
          "SELECT pWtrMaxD *" + relocationCostPerInchDiameter.ToString() + "+ " + relocationCostBase.ToString() + " " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID AND pWtr > 0); ";
    }

    /// <summary>
    /// Sets the utility crossing cost
    /// </summary>
    /// <param name="utilityCrossingCost">Cost for utility crossing</param>
    /// <returns>Query string for setting the utility crossing cost</returns>
    public static string SetCrossingRelocation(double utilityCrossingCost)
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    [CrossingRelocation] = ( " +
          "SELECT (IFNULL(xSewer,0)+IFNULL(xWtr,0)+IFNULL(xGas,0)+IFNULL(xFiber,0)) *" + utilityCrossingCost.ToString() + " " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the hazardous materials cost
    /// </summary>
    /// <param name="hazardousMaterialsCost">Cost for hazardous materials</param>
    /// <returns>Query string for setting the hazardous materials cost</returns>
    public static string SetHazardousMaterials(double hazardousMaterialsCost)
    {
      return
        "UPDATE AMStudio_PipeDetails " +
        "SET    [HazardousMaterials] = ( " +
          "SELECT (CASE WHEN [length] > 50 THEN 50 ELSE [length] END) * [ExcavationVolume] * " + hazardousMaterialsCost.ToString() + " " +
          "FROM   XPData " +
          "WHERE  xECSI > 0 AND XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the environmental mitigation cost
    /// </summary>
    /// <param name="envMitigationCost">Cost for environmental mitigation</param>
    /// <param name="envMitigationWidth">Width around pipe assumed to be affected</param>
    /// <returns>Query string for setting the environmental mitigation cost</returns>
    public static string SetEnvironmentalMitigation(double envMitigationCost, double envMitigationWidth)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [EnvironmentalMitigation] = ( " +
          "SELECT " + envMitigationCost.ToString() + "*((IFNULL(xFtEzonC,0) + IFNULL(xFtEzonP,0)) *" + envMitigationWidth.ToString() + ")/43560.0  " + // 43560 squre feet per acre
          "FROM   XPData " +
          "WHERE  (IFNULL(xFtEzonC,0) + IFNULL(xFtEzonP,0)) > 0 AND XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the asphalt removal cost
    /// </summary>
    /// <param name="asphaltRemovalCost">Cost for asphalt removal</param>
    /// <param name="excessAsphaltWidth">The width to assume for excess</param>
    /// <returns>Query string for setting the asphalt removal cost</returns>
    public static string SetAsphaltRemoval(double asphaltRemovalCost, double excessAsphaltWidth)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [AsphaltRemoval] = ( " +
          "SELECT " + asphaltRemovalCost.ToString() + "*((TrenchBaseWidth + " + excessAsphaltWidth.ToString() + ") / 9.0) * [length]  " + // 9 square feet per square yard
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the asphalt base course cost
    /// </summary>
    /// <param name="asphaltBaseCourseDepth">The depth of asphalt base course in inches</param>
    /// <param name="excessAsphaltWidth">The width to assume for excess</param>
    /// <param name="asphaltTrenchPatchBaseCourseCost">Cost for asphalt trench patch</param>
    /// <returns>Query string for setting the asphalt base course cost</returns>
    public static string SetAsphaltBaseCourse(
      double asphaltBaseCourseDepth,
      double excessAsphaltWidth,
      double asphaltTrenchPatchBaseCourseCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [AsphaltBaseCourse] = ( " +
          "SELECT ((TrenchBaseWidth + " + 
            excessAsphaltWidth.ToString() + ")*" + asphaltBaseCourseDepth.ToString() + "/27.0) * " + 
            asphaltTrenchPatchBaseCourseCost.ToString() + "* [length]  " + // 27 cubic feet per cubic yard
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the trench patch cost
    /// </summary>
    /// <param name="excessAsphaltWidth">The width to assume for excess</param>
    /// <param name="eightInchPatchCost">The cost for patching eight inches per foot</param>
    /// <param name="sixInchPatchCost">The cost for patching six inches per foot</param>
    /// <param name="fourInchPatchCost">The cost for patching four inches per foot</param>
    /// <returns>Query string for setting the trench patch cost</returns>
    public static string SetTrenchPatch(
      double excessAsphaltWidth,
      double eightInchPatchCost,
      double sixInchPatchCost,
      double fourInchPatchCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [AsphaltTrenchPatch] = ( " +
          "SELECT ((TrenchBaseWidth + " + excessAsphaltWidth.ToString() + ") /9.0) * " + // 9 square feet per square yard
          "(CASE WHEN IFNULL(HardArea,0)> 0 THEN " + eightInchPatchCost.ToString() + " " +
          "WHEN pStrtText = 'A' THEN " + eightInchPatchCost.ToString() + " " +
          "WHEN pStrtText = 'S' THEN " + sixInchPatchCost.ToString() + " " +
          "WHEN pStrtText = 'H' THEN " + fourInchPatchCost.ToString() + " " +
          "END ) * [length]  " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the fill above pipe zone cost
    /// </summary>
    /// <param name="asphaltBaseCourseDepth">The depth of asphalt base course in inches</param>
    /// <param name="pipeZoneDepthAdditionalInches">Additional depth of pipe zone to assume, in inches</param>
    /// <param name="fillAbovePipeZoneCost">The cost per cubic foot per foot of pipe for the fill above pipe zone</param>
    /// <returns>Query string for setting the fill above pipe zone cost</returns>
    public static string SetFillAbovePipeZone(
      double asphaltBaseCourseDepth,
      double pipeZoneDepthAdditionalInches,
      double fillAbovePipeZoneCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [FillAbovePipeZone] = ( " +
          "SELECT  (ExcavationVolume - " +
          "  TrenchBaseWidth*((" + 
            asphaltBaseCourseDepth.ToString() + 
          "+OutsideDiameter+" + 
            pipeZoneDepthAdditionalInches.ToString() + ")/12.0)/27.0 " + // 12 inches per foot, 27 cubic feet per cubic yard
          "       )*[length]*" + fillAbovePipeZoneCost.ToString() + " " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the pipe zone backfill cost
    /// </summary>
    /// <param name="pipeZoneDepthAdditionalInches">Additional depth of pipe zone to assume, in inches</param>
    /// <param name="pipeZoneBackfillCost">The cost per cubic foot per foot of pipe for the backfill</param>
    /// <returns>Query string for setting the pipe zone backfill cost</returns>
    public static string SetPipeZoneBackfill(double pipeZoneDepthAdditionalInches, double pipeZoneBackfillCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [PipeZoneBackfill] = ( " +
          "SELECT  (TrenchBaseWidth * ((OutsideDiameter + " + 
            pipeZoneDepthAdditionalInches.ToString() + ")/12.0) " + // 12 inches per foot
          "        /36 - (((OutsideDiameter*OutsideDiameter) * 12.0 * (3.14159/4.0))/46656.0) " + // 12 inches per foot, 36 cubic feet per cubic yard, 46656 cubic inches per cubic yard
          "        ) * [length] * " + pipeZoneBackfillCost.ToString() + " " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the asphalt saw cutting cost
    /// </summary>
    /// <param name="sawcutPavementLength">The length of pavement in feet</param>
    /// <param name="sawcutPavementUnitCost">The cost of sawcutting</param>
    /// <returns>Query string for setting the asphalt saw cutting cost</returns>
    public static string SetAsphaltSawCutting(double sawcutPavementLength, double sawcutPavementUnitCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [SawcuttingAC] = ( " +
          "SELECT  " + sawcutPavementLength.ToString() + " * " + sawcutPavementUnitCost.ToString() + " * [length]" +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the trench excavation cost
    /// </summary>
    /// <returns>Query string for setting the trench excavation cost</returns>
    public static string SetTrenchExcavation()
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [TrenchExcavation] = ( " +
          "SELECT costPerCuYd " +
          "FROM   DepthToTrenchExcavationCostRecord INNER JOIN XPData ON (uDepth+dDepth)/2.0 <= depthFt " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID ORDER BY depthFt ASC LIMIT 1); ";
    }

    /// <summary>
    /// Sets the truck haul cost
    /// </summary>
    /// <param name="excavationVolumeFactor">The "growth" factor to use for excavation volume (generally >= 1.0)</param>
    /// <param name="truckHaulSpoilsUnitCost">The spoils cost per cubic yard</param>
    /// <returns>Query string for setting the truck haul cost</returns>
    public static string SetTruckHaul(double excavationVolumeFactor, double truckHaulSpoilsUnitCost)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [TruckHaul] = ( " +
          "SELECT (ExcavationVolume * " + excavationVolumeFactor.ToString() + ") * " + truckHaulSpoilsUnitCost.ToString() + " * [length] " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the trench shoring cost
    /// </summary>
    /// <param name="shoringSquareFeetPerFoot">The amount of shoring to assume per foot of pipe</param>
    /// <param name="shoringCostPerSquareFoot">The cost of shoring per square foot</param>
    /// <param name="minShoringDepth">The minimum shoring depth</param>
    /// <returns>Query string for setting the trench shoring cost</returns>
    public static string SetTrenchShoring(
      double shoringSquareFeetPerFoot,
      double shoringCostPerSquareFoot,
      double minShoringDepth)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [TrenchShoring] = IFNULL(( " +
          "SELECT " + shoringSquareFeetPerFoot.ToString() + " * " + shoringCostPerSquareFoot.ToString() + " * [length] * (uDepth+dDepth)/2.0 " +
          "FROM   DepthToTrenchExcavationCostRecord INNER JOIN XPData ON (uDepth+dDepth)/2 >= depthFt " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID AND (uDepth+dDepth)/2.0 >= " + minShoringDepth.ToString() + "),0); ";
    }

    /// <summary>
    /// Sets the pipe material base cost per foot
    /// </summary>
    /// <param name="material">The material code</param>
    /// <returns>Query string for setting the pipe material base cost per foot</returns>
    public static string SetPipeMaterialBaseCostPerFoot(string material)
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [PipeMaterialBaseCostPerFoot] = IFNULL(( " +
          "SELECT Cost " +
          "FROM   PipeMaterialCosts INNER JOIN XPData ON diamWidth >= minDiameter " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID AND PipeMaterialCosts.Material like '" + material + "' ORDER BY minDiameter DESC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the pipe depth difficulty factor
    /// </summary>
    /// <returns>Query string for setting the pipe depth difficulty factor</returns>
    public static string SetPipeDepthDifficultyFactor()
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    [PipeDepthDifficultyFactor] = IFNULL(( " +
          "SELECT DifficultyFactor " +
          "FROM   DiameterDepthDifficultyFactor INNER JOIN XPData ON diamWidth >= smallestDiameter AND (uDepth+dDepth)/2.0 >= Depth " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID ORDER BY smallestDiameter DESC, Depth DESC LIMIT 1),1); ";
    }

    /// <summary>
    /// Sets the pipe material cost
    /// </summary>
    /// <returns>Query string setting the pipe material cost</returns>
    public static string SetPipeMaterial()
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    PipeMaterial = IFNULL(( " +
          "SELECT PipeMaterialBaseCostPerFoot * PipeDepthDifficultyFactor * [length] " +
          "FROM   XPData " +
          "WHERE  XPData.ID = AMStudio_PipeDetails.ID ),0); ";
    }

    /// <summary>
    /// Sets the manhole size
    /// </summary>
    /// <returns>Query string setting the manhole size</returns>
    public static string SetManholeSize()
    {
      return 
        "UPDATE AMStudio_PipeDetails " +
        "SET    ManholeSize = IFNULL(( " +
          "SELECT manholeDiameterInches " +
          "FROM   InsideDiameterToManholeDiameter " +
          "WHERE  AMStudio_PipeDetails.DiamWidth <= insideDiameterInches ORDER BY insideDiameterInches ASC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the manhole base cost
    /// </summary>
    /// <returns>Query string setting the manhole base cost</returns>
    public static string SetManholeBaseCost()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    ManholeBaseCost = IFNULL(( " +
               "SELECT BaseCost " +
               "FROM   ManholeCostTable " +
               "WHERE  AMStudio_PipeDetails.ManholeSize <= manholeDiameter ORDER BY manholeDiameter ASC LIMIT 1 ),0); ";
    }

    /// <summary>
    /// Sets the manhole cost per foot if we have a manhole diameter smaller than the minimum provided
    /// </summary>
    /// <returns>Query string setting the manhole cost per foot if smaller than minimum provided</returns>
    public static string SetManholeCostPerFootBeyondMinimum()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    ManholeCostPerFootBeyondMinimum = IFNULL(( " +
               "SELECT CostPerFootAbove8Ft " +
               "FROM   ManholeCostTable " +
               "WHERE  AMStudio_PipeDetails.ManholeSize <= manholeDiameter ORDER BY manholeDiameter ASC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the manhole rim frame cost
    /// </summary>
    /// <returns>Query string setting the manhole rim frame cost</returns>
    public static string SetManholeRimFrameCost()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    ManholeRimFrameCost = IFNULL(( " +
               "SELECT RimFrameCost " +
               "FROM   ManholeCostTable " +
               "WHERE  AMStudio_PipeDetails.ManholeSize <= manholeDiameter ORDER BY manholeDiameter ASC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the manhole depth factor
    /// </summary>
    /// <returns>Query string setting the manhole depth factor</returns>
    public static string SetManholeDepthFactor()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    ManholeDepthFactor = IFNULL(( " +
               "SELECT Factor " +
               "FROM   ManholeCostDepthFactorTable INNER JOIN XPData ON (uDepth+dDepth)/2 >= manholeMinDepth " +
               "WHERE  XPData.ID = AMStudio_PipeDetails.ID AND AMStudio_PipeDetails.ManholeSize >= manholeMinSize ORDER BY manholeMinSize DESC, manholeMinDepth DESC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the manhole cost
    /// </summary>
    /// <returns>Query string setting the manhole cost</returns>
    public static string SetManhole()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    Manhole = ( " +
             "SELECT ManholeDepthFactor*(ManholeBaseCost + ManholeCostPerFootBeyondMinimum * CASE WHEN (uDepth +dDepth)/2.0 - 8.0 <= 0 THEN 0 ELSE (uDepth +dDepth)/2.0 - 8.0 END + ManholeRimFrameCost) " +
             "FROM   XPData " +
             "WHERE  XPData.ID = AMStudio_PipeDetails.ID); ";
    }

    /// <summary>
    /// Sets the base open cut repair time
    /// </summary>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <param name="excavationDuration">Duration of excavation work in days</param>
    /// <param name="paveDuration">Duration of pavement replacement work in days</param>
    /// <param name="utilityCrossingDuration">Duration of working with crossing utility in days</param>
    /// <returns>Query string setting the base open cut repair time</returns>
    public static string SetBaseOpenCutRepairTime(
      double workingHoursPerDay,
      double excavationDuration,
      double paveDuration,
      double utilityCrossingDuration)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    baseOpenCutRepairTime = ( " +
             "SELECT (((((uDepth + dDepth)/2)*TrenchBaseWidth/27.0)/" + excavationDuration.ToString() + ") + (XPData.[length]/" + paveDuration.ToString() + "))*" + workingHoursPerDay.ToString() + " " + // 27 cubic feet per cubic yard
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID); " +

             "UPDATE AMStudio_ConstructionDurations " +
             "SET    baseOpenCutRepairTime = ( " +
             "SELECT baseOpenCutRepairTime + (" + utilityCrossingDuration.ToString() + " * " + workingHoursPerDay.ToString() + " * (IFNULL(xWtr, 0) + IFNULL(xGas, 0) + IFNULL(xFiber, 0) + IFNULL(xSewer, 0))) " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID); ";
    }

    /// <summary>
    /// Sets the base pipe burst repair time
    /// </summary>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <returns>Query string setting the base pipe burst repair time</returns>
    public static string SetBasePipeBurstRepairTime(double workingHoursPerDay)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    basePipeBurstRepairTime = " + workingHoursPerDay.ToString() + " WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the base boring/jacking repair time
    /// </summary>
    /// <param name="smallBoreJackDiameter">The low end of the boring/jacking diameter range</param>
    /// <param name="largeBoreJackDiameter">The high end of the boring/jacking diameter range</param>
    /// <param name="fastBoreRate">The fast boring rate in feet per day</param>
    /// <param name="slowBoreRate">The slow boring rate in feet per day</param>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <param name="boreJackCasingAndGroutingDays">Number of days for installing casing and grouting </param>
    /// <returns>Query string setting the base boring/jacking repair time</returns>
    public static string SetBaseBoreJackRepairTime(
      double smallBoreJackDiameter,
      double largeBoreJackDiameter,
      double fastBoreRate,
      double slowBoreRate,
      double workingHoursPerDay,
      double boreJackCasingAndGroutingDays)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    baseBoreJackRepairTime =  ( " +
             "SELECT CASE " +
             "         WHEN  XPData.diamWidth < " + smallBoreJackDiameter.ToString() + " " +
             "         THEN  XPData.[length]/" + fastBoreRate.ToString() + " + " + boreJackCasingAndGroutingDays.ToString() + " * " + workingHoursPerDay.ToString() + " " +  // 1 day to install pipe in casing and 1 day for grouting
             "         WHEN  XPData.diamWidth >= " + smallBoreJackDiameter.ToString() + " AND XPData.diamWidth <= " + largeBoreJackDiameter.ToString() + " " +
             "         THEN  XPData.[length]/" + slowBoreRate.ToString() + " + " + boreJackCasingAndGroutingDays.ToString() + " * " + workingHoursPerDay.ToString() + " " +  // 1 day to install pipe in casing and 1 day for grouting
             "         WHEN  XPData.diamWidth > " + largeBoreJackDiameter.ToString() + " " +
             "         THEN  XPData.[length]/" + slowBoreRate.ToString() + " " + // at this size, the casing will be the host pipe, so the 2 days is not applicable
             "       END " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID) WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the base CIPP repair time
    /// </summary>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <param name="cippRepairDays">Number of days to install CIPP</param>
    /// <returns>Query string for setting the base CIPP repair time</returns>
    public static string SetBaseCippRepairTime(double workingHoursPerDay, double cippRepairDays)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    baseCIPPRepairTime = " + workingHoursPerDay.ToString() + " * " + cippRepairDays.ToString() + " WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the base spot repair time
    /// </summary>
    /// <param name="shallowSpotDepthCutoff">The shallow spot depth cutoff in feet</param>
    /// <param name="shallowSpotRepairTime">The shallow spot repair duration in days</param>
    /// <param name="deepSpotRepairTime">The deep spot repair duration in days</param>
    /// <returns>Query string setting the base spot repair time</returns>
    public static string SetBaseSpotRepairTime(
      double shallowSpotDepthCutoff,
      double shallowSpotRepairTime,
      double deepSpotRepairTime)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    baseSPRepairTime =  ( " +
             "SELECT CASE " +
             "         WHEN  (uDepth + dDepth)/2 < " + shallowSpotDepthCutoff.ToString() + " " +
             "         THEN  " + shallowSpotRepairTime.ToString() + " " +
             "         WHEN  (uDepth + dDepth)/2 >= " + shallowSpotDepthCutoff.ToString() + " " +
             "         THEN  " + deepSpotRepairTime.ToString() + " " +
             "       END " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID) WHERE cutno > 0; ";
    }

    /// <summary>
    /// Sets the traffic control mobilization duration
    /// </summary>
    /// <param name="streetTypeStreetTrafficControlMobilization">Street-volume traffic control mobilization in days</param>
    /// <param name="streetTypeArterialTrafficControlMobilization">Arterial-volume traffic control mobilization in days</param>
    /// <param name="streetTypeMajorArterialTrafficControlMobilization">Major arterial-volume traffic control mobilization in days</param>
    /// <returns>Query string setting the traffic control mobilization</returns>
    public static string SetTrafficControlMobilization(
      double streetTypeStreetTrafficControlMobilization,
      double streetTypeArterialTrafficControlMobilization,
      double streetTypeMajorArterialTrafficControlMobilization)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    trafficControl =  ( " +
             "SELECT CASE " +
             "         WHEN  pStrtText like 'S' " +
             "         THEN " + streetTypeStreetTrafficControlMobilization.ToString() + " " +
             "         WHEN  pStrtText like 'A' " +
             "         THEN  " + streetTypeArterialTrafficControlMobilization.ToString() + " " +
             "         WHEN  pStrtText like 'M' " +
             "         THEN " + streetTypeMajorArterialTrafficControlMobilization.ToString() + " " +
             "         ELSE  0 " +
             "       END " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID); " +

             "UPDATE AMStudio_ConstructionDurations " +
             "SET    trafficControl =  ( " +
             "SELECT MAX(ifnull(trafficControl,0)) " +
             "FROM   AMStudio_ConstructionDurations AS A " +
             "WHERE  A.compkey = AMStudio_ConstructionDurations.compkey GROUP BY A.compkey) WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the mainline bypass mobilization duration
    /// </summary>
    /// <param name="shallowTrenchDepthCutoff">Shallow trench depth cutoff in feet</param>
    /// <param name="smallMainlineBypassCutoff">Small mainline bypass cutoff diameter in inches</param>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <returns>Query string setting the mainline bypass mobilization</returns>
    public static string SetMainlineBypassMobilization(
      double shallowTrenchDepthCutoff,
      double smallMainlineBypassCutoff,
      double workingHoursPerDay)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    mainlineBypass =  ( " +
             "SELECT CASE " +
             "         WHEN  (uDepth + dDepth)/2.0 < " + shallowTrenchDepthCutoff.ToString() + " AND XPData.diamwidth <= " + smallMainlineBypassCutoff.ToString() + " " +
             "         THEN  " + workingHoursPerDay.ToString() + "/2.0 " +
             "         ELSE  " + workingHoursPerDay.ToString() + " " +
             "       END " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID); ";
    }

    /// <summary>
    /// Sets the manhole replacement cost
    /// </summary>
    /// <param name="manholeBuildRate">Manhole build rate in feet per day</param>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <returns>Query string setting the manhole replacement cost</returns>
    public static string SetManholeReplacement(
      double manholeBuildRate,
      double workingHoursPerDay)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    manholeReplacement =  ( " +
             "SELECT (( " +
             "         CASE " +
             "           WHEN (uDepth + dDepth)/2.0 < 10.0 " +
             "           THEN 10.0 " +
             "           ELSE (uDepth + dDepth)/2.0 " +
             "         END " +
             "       )/" + manholeBuildRate.ToString() + ") * " + workingHoursPerDay.ToString() + " " +
             "FROM   XPData INNER JOIN AMStudio_PipeDetails ON XPData.ID = AMStudio_PipeDetails.ID " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID AND XPData.Cutno = 0); " +

             "UPDATE AMStudio_ConstructionDurations " +
             "SET    manholeReplacement =  ( " +
             "SELECT manholeReplacement " +
             "FROM   AMStudio_ConstructionDurations AS A  " +
             "WHERE  A.Compkey = AMStudio_ConstructionDurations.Compkey AND A.Cutno = 0) WHERE AMStudio_ConstructionDurations.Cutno = 1; ";
    }

    /// <summary>
    /// Sets the lateral bypass cost
    /// </summary>
    /// <param name="lateralTrenchWidth">Width of the lateral trench in feet</param>
    /// <param name="lateralShoringLength">Length of the shoring in feet</param>
    /// <param name="excavationDuration">Rate of volume excavation in cubic feet per day</param>
    /// <param name="paveDuration">Duration of pavement work in days</param>
    /// <returns>Query string setting the lateral bypass cost</returns>
    public static string SetLateralBypass(
      double lateralTrenchWidth,
      double lateralShoringLength,
      double excavationDuration,
      double paveDuration)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    lateralBypass =  ( " +
             "SELECT LateralCount * " +
                    "( " +
                    "  ( " +
                    "    (" + lateralTrenchWidth.ToString() + "*" + lateralShoringLength.ToString() + "*((uDepth + dDepth)/2.0))/(27.0*" + excavationDuration.ToString() + ") " + // 27 cubic feet per cubic yard
                    "  ) " +
                    "  + " +
                    "  ( " +
                    "    " + lateralShoringLength.ToString() + "/" + paveDuration.ToString() + " " +
                    "  ) " +
                    ") " +
             "FROM   XPData  " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID ); ";
    }

    /// <summary>
    /// Sets the boring/jacking pit excavation cost
    /// </summary>
    /// <param name="boreJackArea">Area in sq ft per ft of pipe to assume for boring/jacking</param>
    /// <param name="excavationDuration">Rate of volume excavation in cubic feet per day</param>
    /// <returns>Query string for setting the boring/jacking pit excavation cost</returns>
    public static string SetBoreJackPitExcavation(
      double boreJackArea,
      double excavationDuration)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    lateralBypass =  ( " +
             "SELECT boreJackPitExcavation = LateralCount * " +
                    "( " +
                    "  (" + boreJackArea.ToString() + "*(uDepth+dDepth)/2.0)/(27.0*" + excavationDuration.ToString() + ") " + // 27 cubic feet per cubic yard
                    ") " +
             "FROM   XPData  " +
             "WHERE  XPData.ID = AMStudio_ConstructionDurations.ID ); ";
    }

    /// <summary>
    /// Sets the open cut construction duration in days
    /// </summary>
    /// <returns>Query string for setting the open cut construction duration</returns>
    public static string SetOpenCutConstructionDuration()
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    ocConstructionDuration =  IFNULL(baseOpenCutRepairTime,0) + IFNULL(manholeReplacement,0) + IFNULL(trafficControl,0) + IFNULL(mainlineBypass,0) WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the boring/jacking and microtunneling construction duration in days
    /// </summary>
    /// <returns>Query string for setting the boring/jacking/microtunneling construction duration</returns>
    public static string SetBjMicroTConstructionDuration()
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    BJMicroTConstructionDuration =  IFNULL(baseBoreJackRepairTime,0) + IFNULL(trafficControl,0) + IFNULL(mainlineBypass,0) + IFNULL(lateralBypass,0) + IFNULL(boreJackPitExcavation,0) WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the CIPP construction duration in days
    /// </summary>
    /// <returns>Query string for setting the CIPP construction duration</returns>
    public static string SetCippConstructionDuration()
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    cippConstructionDuration =  IFNULL(baseCIPPRepairTime,0) + IFNULL(baseSPRepairTime,0) + IFNULL(trafficControl,0) + IFNULL(mainlineBypass,0) + IFNULL(lateralBypass,0) WHERE cutno = 0; ";
    }

    /// <summary>
    /// Sets the spot repair construction duration in days
    /// </summary>
    /// <returns>Query string for setting the spot repair construction duration</returns>
    public static string SetSpOnlyConstructionDuration()
    {
      return "UPDATE AMStudio_ConstructionDurations " +
             "SET    spOnlyConstructionDuration =  IFNULL(baseSPRepairTime,0) + IFNULL(trafficControl,0) + IFNULL(mainlineBypass,0); ";
    }

    /// <summary>
    /// remove open cut options from cases where bore/jack is required
    /// </summary>
    /// <param name="boreJackDepth">Depth in ft. triggering boring/jacking</param>
    /// <returns>Query string modifying cases where bore/jack is required</returns>
    public static string RemoveOpenCutOptions(double boreJackDepth)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
                 "SET    ocConstructionDuration =  IFNULL( " +
                 "(SELECT 0 " +
                 " FROM   XPData  " +
                 " WHERE  XPData.ID = AMStudio_ConstructionDurations.ID AND XPData.Cutno = 0 " +
                 "        AND " +
                 "        ( " +
                 "         (uDepth + dDepth)/2.0 > " + boreJackDepth.ToString() + " " +
                 "         OR " +
                 "         xBldg > 0 " +
                 "         OR " +
                 "         xLRT > 0 " +
                 "         OR " +
                 "         xRail > 0 " +
                 "         OR " +
                 "         xFrwy > 0 " +
                 "        ) " +
                 "),ocConstructionDuration) ; " +

                 "UPDATE AMStudio_ConstructionDurations " +
                 "SET    baseOpenCutRepairTime = NULL WHERE ocConstructionDuration = 0; " +

                 "UPDATE AMStudio_ConstructionDurations " +
                 "SET    ocConstructionDuration = NULL WHERE ocConstructionDuration = 0; ";
    }

    /// <summary>
    /// Removes bore/jack options from cases where bore/jack is not required
    /// </summary>
    /// <param name="boreJackDepth">Depth in ft. triggering boring/jacking</param>
    /// <returns>Query string modifying cases where bore/jack is not required</returns>
    public static string RemoveBoreJackOptions(double boreJackDepth)
    {
      return "UPDATE AMStudio_ConstructionDurations " +
                 "SET    baseBoreJackRepairTime =  IFNULL( " +
                 "(SELECT 0 " +
                 " FROM   XPData  " +
                 " WHERE  XPData.ID = AMStudio_ConstructionDurations.ID AND XPData.Cutno = 0 " +
                 "        AND " +
                 "        ( " +
                 "         (uDepth + dDepth)/2.0 <= " + boreJackDepth.ToString() + " " +
                 "         AND " +
                 "         xBldg = 0 " +
                 "         AND " +
                 "         xLRT = 0 " +
                 "         AND " +
                 "         xRail = 0 " +
                 "         AND " +
                 "         xFrwy = 0 " +
                 "        ) " +
                 "),baseBoreJackRepairTime) ; " +

                 "UPDATE AMStudio_ConstructionDurations " +
                 "SET    boreJackPitExcavation = NULL WHERE baseBoreJackRepairTime = 0; " +

                 "UPDATE AMStudio_ConstructionDurations " +
                 "SET    BJMicroTConstructionDuration = NULL WHERE baseBoreJackRepairTime = 0; " +

                 "UPDATE AMStudio_ConstructionDurations " +
                 "SET    baseBoreJackRepairTime = NULL WHERE baseBoreJackRepairTime = 0; ";
    }

    /// <summary>
    /// Sets non-mobilization construction duration
    /// </summary>
    /// <returns>Query string updating non-mobilization construction duration</returns>
    public static string SetNonMobilizationConstructionDuration()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    nonMobilizationConstructionDuration = IFNULL(( " +
               "SELECT BaseOpenCutRepairTime + ManholeReplacement " +
               "FROM   AMStudio_ConstructionDurations " +
               "WHERE  AMStudio_PipeDetails.ID = AMStudio_ConstructionDurations.ID AND AMStudio_ConstructionDurations.cutno = 0),0); ";
    }

    /// <summary>
    /// Sets mobilization construction duration
    /// </summary>
    /// <returns>Query string to set mobilization construction duration</returns>
    public static string SetMobilizationConstructionDuration()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    MobilizationConstructionDuration = IFNULL(( " +
               "SELECT trafficControl + mainlineBypass " +
               "FROM   AMStudio_ConstructionDurations " +
               "WHERE  AMStudio_PipeDetails.ID = AMStudio_ConstructionDurations.ID AND AMStudio_ConstructionDurations.cutno = 0),0); ";
    }

    /// <summary>
    /// Sets bypass pumping cost
    /// </summary>
    /// <param name="fractionalFlow">How much of full flow to assume</param>
    /// <param name="kn">Factor to modify pipe roughness</param>
    /// <param name="manningsN">Pipe roughness</param>
    /// <param name="assumedSlope">Slope to assume for calculating flow</param>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <returns>Query string to set bypass pumping cost</returns>
    public static string SetBypassPumping(
      double fractionalFlow,
      double kn,
      double manningsN,
      double assumedSlope,
      double workingHoursPerDay)
    {
      // bypass pumping rates are in dollars/day, not dollars/hour
      return "UPDATE AMStudio_PipeDetails " +
             "SET    BypassFlow = IFNULL(( " +
               "SELECT CASE WHEN XPData.xPipSlope > 0 THEN (" + fractionalFlow.ToString() + " * " + kn.ToString() + "/" + manningsN.ToString() + ") * Power(IFNULL(diamWidth,0)/(4.0*12.0), 2.0/3.0) * Power( XPData.xPipSlope, 0.5) " +
               "                               ELSE (" + fractionalFlow.ToString() + " * " + kn.ToString() + "/" + manningsN.ToString() + ") * Power(IFNULL(diamWidth,0)/(4.0*12.0), 2.0/3.0) * Power( " + assumedSlope.ToString() + ", 0.5) " +
               "                               END " +
               "FROM   AMStudio_ConstructionDurations " +
               "       INNER JOIN " +
               "       XPData " +
               "       ON AMStudio_ConstructionDurations.ID = XPData.ID " +
               "WHERE  AMStudio_PipeDetails.ID = AMStudio_ConstructionDurations.ID AND AMStudio_ConstructionDurations.cutno = 0),0); " +

               "UPDATE AMStudio_PipeDetails " +
               "SET    BypassPumping = IFNULL(( SELECT BypassCost * (AMStudio_PipeDetails.nonMobilizationConstructionDuration + AMStudio_PipeDetails.mobilizationConstructionDuration)/ " + workingHoursPerDay.ToString() + " " +
               "FROM   BypassPumpingUnitRates " +
               "WHERE  AMStudio_PipeDetails.BypassFlow > BypassPumpingUnitRates.BypassFlowGPM ORDER BY BypassFlowGPM DESC LIMIT 1),0); " +

               "UPDATE  AMStudio_CapitalCostsMobilizationRatesAndTimes " +
               "SET     CapitalMobilizationRate = IFNULL(( SELECT BypassCost / " + workingHoursPerDay.ToString() + " " +
               "FROM   BypassPumpingUnitRates " +
               "       INNER JOIN " +
               "       AMStudio_PipeDetails " +
               "       ON AMStudio_PipeDetails.BypassFlow > BypassPumpingUnitRates.BypassFlowGPM   " +
               "          AND         " +
               "          AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
               "ORDER BY BypassFlowGPM DESC LIMIT 1),0) WHERE ID < 40000000; ";
    }

    /// <summary>
    /// Sets the traffic control cost
    /// </summary>
    /// <param name="streetTypeStreetTrafficControlCost">Cost for street-volume traffic control</param>
    /// <param name="streetTypeArterialTrafficControlCost">Cost for arterial-volume traffic control</param>
    /// <param name="streetTypeMajorArterialTrafficControlCost">Cost for major arterial-volume traffic control</param>
    /// <param name="streetTypeFreewayTrafficCost">Cost for freeway-volume traffic control</param>
    /// <param name="workingHoursPerDay">Number of working hours per day</param>
    /// <returns>Query string to set traffic control cost</returns>
    public static string SetTrafficControl(
      double streetTypeStreetTrafficControlCost,
      double streetTypeArterialTrafficControlCost,
      double streetTypeMajorArterialTrafficControlCost,
      double streetTypeFreewayTrafficCost,
      double workingHoursPerDay)
    {
      // this value is in dollars/day, but our times are in hours,
      // so divide by workinghoursperday
      return "UPDATE AMStudio_PipeDetails " +
             "SET    trafficControl =  ( " +
             "SELECT CASE " +
             "         WHEN  pStrtText like 'F' " +
             "         THEN " + streetTypeFreewayTrafficCost.ToString() + " " +
             "         WHEN  pStrtText like 'A' " +
             "         THEN " + streetTypeArterialTrafficControlCost.ToString() + " " +
             "         WHEN  pStrtText like 'M' " +
             "         THEN  " + streetTypeMajorArterialTrafficControlCost.ToString() + " " +
             "         WHEN  pStrtText like 'S' " +
             "         THEN " + streetTypeStreetTrafficControlCost.ToString() + " " +
             "         ELSE  0 " +
             "       END * ((CASE WHEN IFNULL(uxCLx,0) <2 THEN 0 ELSE uxCLX - 2 END)/2 + 1) " + // assumes at least 2 streets at every intersection and bases cost on every additional street.
             "FROM   XPData  " +
             "WHERE  XPData.ID = AMStudio_PipeDetails.ID); " +

             "UPDATE AMStudio_PipeDetails " +
             "SET    trafficControl =  IFNULL(trafficControl,0) * " +
             "       IFNULL( AMStudio_PipeDetails.nonMobilizationConstructionDuration + AMStudio_PipeDetails.mobilizationConstructionDuration " +
             "        ,0)/" + workingHoursPerDay.ToString() + " WHERE ID < 40000000; " +

             "UPDATE  AMStudio_CapitalCostsMobilizationRatesAndTimes " +
               "SET     CapitalMobilizationRate = IFNULL(CapitalMobilizationRate,0) + IFNULL(( SELECT trafficControl " +
               "FROM   AMStudio_PipeDetails " +
               "WHERE  AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID ),0) WHERE ID < 40000000; ";
    }

    /// <summary>
    /// Sets boring and jacking costs
    /// </summary>
    /// <param name="boringJackingCost">Cost per foot to conduct boring and jacking</param>
    /// <param name="baseENR">The base ENR/CCI value</param>
    /// <param name="jackingENR">The current ENR/CCI value</param>
    /// <returns>Query string to set boring and jacking costs</returns>
    public static string SetBoringJacking(
      double boringJackingCost,
      double baseENR,
      double jackingENR)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    boringJacking =  IFNULL(( " +
             "SELECT " + boringJackingCost.ToString() + " * (" + baseENR.ToString() + " / " + jackingENR.ToString() + ")* Power(2.71828, (0.0119*diamWidth)) * [length] " +
             "FROM   XPData  " +
             "WHERE  XPData.ID = AMStudio_PipeDetails.ID " +
             "       AND " +
             "       ( " +
             "         ((uDepth + dDepth)/2.0 > 25) " +
             "         OR " +
             "         xFrwy > 0 " +
             "         OR " +
             "         xBldg > 0 " +
             "         OR " +
             "         xLRT > 0 " +
             "         OR " +
             "         xRail > 0 " +
             "       ) " +
             "),0); ";
    }

    /// <summary>
    /// Sets the difficult area factor
    /// </summary>
    /// <param name="difficultAreaFactor">Difficult area factor</param>
    /// <returns>Query string to set the difficult area factor</returns>
    public static string SetDifficultArea(double difficultAreaFactor)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    DifficultArea = IFNULL( (" +
             "       SELECT Power( " + difficultAreaFactor.ToString() + ", " +
             "                    CASE WHEN IFNULL(HardArea,0) > 0 THEN 1 ELSE 0 END + " +
             "                    CASE WHEN IFNULL(pRail, 0) > 0 THEN 1 ELSE 0 END + " +
             "                    CASE WHEN IFNULL(pLRT, 0) > 0 THEN 1 ELSE 0 END + " +
             "                    CASE WHEN ABS(IFNULL(gSlope, 0)) >= 0.1 AND (pStrt = 0 OR pStrt IS NULL) THEN 1 ELSE 0 END " +
             "               ) " +
             "        FROM   XPData " +
             "        WHERE  XPData.ID = AMStudio_PipeDetails.ID),0); ";
    }

    /// <summary>
    /// Sets the lateral count
    /// </summary>
    /// <returns>Query string to set the lateral count</returns>
    public static string SetLaterals()
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    Lateral = IFNULL(( " +
             "       SELECT LateralCount " +
             "       FROM   XPData " +
             "       WHERE  AMStudio_PipeDetails.ID = XPData.ID),0);";
    }

    /// <summary>
    /// Sets the direct construction cost
    /// </summary>
    /// <param name="currentEnr">The current ENR/CCI value</param>
    /// <param name="baseEnr">The base ENR/CCI value</param>
    /// <returns>Query string to set the direct construction cost</returns>
    public static string DirectConstructionCost(double currentEnr, double baseEnr)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    DirectConstructionCost = (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + ")) " +
                                             "*" +
                                             "(" +
                                               "IFNULL(TrafficControl,0) " +
                                               "+ IFNULL(BypassPumping, 0) " +
                                               "+ IFNULL(TrenchShoring, 0) " +
                                               "+ IFNULL(TrenchExcavation, 0 ) " +
                                               "+ IFNULL(TruckHaul, 0) " +
                                               "+ IFNULL(PipeMaterial, 0) " +
                                               "+ IFNULL(Manhole, 0) " +
                                               "+ IFNULL(FillAbovePipeZone, 0) " +
                                               "+ IFNULL(PipeZoneBackfill, 0) " +
                                               "+ IFNULL(SawcuttingAC, 0) " +
                                               "+ IFNULL(AsphaltRemoval, 0) " +
                                               "+ IFNULL(AsphaltTrenchPatch, 0) " +
                                               "+ IFNULL(AsphaltBaseCourse, 0) " +
                                               "+ IFNULL(ParallelWaterRelocation, 0) " +
                                               "+ IFNULL(CrossingRelocation, 0) " +
                                               "+ IFNULL(HazardousMaterials, 0) " +
                                               "+ IFNULL(EnvironmentalMitigation, 0) " +
                                             "); " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(( " +
             "                               SELECT (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + ")) " +
                                             "*" +
                                             "(" +
                                               " IFNULL(TrenchShoring, 0) " +
                                               "+ IFNULL(TrenchExcavation, 0 ) " +
                                               "+ IFNULL(TruckHaul, 0) " +
                                               "+ IFNULL(PipeMaterial, 0) " +
                                               "+ IFNULL(Manhole, 0) " +
                                               "+ IFNULL(FillAbovePipeZone, 0) " +
                                               "+ IFNULL(PipeZoneBackfill, 0) " +
                                               "+ IFNULL(SawcuttingAC, 0) " +
                                               "+ IFNULL(AsphaltRemoval, 0) " +
                                               "+ IFNULL(AsphaltTrenchPatch, 0) " +
                                               "+ IFNULL(AsphaltBaseCourse, 0) " +
                                               "+ IFNULL(ParallelWaterRelocation, 0) " +
                                               "+ IFNULL(CrossingRelocation, 0) " +
                                               "+ IFNULL(HazardousMaterials, 0) " +
                                               "+ IFNULL(EnvironmentalMitigation, 0) " +
                                             ") " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(( " +
             "                               SELECT (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + ")) " +
                                             "*" +
                                             "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; ";
    }

    /// <summary>
    /// Sets standard pipe factor costs
    /// </summary>
    /// <param name="generalConditionsFactor">The fraction by which to increase the cost to account for general conditions</param>
    /// <param name="wasteAllowanceFactor">The fraction by which to increase the cost to account for waste</param>
    /// <returns>Query string for setting standard pipe factor costs</returns>
    public static string StandardPipeFactorCosts(double generalConditionsFactor, double wasteAllowanceFactor)
    {
      return "UPDATE AMStudio_PipeDetails SET StandardPipeFactorCost = DirectConstructionCost * ( 1.0 + " + generalConditionsFactor.ToString() + " + " + wasteAllowanceFactor.ToString() + ") WHERE ID < 40000000;" +

          "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(( " +
             "                               SELECT IFNULL(CapitalNonMobilization,0)* (1.0 + " + generalConditionsFactor.ToString() + " + " + wasteAllowanceFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(( " +
             "                               SELECT IFNULL(CapitalMobilizationRate,0)* (1.0 + " + generalConditionsFactor.ToString() + " + " + wasteAllowanceFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; ";
    }

    /// <summary>
    /// Sets the contingency factor
    /// </summary>
    /// <param name="contingencyFactor">Contingency factor</param>
    /// <returns>Query string for setting the contingency factor</returns>
    public static string ContingencyCost(double contingencyFactor)
    {
      return "UPDATE AMStudio_PipeDetails SET ContingencyCost = StandardPipeFactorCost * ( 1.0 + " + contingencyFactor.ToString() + ") WHERE ID < 40000000;" +
          "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(( " +
             "                               SELECT IFNULL(CapitalNonMobilization,0)* (1.0 + " + contingencyFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(( " +
             "                               SELECT IFNULL(CapitalMobilizationRate,0)* (1.0 + " + contingencyFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; ";
    }

    /// <summary>
    /// Sets the capital cost
    /// </summary>
    /// <param name="constructionManagementInspectionTestingFactor">The fraction by which to increase the cost to account for construction, management, inspection, and testing</param>
    /// <param name="designFactor">The fraction by which to increase the cost to account for design work</param>
    /// <param name="publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor">The fraction by which to increase the cost to account for PI, I and C, easement, and environmental factors</param>
    /// <param name="startupCloseoutFactor">The fraction by which to increase the cost to account for startup and closeout</param>
    /// <returns>Query string for the capital cost</returns>
    public static string SetCapitalCost(
      double constructionManagementInspectionTestingFactor,
      double designFactor,
      double publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor,
      double startupCloseoutFactor)
    {
      return "UPDATE AMStudio_PipeDetails SET CapitalCost = ContingencyCost * (1.0 + " + constructionManagementInspectionTestingFactor.ToString() +
              " + " + designFactor.ToString() + " + " + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + " + " + startupCloseoutFactor.ToString() + ");" +
          "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(( " +
             "                               SELECT IFNULL(CapitalNonMobilization,0)* (1.0 + " + constructionManagementInspectionTestingFactor.ToString() +
              " + " + designFactor.ToString() + " + " + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + " + " + startupCloseoutFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(( " +
             "                               SELECT IFNULL(CapitalMobilizationRate,0)* (1.0 + " + constructionManagementInspectionTestingFactor.ToString() +
              " + " + designFactor.ToString() + " + " + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + " + " + startupCloseoutFactor.ToString() + ") " +

        // "*" +
        // "CapitalMobilizationRate " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig'; ";
    }

    /// <summary>
    /// Sets liner traffic control costs
    /// </summary>
    /// <param name="streetTypeStreetTrafficControlCost">Cost for street-volume traffic control</param>
    /// <param name="streetTypeArterialTrafficControlCost">Cost for arterial-volume traffic control</param>
    /// <param name="streetTypeMajorArterialTrafficControlCost">Cost for major arterial-volume traffic control</param>
    /// <param name="streetTypeFreewayTrafficCost">Cost for freeway traffic control</param>
    /// <param name="daysForWholePipeLinerConstruction">The number of days to line a whole pipe</param>
    /// <returns>Query string to set the liner traffic control costs</returns>
    public static string SetLinerTrafficControl(
      double streetTypeStreetTrafficControlCost,
      double streetTypeArterialTrafficControlCost,
      double streetTypeMajorArterialTrafficControlCost,
      double streetTypeFreewayTrafficCost,
      double daysForWholePipeLinerConstruction)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    linerTrafficControl =  ( " +
             "SELECT CASE " +
             "         WHEN  pStrtText like 'F' " +
             "         THEN " + streetTypeFreewayTrafficCost.ToString() + " " +
             "         WHEN  pStrtText like 'A' " +
             "         THEN " + streetTypeArterialTrafficControlCost.ToString() + " " +
             "         WHEN  pStrtText like 'M' " +
             "         THEN  " + streetTypeMajorArterialTrafficControlCost.ToString() + " " +
             "         WHEN  pStrtText like 'S' " +
             "         THEN " + streetTypeStreetTrafficControlCost.ToString() + " " +
             "         ELSE  0 " +
             "       END * ((CASE WHEN IFNULL(uxCLx,0) <2 THEN 0 ELSE uxCLX - 2 END)/2 + 1) " + // assumes at least 2 streets at every intersection and bases cost on every additional street.
             "FROM   XPData  " +
             "WHERE  XPData.ID = AMStudio_PipeDetails.ID); " +

             "UPDATE AMStudio_PipeDetails " +
             "SET    linerTrafficControl =  IFNULL(linerTrafficControl,0) * " + daysForWholePipeLinerConstruction.ToString() +
             "       ; ";
    }

    /// <summary>
    /// Sets the bypass pumping cost
    /// </summary>
    /// <param name="daysForWholePipeLinerConstruction">The number of days to line a whole pipe</param>
    /// <returns>Query string to set the bypass pumping cost</returns>
    public static string SetLinerBypassPumping(double daysForWholePipeLinerConstruction)
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    linerBypassPumping = IFNULL(( SELECT BypassCost * " + daysForWholePipeLinerConstruction.ToString() + " " +
               "FROM   BypassPumpingUnitRates " +
               "WHERE  AMStudio_PipeDetails.BypassFlow > BypassPumpingUnitRates.BypassFlowGPM ORDER BY BypassFlowGPM DESC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the liner build duration
    /// </summary>
    /// <param name="daysForWholePipeLinerConstruction">The number of days to line a whole pipe</param>
    /// <returns>Query string to set the liner build duration</returns>
    public static string SetLinerBuildDuration(double daysForWholePipeLinerConstruction)
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    SpotLineBuildDuration = " + daysForWholePipeLinerConstruction.ToString() + ";";
    }

    /// <summary>
    /// Sets the liner lateral
    /// </summary>
    /// <returns>Query string for setting the liner lateral</returns>
    public static string SetLinerLaterals()
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    LinerLaterals = Lateral;";
    }

    /// <summary>
    /// Sets the liner pipe material cost
    /// </summary>
    /// <returns>Query string for setting liner pipe material cost</returns>
    public static string SetLinerPipeMaterial()
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    LinerPipeMaterial = Length * IFNULL(( SELECT Cost " +
               "FROM   LinerCostsTable " +
               "WHERE  AMStudio_PipeDetails.DiamWidth > LinerCostsTable.DiameterInches ORDER BY LinerCostsTable.DiameterInches DESC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the liner tv cleaning cost
    /// </summary>
    /// <returns>Query string for setting the liner tv cleaning cost</returns>
    public static string SetLinerTvCleaning()
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    LinerTVCleaning = Length * IFNULL(( SELECT Cost " +
               "FROM   LinerTVCleaningCosts " +
               "WHERE  AMStudio_PipeDetails.DiamWidth > LinerTVCleaningCosts.Diameter ORDER BY LinerTVCleaningCosts.Diameter DESC LIMIT 1),0); ";
    }

    /// <summary>
    /// Sets the liner manhole
    /// </summary>
    /// <returns>Query string to set the liner manhole</returns>
    public static string SetLinerManhole()
    {
      return "UPDATE AMStudio_PipeDetails " +
               "SET    LinerManhole = Manhole;";
    }

    /// <summary>
    /// Sets the liner direct construction cost
    /// </summary>
    /// <param name="currentEnr">The current ENR/CCI value</param>
    /// <param name="baseEnr">The base ENR/CCI value</param>
    /// <returns>The query string to set the liner direct construction cost</returns>
    public static string LinerDirectConstructionCost(double currentEnr, double baseEnr)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    LinerDirectConstructionCost = (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + ")) " +
                                             "*" +
                                             "(" +
                                               "IFNULL(LinerTrafficControl,0) " +
                                               "+ IFNULL(LinerBypassPumping, 0) " +
                                               "+ IFNULL(LinerPipeMaterial, 0) " +
                                               "+ IFNULL(LinerTVCleaning, 0) " +
                                             ") " +
             "WHERE ID < 40000000; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(( " +
             "                               SELECT (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + ")) " +
                                             "*" +
                                             "(" +
                                               "IFNULL(LinerTrafficControl,0) " +
                                               "+ IFNULL(LinerBypassPumping, 0) " +
                                               "+ IFNULL(LinerPipeMaterial, 0) " +
                                               "+ IFNULL(LinerTVCleaning, 0) " +
                                             ") " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = CapitalMobilizationRate * IFNULL(( " +
             "                               SELECT (IFNULL(DifficultArea, 1) * (" + currentEnr.ToString() + "/" + baseEnr.ToString() + "))  " +
                                             "FROM AMStudio_PipeDetails " +
                                             "WHERE AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
                                             "),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; ";
    }

    /// <summary>
    /// Sets the liner mobilization times
    /// </summary>
    /// <returns>The query string to set liner mobilization times</returns>
    public static string LinerMobilizationTimes()
    {
      return "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    MobilizationTime = IFNULL(MobilizationTime,0) + IFNULL((SELECT  CASE WHEN (IFNULL(XPData.xArt, 0) + IFNULL(XPData.xMJArt, 0)) > 0 THEN 1 ELSE 0 END  " +
             "FROM   AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "       INNER JOIN " +
             "       XPData " +
             "       ON  AMStudio_CapitalCostsMobilizationRatesAndTimes.ID = XPData.ID " +
             "           AND AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line' ),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    MobilizationTime = IFNULL([MobilizationTime], 0) * IFNULL((SELECT lateralBypass  " +
             "FROM   AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "       INNER JOIN " +
             "       AMStudio_ConstructionDurations " +
             "       ON  AMStudio_CapitalCostsMobilizationRatesAndTimes.ID = AMStudio_ConstructionDurations.ID " +
             "           AND AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line' ),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    MobilizationTime = IFNULL(MobilizationTime,0) + IFNULL((SELECT  + IFNULL(trafficControl, 0) + IFNULL(mainlineBypass, 0) " +
             "FROM   AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "       INNER JOIN " +
             "       AMStudio_ConstructionDurations " +
             "       ON  AMStudio_CapitalCostsMobilizationRatesAndTimes.ID = AMStudio_ConstructionDurations.ID " +
             "           AND AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line' ),0) " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; ";
    }

    /// <summary>
    /// Sets the liner standard pipe factor cost
    /// </summary>
    /// <param name="generalConditionsFactor">The fraction used to increase cost to account for general conditions</param>
    /// <param name="wasteAllowanceFactor">The fraction used to increase cost to account for waste</param>
    /// <returns>Query string for setting the liner standard pipe factor cost</returns>
    public static string LinerStandardPipeFactorCost(double generalConditionsFactor, double wasteAllowanceFactor)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    LinerStandardPipeFactorCost = LinerDirectConstructionCost * (1.0 + " + generalConditionsFactor.ToString() + "+" + wasteAllowanceFactor.ToString() + ") " +
             "WHERE ID < 40000000; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(CapitalNonMobilization,0) * (1.0 + " + generalConditionsFactor.ToString() + "+" + wasteAllowanceFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(CapitalMobilizationRate,0) * (1.0 + " + generalConditionsFactor.ToString() + "+" + wasteAllowanceFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; ";
    }

    /// <summary>
    /// Sets the liner contingency cost
    /// </summary>
    /// <param name="contingencyFactor">The fraction used to increase cost to account for contingency</param>
    /// <returns>Query string to update liner contingency costs</returns>
    public static string LinerContingencyCost(double contingencyFactor)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    LinerContingencyCost = LinerStandardPipeFactorCost * (1.0 + " + contingencyFactor.ToString() + ") " +
             "WHERE ID < 40000000; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(CapitalNonMobilization,0) * (1.0 + " + contingencyFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(CapitalMobilizationRate,0) * (1.0 + " + contingencyFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; ";
    }

    /// <summary>
    /// Sets the liner cost
    /// </summary>
    /// <param name="constructionManagementInspectionTestingFactor">The fraction used to increase cost to account for construction, management, inspection, and testing</param>
    /// <param name="designFactor">The fraction used to increase cost to account for design work</param>
    /// <param name="publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor">The fraction used to increase cost to account for PI, I and C, Easements, and Environmental factors</param>
    /// <param name="startupCloseoutFactor">The fraction used to increase cost to account for startup and closeout</param>
    /// <returns>Query string to update liner costs</returns>
    public static string LinerCapitalCost(
      double constructionManagementInspectionTestingFactor,
      double designFactor,
      double publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor,
      double startupCloseoutFactor)
    {
      return "UPDATE AMStudio_PipeDetails " +
             "SET    LinerContingencyCost = LinerStandardPipeFactorCost * (1.0 + "
             + constructionManagementInspectionTestingFactor.ToString() + "+"
             + designFactor.ToString() + "+"
             + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + "+"
             + startupCloseoutFactor.ToString() + ") " +
             "WHERE ID < 40000000; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalNonMobilization = IFNULL(CapitalNonMobilization,0) * (1.0 + "
             + constructionManagementInspectionTestingFactor.ToString() + "+"
             + designFactor.ToString() + "+"
             + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + "+"
             + startupCloseoutFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    CapitalMobilizationRate = IFNULL(CapitalMobilizationRate,0) * (1.0 + "
             + constructionManagementInspectionTestingFactor.ToString() + "+"
             + designFactor.ToString() + "+"
             + publicInvolvementInstrumentationAndControlsEasementEnvironmentalFactor.ToString() + "+"
             + startupCloseoutFactor.ToString() + ") " +
             "WHERE AMStudio_CapitalCostsMobilizationRatesAndTimes.ID < 40000000 " +
             "      AND " +
             "      AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Line'; ";
    }

    /// <summary>
    /// Sets the base and mobilization times
    /// </summary>
    /// <param name="workingHoursPerDay">The assumed number of working hours per day</param>
    /// <param name="daysForWholePipeLinerConstruction">The assumed number of days assumed to complete a pipe liner job</param>
    /// <returns>Query string to update the base and mobilization times</returns>
    public static string BaseTimesAndMobilizationTimes(
      double workingHoursPerDay,
      double daysForWholePipeLinerConstruction)
    {
      return "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    BaseTime =  " + (workingHoursPerDay * daysForWholePipeLinerConstruction).ToString() + " WHERE Type = 'Line';" +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    BaseTime =  ( SELECT AMStudio_PipeDetails.nonMobilizationConstructionDuration " +
             "                     FROM   AMStudio_PipeDetails " +
             "                     WHERE  AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
             "                            AND " +
             "                            AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig' " +
             "                   ) " +
             "WHERE   Type = 'Dig'; " +

             "UPDATE AMStudio_CapitalCostsMobilizationRatesAndTimes " +
             "SET    MobilizationTime =  ( SELECT AMStudio_PipeDetails.mobilizationConstructionDuration " +
             "                     FROM   AMStudio_PipeDetails " +
             "                     WHERE  AMStudio_PipeDetails.ID = AMStudio_CapitalCostsMobilizationRatesAndTimes.ID " +
             "                            AND " +
             "                            AMStudio_CapitalCostsMobilizationRatesAndTimes.Type = 'Dig' " +
             "                   ) " +
             "WHERE   Type = 'Dig'; ";
    }
  }
}
