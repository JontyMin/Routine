﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Product { get; set; }
        public string Introduction { get; set; }
    }
}
