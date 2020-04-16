using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="genderDisplay"></param>
        /// <param name="q"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据公司获取员工
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据公司创建员工
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 整体更新
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="employeeId">员工ID</param>
        /// <param name="employee">更新信息</param>
        /// <returns></returns>
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployeeForCompany(
            Guid companyId, 
            Guid employeeId,
            EmployeeUpdateDto employee
            )
        {
            //判断该公司是否存在
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            
            //该公司没有这个员工
            if (employeeEntity==null)
            {
                //创建员工
                var employeeToAddEntity = _mapper.Map<Employee>(employee);
                employeeToAddEntity.Id = employeeId;
                _companyRepository.AddEmployee(companyId, employeeToAddEntity);
                await _companyRepository.SaveAsync();
                var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAddEntity);

                return CreatedAtRoute(nameof(GetEmployeeForCompany), new
                {
                    companyId,
                    employeeId = dtoToReturn.Id
                }, dtoToReturn);
            }

            //entity 转化为 updateDto
            //把传进来的employee的值更新到updateDto
            //把updateDto映射回entity

            _mapper.Map(employee, employeeEntity);

            _companyRepository.UpdateEmployee(employeeEntity);

            await _companyRepository.SaveAsync();

            return NoContent();

        }

        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
            Guid companyId,
            Guid employeeId,
            JsonPatchDocument<EmployeeUpdateDto> patchDocument)
        {
            //判断该公司是否存在
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            //该公司没有这个员工
            if (employeeEntity == null)
            {
                var employeeDto = new EmployeeUpdateDto();
                patchDocument.ApplyTo(employeeDto,ModelState);

                if (!TryValidateModel(employeeDto))
                {
                    return ValidationProblem(ModelState);
                }

                var employeeToAdd = _mapper.Map<Employee>(employeeDto);
                employeeToAdd.Id = employeeId;

                _companyRepository.AddEmployee(companyId,employeeToAdd);
                await _companyRepository.SaveAsync();

                var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAdd);

                return CreatedAtRoute(nameof(GetEmployeeForCompany), new
                {
                    companyId,
                    employeeId = dtoToReturn.Id
                }, dtoToReturn);
            }

            var dtoToPatch = _mapper.Map<EmployeeUpdateDto>(employeeEntity);

            //需要处理验证错误 
            patchDocument.ApplyTo(dtoToPatch);

            if (!TryValidateModel(dtoToPatch))
            {
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(dtoToPatch, employeeEntity);

            _companyRepository.UpdateEmployee(employeeEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="companyId">公司id</param>
        /// <param name="employeeId">员工id</param>
        /// <returns></returns>
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            //判断该公司是否存在
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            //该公司没有这个员工
            if (employeeEntity == null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            _companyRepository.DeleteEmployee(employeeEntity);
            await _companyRepository.SaveAsync();
            return NoContent();
        }
    }
}