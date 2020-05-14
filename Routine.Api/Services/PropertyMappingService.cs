using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _companyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyName",new PropertyMappingValue(new List<string>{"Name"}) },
                {"Country",new PropertyMappingValue(new List<string>{"Country"}) },
                {"Industry",new PropertyMappingValue(new List<string>{"Industry"}) },
                {"Product",new PropertyMappingValue(new List<string>{"Product"}) },
                {"Introduction",new PropertyMappingValue(new List<string>{"Introduction"}) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _employeePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string> {"Id"})},
                {"CompanyId", new PropertyMappingValue(new List<string> {"CompanyId"})},
                {"EmployeeNo", new PropertyMappingValue(new List<string> {"EmployeeNo"})},
                {"Name", new PropertyMappingValue(new List<string> {"FirstName", "LastName"})},
                {"GenderDisplay", new PropertyMappingValue(new List<string> {"Gender"})},
                {"Age", new PropertyMappingValue(new List<string> {"DateOfBirth"}, true)},

            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new ProperMapping<EmployeeDto,Employee>(_employeePropertyMapping));
            _propertyMappings.Add(new ProperMapping<CompanyDto,Company>(_companyPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<ProperMapping<TSource, TDestination>>();

            var properMappings = matchingMapping.ToList();
            if (properMappings.Count()==1)
            {
                return properMappings.First().MappingDictionary;
            }
            throw new Exception($"无法找到唯一的映射关系：{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldAfterSplit = fields.Split(",");
            foreach (var field in fieldAfterSplit)
            {
                var trimmedFields = fields.Trim();
                var indexOfFirstSpace = trimmedFields.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedFields
                    : trimmedFields.Remove(indexOfFirstSpace);
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }
    } 
}
