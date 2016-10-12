// <copyright file="AmStudioPipeXpQueries.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>CostEstimatorAddIn class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading.Tasks;

  /// <summary>
  /// Query strings for PipeXP queries
  /// </summary>
  public class AmStudioPipeXpQueries
  {
    /// <summary>
    /// Prepares Pipe XP queries; this is where we make sure that we have a working table, and that
    /// it is empty and everything is as we generally should expect it to be
    /// </summary>
    /// <returns>Query string for prepping database for PipeXP</returns>
    public static string PrepPipeXp()
    {
      return "DROP TABLE IF EXISTS AMStudio_PIPEXP; " +
        "CREATE TABLE AMStudio_PIPEXP( " +
        " ID integer NOT NULL, " +
        "	GLOBALID integer NULL, " +
        "	us_node_id TEXT NULL, " +
        "	ds_node_id TEXT NULL, " +
        "	hansen_compkey integer NULL, " +
        "	xWtr integer NULL, " +
        "	xWMinD real NULL, " +
        "	xWMaxD real NULL, " +
        "	pWtr integer NULL, " +
        "	pWtrMaxD real NULL, " +
        "	pFt2Wtr real NULL, " +
        "	xSewer integer NULL, " +
        "	xSwrMinD real NULL, " +
        "	xSwrMaxD real NULL, " +
        "	pSewer integer NULL, " +
        "	pSwrMaxD real NULL, " +
        "	pFt2Swr real NULL, " +
        "	xStrt integer NULL, " +
        "	xArt integer NULL, " +
        "	xMJArt integer NULL, " +
        "	xFrwy integer NULL, " +
        "	pStrt integer NULL, " +
        "	pStrtTyp integer NULL, " +
        "   pStrtText TEXT NULL, " +
        "	pFt2Strt real NULL, " +
        "	pTraffic integer NULL, " +
        "	uxCLx integer NULL, " +
        "	uxFt2CLx real NULL, " +
        "	dxCLx integer NULL, " +
        "	dxFt2CLx real NULL, " +
        "	xFiber integer NULL, " +
        "	pFiber integer NULL, " +
        "	pFt2Fiber real NULL, " +
        "	xGas integer NULL, " +
        "	pGas integer NULL, " +
        "	pFt2Gas real NULL, " +
        "	xRail integer NULL, " +
        "	pRail integer NULL, " +
        "	pFt2Rail real NULL, " +
        "	xLRT integer NULL, " +
        "	pLRT integer NULL, " +
        "	pFt2LRT real NULL, " +
        "	xEmt integer NULL, " +
        "	pEmt integer NULL, " +
        "	pFt2Emt real NULL, " +
        "	xEzonC integer NULL, " +
        "	xEzonP integer NULL, " +
        "	xFtEzonC real NULL, " +
        "	xFtEzonP real NULL, " +
        "	xEzAreaC real NULL, " +
        "	xEzAreaP real NULL, " +
        "	uxMS4 integer NULL, " +
        "	uxUIC integer NULL, " +
        "	uDepth real NULL, " +
        "	dDepth real NULL, " +
        "	xPipSlope real NULL, " +
        "	gSlope real NULL, " +
        "	xEcsi integer NULL, " +
        "	xFt2Ecsi real NULL, " +
        "	xEcsiLen real NULL, " +
        "	xEcsiVol real NULL, " +
        "	xSchl integer NULL, " +
        "	xFt2Schl real NULL, " +
        "	xHosp integer NULL, " +
        "	xFt2Hosp real NULL, " +
        "	xPol integer NULL, " +
        "	xFt2Pol real NULL, " +
        "	xFire integer NULL, " +
        "	xFt2Fire real NULL, " +
        "	xBldg integer NULL, " +
        "	xFt2Bldg real NULL, " +
        "	xHyd integer NULL, " +
        "	xFt2Hyd real NULL, " +
        "	HardArea integer NULL, " +
        "	Length real NULL, " +
        "	ParentLength real NULL, " +
        "	DiamWidth real NULL, " +
        "	Height real NULL, " +
        "	Cutno integer NULL, " +
        "	Poverty integer NULL, " +
        "	xSFR integer NULL, " +
        "	LateralCount integer NULL); " +
        
        // Dont forget the indexes
        " CREATE UNIQUE INDEX IDX_PIPEXP_ID ON AMStudio_PIPEXP (ID); " +
        " CREATE INDEX IDX_PIPEXP_COMPKEY ON AMStudio_PIPEXP (hansen_compkey); " +
        "CREATE INDEX IDX_PIPEXP_GLOBALID ON AMStudio_PIPEXP (GLOBALID); ";

      // Now I feel like something is missing?  shape data?  I think we take care of that in the Segments table, so we should be good.
    }

    /// <summary>
    /// This is where we transfer the base data to the working table
    /// </summary>
    /// <param name="maxBoundary">Envelope maximum boundary</param>
    /// <returns>Query string for transferring base data</returns>
    public static string TransferBase(double maxBoundary)
    {
      return "INSERT INTO AMStudio_PIPEXP (ID, GLOBALID, us_node_id, ds_node_id, hansen_compkey, Length, ParentLength, DiamWidth, Height, Cutno) " +
        "SELECT OBJECTID, GLOBALID, us_node_id, ds_node_id, hansen_compkey, length, ParentLength, PIPESIZE, PipeHeight, CutNo " +
        "FROM REHABSegments;" +

        // Drop a buffer of the largest distance into this shape.
        "DROP TABLE IF EXISTS PipeArea; " +
        "CREATE TABLE PipeArea (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'PipeArea', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO PipeArea(OBJECTID, Shape) SELECT  1, ST_Envelope(ST_Buffer(st_aggr_Union(Shape)," + maxBoundary.ToString() + ")) FROM REHABSegments; " +

        // Drop a aggregate of all the pipes as well
        "DROP TABLE IF EXISTS PipeAgg; " +
        "CREATE TABLE PipeAgg (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'PipeAgg', 'Shape', 2913, 'multilinestring', 'xy', 'null'); " +
        "INSERT INTO PipeAgg(OBJECTID, Shape) SELECT  1, st_aggr_Union(Shape) FROM REHABSegments; ";
    }

    /// <summary>
    /// Sets Proximity to hard areas
    /// </summary>
    /// <returns>Query string setting Proximity to hard areas</returns>
    public static string ProximityToHardAreas()
    {
      // First we create the table that will hold the results of the spatial query
      return "DROP TABLE IF EXISTS ResultsHardAreas; " +
        "CREATE TABLE ResultsHardAreas AS " +
        "SELECT  A.OBJECTID AS OBJECTID, COUNT(*) AS theCount  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PullTables.HardAreas AS B " +
        "        ON ST_EnvIntersects(A.Shape, B.Shape) = 1 " +
        "GROUP BY A.OBJECTID; " +
        "CREATE INDEX IDX_ResultsHardAreas ON ResultsHardAreas(OBJECTID); " +

        // Then we update the main table
        "UPDATE AMStudio_PIPEXP " +
        "SET HardArea = IFNULL(( " +
        "SELECT  theCount " +
        "FROM    ResultsHardAreas AS A " +
        "WHERE   A.OBJECTID = AMStudio_PIPEXP.ID ),0); " +

        // Then we drop the intersection table
        "DROP INDEX IDX_ResultsHardAreas; " +
        "DROP TABLE ResultsHardAreas;";
        /*"UPDATE AMStudio_PIPEXP " +
        "SET HardArea = IFNULL(( " +
        "SELECT  COUNT(*) AS theCount  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PullTables.HardAreas AS B " +
        "        ON ST_Intersects(A.Shape, B.Shape) = 1 " +
        "WHERE   A.OBJECTID = AMStudio_PIPEXP.ID "+
        "GROUP BY A.OBJECTID ),0); ";*/
    }

    /// <summary>
    /// Sets Proximity to hydrants
    /// </summary>
    /// <param name="maxDistanceToHydrant">Maximum distance to detect hydrants</param>
    /// <returns>Query string to set Proximity to hydrants</returns>
    public static string ProximityToHydrantsPdx(double maxDistanceToHydrant)
    {
      // First we create the table that will hold the results of the spatial query
      return
        /*"DROP TABLE IF EXISTS HydrantsClose; " +
        "CREATE TABLE HydrantsClose (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'HydrantsClose', 'Shape', 2913, 'point', 'xy', 'null'); " +
        "INSERT INTO HydrantsClose (OBJECTID, Shape) SELECT A.OBJECTID, A.Shape FROM Hydrants_pdx AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS HydrantsBuffer; " +
        "CREATE TABLE HydrantsBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'HydrantsBuffer', 'Shape', 2913, 'polygon', 'xy', 'null'); " +
        "INSERT INTO HydrantsBuffer (OBJECTID, Shape) SELECT OBJECTID, ST_Buffer(Shape, " + maxDistanceToHydrant.ToString() + ") FROM HydrantsClose ;" +*/
        CreateFoundationTables("Hydrants_pdx", "Hydrants_pdx", "point", maxDistanceToHydrant.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xHyd = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Hydrants_pdxIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE    ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        // And the minimum distances
        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Hyd = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Hydrants_pdxInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.xHyd > 0 AND AMStudio_PIPEXP.ID = A.OBJECTID " +
        "GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to buildings
    /// </summary>
    /// <param name="maxDistanceToBuilding">Maximum distance to detect buildings</param>
    /// <returns>Query string to set Proximity to buildings</returns>
    public static string ProximityToBuildings(double maxDistanceToBuilding)
    {
      // First we create the table that will hold the results of the spatial query
      return
        CreateFoundationTables("BLDGS", "BLDGS", "multipolygon", maxDistanceToBuilding.ToString()) +

        /*"DROP TABLE IF EXISTS BLDGSBuffer; " +
        "CREATE TABLE BLDGSBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'BLDGSBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO BLDGSBuffer (OBJECTID, Shape) SELECT OBJECTID, ST_mpolyfromText(ST_ASText(Shape), 2913) FROM BLDGS; " +
        */
        "UPDATE AMStudio_PIPEXP " +
        "SET xBldg = IFNULL(( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        BLDGSIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE    ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID),0); " +

        // And the minimum distances
        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Bldg = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        BLDGSInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.xBldg > 0 AND AMStudio_PIPEXP.ID = A.OBJECTID " +
        "GROUP BY A.OBJECTID); ";

        // "DROP TABLE BLDGSBuffer; ";
    }

    /// <summary>
    /// Sets depths and slopes
    /// </summary>
    /// <returns>Query string setting depths and slopes</returns>
    public static string SetDepthAndSlopes()
    {
      return 
        "UPDATE AMStudio_PIPEXP " +
        "SET    uDepth =  " +
        "       ( " +
        "         SELECT  FRM_DEPTH " +
        "         FROM    REHABSegments " +
        "         WHERE   REHABSegments.OBJECTID = AMStudio_PIPEXP.ID " +
        "       ); " +
        "UPDATE AMStudio_PIPEXP " +
        "SET    dDepth =  " +
        "       ( " +
        "         SELECT  TO_DEPTH " +
        "         FROM    REHABSegments " +
        "         WHERE   REHABSegments.OBJECTID = AMStudio_PIPEXP.ID " +
        "       ); " +
        "UPDATE AMStudio_PIPEXP " +
        "SET    gSlope =  " +
        "       ( " +
        "         SELECT  CASE " +
        "                  WHEN [length] = 0 OR [length] IS NULL " +
        "                  THEN NULL " +
        "                  ELSE ((FRM_ELEV+FRM_DEPTH)-(TO_ELEV+TO_DEPTH))/[length]  " +
        "                END AS gSlope " +
        "         FROM    REHABSegments " +
        "         WHERE   REHABSegments.OBJECTID = AMStudio_PIPEXP.ID " +
        "       ); " +
        "UPDATE AMStudio_PIPEXP " +
        "SET    xPipSlope =  " +
        "       ( " +
        "         SELECT  CASE " +
        "                  WHEN [length] = 0 OR [length] IS NULL " +
        "                  THEN NULL " +
        "                  ELSE(FRM_ELEV-TO_ELEV)/[length]  " +
        "                END AS xPipSlope   " +
        "         FROM    REHABSegments " +
        "         WHERE   REHABSegments.OBJECTID = AMStudio_PIPEXP.ID " +
        "       ); ";
    }

    /// <summary>
    /// Sets Proximity to UIC
    /// </summary>
    /// <returns>Query string setting Proximity to UIC</returns>
    public static string ResultsUIC()
    {
      return
        "DROP TABLE IF EXISTS UICClose; " +
        "CREATE TABLE UICClose (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'UICClose', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO UICClose (OBJECTID, Shape) SELECT A.ID, A.Shape FROM UIC AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;" +

        "UPDATE AMStudio_PIPEXP " +
        "SET uxUIC = ( " +
        "SELECT  1  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        UICClose AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE    ST_Intersects(ST_startpoint(ST_geometryn(A.Shape, 1)), B.Shape) = 1); ";
    }

    /// <summary>
    /// Sets Proximity to MS4
    /// </summary>
    /// <returns>Query string setting Proximity to MS4</returns>
    public static string ResultsMS4()
    {
      // Don't forget when importing this layer to ensure boundary type IN ('MS4', 'Other')
      return
        "DROP TABLE IF EXISTS MS4Close; " +
        "CREATE TABLE MS4Close (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'MS4Close', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO MS4Close (OBJECTID, Shape) SELECT A.ID, A.Shape FROM MS4 AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;" +

        "UPDATE AMStudio_PIPEXP " +
        "SET uxMS4 = ( " +
        "SELECT  1  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        MS4Close AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE    ST_Intersects(ST_startpoint(ST_geometryn(A.Shape, 1)), B.Shape) = 1); ";
    }

    /// <summary>
    /// Sets Proximity to emergency routes
    /// </summary>
    /// <param name="distanceToEMT">Maximum distance to detect EMT route</param>
    /// <returns>Query string setting Proximity to emergency routes</returns>
    public static string ResultsEMT(double distanceToEMT)
    {
      return
        /*"DROP TABLE IF EXISTS EMTClose; " +
        "CREATE TABLE EMTClose (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'EMTClose', 'Shape', 2913, 'multilinestring', 'xy', 'null'); " +
        "INSERT INTO EMTClose (OBJECTID, Shape) SELECT A.ID, A.Shape FROM EMT AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS EMTBuffer; " +
        "CREATE TABLE EMTBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'EMTBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO EMTBuffer (OBJECTID, Shape) SELECT OBJECTID, ST_Buffer(Shape, " + distanceToEMT.ToString() + ") FROM EMTClose ;" +*/
        CreateFoundationTables("EMT", "EMT", "multilinestring", distanceToEMT.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEMT = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        EMTInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pEMT = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        EMTIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2EMT = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        EMTInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.pEMT > 0 OR AMStudio_PIPEXP.xEMT > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to fire stations
    /// </summary>
    /// <param name="distanceToFireStation">Maximum distance to detect fire station</param>
    /// <returns>Query string to set Proximity to fire stations</returns>
    public static string ResultsFire(double distanceToFireStation)
    {
      // [COSTEST_FIRE_STATIONS_METRO]
      return
        /*"DROP TABLE IF EXISTS FireClose; " +
        "CREATE TABLE FireClose (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'FireClose', 'Shape', 2913, 'point', 'xy', 'null'); " +
        "INSERT INTO FireClose (OBJECTID, Shape) SELECT A.ID, A.Shape FROM FireStations AS A INNER JOIN PipeArea AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS FireBuffer; " +
        "CREATE TABLE FireBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, 'FireBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO FireBuffer (OBJECTID, Shape) SELECT OBJECTID, ST_Buffer(Shape, " + distanceToFireStation.ToString() + ") FROM FireClose ;" +*/
        CreateFoundationTables("FireStations", "FireStations", "multilinestring", distanceToFireStation.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFire = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        FireStationsIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(B.Shape, A.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Fire = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        FireStationsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xFire > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Creates foundation table
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <param name="geometryType">Geometry type to use</param>
    /// <param name="distance">Buffer distance in feet</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTables(
      string tableName,
      string tablePrefix,
      string geometryType,
      string distance)
    {
      return

        // tableClose is the objects in the general area
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Close', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Close (OBJECTID, Shape) SELECT A.ID, A.Shape FROM " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        // tableBuffer is the buffered objects in the general area
        "DROP TABLE IF EXISTS " + tablePrefix + "Buffer; " +
        "CREATE TABLE " + tablePrefix + "Buffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Buffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Buffer (OBJECTID, Shape) SELECT OBJECTID, ST_Buffer(Shape, " + distance + ") FROM " + tablePrefix + "Close ;" +

        // tableIntersectsBuffer is the buffered objects that are within the buffer distance
        "DROP TABLE IF EXISTS " + tablePrefix + "IntersectsBuffer; " +
        "CREATE TABLE " + tablePrefix + "IntersectsBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "IntersectsBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "IntersectsBuffer (OBJECTID, Shape) SELECT A.OBJECTID, A.Shape FROM " + tableName + "Buffer AS A INNER JOIN PipeAgg AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        // tableInBuffer is the unbuffered objects that are within the buffer distance
        "DROP TABLE IF EXISTS " + tablePrefix + "InBuffer; " +
        "CREATE TABLE " + tablePrefix + "InBuffer (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "InBuffer', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "InBuffer (OBJECTID, Shape) SELECT A.OBJECTID, A.Shape FROM " + tableName + "Close AS A INNER JOIN " + tableName + "IntersectsBuffer AS B ON A.OBJECTID = B.OBJECTID;";
    }

    /// <summary>
    /// Creates foundation table
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <param name="geometryType">Geometry type to use</param>
    /// <param name="distance">Buffer distance in feet</param>
    /// <param name="columnToKeep">Column to keep</param>
    /// <param name="columnType">Column type</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTables(
      string tableName,
      string tablePrefix,
      string geometryType,
      string distance,
      string columnToKeep,
      string columnType)
    {
      return
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close (OBJECTID integer, " + columnToKeep + " " + columnType + "); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Close', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Close (OBJECTID, " + columnToKeep + ", Shape) SELECT A.ID, A." + columnToKeep + ", A.Shape FROM " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS " + tablePrefix + "Buffer; " +
        "CREATE TABLE " + tablePrefix + "Buffer (OBJECTID integer, " + columnToKeep + " " + columnType + "); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Buffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Buffer (OBJECTID, " + columnToKeep + ", Shape) SELECT OBJECTID, " + columnToKeep + ", ST_Buffer(Shape, " + distance + ") FROM " + tablePrefix + "Close ;" +

        // tableIntersectsBuffer is the buffered objects that are within the buffer distance
        "DROP TABLE IF EXISTS " + tablePrefix + "IntersectsBuffer; " +
        "CREATE TABLE " + tablePrefix + "IntersectsBuffer (OBJECTID integer, " + columnToKeep + " " + columnType + "); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "IntersectsBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "IntersectsBuffer (OBJECTID, " + columnToKeep + ",Shape) SELECT A.OBJECTID, " + columnToKeep + ", A.Shape FROM " + tablePrefix + "Buffer AS A INNER JOIN PipeAgg AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        // tableInBuffer is the unbuffered objects that are within the buffer distance
        "DROP TABLE IF EXISTS " + tablePrefix + "InBuffer; " +
        "CREATE TABLE " + tablePrefix + "InBuffer (OBJECTID, " + columnToKeep + " integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "InBuffer', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "InBuffer (OBJECTID," + columnToKeep + ", Shape) SELECT A.OBJECTID, A." + columnToKeep + ", A.Shape FROM " + tablePrefix + "Close AS A INNER JOIN " + tablePrefix + "IntersectsBuffer AS B ON A.OBJECTID = B.OBJECTID;";
    }

    /// <summary>
    /// Creates foundation table
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <param name="geometryType">Geometry type to use</param>
    /// <param name="columnToKeep">Column to keep</param>
    /// <param name="columnType">Column type</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTables(
      string tableName,
      string tablePrefix,
      string geometryType,
      string columnToKeep,
      string columnType)
    {
      return
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close (OBJECTID integer, " + columnToKeep + " " + columnType + "); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Close', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Close (OBJECTID, " + columnToKeep + ", Shape) SELECT A.ID, A." + columnToKeep + ", A.Shape FROM " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;";
    }

    /// <summary>
    /// Creates foundation table
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <param name="geometryType">Geometry type to use</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTables(
      string tableName,
      string tablePrefix,
      string geometryType)
    {
      return
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close (OBJECTID integer); " +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Close', 'Shape', 2913, '" + geometryType + "', 'xy', 'null'); " +
        "INSERT INTO " + tablePrefix + "Close (OBJECTID, Shape) SELECT A.ID, A.Shape FROM " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;";
    }

    /// <summary>
    /// Creates foundation table (close)
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTablesLazyClose(string tableName, string tablePrefix)
    {
      return
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close AS SELECT A.* FROM  " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;";
    }

    /// <summary>
    /// Creates foundation table (close)
    /// </summary>
    /// <param name="tableName">Name of table to pull from</param>
    /// <param name="tablePrefix">Prefix of table</param>
    /// <param name="distance">Max distance to use to detect close items</param>
    /// <returns>Query string to create foundation table</returns>
    public static string CreateFoundationTablesLazyDistance(
      string tableName,
      string tablePrefix,
      string distance)
    {
      return
        "DROP TABLE IF EXISTS " + tablePrefix + "Close; " +
        "CREATE TABLE " + tablePrefix + "Close AS SELECT A.* FROM  " + tableName + " AS A INNER JOIN PipeArea AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS " + tablePrefix + "Buffer; " +
        "CREATE TABLE " + tablePrefix + "Buffer AS SELECT * FROM  " + tableName + "Close;" +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "Buffer', 'ShapeBuffer', 2913, 'multipolygon', 'xy', 'null'); " +
        "UPDATE " + tableName + "Buffer SET ShapeBuffer = ST_Buffer(Shape, " + distance + ");" +

        "DROP TABLE IF EXISTS " + tablePrefix + "IntersectsBuffer; " +
        "CREATE TABLE " + tablePrefix + "IntersectsBuffer AS SELECT A.* FROM  " + tableName + "Buffer AS A INNER JOIN PipeAgg AS B ON ST_Intersects(A.Shape, B.Shape) = 1;" +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "IntersectsBuffer', 'ShapeIntersectsBuffer', 2913, 'multipolygon', 'xy', 'null'); " +
        "UPDATE " + tableName + "IntersectsBuffer SET ShapeIntersectsBuffer = ShapeBuffer;" +

        // tableInBuffer is the unbuffered objects that are within the buffer distance
        "DROP TABLE IF EXISTS " + tablePrefix + "InBuffer; " +
        "CREATE TABLE " + tablePrefix + "InBuffer AS SELECT A.*, B.ShapeIntersectsBuffer FROM " + tableName + "Close AS A INNER JOIN " + tableName + "IntersectsBuffer AS B ON A.OBJECTID = B.OBJECTID;" +
        "SELECT AddGeometryColumn(null, '" + tablePrefix + "InBuffer', 'ShapeInBuffer', 2913, 'multipolygon', 'xy', 'null'); " +
        "UPDATE " + tableName + "InBuffer SET ShapeInBuffer = (SELECT Shape FROM " + tablePrefix + "Close WHERE " + tablePrefix + "Close.OBJECTID =  " + tableName + "InBuffer.OBJECTID);";
    }

    /// <summary>
    /// Sets Proximity to police facilities
    /// </summary>
    /// <param name="distanceToPoliceFacility">Maximum distance to detect police facility</param>
    /// <returns>Query string to set Proximity to police facilities</returns>
    public static string ResultsPolice(double distanceToPoliceFacility)
    {
      // COSTEST_PORTLAND_POLICE_FACILITIES_PDX
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Police", "Police", "point", distanceToPoliceFacility.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xPol = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PoliceIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Pol = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PoliceInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xPol > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to hospitals
    /// </summary>
    /// <param name="distanceToHospital">Maximum distance to detect hospital</param>
    /// <returns>Query string to set Proximity to hospital</returns>
    public static string ResultsHospital(double distanceToHospital)
    {
      // COSTEST_Hospitals_Metro
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Hospitals", "Hospitals", "point", distanceToHospital.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xHosp = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        HospitalsIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Hosp = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        HospitalsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xHosp > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to schools
    /// </summary>
    /// <param name="distanceToSchool">Maximum distance to detect school</param>
    /// <returns>Query string to set Proximity to schools</returns>
    public static string ResultsSchool(double distanceToSchool)
    {
      // COSTEST_Schools_Metro
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Schools", "Schools", "point", distanceToSchool.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xSchl = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SchoolsIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFt2Schl = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SchoolsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xSchl > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to ECSI/LUST (underground contamination sites)
    /// </summary>
    /// <returns>Query string to set Proximity to ECSI/LUST sites</returns>
    public static string ResultsECSI()
    {
      // COSTEST_ECSILust
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("ECSILust", "ECSI", "multipolygon") +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEcsi = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        ECSIClose AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        // This may encounter trouble for elements that do interesect tangentially
        "UPDATE AMStudio_PIPEXP " +
        "SET xECSILen = ( " +
        "SELECT  SUM(ST_Length(ST_Intersection(A.Shape, B.Shape)))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        ECSIClose AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xEcsi > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to residential zone
    /// </summary>
    /// <returns>Query string to set Proximity to residential zone</returns>
    public static string ResultsResidential()
    {
      // COSTEST_ZONING_PDX Residential
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Residential", "Residential", "multipolygon") +

        "UPDATE AMStudio_PIPEXP " +
        "SET xSFR = ( " +
        "SELECT 1  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        ResidentialClose AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1); ";
    }

    /// <summary>
    /// Sets Proximity to environmental protection zone
    /// </summary>
    /// <param name="protectionZoneBuffer">Distance of buffer around pipes to use</param>
    /// <returns>Query string setting Proximity to environmental protection zone</returns>
    public static string ResultsPZone(double protectionZoneBuffer)
    {
      // COSTEST_ZONING_PDX PZone
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("PZone", "PZone", "multipolygon", protectionZoneBuffer.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEzonP = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PZoneInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        // This may encounter trouble for elements that do interesect tangentially
        "UPDATE AMStudio_PIPEXP " +
        "SET xFtEzonP = ( " +
        "SELECT  SUM(ST_Length(ST_Intersection(A.Shape, B.Shape)))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PZoneInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xEZonP > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEzAreaP = ( " +
        "SELECT  SUM(ST_Area(ST_Intersection(ST_Buffer(A.Shape," + protectionZoneBuffer.ToString() + "), B.Shape)))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PZoneInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xEZonP > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to environmental conservation zone
    /// </summary>
    /// <param name="conservationZoneBuffer">Distance of buffer around pipes to use</param>
    /// <returns>Query string setting Proximity to environmental conservation zone</returns>
    public static string ResultsCZone(double conservationZoneBuffer)
    {
      // COSTEST_ZONING_PDX CZone
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("CZone", "CZone", "multipolygon", conservationZoneBuffer.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEzonC = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        CZoneIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        // This may encounter trouble for elements that do interesect tangentially
        "UPDATE AMStudio_PIPEXP " +
        "SET xFtEzonC = IFNULL(( " +
        "SELECT  SUM(ST_Length(ST_Intersection(A.Shape, B.Shape)))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        CZoneInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xEzonC > 0 GROUP BY A.OBJECTID),0); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xEzAreaC = IFNULL(( " +
        "SELECT  SUM(ST_Area(ST_Intersection(ST_Buffer(A.Shape," + conservationZoneBuffer.ToString() + "), B.Shape)))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        CZoneIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.xEzonC > 0 GROUP BY A.OBJECTID),0); ";
    }

    /// <summary>
    /// Sets Proximity to gas lines
    /// </summary>
    /// <param name="distanceToGas">Maximum distance to detect gas line</param>
    /// <returns>Query string setting Proximity to gas lines</returns>
    public static string ResultsGas(double distanceToGas)
    {
      // COSTEST_MAJOR_GAS_LINES_METRO
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("GasLines", "GasLines", "multilinestring", distanceToGas.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xGas = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        GasLinesInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pGas = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        GasLinesIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Gas = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        GasLinesInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.pGas > 0 OR AMStudio_PIPEXP.xGas > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to fiber optic lines
    /// </summary>
    /// <param name="distanceToFiber">Maximum distance to detect fiber optic lines</param>
    /// <returns>Query string setting Proximity to fiber optic lines</returns>
    public static string ResultsFiber(double distanceToFiber)
    {
      // COSTEST_FIBER_ROUTES_PDX
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("FiberRoutes", "FiberRoutes", "multilinestring", distanceToFiber.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFiber = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        FiberRoutesInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFiber = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        FiberRoutesIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Fiber = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        FiberRoutesInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.pFiber > 0 OR AMStudio_PIPEXP.xFiber > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to light rail transit lines
    /// </summary>
    /// <param name="distanceToLrt">Maximum distance to detect LRT lines</param>
    /// <returns>Query string setting Proximity to LRT lines</returns>
    public static string ResultsLrt(double distanceToLrt)
    {
      // COSTEST_LIGHT_RAIL_LINES_METRO_Segs
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("LightRail", "LightRail", "multilinestring", distanceToLrt.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xLRT = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        LightRailInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pLRT = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        LightRailIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2LRT = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        LightRailInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.pLRT > 0 OR AMStudio_PIPEXP.xLRT > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to railroads
    /// </summary>
    /// <param name="distanceToRail">Maximum distance to detect railroad lines</param>
    /// <returns>Query string setting Proximity to railroads</returns>
    public static string ResultsRail(double distanceToRail)
    {
      // COSTEST_RAILROADS_METRO
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Railroads", "Railroads", "multilinestring", distanceToRail.ToString()) +

        "UPDATE AMStudio_PIPEXP " +
        "SET xRail = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        RailroadsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pRail = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        RailroadsIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Rail = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        RailroadsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.pRail > 0 OR AMStudio_PIPEXP.xRail > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets Proximity to street intersections
    /// </summary>
    /// <param name="distanceToIntersection">Maximum distance to detect intersections</param>
    /// <returns>Query string setting Proximity to street intersections</returns>
    public static string ResultsIntersections(double distanceToIntersection)
    {
      // COSTEST_STREET_INTERSECTIONS
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Street_Intersections", "Street_Intersections", "point", distanceToIntersection.ToString(), "countStreets", "integer") +

        "UPDATE AMStudio_PIPEXP " +
        "SET uxCLx = ( " +
        "SELECT  MAX(countStreets)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Street_IntersectionsIntersectsBuffer AS B " +
        "        ON ST_Intersects(A.Shape, B.Shape) = 1  " +
        "WHERE   AMStudio_PIPEXP.ID = A.OBJECTID GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET uxFt2CLx = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Street_IntersectionsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "WHERE   AMStudio_PIPEXP.uxCLx > 0 GROUP BY A.OBJECTID); ";
    }

    /// <summary>
    /// Sets street type on which pipe lies
    /// </summary>
    /// <param name="distanceToCenterline">Maximum distance to street centerline to analyze</param>
    /// <returns>Query string setting street type</returns>
    public static string ResultsStreetType(double distanceToCenterline)
    {
      // [COSTEST_STREETS_PDX]
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Streets", "Streets", "multilinestring", distanceToCenterline.ToString(), "Value", "integer") +

        "UPDATE AMStudio_PIPEXP " +
        "SET pStrtTyp = ( " +
        "SELECT  MIN(Value)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        StreetsIntersectsBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND ST_Intersects(A.Shape, B.Shape) = 1 GROUP BY A.OBJECTID) ; " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pStrt = 1 WHERE pStrtTyp > 0; " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pStrtText = ( " +
        "SELECT  Type  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        StreetType AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            B.StreetValue = AMStudio_PIPEXP.pStrtTyp " +
        "WHERE   AMStudio_PIPEXP.pStrtTyp > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Strt = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        StreetsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            AMStudio_PIPEXP.pStrtTyp > 0  GROUP BY A.OBJECTID); ";
        /*
        "DROP TABLE IF EXISTS PipeGroup; " +
        "CREATE TABLE PipeGroup AS " +
        "SELECT  1 AS OBJECTID, st_aggr_convexhull(Shape) AS [Shape]  " +
        "FROM    REHABSegments " +
        "; " +
        "CREATE INDEX IDX_PipeGroup ON PipeGroup(OBJECTID); " +

        "DROP TABLE IF EXISTS MatchingStreets; " +
        "CREATE TABLE MatchingStreets AS " +
        "SELECT  B.ID AS ID, Value, B.Shape " +
        "FROM    PipeGroup AS A " +
        "        INNER JOIN " +
        "        PullTables.Streets AS B " +
        "        ON ST_Intersects(ST_Buffer(A.Shape, " + distanceToCenterline.ToString() + "), B.Shape) = 1 " +
        "; " +
        "CREATE INDEX IDX_MatchingStreets ON MatchingStreets(ID); "+

        "DROP TABLE IF EXISTS ResultsStreets; " +
        "CREATE TABLE ResultsStreets AS " +
        "SELECT  A.OBJECTID AS OBJECTID, Value, ST_Distance(A.Shape, B.Shape) AS [Distance]  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        MatchingStreets AS B " +
        "        ON ST_Intersects(ST_Buffer(A.Shape, " + distanceToCenterline.ToString() + "), B.Shape) = 1 " +
        "; " +
        "CREATE INDEX IDX_ResultsStreets ON ResultsStreets(OBJECTID); "+
               
        "UPDATE AMStudio_PIPEXP " +
        "SET pStrtTyp = ( " +
        "SELECT  MIN(Value)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        ResultsStreets AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  A.OBJECTID = B.OBJECTID AND B.Distance <= " + distanceToCenterline.ToString() + " GROUP BY A.OBJECTID) ; "+
                
        "UPDATE AMStudio_PIPEXP " +
        "SET pStrtText = ( " +
        "SELECT  Type  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        StreetType AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            B.StreetValue = AMStudio_PIPEXP.pStrtTyp " +
        "WHERE   AMStudio_PIPEXP.pStrtTyp > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Strt = ( " +
        "SELECT  MIN(Distance)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        ResultsStreets AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            AMStudio_PIPEXP.pStrtTyp > 0 " +
        "WHERE   A.OBJECTID = B.OBJECTID AND B.Distance <= " + distanceToCenterline.ToString() + " GROUP BY A.OBJECTID); ";
        */
        /*"UPDATE AMStudio_PIPEXP " +
        "SET pStrtTyp = ( " +
        "SELECT  MIN(Value)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Streets AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  ST_Intersects(ST_Buffer(A.Shape, " + distanceToCenterline.ToString() + "), B.Shape) = 1 GROUP BY A.OBJECTID) ; "+
                
        "UPDATE AMStudio_PIPEXP " +
        "SET pStrtText = ( " +
        "SELECT  Type  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        StreetType AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            B.StreetValue = AMStudio_PIPEXP.pStrtTyp " +
        "WHERE   AMStudio_PIPEXP.pStrtTyp > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Strt = ( " +
        "SELECT  MIN(ST_Distance(A.Shape, B.Shape))  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        Streets AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "            AND " +
        "            AMStudio_PIPEXP.pStrtTyp > 0 " +
        "WHERE   ST_Intersects(ST_Buffer(A.Shape, " + distanceToCenterline.ToString() + "), B.Shape) = 1 GROUP BY A.OBJECTID); ";*/
    }

    /// <summary>
    /// Sets how many streets are crossed
    /// </summary>
    /// <returns>Query string setting how many streets are crossed</returns>
    public static string ResultsXStreet()
    {
      // COSTEST_STREETS_PDX
      return

        // create xClose, xBuffer tables
        CreateFoundationTables("Streets", "xStreets", "multilinestring", "0", "Type", "text") +

        "UPDATE AMStudio_PIPEXP " +
        "SET xStrt = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        xStreetsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  ST_Intersects(A.Shape, B.Shape) = 1 AND B.type = 'S' GROUP BY A.OBJECTID) ; " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xArt = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        xStreetsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  ST_Intersects(A.Shape, B.Shape) = 1 AND B.type = 'A' GROUP BY A.OBJECTID) ; " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xMjArt = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        xStreetsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  ST_Intersects(A.Shape, B.Shape) = 1 AND B.type = 'M' GROUP BY A.OBJECTID) ; " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xFrwy = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        xStreetsInBuffer AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "AND  ST_Intersects(A.Shape, B.Shape) = 1 AND B.type = 'F' GROUP BY A.OBJECTID) ; ";
    }

    /// <summary>
    /// Sets how many sewers are crossed or are close by, and what the diameter ranges of those pipes are
    /// </summary>
    /// <param name="distanceToSewer">Maximum distance to detect nearby sewers</param>
    /// <returns>Query string setting sewer crossings, close-bys, and diameter ranges</returns>
    public static string ResultsSewer(double distanceToSewer)
    {
      // Sewer XP
      return
        "DROP TABLE IF EXISTS BESPipesClose; " +
        "CREATE TABLE BESPipesClose (OBJECTID integer, PIPESIZE real, us_node_id TEXT, ds_node_id TEXT, hansen_compkey integer); " +
        "SELECT AddGeometryColumn(null, 'BESPipesClose', 'Shape', 2913, 'multilinestring', 'xy', 'null'); " +
        "INSERT INTO BESPipesClose (OBJECTID, PIPESIZE, us_node_id, ds_node_id, hansen_compkey, Shape) SELECT A.OBJECTID, A.PIPESIZE, A.us_node_id, A.ds_node_id, A.hansen_compkey, A.Shape FROM BESPipes AS A INNER JOIN PipeArea AS B ON ST_EnvIntersects(A.Shape, B.Shape) = 1;" +

        "DROP TABLE IF EXISTS BESPipesBuffer; " +
        "CREATE TABLE BESPipesBuffer (OBJECTID integer, PIPESIZE real, us_node_id TEXT, ds_node_id TEXT, hansen_compkey integer); " +
        "SELECT AddGeometryColumn(null, 'BESPipesBuffer', 'Shape', 2913, 'multipolygon', 'xy', 'null'); " +
        "INSERT INTO BESPipesBuffer (OBJECTID, PIPESIZE, us_node_id, ds_node_id, hansen_compkey, Shape) SELECT A.OBJECTID, A.PIPESIZE, A.us_node_id, A.ds_node_id, A.hansen_compkey, ST_Buffer(A.Shape, " + distanceToSewer.ToString() + ") FROM BESPipesClose AS A; " +

        "DROP TABLE IF EXISTS SewerXP; " +
        "CREATE TABLE SewerXP AS " +
        "SELECT  A.OBJECTID AS AOBJECTID, B.OBJECTID AS BOBJECTID, B.PIPESIZE AS pSwrD , NULL AS pFt2Swr, A.us_node_id as au, A.ds_node_id AS ad, B.us_node_id AS bu, B.ds_node_id AS bd, A.Shape AS AShape, NULL AS BShape " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        BESPipesBuffer AS B " +
        "        ON ST_Intersects(A.Shape, B.Shape) = 1; " +
        /*"        AND  A.us_node_id <> B.us_node_id " +
        "        AND  A.ds_node_id <> B.ds_node_id " +
        "        AND  A.us_node_id <> B.ds_node_id " +
        "        AND  A.ds_node_id <> B.us_node_id " +
        "        AND  B.hansen_compkey > 0; " +*/
        "CREATE INDEX IDX_ResultsSewerXP ON SewerXP(AOBJECTID); " +

        "DELETE FROM SewerXP WHERE  trim(au) like trim(bu);" +
        "DELETE FROM SewerXP WHERE  trim(ad) like trim(bd);" +
        "DELETE FROM SewerXP WHERE  trim(au) like trim(bd);" +
        "DELETE FROM SewerXP WHERE  trim(ad) like trim(bu);" +
        "UPDATE SewerXP SET BShape = (SELECT Shape FROM BESPipesClose WHERE OBJECTID = SewerXP.BOBJECTID); " +
        "UPDATE SewerXP SET pFt2Swr = ST_Distance(AShape, BShape); " +

        // Then we update the main table
        "UPDATE AMStudio_PIPEXP " +
        "SET xSewer = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xSwrMaxD = ( " +
        "SELECT  MAX(pSwrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xSwrMinD = ( " +
        "SELECT  MIN(pSwrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pSewer = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pSwrMaxD = ( " +
        "SELECT  MAX(pSwrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Swr = ( " +
        "SELECT  MIN(pFt2Swr)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        SewerXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Swr > 0 GROUP BY A.OBJECTID); ";

        // Then we drop the intersection table
        // "DROP INDEX IDX_ResultsSewerXP; " +
        // "DROP TABLE SewerXP;";
    }

    /// <summary>
    /// Sets Proximity to impoverished areas
    /// </summary>
    /// <returns>Query string setting Proximity to impoverished areas</returns>
    public static string ProximityToPoverty()
    {
      // First we create the table that will hold the results of the spatial query
      return
        "DROP TABLE IF EXISTS ResultsPoverty; " +
        "CREATE TABLE ResultsPoverty AS " +
        "SELECT  A.OBJECTID AS OBJECTID, COUNT(*) AS theCount  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PullTables.Poverty AS B " +
        "        ON ST_Intersects(A.Shape, B.Shape) = 1 " +
        "GROUP BY A.OBJECTID; " +
        "CREATE INDEX IDX_ResultsPoverty ON ResultsPoverty(OBJECTID); " +

          // Then we update the main table
        "UPDATE AMStudio_PIPEXP " +
        "SET Poverty = IFNULL(( " +
        "SELECT  theCount " +
        "FROM    ResultsPoverty AS A " +
        "WHERE   A.OBJECTID = AMStudio_PIPEXP.ID ),0); " +

        // Then we drop the intersection table
        "DROP INDEX IDX_ResultsPoverty; " +
        "DROP TABLE ResultsPoverty;";
    }

    /// <summary>
    /// Sets how many sewers are crossed or are close by, and what the diameter ranges of those pipes are
    /// </summary>
    /// <param name="distanceToWater">Maximum distance to detect water lines</param>
    /// <returns>Query string for setting sewer crossings, close-bys, and diameter ranges</returns>
    public static string ResultsWater(double distanceToWater)
    {
      // [Water XP]
      return

        // create xClose, xBuffer tables
        CreateFoundationTablesLazyDistance("PressurizedWaterMains", "PressurizedWaterMains", distanceToWater.ToString()) +

        "DROP TABLE IF EXISTS WaterXP; " +
        "CREATE TABLE WaterXP AS " +
        "SELECT  A.OBJECTID AS AOBJECTID, B.OBJECTID AS BOBJECTID, B.Mainsize AS pWtrD, ST_Distance(A.Shape, B.ShapeInBuffer) AS pFt2Wtr   " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        PressurizedWaterMainsInBuffer AS B " +
        "        ON ST_Intersects(A.Shape, B.ShapeIntersectsBuffer) = 1 " +
        "        AND  B.Status NOT LIKE 'ABN'; " +
        "CREATE INDEX IDX_ResultsWaterXP ON WaterXP(AOBJECTID); " +

        // Then we update the main table
        "UPDATE AMStudio_PIPEXP " +
        "SET xWtr = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xWMaxD = ( " +
        "SELECT  MAX(pWtrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET xWMinD = ( " +
        "SELECT  MIN(pWtrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr = 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pWtr = ( " +
        "SELECT  COUNT(*)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pWtrMaxD = ( " +
        "SELECT  MAX(pWtrD)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr > 0 GROUP BY A.OBJECTID); " +

        "UPDATE AMStudio_PIPEXP " +
        "SET pFt2Wtr = ( " +
        "SELECT  MIN(pFt2Wtr)  " +
        "FROM    REHABSegments AS A " +
        "        INNER JOIN " +
        "        WaterXP AS B " +
        "        ON  AMStudio_PIPEXP.ID = A.OBJECTID " +
        "        AND  B.AObjectID = A.OBJECTID AND pFt2Wtr > 0 GROUP BY A.OBJECTID); " +

        // Then we drop the intersection table
        "DROP INDEX IDX_ResultsWaterXP; ";

        // "DROP TABLE WaterXP;";
    }

    /// <summary>
    /// Sets the lateral count
    /// </summary>
    /// <returns>Query string setting the lateral count</returns>
    public static string CountLaterals()
    {
      return
        /*"UPDATE AMStudio_PIPEXP " +
        "SET     LateralCount = " +
        "        ( " +
        "          SELECT COUNT(*)  " +
        "          FROM   Laterals " +
        "          WHERE  Compkey =  AMStudio_PIPEXP.hansen_compkey " +
        "          GROUP BY Compkey " +
        "        ) ; ";*/
        "UPDATE	 AMStudio_PIPEXP " +
        "SET     LateralCount = 2*Length/50; ";
    }
  }
}
