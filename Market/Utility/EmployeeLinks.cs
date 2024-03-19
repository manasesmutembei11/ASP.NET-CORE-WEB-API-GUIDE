using Contracts;
using Shared.DataTransferObjects;

namespace Market.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDto> _dataShaper;
        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto>
       dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }
    }

}
