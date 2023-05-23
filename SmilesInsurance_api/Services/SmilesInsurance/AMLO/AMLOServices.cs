using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SmilesInsurance_api.Data;
using SmilesInsurance_api.DTOs.SmilesInsurance.AMLO;
using SmilesInsurance_api.Helpers;
using SmilesInsurance_api.Models;
using SmilesInsurance_api.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SmilesInsurance_api.Services.SmilesInsurance.AMLO
{
    public class AMLOServices : IAMLOServices
    {
        private readonly AppDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly ILoginDetailServices _login;
        private readonly IHttpContextAccessor _httpcontext;
        private const string TEXTSUCCESS = "Success";

        public AMLOServices(AppDBContext dBContext, IMapper mapper, ILoginDetailServices login, IHttpContextAccessor httpcontext)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _login = login;
            _httpcontext = httpcontext;
        }

        public async Task<ServiceResponseWithPagination<List<GetAMLOLetterResponseDto>>> GetAMLOLetterPagination(GetAMLOLetterRequestDto filter)
        {
            try
            {
                Log.Information("[GetAMLOPagination] - start {date}", DateTime.Now);
                var data = _dBContext.AMLOLetter.Include(x => x.AMLOLists)
                    .Select(x => new GetAMLOLetterResponseDto
                    {
                        AMLOLetterId = x.AMLOLetterId,
                        AMLOLetterName = x.AMLOLetterName,
                        AMLOListCount = x.AMLOLists.Count,
                        CreatedByUserId = x.CreatedByUserId,
                        CreatedDate = x.CreatedDate
                    }).AsQueryable();

                Log.Information("[GetAMLOPagination] - Param {@filter}", filter);
                if (!string.IsNullOrEmpty(filter.AMLOLetterName))
                {
                    data = data.Where(x => x.AMLOLetterName.Contains(filter.AMLOLetterName)).AsQueryable();
                };

                //Ordering
                if (!string.IsNullOrWhiteSpace(filter.OrderingField))
                {
                    try
                    {
                        data = data.OrderBy($"{filter.OrderingField} {(filter.AscendingOrder ? "ascending" : "descending")}");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, "[GetAMLOPagination] - An error occurred.");
                        return ResponseResultWithPagination.Failure<List<GetAMLOLetterResponseDto>>($"Could not order by field: {filter.OrderingField}");
                    }
                }

                //Pagination
                var paginationResult = await _httpcontext.HttpContext.InsertPaginationParametersInResponse(data, filter.RecordsPerPage, filter.Page);
                var dto = await data.Paginate(filter).ToListAsync();

                //mapping dto response
                var dtoOutput = _mapper.Map<List<GetAMLOLetterResponseDto>>(dto);

                Log.Information("[GetAMLOPagination] - Done! {date}", DateTime.Now);
                return ResponseResultWithPagination.Success(dtoOutput, paginationResult, TEXTSUCCESS);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[GetAMLOPagination] - An error occurred");
                return ResponseResultWithPagination.Failure<List<GetAMLOLetterResponseDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse<InsertAMLOLetterResponseDto>> InsertAMLOLetter(InsertAMLOLetterRequestDto input)
        {
            try
            {
                Log.Information("[InsertAMLOLetter] - start {@input} ,Date: {@Date}", input, DateTime.Now);
                var isValid = await _dBContext.AMLOLetter.Where(x => x.AMLOLetterName.Equals(input.AMLOLetterName) && x.IsActive.Equals(true)).CountAsync();
                if (isValid != 0)
                {
                    Log.Information("[InsertAMLOLetter] - AMLOLetterName Duplicate");
                    return ResponseResult.Failure<InsertAMLOLetterResponseDto>($"ชื่อซ้ำ! {input.AMLOLetterName}");
                }
                //map filter dto
                Log.Information("[InsertAMLOLetter] - Mapper data Insert");
                var letterId = Guid.NewGuid();
                var amlo = _mapper.Map<AMLOLetter>(input);
                amlo.AMLOLetterId = letterId;
                amlo.IsActive = true;
                amlo.CreatedByUserId = _login.GetClaim().UserId;
                amlo.CreatedDate = DateTime.Now;
                amlo.UpdateByUserId = _login.GetClaim().UserId;
                amlo.UpdateDate = DateTime.Now;

                Log.Information("[InsertAMLOLetter] - add data Insert");
                _dBContext.AMLOLetter.Add(amlo);

                Log.Information("[InsertAMLOLetter] - Save to database");
                await _dBContext.SaveChangesAsync();

                var output = new InsertAMLOLetterResponseDto
                {
                    IsResult = true,
                    Data = letterId.ToString(),
                    Message = TEXTSUCCESS
                };
                Log.Information("[InsertAMLOLetter] - Done! Response: {@res} Time: {time}", output, DateTime.Now);
                return ResponseResult.Success(output);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[GetAMLOPagination] - An error occurred");
                return ResponseResult.Failure<InsertAMLOLetterResponseDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse<InsertAMLOListResponseDto>> InsertAMLOList(InsertAMLOListRequestDto input)
        {
            try
            {
                Log.Information("[InsertAMLOList] - start {@input} ,Date: {@Date}", input, DateTime.Now);

                //map insert
                Log.Information("[InsertAMLOList] - Mapper data Insert");
                var listId = Guid.NewGuid();
                var amlo = _mapper.Map<AMLOList>(input);
                amlo.AMLOListId = listId;
                amlo.SearchText = string.Format("{0},{1},{2}", input.IdCardNo, input.FirstName, input.LastName);
                amlo.IsBlacklist = true;
                amlo.Remark = string.Empty;
                amlo.IsActive = true;
                amlo.CreatedByUserId = _login.GetClaim().UserId;
                amlo.CreatedDate = DateTime.Now;
                amlo.UpdateByUserId = _login.GetClaim().UserId;
                amlo.UpdateDate = DateTime.Now;

                Log.Information("[InsertAMLOList] - add data Insert");
                _dBContext.AMLOList.Add(amlo);

                Log.Information("[InsertAMLOList] - Save to database");
                await _dBContext.SaveChangesAsync();

                var output = new InsertAMLOListResponseDto
                {
                    IsResult = true,
                    Data = listId.ToString(),
                    Message = TEXTSUCCESS
                };
                Log.Information("[InsertAMLOList] - Done! Response: {@res} Time: {time}", output, DateTime.Now);
                return ResponseResult.Success(output);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[InsertAMLOList] - An error occurred");
                return ResponseResult.Failure<InsertAMLOListResponseDto>(ex.Message);
            }
        }

        public async Task<ServiceResponseWithPagination<List<GetAMLOListResponseDto>>> GetAMLOListPagination(GetAMLOListRequestDto filter)
        {
            try
            {
                Log.Information("[GetAMLOListPagination] - start Date: {@Date}", DateTime.Now);
                if (filter.AMLOLetterId is null && string.IsNullOrEmpty(filter.SearchText))
                {
                    Log.Information("[GetAMLOListPagination] - Param is null");
                    return ResponseResultWithPagination.Failure<List<GetAMLOListResponseDto>>($"Param is null");
                }

                Log.Information("[GetAMLOListPagination] - Param {@filter}", filter);
                var data = _dBContext.AMLOList.Where(x => x.IsActive.Equals(true)).AsQueryable();
                if (filter.AMLOLetterId.HasValue)
                {
                    data = data.Where(x => x.AMLOLetter.Equals(filter.AMLOLetterId));
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    data = data.Where(x => x.IdCardNo.Equals(filter.SearchText) || x.FirstName.Contains(filter.SearchText) || x.LastName.Contains(filter.SearchText));
                }

                //Pagination
                var paginationResult = await _httpcontext.HttpContext.InsertPaginationParametersInResponse(data, filter.RecordsPerPage, filter.Page);
                var dto = await data.Paginate(filter).ToListAsync();

                //mapping dto response
                var dtoOutput = _mapper.Map<List<GetAMLOListResponseDto>>(dto);

                Log.Information("[GetAMLOListPagination] - Done! {date}", DateTime.Now);
                return ResponseResultWithPagination.Success(dtoOutput, paginationResult, TEXTSUCCESS);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[GetAMLOListPagination] - An error occurred");
                return ResponseResultWithPagination.Failure<List<GetAMLOListResponseDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse<GetAMLOListByIdResponseDto>> GetAMLOList(Guid AMLOListId)
        {
            try
            {
                Log.Information("[GetAMLOList] - start Param:{param} Date: {@Date}", AMLOListId, DateTime.Now);
                var data = await _dBContext.AMLOList.Where(x => x.IsActive.Equals(true) && x.AMLOListId.Equals(AMLOListId)).ToListAsync();
                var dtoOut = _mapper.Map<List<GetAMLOListByIdResponseDto>>(data);

                Log.Information("[GetAMLOList] - Done! {date}", DateTime.Now);
                return ResponseResult.Success(dtoOut.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[GetAMLOList] - An error occurred");
                return ResponseResult.Failure<GetAMLOListByIdResponseDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse<GetAMLOLetterByIdResponseDto>> GetAMLOLetter(Guid AMLOLetterId)
        {
            try
            {
                Log.Information("[GetAMLOLetter] - start Param:{param} Date: {@Date}", AMLOLetterId, DateTime.Now);
                var data = await _dBContext.AMLOLetter.Where(x => x.IsActive.Equals(true) && x.AMLOLetterId.Equals(AMLOLetterId)).ToListAsync();
                var dtoOut = _mapper.Map<List<GetAMLOLetterByIdResponseDto>>(data);

                Log.Information("[GetAMLOLetter] - Done! {date}", DateTime.Now);
                return ResponseResult.Success<GetAMLOLetterByIdResponseDto>(dtoOut.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[GetAMLOLetter] - An error occurred");
                return ResponseResult.Failure<GetAMLOLetterByIdResponseDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse<InsertAMLOListResponseDto>> UpdateAMLOListIsBlacklist(UpdateAMLOListIsBlacklistRequestDto input)
        {
            try
            {
                Log.Information("[UpdateAMLOListIsBlacklist] - start Param:{@param} Date: {@Date}", input, DateTime.Now);
                var data = await _dBContext.AMLOList.Where(x => x.IsActive.Equals(true) && x.AMLOListId.Equals(input.AMLOListId)).ToListAsync();
                if (data.Count() == 0)
                {
                    Log.Information("[UpdateAMLOListIsBlacklist] - data not fount");
                    return ResponseResult.Failure<InsertAMLOListResponseDto>($"data not fount");
                }

                if (data.Where(x => x.IsBlacklist.Equals(input.IsBlacklist)).Count() != 0 && input.IsBlacklist.Equals(true))
                {
                    Log.Information("[UpdateAMLOListIsBlacklist] - IsBlacklist not fount");
                    return ResponseResult.Failure<InsertAMLOListResponseDto>($"{data.FirstOrDefault().FirstName} ติด Blacklist อยู่แล้ว");
                }

                if (data.Where(x => x.IsBlacklist.Equals(input.IsBlacklist)).Count() != 0 && input.IsBlacklist.Equals(false))
                {
                    Log.Information("[UpdateAMLOListIsBlacklist] - IsBlacklist not fount");
                    return ResponseResult.Failure<InsertAMLOListResponseDto>($"{data.FirstOrDefault().FirstName} ถูกปลด Blacklist ไปแล้ว");
                }

                data.FirstOrDefault().Remark = input.Remark;
                data.FirstOrDefault().IsBlacklist = input.IsBlacklist;
                data.FirstOrDefault().UpdateByUserId = _login.GetClaim().UserId;
                data.FirstOrDefault().UpdateDate = DateTime.Now;
                _dBContext.UpdateRange(data);
                await _dBContext.SaveChangesAsync();

                var dtoOut = new InsertAMLOListResponseDto
                {
                    IsResult = true,
                    Message = TEXTSUCCESS
                };

                Log.Information("[UpdateAMLOListIsBlacklist] - Done! {date}", DateTime.Now);
                return ResponseResult.Success<InsertAMLOListResponseDto>(dtoOut);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, "[UpdateAMLOListIsBlacklist] - An error occurred");
                return ResponseResult.Failure<InsertAMLOListResponseDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GetAMLOLetterListResponseDto>>> GetAMLOLetter(GetAMLOLetterListRequestDto filter)
        {
            try
            {
                var data = _dBContext.AMLOLetter.Where(x => x.IsActive.Equals(true)).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (!string.IsNullOrEmpty(filter.AMLOLetterName))
                {
                    data = data.Where(x => x.AMLOLetterName.Equals(filter.AMLOLetterName));
                }

                var dataOut = _mapper.Map<List<GetAMLOLetterListResponseDto>>(await data.ToListAsync());

                return ResponseResult.Success<List<GetAMLOLetterListResponseDto>>(dataOut);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}