using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.ResourceParameters;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _proertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(
            ICompanyRepository companyRepository,
            IMapper mapper,
            IPropertyMappingService proertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper= mapper  ?? throw new ArgumentNullException(nameof(mapper));
            _proertyMappingService = proertyMappingService ?? throw new ArgumentNullException(nameof(proertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService)); ;
        }

        /// <summary>
        /// 获取所有公司
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] CompanyDtoParameters company)
        {
            //throw new Exception("An Exception");
            if (!_proertyMappingService.ValidMappingExistsFor<CompanyDto,Company>(company.OrderBy))
            {
                return BadRequest();
            }



            var companies = await _companyRepository.GetCompaniesAsync(company);

            var perviousLink = companies.HasPrevious
                ? CreateCompaniesResourceUri(company, ResourceUriType.PreviousPage)
                : null;

            var nextLink = companies.HasNext
                ? CreateCompaniesResourceUri(company, ResourceUriType.NextPage)
                : null;

            var paginationMetadate = new
            {
                totalCOunt = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPage = companies.TotalPages,
                perviousLink,
                nextLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadate, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companyDtos.ShapeData(company.Fields));
        }

        /// <summary>
        /// 根据id查询公司
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}",Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId,string fields)
        {
            //判断是否存在
            //var exist = await _companyRepository.CompanyExistsAsync(companyId);
            //if (!exist)
            //{
            //    return NotFound();
            //}
            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }
            var company = await _companyRepository.GetCompanyAsync(companyId);
            //如果找不到 返回404
            if (company==null)
            {
                return NotFound();
            }

            var links = CreateLinksForCompany(companyId, fields);

            var linkedDict = _mapper.Map<CompanyDto>(company).ShapeData(fields)
                as IDictionary<string ,object>;

            linkedDict.Add("links", links);

            return Ok(linkedDict);
        }

        /// <summary>
        /// 创建公司
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody]CompanyAddDto company)
        {
            //xml格式 Introduction null
            //把Introduction 放在Name字段前可识别
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var retuenDto = _mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(retuenDto.Id, null);

            var linkedDict = retuenDto.ShapeData(null)
                as IDictionary<string,object>;

            linkedDict.Add("links",links);
            
            //返回一个路径，获取当前添加的资源
            return CreatedAtRoute(nameof(GetCompany), new {companyId = linkedDict["Id"]}, retuenDto);
        }


        /// <summary>
        /// 创建资源集合
        /// </summary>
        /// <param name="companyCollection"></param>
        /// <returns></returns>
        [HttpPost("CreateCompanyCollection")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>>
            CreateCompanyCollection(IEnumerable<CompanyAddDto> companyCollection)
        {
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _companyRepository.AddCompany(company);
            }

            await _companyRepository.SaveAsync();
            //return CreatedAtRoute();
            var dtosToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var idsString = string.Join(",", dtosToReturn.Select(x => x.Id));
            return CreatedAtRoute(nameof(GetCompanyCollection), new {ids = idsString}, dtosToReturn);
        }


        [HttpGet("({ids})",Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids==null)
            {
                return BadRequest();
            }

            var entities = await _companyRepository.GetCompaniesAsync(ids);
            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }

            var dtoToReturn = _mapper.Map<IEnumerable<CompanyDto>>(entities);
            return Ok(dtoToReturn);
        }

        /// <summary>
        /// 根据id删除公司
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete("{companyId}",Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);

            if (companyEntity==null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeeAsync(companyId, null);
            _companyRepository.DeleteCompany(companyEntity);
            await _companyRepository.SaveAsync();
            return NoContent();

        }
        /// <summary>
        /// option请求 获取api的通信信息
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow","GET,POST,OPTIONS");
            return Ok();
        }

        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields=parameters.Fields,
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany),
                    new{companyId}),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany),
                        new { companyId ,fields}),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(Url.Link(nameof(DeleteCompany),
                    new { companyId}),
                "delete_company",
                "DELETE"));
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployeeForCompany),
                        new { companyId }),
                    "create_employee_for_company",
                    "POST"));
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.GetEmployeesForCompany),
                    new { companyId }),
                "employee",
                "GET"));
            return links;
        }
    }
}