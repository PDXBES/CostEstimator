// <copyright file="DockableWindowCE.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>DockableWindowCE class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Data.SQLite;
  using System.Drawing;
  using System.IO;
  using System.Text;
  using System.Windows.Forms;
  using ESRI.ArcGIS.ArcMapUI;
  using ESRI.ArcGIS.Catalog;
  using ESRI.ArcGIS.CatalogUI;
  using ESRI.ArcGIS.Desktop;
  using ESRI.ArcGIS.EngineCore;
  using ESRI.ArcGIS.esriSystem;
  using ESRI.ArcGIS.Framework;

  /// <summary>
  /// Designer class of the dockable window add-in. It contains user interfaces that
  /// make up the dockable window.
  /// </summary>
  public partial class DockableWindowCE : UserControl
  {
    /// <summary>
    /// Initializes a new instance of the DockableWindowCE class
    /// </summary>
    /// <param name="hook">Window to hook to</param>
    public DockableWindowCE(object hook)
    {
      this.InitializeComponent();
      this.Hook = hook;
    }

    /// <summary>
    /// Gets or sets the Host object of the dockable window
    /// </summary>
    private object Hook
    {
      get;
      set;
    }

    private void Button1Click(object sender, EventArgs e)
    {
      // Create a new TranslateArcToSQLite object

      // locate the folder where the emgaats model lives
      IGxDialog openFolderDialog = new GxDialog();
      openFolderDialog.RememberLocation = true;
      openFolderDialog.AllowMultiSelect = false;
      openFolderDialog.Title = "Locate model folder";
      IGxObject openFolderObject;
      IEnumGxObject openFolderEnum;

      // sName is the name of the EMGAATS gdb, ie: "EmgaatsModel.gdb"
      string emgaatsGeodatabaseName;

      // sFolder is the folder containing the EMGAATS gdb
      string selectedFolder;

      // get the location of the EMGAATS gdb
      if (openFolderDialog.DoModalOpen(0, out openFolderEnum))
      {
        openFolderObject = openFolderEnum.Next();
        FileInfo selectedFile = new FileInfo(openFolderObject.FullName);
        emgaatsGeodatabaseName = openFolderObject.Name;
        selectedFolder = selectedFile.Directory.FullName;

        ESRI.ArcGIS.Framework.IMouseCursor theCursor = new ESRI.ArcGIS.Framework.MouseCursor();
        theCursor.SetCursor(2);
        TranslateArcToSQLite tats = new TranslateArcToSQLite();
        tats.TranslateEmgaatsModel(selectedFile);
        theCursor.SetCursor(0);
        theCursor.SetCursor(2);
        tats.CreateDmePipesFromEmgaatsTables(selectedFile);
        theCursor.SetCursor(0);

        // now that that is done, we can start pipXP
        CostEstimatorClass cec = new CostEstimatorClass();
        theCursor.SetCursor(2);
        cec.PerformPipeXp(selectedFile);
        theCursor.SetCursor(0);
        theCursor.SetCursor(2);
        cec.PerformCostEstimates(selectedFile);
        theCursor.SetCursor(0);
      }
    }

    private void Test_Click(object sender, EventArgs e)
    {
      // get the location of the input database, 
      PipeXpArc pa;

      // locate the folder where the Emgaats model lives
      IGxDialog openFolderDialog = new GxDialog();
      openFolderDialog.RememberLocation = true;
      openFolderDialog.AllowMultiSelect = false;
      openFolderDialog.Title = "Locate model folder";
      IGxObject openFolderObject;
      IEnumGxObject openFolderEnum;
      string name;

      // sFolder is the folder containing the EMGAATS gdb
      string emgaatsFolder;

      // get the location of the EMGAATS gdb
      if (!openFolderDialog.DoModalOpen(0, out openFolderEnum))
      {
        return;
      }

      string modelLinksLayer = "Links";
      string modelLinksPZoneLayer = modelLinksLayer + "PZone";
      string modelLinksCZoneLayer = modelLinksLayer + "CZone";
      string modelNodesLayer = "Nodes";
      string pipeXPFolderName = "PipeXP";
      string pipeXPSQLiteDBName = "PipeXP";

      // Load the background files
      // it is important that this process be treated as separate from working
      // on the background files, in case we need to keep them open for multiple
      // processing events
      openFolderObject = openFolderEnum.Next();
      FileInfo selectedFile = new FileInfo(openFolderObject.FullName);
      name = openFolderObject.Name;
      emgaatsFolder = selectedFile.Directory.FullName;
      pa = new PipeXpArc(openFolderObject.FullName, "EGH_PUBLIC.ARCMAP_ADMIN", "PWBWATER.ARCMAP_ADMIN");

      ESRI.ArcGIS.Framework.IMouseCursor theCursor = new ESRI.ArcGIS.Framework.MouseCursor();
      theCursor.SetCursor(2);

      if (Directory.Exists(selectedFile.DirectoryName + "\\" + pipeXPFolderName))
      {
        Directory.Delete(selectedFile.DirectoryName + "\\" + pipeXPFolderName, true);
      }

      Directory.CreateDirectory(selectedFile.DirectoryName + "\\" + pipeXPFolderName);
      SQLiteConnection conn = new SQLiteConnection("Data Source = '" + selectedFile.DirectoryName + "\\" + pipeXPFolderName + "\\" + pipeXPSQLiteDBName + ".sqlite';Version=3", true);
      conn.Open();
      SQLiteBasicStrings.EnableSpatial(conn);
      pa.NqSqlite(SQLiteBasicStrings.CreateArcSpatialEnvironment(), conn);
      conn.Close();

      // Some of the changes below can be applied to the output tables that Proximity() creates,
      // but I just wanted to make sure that the notes are there while I did this.
      pa.Proximity(selectedFile, modelLinksLayer, "0", "pipxp_hardareas_bes_pdx", "XP_HardAreas", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "25", "hydrants_pdx", "XP_HYDRANTS", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "10", "impervious_area_bes_pdx", "XP_BLDGS", "[GENSOURCE] = 'BLDG'");

      // This should be the upstream node of the WHOLE PIPE (not segments)
      pa.Proximity(selectedFile, modelLinksLayer, "0", "uic_sc_auto_bes_pdx", "XP_UICLinks", string.Empty);
      pa.Proximity(selectedFile, modelNodesLayer, "0", "uic_sc_auto_bes_pdx", "XP_UICNodes", string.Empty);

      // This should be the upstream node of the WHOLE PIPE (not segments)
      pa.Proximity(selectedFile, modelLinksLayer, "0", "of_drainage_bounds_bes_pdx", "XP_MS4Links", "Boundary_Type ='MS4' OR Boundary_Type = 'Other'");
      pa.Proximity(selectedFile, modelNodesLayer, "0", "of_drainage_bounds_bes_pdx", "XP_MS4Nodes", "Boundary_Type ='MS4' OR Boundary_Type = 'Other'");
      pa.Proximity(selectedFile, modelLinksLayer, "50", "emergency_routes_pdx", "XP_EMT", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "250", "fire_stations_metro", "XP_Fire", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "250", "portland_police_facilities_pdx", "XP_Police", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "250", "hospitals_metro", "XP_Hospitals", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "250", "schools_metro", "XP_Schools", string.Empty);

      // ECSI (and lust) used to attempt to identify volumes and lengths, but that is no longer necessary
      pa.Proximity(selectedFile, modelLinksLayer, "0", "ecsi_sites_bes_pdx", "XP_ECSI", string.Empty);

      // ECSI and lust share the same XP field, 'xECSI'.  This may change in the future, but for now
      // when it comes to updating the PipXP table, simply combine both results into the ECSI fields.
      pa.Proximity(selectedFile, modelLinksLayer, "0", "lust_bes_pdx", "XP_Lust", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "0", "zoning_pdx", "XP_SFR", "[ZONE] IN ('R20', 'R10', 'R7', 'R5', 'R2.5')");

      // pZone (and cZone) need to know the AREA AND LENGTH of intersection
      // the area of intersection is xFtEzonP = Intersection(Buffer(Pipe, 12.5), Ezone)
      // the length of intersection is just the length of the unbuffered pipe that lies in the zone
      // This means that I should create a pipe/segment buffer layer and intersect it with
      // these two layers.  Then just get the areas from those intersection results.
      pa.Proximity(selectedFile, modelLinksLayer, "12.5", "zoning_pdx", "XP_PZone", "[OVRLY] like '%p%'");
      pa.CreateBufferTable("12.5", modelLinksLayer, modelLinksPZoneLayer);
      pa.Proximity(selectedFile, modelLinksPZoneLayer, "0", "zoning_pdx", "XP_PZoneBuffer", "[OVRLY] like '%p%'");

      // the length of intersection is just the length of the unbuffered pipe that lies in the zone
      // the area of intersection is xFtEzonC = Intersection(Buffer(Pipe, 12.5), Ezone)
      pa.Proximity(selectedFile, modelLinksLayer, "12.5", "zoning_pdx", "XP_CZone", "[OVRLY] like '%c%'");
      pa.CreateBufferTable("12.5", modelLinksLayer, modelLinksCZoneLayer);
      pa.Proximity(selectedFile, modelLinksCZoneLayer, "0", "zoning_pdx", "XP_CZoneBuffer", "[OVRLY] like '%c%'");
      pa.Proximity(selectedFile, modelLinksLayer, "10", "major_gas_lines_metro", "XP_Gas", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "10", "fiber_routes_pdx", "XP_FiberOptic", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "25", "light_rail_lines_metro", "XP_LightRail", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "25", "railroads_metro", "XP_RailRoads", string.Empty);
      pa.Proximity(selectedFile, modelLinksLayer, "30", "streets_pdx", "XP_Streets", string.Empty);

      // after creating the close streets layer, the program needs to create an intersections layer from the close streets layer
      //
      // This can be accomplished with the 'Intersection' tool, using the same layer as input twice.
      pa.IntersectionsPoint(selectedFile, "XP_Streets", "XP_Streets", "XP_IntersectionsAll");

      // Eliminate false intersections by keeping only the records where PRI_NM_ID <> PRI_NM_ID_1
      pa.CopyFeatureClass(selectedFile, "XP_IntersectionsAll", selectedFile.FullName, "XP_Intersections", "PRI_NM_ID <> PRI_NM_ID_1");

      // These intersection points should be buffered by about 15 feet, and unioned based on
      // pa.CreateBufferTable("15", "XP_Intersections", "XP_IntersectionsBuffer", "FULL", "ROUND", "ALL");
      // whether they overlap.
      // Then the number of streets that intersect the buffered point is summed.
      // This can be done by simply doing a 'createNear' using "XP_IntersectionsBuffer" and "XP_Streets", but this does double up on XP_StreetsNear
      pa.CreateNearTable("20", "XP_Intersections", "XP_Streets", "XP_StreetsNearIntersections");

      // Next, that information needs to be placed back into the unbuffered nodes.
      // Finally, do a uxCLx, dxCLx, and CLx, along with ft2clx: two for whole pipe downstream, two for whole pipe upstream, and two for object nearby (segment or whole pipe)
      // the limit of clx, ft2clx is 30 ft.
      // So, just do this for both pipes and nodes and then sum those results up in the database queries
      // also need to keep track of the street type and the 'streetvalue', essentially the stuff that associates 1400 with Street or Arterial or whatever

      // make sure the results of this are not close to or intersecting pipes or segments that are actually connected to the object in question
      // This is where the angles come in (since we care about angles in some versions of this query set.
      pa.Proximity(selectedFile, modelLinksLayer, "12", "collection_lines_bes_pdx", "XP_Sewer", "([LAYER_GROUP] = 'SEWER PIPES' OR [LAYER_GROUP] = 'STORM PIPES') AND ([SERVSTAT] NOT IN ('ABAN', 'TBAB'))");

      pa.Proximity(selectedFile, modelLinksLayer, "0", "census_blockgroup_2010_metro", "XP_Census", string.Empty);
      pa.TranslateCensus(selectedFile);
      pa.ProximityToPressurizedWater(selectedFile, modelLinksLayer, "12");

      // Finally need to count the laterals as well.  This would be a database query.
      theCursor.SetCursor(0);
      MessageBox.Show("Completed PipeXP calculations.");
    }
  }
}
