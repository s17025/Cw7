﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSampleWebApp.DTOs
{
    public class PromoteStudentRequest
    {
        public int Semester { get; set; }
        public string Studies { get; set; }
    }
}
