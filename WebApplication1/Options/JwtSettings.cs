﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Post_Surfer.Options
{
    public class JwtSettings
    {
            public  string Secret { get; set; }
            public TimeSpan TokenLifetime { get; set; }
    }
}
