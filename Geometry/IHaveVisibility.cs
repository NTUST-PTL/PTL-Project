﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry
{
    public interface IHaveVisibility
    {
        bool? Visible { get; set; }
    }
}
