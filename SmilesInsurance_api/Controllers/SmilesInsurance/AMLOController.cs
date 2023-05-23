using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmilesInsurance_api.DTOs.SmilesInsurance.AMLO;
using SmilesInsurance_api.Services.SmilesInsurance.AMLO;
using System;
using System.Threading.Tasks;

namespace SmilesInsurance_api.Controllers.SmilesInsurance
{
    [Authorize(Permission.Base)]
    [ApiController]
    [Route("api/[controller]")]
    public class AMLOController : ControllerBase
    {
        private readonly IAMLOServices _services;

        public AMLOController(IAMLOServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Get AMLOLetter By AMLOLetterId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/AMLOLetterId")]
        public async Task<IActionResult> InsertAMLOLetter(Guid id)
        {
            var data = await _services.GetAMLOLetter(id);
            return Ok(data);
        }

        /// <summary>
        /// Get AMLOLetter Dropdown
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("letterlist")]
        public async Task<IActionResult> InsertAMLOLetter([FromQuery] GetAMLOLetterListRequestDto param)
        {
            var data = await _services.GetAMLOLetter(param);
            return Ok(data);
        }

        /// <summary>
        /// Get AMLOList By AMLOListId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/AMLOListId")]
        public async Task<IActionResult> InsertAMLOList(Guid id)
        {
            var data = await _services.GetAMLOList(id);
            return Ok(data);
        }

        /// <summary>
        /// AMLOLetter
        /// </summary>
        /// <param name="param">headerId</param>
        /// <returns></returns>
        [HttpGet("letter/filter")]
        public async Task<IActionResult> GetAMLOLetterPagination([FromQuery] GetAMLOLetterRequestDto param)
        {
            var data = await _services.GetAMLOLetterPagination(param);
            return Ok(data);
        }

        /// <summary>
        /// AMLOList
        /// </summary>
        /// <param name="param">headerId</param>
        /// <returns></returns>
        [HttpGet("list/filter")]
        public async Task<IActionResult> GetAMLOListPagination([FromQuery] GetAMLOListRequestDto param)
        {
            var data = await _services.GetAMLOListPagination(param);
            return Ok(data);
        }

        /// <summary>
        /// insert AMLOLetter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("insertamloletter")]
        public async Task<IActionResult> InsertAMLOLetter(InsertAMLOLetterRequestDto input)
        {
            var data = await _services.InsertAMLOLetter(input);
            return Ok(data);
        }

        /// <summary>
        /// insert AMLOList
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("insertamlolist")]
        public async Task<IActionResult> InsertAMLOList(InsertAMLOListRequestDto input)
        {
            var data = await _services.InsertAMLOList(input);
            return Ok(data);
        }

        /// <summary>
        /// update AMLOList IsBlacklist
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("updateisblacklist")]
        public async Task<IActionResult> UpdateAMLOListIsBlacklist(UpdateAMLOListIsBlacklistRequestDto input)
        {
            var data = await _services.UpdateAMLOListIsBlacklist(input);
            return Ok(data);
        }
    }
}