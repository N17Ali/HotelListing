using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    public CountryController(IUnitOfWork unitOfWork,
        ILogger<CountryController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    #region Get Countries
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
    {
        var countries = await _unitOfWork.Countires.GetPagedList(requestParams);
        var results = _mapper.Map<IList<CountryDTO>>(countries);
        return Ok(results);
    }
    #endregion

    #region Get Country
    [HttpGet("{id:int}", Name = "GetCountry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountry(int id)
    {
        var country = await _unitOfWork.Countires.Get(
            q => q.Id == id, new List<string> { "Hotels" });
        var result = _mapper.Map<CountryDTO>(country);
        return Ok(result);
    }
    #endregion

    #region Create Country
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
    {
        _logger.LogInformation("attempting to Create Country: {@countryDTO}", countryDTO);

        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
            return BadRequest(ModelState);
        }

        var country = _mapper.Map<Country>(countryDTO);
        await _unitOfWork.Countires.Insert(country);
        await _unitOfWork.Save();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }
    #endregion

    #region Update Country
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
    {
        _logger.LogInformation($"attempting to Update Country with {id} and {countryDTO}", countryDTO);

        if (!ModelState.IsValid || id < 0)
        {
            _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
            return BadRequest(ModelState);
        }

        try
        {
            var country = await _unitOfWork.Countires.Get(q => q.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
                return BadRequest("Submitted data is invalid!");
            }

            _mapper.Map(countryDTO, country);
            _unitOfWork.Countires.Update(country);
            await _unitOfWork.Save();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to Update country: {ex}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Internal Server Error. Please Try Agian Later!");
        }
    } 
    #endregion

    #region Delete Country
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        _logger.LogInformation($"attempting to DELETE country with id: {id}");

        if (id < 1)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
            return BadRequest();
        }

        var country = await _unitOfWork.Countires.Get(q => q.Id == id);
        if (country == null)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
            return BadRequest("Submitted data is invalid!");
        }

        await _unitOfWork.Countires.Delete(id);
        await _unitOfWork.Save();

        return NoContent();
    } 
    #endregion
}
