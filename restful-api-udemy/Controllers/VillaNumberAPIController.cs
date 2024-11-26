using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;

        public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository, IVillaRepository villaRepository,IMapper mapper)
        {
            this._villaNumberRepository = villaNumberRepository;
            this._villaRepository = villaRepository;
            this._mapper = mapper;
            this._response = new ();
        }
        // GET: api/values 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                var villaNumbersList = await _villaNumberRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
                _response.Status = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;     
        }

        // GET api/values/5
        [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _response.Status = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(u => u.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _response.Status = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.Status = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDTO villaNumberDTO)
        {
            try
            {
                if (await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNumberDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Number Already exists");
                }

                if (villaNumberDTO == null)
                {
                    return BadRequest(villaNumberDTO);
                }

                if (await _villaRepository.GetAsync(u => u.Id == villaNumberDTO.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa does not exist");
                    return BadRequest(villaNumberDTO);
                }

                if (villaNumberDTO.VillaNo < 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberDTO);

                await _villaNumberRepository.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.Status = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { villaNo = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody]VillaNumberUpdateDTO villaNumberDTO)
        {
            try
            {
                if (villaNumberDTO == null || villaNo != villaNumberDTO.VillaNo)
                {
                    _response.Status = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _villaRepository.GetAsync(u => u.Id == villaNumberDTO.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa does not exist");
                    return BadRequest(villaNumberDTO);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberDTO);

                await _villaNumberRepository.UpdateAsync(villaNumber);

                _response.IsSuccess = true;
                _response.Status = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{villaNo:int}", Name = "UpdatePartialVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, JsonPatchDocument<VillaNumberUpdateDTO> patchDto)
        {
            try
            {
                if (patchDto == null || villaNo == 0)
                {
                    return BadRequest();
                }

                var villa = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);

                if (villa == null)
                {
                    return NotFound();
                }

                VillaNumberUpdateDTO villaNumberDTO = new()
                {
                    VillaNo = villa.VillaNo,
                    SpecialDetails = villa.SpecialDetails
                };

                patchDto.ApplyTo(villaNumberDTO, ModelState);

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberDTO);

                await _villaNumberRepository.UpdateAsync(villaNumber);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _response.Status = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _response.Status = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _villaNumberRepository.DeleteAsync(villaNumber);
                await _villaNumberRepository.SaveAsync();
                _response.Status = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }
    }
}

