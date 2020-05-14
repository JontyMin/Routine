using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Routine.Api.Data;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.ResourceParameters;

namespace Routine.Api.Services
{
    public class CompanyRepository:ICompanyRepository
    {
        private readonly RoutineDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public CompanyRepository(RoutineDbContext context,IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }
        public async Task<PagedList<Company>> GetCompaniesAsync(CompanyDtoParameters companyDtoParameters)
        {
            if (companyDtoParameters == null)
            {
                throw new ArgumentNullException(nameof(companyDtoParameters));
            }

            //if (string.IsNullOrWhiteSpace(companyDtoParameters.CompanyName) &&
            //    string.IsNullOrWhiteSpace(companyDtoParameters.SearchTerm))
            //{
            //    return await _context.Companies.ToListAsync();
            //}

            var queryExpression = _context.Companies as IQueryable<Company>;

            if (!string.IsNullOrWhiteSpace(companyDtoParameters.CompanyName))
            {
                companyDtoParameters.CompanyName = companyDtoParameters.CompanyName.Trim();
                queryExpression = queryExpression.Where(x =>
                    x.Name.Contains(companyDtoParameters.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(companyDtoParameters.SearchTerm))
            {
                companyDtoParameters.SearchTerm = companyDtoParameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x =>
                    x.Name.Contains(companyDtoParameters.SearchTerm) ||
                    x.Introduction.Contains(companyDtoParameters.SearchTerm));
            }

            //queryExpression = queryExpression.Skip(companyDtoParameters.PageSize * (companyDtoParameters.PageIndex - 1))
            //    .Take(companyDtoParameters.PageSize);

            //return await queryExpression.ToListAsync();

            var mappingDictionary = _propertyMappingService.GetPropertyMapping<CompanyDto, Company>();
            queryExpression = queryExpression.ApplySort(companyDtoParameters.OrderBy, mappingDictionary);

            return await PagedList<Company>.Create(queryExpression, companyDtoParameters.PageIndex,
                companyDtoParameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId==Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return await _context.Companies
                .Where(x => companyIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id=Guid.NewGuid();
            if (company.Employees != null)
            {
                foreach (var employee in company.Employees)
                {
                    employee.Id = Guid.NewGuid();
                }
            }



            _context.Companies.Add(company);
        }

        public void UpdateCompany(Company company)
        {
            _context.Entry(company).State = EntityState.Modified;
        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Companies.Remove(company);
        }

        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId ==Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.AnyAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeeAsync(Guid companyId,EmployeeDtoParameters parameters)
        {
            if (companyId==Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var items = _context.Employees.Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(parameters.Gender))
            {
                parameters.Gender = parameters.Gender.Trim();
                var gender = Enum.Parse<Gender>(parameters.Gender);

                items = items.Where(x => x.Gender == gender);
               
            }

            //搜索
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                items = items.Where(x => x.EmployeeNo.Contains(parameters.SearchTerm)
                                         || x.FirstName.Contains(parameters.SearchTerm)
                                         || x.LastName.Contains(parameters.SearchTerm));

            }

            //排序
            //if (!string .IsNullOrWhiteSpace(parameters.OrderBy))
            //{
            //    if (parameters.OrderBy.ToLowerInvariant()=="name")
            //    {
            //        items = items.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            //    }
            //}

            var mappingDictionary = _propertyMappingService.GetPropertyMapping<EmployeeDto, Employee>();
            items = items.ApplySort(parameters.OrderBy, mappingDictionary);
            return await items.ToListAsync();

        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await _context.Employees
                .Where(x => x.CompanyId == companyId && x.Id == employeeId)
                .FirstOrDefaultAsync();
            
        }

        public void AddEmployee(Guid companyId,Employee employee)
        {
            if (companyId==Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (employee==null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            employee.CompanyId = companyId;
            _context.Employees.Add(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            //throw new NotImplementedException();
            //状态是跟踪的
            //_context.Entry(employee).State = EntityState.Modified;
        }

        public void DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}