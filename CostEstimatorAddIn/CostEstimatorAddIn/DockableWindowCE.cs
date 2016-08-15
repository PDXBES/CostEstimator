using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Desktop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.EngineCore;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;

namespace CostEstimatorAddIn
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowCE : UserControl
    {
        public DockableWindowCE(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowCE m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindowCE(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Create a new TranslateArcToSQLite object
            
            
            //locate the folder where the emgaats model lives
            IGxDialog pGxDialog = new GxDialog();
            pGxDialog.RememberLocation = true;
            pGxDialog.AllowMultiSelect = false;
            pGxDialog.Title = "Locate model folder";
            IGxObject pGxObject;
            IEnumGxObject pEnumGx;
            //sName is the name of the EMGAATS gdb, ie: "EmgaatsModel.gdb"
            string sName;
            //sFolder is the folder containing the EMGAATS gdb
            string sFolder;

            //get the location of the EMGAATS gdb
            if(pGxDialog.DoModalOpen(0, out pEnumGx))
            {
                pGxObject = pEnumGx.Next();
                FileInfo sFile = new FileInfo(pGxObject.FullName);
                sName = pGxObject.Name;
                sFolder = sFile.Directory.FullName;

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
            }
        }
    }
}
