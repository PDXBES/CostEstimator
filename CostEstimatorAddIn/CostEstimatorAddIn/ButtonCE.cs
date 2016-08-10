using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;

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
            ESRI.ArcGIS.esriSystem.UID dockWinID = new ESRI.ArcGIS.esriSystem.UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowCE;

            // Use GetDockableWindow directly as we want the client IDockableWindow not the internal class  
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
            dockWindow.Caption = "System Cost Estimator";
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
