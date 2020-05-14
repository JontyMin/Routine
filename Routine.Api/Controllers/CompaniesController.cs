﻿using System;
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

        public CompaniesController(ICompanyRepository companyRepository,IMapper mapper,IPropertyMappingService proertyMappingService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper= mapper  ?? throw new ArgumentNullException(nameof(mapper));
            _proertyMappingService = proertyMappingService ?? throw new ArgumentNullException(nameof(proertyMappingService));
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
            return Ok(companyDtos);
        }

        /// <summary>
        /// 根据id查询公司
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("{companyId}",Name = nameof(GetCompany))]
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
            return Ok(_mapper.Map<CompanyDto>(company));
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
            
            //返回一个路径，获取当前添加的资源
            return CreatedAtRoute(nameof(GetCompany), new {companyId = retuenDto.Id}, retuenDto);
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
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }
    }
}