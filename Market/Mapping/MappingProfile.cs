using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace Market.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
            .ForCtorParam("FullAddress",
            opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
        }
    }

}
