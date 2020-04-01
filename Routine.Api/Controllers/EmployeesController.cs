using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Entities;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [Route("api/Companies/{companyId}/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EmployeesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> 
            GetEmployeesForCompany(Guid companyId,[FromQuery(Name="gender")]string genderDisplay,string q)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employees = await _companyRepository.GetEmployeeAsync(companyId,genderDisplay,q);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDto);
        }


        [HttpGet("{employeeId}",Name=nameof(GetEmployeeForCompany))]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeeForCompany(Guid companyId,Guid employeeId)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employees = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employees == null)
            {
                return NotFound();
            }

            // var employees = await _companyRepository.GetEmployeeAsync(companyId);
            var employeesDto = _mapper.Map<EmployeeDto>(employees);
            return Ok(employeesDto);
        }
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> 
            CreateEmployeeForCompany(Guid companyId,EmployeeAddDto employee)
        {
            if (! await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var entity = _mapper.Map<Employee>(employee);

            _companyRepository.AddEmployee(companyId,entity);
            await _companyRepository.SaveAsync();

            var dtoToReturn = _mapper.Map<EmployeeDto>(entity);

            return CreatedAtRoute(nameof(GetEmployeeForCompany), new
            {
                companyId,
                employeeId = dtoToReturn.Id
            }, dtoToReturn);
        }
    }
}