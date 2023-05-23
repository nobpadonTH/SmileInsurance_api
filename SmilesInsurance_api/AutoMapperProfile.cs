using AutoMapper;
using SmilesInsurance_api.DTOs.SmilesInsurance.AMLO;
using SmilesInsurance_api.Models;

namespace SmilesInsurance_api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AMLOLetter, GetAMLOLetterResponseDto>();
            CreateMap<AMLOList, GetAMLOListResponseDto>();
            CreateMap<InsertAMLOLetterRequestDto, AMLOLetter>();
            CreateMap<InsertAMLOListRequestDto, AMLOList>();
            CreateMap<AMLOLetter, GetAMLOLetterByIdResponseDto>();
            CreateMap<AMLOList, GetAMLOListByIdResponseDto>();
        }
    }
}