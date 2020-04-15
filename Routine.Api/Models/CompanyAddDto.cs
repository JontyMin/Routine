using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class CompanyAddDto
    {
        [Display(Name = "公司名")]
        [Required(ErrorMessage = "{0}公司名不能为空")]
        [MaxLength(100,ErrorMessage = "{0}最大长度不可以超过{1}")]
        public string Name { get; set; }

        [Display(Name="公司简介")]
        [StringLength(500,MinimumLength = 10,ErrorMessage = "{0}的长度范围{2}到{1}")]
        public string Introduction { get; set; }

        public ICollection<EmployeeAddDto> Employees { get; set; }=new List<EmployeeAddDto>();
    }
}
