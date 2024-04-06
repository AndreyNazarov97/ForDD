using AutoMapper;
using ForDD.Domain.Dto.Report;
using ForDD.Domain.Entity;

namespace ForDD.Application.Mapping
{
    public class ReportMapping : Profile
    {
        public ReportMapping()
        {
            CreateMap<Report, ReportDto>().ReverseMap();
        }

    }
}
