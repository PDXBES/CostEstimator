using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Desktop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.EngineCore;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;

namespace CostEstimatorAddIn
{
  public class ButtonCE : ESRI.ArcGIS.Desktop.AddIns.Button
  {
    public ButtonCE()
    {
    }

    protected override void OnClick()
    {
      //
      //  TODO: Sample code showing how to access button host
      //
      ArcMap.Application.CurrentTool = null;

      //locate the folder where the emgaats model lives
      IGxDialog pGxDialog = new GxDialog();
      pGxDialog.RememberLocation = true;
      pGxDialog.AllowMultiSelect = false;
      pGxDialog.Title = "Locate model folder";
      IGxObject pGxObject;
      IEnumGxObject pEnumGx;
      string costEstimatorFolderName = "CostEstimates";

      //sName is the name of the EMGAATS gdb, ie: "EmgaatsModel.gdb"
      string sName;
      //sFolder is the folder containing the EMGAATS gdb
      string sFolder;

      //If there is a cost estimator folder in the model directory, delete it


      //get the location of the EMGAATS gdb
      if (pGxDialog.DoModalOpen(0, out pEnumGx))
      {
        pGxObject = pEnumGx.Next();
        FileInfo sFile = new FileInfo(pGxObject.FullName);
        sName = pGxObject.Name;
        sFolder = sFile.Directory.FullName;

        if (Directory.Exists(sFile.DirectoryName + "\\" + costEstimatorFolderName))
        {
          Directory.Delete(sFile.DirectoryName + "\\" + costEstimatorFolderName, true);
        }

        ESRI.ArcGIS.Framework.IMouseCursor theCursor = new ESRI.ArcGIS.Framework.MouseCursor();
        theCursor.SetCursor(2);
        TranslateArcToSQLite tats = new TranslateArcToSQLite();
        tats.TranslateEmgaatsModel(sFile);
        theCursor.SetCursor(0);
        theCursor.SetCursor(2);
        tats.createDMEPipesFromEmgaatsTables(sFile);
        theCursor.SetCursor(0);
        //now that that is done, we can start pipXP
        CostEstimatorClass cec = new CostEstimatorClass();
        theCursor.SetCursor(2);
        cec.PerformPipeXP(sFile);
        theCursor.SetCursor(0);
        theCursor.SetCursor(2);
        cec.PerformCostEstimates(sFile);
        theCursor.SetCursor(0);
        MessageBox.Show("Cost estimates complete!");
      }
    }
    protected override void OnUpdate()
    {
      Enabled = ArcMap.Application != null;
    }
  }

}
