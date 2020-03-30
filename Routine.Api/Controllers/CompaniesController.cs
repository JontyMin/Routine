using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepository.GetCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            //判断是否存在
            //var exist = await _companyRepository.CompanyExistsAsync(companyId);
            //if (!exist)
            //{
            //    return NotFound();
            //}

            var company = await _companyRepository.GetCompanyAsync(companyId);
            //如果找不到 返回404
            if (company==null)
            {
                return NotFound();
            }
            return Ok(company);
        }
    }
}