using System;
using System.Collections.Generic;
using System.Text;

namespace Schefco.TaskFlow.Application.Common.Settings
{
    // Holds the Owner email so Application doesn't depend on IConfiguration
    public class OwnerSettings
    {
        public string OwnerEmail { get; set; } = string.Empty;
    }
}
