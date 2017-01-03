// <copyright file="SQLiteBasicStrings.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>SQLiteBasicStrings class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.Text;
  using System.Threading.Tasks;

  /// <summary>
  /// Class providing basic SQLite strings
  /// </summary>
  public class SQLiteBasicStrings
  {
    /// <summary>
    /// Attaches a database to the current session
    /// </summary>
    /// <param name="databaseLocation">Path to the database</param>
    /// <param name="aliasName">Alias to use for database in future queries</param>
    /// <returns>Query string attaching database</returns>
    public static string AttachDatabase(string databaseLocation, string aliasName)
    {
      return "ATTACH DATABASE '" + databaseLocation + "' As '" + aliasName + "'; ";
    }

    /// <summary>
    /// Enables spatial extensions for SQLite
    /// </summary>
    /// <param name="conn">Connection object to SQLite database</param>
    public static void EnableSpatial(SQLiteConnection conn)
    {
      // return "SELECT load_extension('C:\\Program Files (x86)\\ArcGIS\\Desktop10.2\\DatabaseSupport\\SQLite\\Windows32\\stgeometry_sqlite.dll', 'SDE_SQL_funcs_init');";
      conn.EnableExtensions(true);
      conn.LoadExtension(@"C:\Program Files (x86)\ArcGIS\Desktop10.2\DatabaseSupport\SQLite\Windows32\stgeometry_sqlite.dll", "SDE_SQL_funcs_init");
    }

    /// <summary>
    /// Creates the spatial environment required for Arc
    /// </summary>
    /// <returns>Query string for creating the Arc spatial environment</returns>
    public static string CreateArcSpatialEnvironment()
    {
      return "SELECT CreateOGCTables();";
    }

    /// <summary>
    /// Creates a basic table with an OBJECTID
    /// </summary>
    /// <param name="tableName">Name of table</param>
    /// <returns>Query string to create table</returns>
    public static string CreateBasicObjectIdTable(string tableName)
    {
      // Some tables transalted from ArcGIS come with an OBJECTID that may need to be kept for legacy reasons.  Hopefully in the future I'll
      // try to make sure we dont depend on that column, at all.  ever.  seriously, arc takes it over and repoopulates it at will.  So no.
      return "CREATE TABLE " + tableName + " (ID integer primary key autoincrement not null, OBJECTID integer);";
    }

    /// <summary>
    /// Creates the Rehab Segments table
    /// </summary>
    /// <returns>Query string creating the Rehab Segments table</returns>
    public static string CreateRehabSegmentsTable()
    {
      return 
      "CREATE TABLE REHABSegments  " +
      "(OBJECTID integer primary key autoincrement not null,  " +
      "XBJECTID int32 check(typeof(XBJECTID) = 'integer' and XBJECTID >= -2147483648 and XBJECTID <= 2147483647) not null,  " +
      "GLOBALID int32 check(typeof(GLOBALID) = 'integer' and GLOBALID >= -2147483648 and GLOBALID <= 2147483647) not null,  " +
      "hansen_compkey int32 check((typeof(hansen_compkey) = 'integer' or typeof(hansen_compkey) = 'null') and hansen_compkey >= -2147483648 and hansen_compkey <= 2147483647),  " +
      "us_node_id text(20) check((typeof(us_node_id) = 'text' or typeof(us_node_id) = 'null') and not length(us_node_id) > 20),  " +
      "ds_node_id text(20) check((typeof(ds_node_id) = 'text' or typeof(ds_node_id) = 'null') and not length(ds_node_id) > 20),  " +
      "seg_us_node_id int32 check((typeof(seg_us_node_id) = 'integer' or typeof(seg_us_node_id) = 'null') and seg_us_node_id >= -2147483648 and seg_us_node_id <= 2147483647),  " +
      "seg_ds_node_id int32 check((typeof(seg_ds_node_id) = 'integer' or typeof(seg_ds_node_id) = 'null') and seg_ds_node_id >= -2147483648 and seg_ds_node_id <= 2147483647),  " +
      "length float64 check(typeof(length) = 'real' or typeof(length) = 'null'),  " +
      "fm float64 check(typeof(fm) = 'real' or typeof(fm) = 'null'),  " +
      "to_ float64 check(typeof(to_) = 'real' or typeof(to_) = 'null'),  " +
      "cutno int32 check((typeof(cutno) = 'integer' or typeof(cutno) = 'null') and cutno >= -2147483648 and cutno <= 2147483647),  " +
      "UNITID text(27) check((typeof(UNITID) = 'text' or typeof(UNITID) = 'null') and not length(UNITID) > 27),  " +
      "UNITTYPE text(50) check((typeof(UNITTYPE) = 'text' or typeof(UNITTYPE) = 'null') and not length(UNITTYPE) > 50),  " +
      "COMPTYPE int32 check((typeof(COMPTYPE) = 'integer' or typeof(COMPTYPE) = 'null') and COMPTYPE >= -2147483648 and COMPTYPE <= 2147483647),  " +
      "OWNRSHIP text(50) check((typeof(OWNRSHIP) = 'text' or typeof(OWNRSHIP) = 'null') and not length(OWNRSHIP) > 50),  " +
      "servstat text(4) check((typeof(servstat) = 'text' or typeof(servstat) = 'null') and not length(servstat) > 4),  " +
      "FRM_ELEV float64 check(typeof(FRM_ELEV) = 'real' or typeof(FRM_ELEV) = 'null'),  " +
      "TO_ELEV float64 check(typeof(TO_ELEV) = 'real' or typeof(TO_ELEV) = 'null'),  " +
      "FRM_DEPTH float64 check(typeof(FRM_DEPTH) = 'real' or typeof(FRM_DEPTH) = 'null'),  " +
      "TO_DEPTH float64 check(typeof(TO_DEPTH) = 'real' or typeof(TO_DEPTH) = 'null'),  " +
      "ParentLength float64 check(typeof(ParentLength) = 'real' or typeof(ParentLength) = 'null'),  " +
      "PIPESIZE float64 check(typeof(PIPESIZE) = 'real' or typeof(PIPESIZE) = 'null'),  " +
      "PIPEHEIGHT float64 check(typeof(PIPEHEIGHT) = 'real' or typeof(PIPEHEIGHT) = 'null'),  " +
      "PIPESHPE text(50) check((typeof(PIPESHPE) = 'text' or typeof(PIPESHPE) = 'null') and not length(PIPESHPE) > 50),  " +
      "MATERIAL text(50) check((typeof(MATERIAL) = 'text' or typeof(MATERIAL) = 'null') and not length(MATERIAL) > 50),  " +
      "JOBNO text(50) check((typeof(JOBNO) = 'text' or typeof(JOBNO) = 'null') and not length(JOBNO) > 50),  " +
      "INSTALL_DATE realdate check((typeof(INSTALL_DATE) = 'real' or typeof(INSTALL_DATE) = 'null') and INSTALL_DATE >= 0.0),  " +
      "tot_segs int32 check((typeof(tot_segs) = 'integer' or typeof(tot_segs) = 'null') and tot_segs >= -2147483648 and tot_segs <= 2147483647),  " +
      "IS_SEGMENT int32 check((typeof(IS_SEGMENT) = 'integer' or typeof(IS_SEGMENT) = 'null') and IS_SEGMENT >= -2147483648 and IS_SEGMENT <= 2147483647) " +
      ");";
    }

    /// <summary>
    /// Adds a geometry field
    /// </summary>
    /// <param name="tableName">Name of table</param>
    /// <param name="geometryType">Type of geometry</param>
    /// <param name="srid">SRID to use</param>
    /// <returns>Query string to add geometry table</returns>
    public static string AddGeometryColumn(string tableName, string geometryType, int srid)
    {
      // geometrytype can be found at the following location:
      // http://desktop.arcgis.com/en/arcmap/10.3/manage-data-using-sql-with-gdbs/register-an-st-geometry-column.html
      // You can get the geometry type of the table being translated using the function in TranslateArcToSQLite:
      return "SELECT AddGeometryColumn(null, '" + tableName + "', 'Shape', " + srid.ToString() + ", '" + geometryType.ToString() + "', 2, 'null');";
    }

    /// <summary>
    /// Gets the shape type for a table
    /// </summary>
    /// <param name="tableName">Name of table</param>
    /// <returns>Shape type</returns>
    public static string GetShapeType(string tableName)
    {
      return "SELECT st_geometrytype(Shape) FROM " + tableName + ";";
    }

    /// <summary>
    /// Populates the Rehab Segments table from the EMGAATS links table
    /// </summary>
    /// <param name="linksTableName">Name of links table</param>
    /// <returns>Query string to update Rehab Segments table</returns>
    public static string PopulateRehabSegmentsFromLinksTable(string linksTableName)
    {
      // this query neglects FRM_DEPTH, TO_DEPTH
      // this query also assumes that all pipes will be circular because I wasn't given any information on how to translate an emgaats shape.
      return "INSERT INTO REHABSegments (Shape, XBJECTID, GLOBALID, hansen_compkey, us_node_id, ds_node_id, seg_us_node_id, seg_ds_node_id, length,    fm,   to_,       cutno, UNITID, UNITTYPE,       COMPTYPE, OWNRSHIP, servstat, FRM_ELEV, TO_ELEV,  ParentLength, PIPESIZE,  PIPEHEIGHT, PIPESHPE, MATERIAL, JOBNO, INSTALL_DATE, tot_segs,    IS_SEGMENT) " +
             "SELECT                     Shape, link_id,  dme_global_id,  link_id,        '',         '',         us_node_id,       ds_node_id,   length_ft, 0.0,  length_ft, 0,     '',     link_flow_type, 0,        'BES',    'IN',     us_ie_ft, ds_ie_ft, length_ft,    height_in, height_in,  'CIRC',   'CSP',    '',    0.0,            1,           0 " +
             "FROM   " + linksTableName + ";";
    }

    /// <summary>
    /// Populates the Rehab Segments table from the EMGAATS Links and Nodes tables
    /// </summary>
    /// <param name="linksTableName">Name of links table</param>
    /// <param name="nodesTableName">Name of nodes table</param>
    /// <returns>Query string to update Rehab Segments table</returns>
    public static string PopulateRehabSegmentsFromLinksNodesJoin(string linksTableName, string nodesTableName)
    {
      return "UPDATE  REHABSegments " +
             "SET     FRM_DEPTH = " +
             "        ( " +
             "          SELECT  ground_elev_ft - us_ie_ft " +
             "          FROM    " + linksTableName + " AS L " +
             "                  INNER JOIN " +
             "                  " + nodesTableName + " AS N " +
             "                  ON L.us_node_id = N.node_id " +
             "          WHERE   link_id = REHABSegments.XBJECTID " +
             "        ); " +
             "UPDATE  REHABSegments " +
             "SET     TO_DEPTH = " +
             "        ( " +
             "          SELECT  ground_elev_ft - ds_ie_ft " +
             "          FROM    " + linksTableName + " AS L " +
             "                  INNER JOIN " +
             "                  " + nodesTableName + " AS N " +
             "                  ON L.ds_node_id = N.node_id " +
             "          WHERE   link_id = REHABSegments.XBJECTID " +
             "        ); ";
    }

    /// <summary>
    /// Populates the Rehab Segments table's node fields
    /// </summary>
    /// <param name="nodesTableName">Name of nodes table</param>
    /// <returns>Query string to update Rehab Segments table with nodes</returns>
    public static string PopulateRehabSegmentsNodes(string nodesTableName)
    {
      return "UPDATE  REHABSegments " +
             "SET     us_node_id = " +
             "        ( " +
             "          SELECT  node_name " +
             "          FROM    " + nodesTableName + " AS N " +
             "          WHERE   node_id = REHABSegments.seg_us_node_id " +
             "        ); " +
             "UPDATE  REHABSegments " +
             "SET     ds_node_id = " +
             "        ( " +
             "          SELECT  node_name " +
             "          FROM    " + nodesTableName + " AS N " +
             "          WHERE   node_id = REHABSegments.seg_ds_node_id " +
             "        ); ";
    }
  }
}
