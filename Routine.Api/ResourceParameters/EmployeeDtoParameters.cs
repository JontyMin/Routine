namespace Routine.Api.ResourceParameters
{
    public class EmployeeDtoParameters
    {
        private const int MaxPageSize = 20;
        public string Gender { get; set; }
        public string SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string OrderBy { get; set; } = "Name";
    }
}