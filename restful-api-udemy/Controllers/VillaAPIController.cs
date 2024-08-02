using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public VillaAPIController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        // GET: api/values 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> Get()
        {
            return Ok(_dbContext.Villas.ToList());
        }

        // GET api/values/5
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> Get(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.FirstOrDefaultAsync(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa); 
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (await _dbContext.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already exists");
            }

            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa villa = new()
            {
                Id = villaDTO.Id,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                Amenity = villaDTO.Amenity,
                ImageURL = villaDTO.ImageURL,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate
            };

            await _dbContext.Villas.AddAsync(villa);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            Villa villa = new()
            {
                Id = villaDTO.Id,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                Amenity = villaDTO.Amenity,
                ImageURL = villaDTO.ImageURL,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate
            };

            _dbContext.Update(villa);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            VillaDTO villaDTO = new()
            {
                Id = villa.Id,
                Details = villa.Details,
                Name = villa.Name,
                Amenity = villa.Amenity,
                ImageURL = villa.ImageURL,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Rate = villa.Rate
            };

            patchDto.ApplyTo(villaDTO, ModelState);

            Villa model = new()
            {
                Id = villaDTO.Id,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                Amenity = villaDTO.Amenity,
                ImageURL = villaDTO.ImageURL,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate
            };

            _dbContext.Villas.Update(model);
            await _dbContext.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            _dbContext.Villas.Remove(villa);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}

