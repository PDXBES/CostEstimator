using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CostEstimatorAddIn
{
  public class ToolCE : ESRI.ArcGIS.Desktop.AddIns.Tool
  {
    public ToolCE()
    {
    }

    protected override void OnUpdate()
    {
      Enabled = ArcMap.Application != null;
    }
  }

}
