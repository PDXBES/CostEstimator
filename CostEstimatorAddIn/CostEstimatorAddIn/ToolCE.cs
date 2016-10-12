// <copyright file="ToolCE.cs" company="City of Portland, BES-ASM">
// </copyright>
// <summary>ToolCE class</summary>

namespace CostEstimatorAddIn
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;

  /// <summary>
  /// Cost estimator tool
  /// </summary>
  public class ToolCE : ESRI.ArcGIS.Desktop.AddIns.Tool
  {
    /// <summary>
    /// Initializes a new instance of the ToolCE class
    /// </summary>
    public ToolCE()
    {
    }

    /// <summary>
    /// Determines whether the tool is enabled
    /// </summary>
    protected override void OnUpdate()
    {
      this.Enabled = ArcMap.Application != null;
    }
  }
}
