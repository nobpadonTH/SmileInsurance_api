using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmilesInsurance_api.Data;
using SmilesInsurance_api.DTOs;
using SmilesInsurance_api.DTOs.Auth;
using SmilesInsurance_api.Helpers;
using SmilesInsurance_api.Models;
using SmilesInsurance_api.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmilesInsurance_api.Services
{
    public class ServiceBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILoginDetailServices _loginDetail;

        private readonly LoginDetailDto _user;

        public ServiceBase(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext, ILoginDetailServices loginDetail)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
            _loginDetail = loginDetail;

            if (loginDetail.IsLogin)
            {
                _user = loginDetail.GetClaim();
            }
            else
            {
                _user = new LoginDetailDto();
            }
        }

        public Func<DateTime> Now { get; private set; } = () => DateTime.Now;

        public void SetNow(DateTime now) => Now = () => now;

        public void ResetNow() => Now = () => DateTime.Now;

        public int GetUserId() => _user.UserId;

        public string GetUsername() => _user.EmployeeCode;

        public IEnumerable<string> GetUserRoles() => _loginDetail.Roles;

        public IEnumerable<string> GetPermissions() => _loginDetail.Permissions;

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDto paginationDto) where TEntity : class
        {
            var queryable = _dbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            await _httpContext.HttpContext.InsertPaginationParametersInResponse(queryable, paginationDto.RecordsPerPage, paginationDto.Page);
            var entities = await queryable.Paginate(paginationDto).ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<TDTO> Get<TEntity, TDTO>(string id) where TEntity : class, IId
        {
            var guid = Guid.Parse(id);
            var entity = await _dbContext.Set<TEntity>().FindAsync(guid);
            var dto = _mapper.Map<TDTO>(entity);
            return dto;
        }

        protected async Task<TDTO> Post<TAdd, TEntity, TDTO>(TAdd newItem) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(newItem);
            entity.Id = new Guid();
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();
            var dto = _mapper.Map<TDTO>(entity);
            return dto;
        }

        protected async Task<TDTO> Put<TUpdate, TEntity, TDTO>(string id, TUpdate newItem) where TEntity : class, IId
        {
            var guid = Guid.Parse(id);
            var entity = await _dbContext.Set<TEntity>().FindAsync(guid);

            entity = _mapper.Map(newItem, entity);

            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<TDTO>(entity);

            return dto;
        }

        protected async Task<TDTO> Delete<TEntity, TDTO>(string id) where TEntity : class, IId, new()
        {
            var guid = Guid.Parse(id);
            //var entity = await _dbContext.Set<TEntity>().FindAsync(guid);
            var entity = new TEntity() { Id = guid };
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<TDTO>(entity);

            return dto;
        }
    }
}