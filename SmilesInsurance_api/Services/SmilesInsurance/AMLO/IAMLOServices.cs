using SmilesInsurance_api.DTOs.SmilesInsurance.AMLO;
using SmilesInsurance_api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmilesInsurance_api.Services.SmilesInsurance.AMLO
{
    public interface IAMLOServices
    {
        Task<ServiceResponseWithPagination<List<GetAMLOLetterResponseDto>>> GetAMLOLetterPagination(GetAMLOLetterRequestDto filter);

        Task<ServiceResponseWithPagination<List<GetAMLOListResponseDto>>> GetAMLOListPagination(GetAMLOListRequestDto filter);

        Task<ServiceResponse<GetAMLOListByIdResponseDto>> GetAMLOList(Guid AMLOListId);

        Task<ServiceResponse<GetAMLOLetterByIdResponseDto>> GetAMLOLetter(Guid AMLOLetterId);

        Task<ServiceResponse<List<GetAMLOLetterListResponseDto>>> GetAMLOLetter(GetAMLOLetterListRequestDto filter);

        Task<ServiceResponse<InsertAMLOLetterResponseDto>> InsertAMLOLetter(InsertAMLOLetterRequestDto input);

        Task<ServiceResponse<InsertAMLOListResponseDto>> InsertAMLOList(InsertAMLOListRequestDto input);

        Task<ServiceResponse<InsertAMLOListResponseDto>> UpdateAMLOListIsBlacklist(UpdateAMLOListIsBlacklistRequestDto input);
    }
}