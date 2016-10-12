// <copyright file="ButtonCE.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>ButtonCE class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
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
  /// Cost Estimator button
  /// </summary>
  public class ButtonCE : ESRI.ArcGIS.Desktop.AddIns.Button
  {
    /// <summary>
    /// Initializes a new instance of the ButtonCE class
    /// </summary>
    public ButtonCE()
    {
    }

    /// <summary>
    /// Executes when the button is clicked
    /// </summary>
    protected override void OnClick()
    {
      ArcMap.Application.CurrentTool = null;

      // locate the folder where the emgaats model lives
      IGxDialog openFolderDialog = new GxDialog();
      openFolderDialog.RememberLocation = true;
      openFolderDialog.AllowMultiSelect = false;
      openFolderDialog.Title = "Locate model folder";
      IGxObject folderObject;
      IEnumGxObject folderEnum;
      string costEstimatorFolderName = "CostEstimates";

      // sName is the name of the EMGAATS gdb, ie: "EmgaatsModel.gdb"
      string geodatabaseName;

      // sFolder is the folder containing the EMGAATS gdb
      string geodatabaseFolder;

      // If there is a cost estimator folder in the model directory, delete it

      // get the location of the EMGAATS gdb
      if (openFolderDialog.DoModalOpen(0, out folderEnum))
      {
        folderObject = folderEnum.Next();
        FileInfo selectedFile = new FileInfo(folderObject.FullName);
        geodatabaseName = folderObject.Name;
        geodatabaseFolder = selectedFile.Directory.FullName;

        if (Directory.Exists(selectedFile.DirectoryName + "\\" + costEstimatorFolderName))
        {
          Directory.Delete(selectedFile.DirectoryName + "\\" + costEstimatorFolderName, true);
        }

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
        MessageBox.Show("Cost estimates complete!");
      }
    }

    /// <summary>
    /// Checks for whether the button should be enabled
    /// </summary>
    protected override void OnUpdate()
    {
      this.Enabled = ArcMap.Application != null;
    }
  }
}
