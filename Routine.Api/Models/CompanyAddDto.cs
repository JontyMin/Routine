using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class CompanyAddDto
    {
        public string Name { get; set; }
        public string Introduction { get; set; }

        public ICollection<EmployeeAddDto> Employees { get; set; }=new List<EmployeeAddDto>();
    }
}
