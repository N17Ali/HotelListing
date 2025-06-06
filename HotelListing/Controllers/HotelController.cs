﻿using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelController> _logger;
    private readonly IMapper _mapper;

    public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    #region Get Hotels
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await _unitOfWork.Hotels.GetAll();
        var results = _mapper.Map<IList<HotelDTO>>(hotels);
        return Ok(results);
    }
    #endregion

    #region Get Hotel
    [HttpGet("{id:int}", Name = "GetHotel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id,
            new List<string> { "Country" });
        var result = _mapper.Map<HotelDTO>(hotel);
        return Ok(result);
    }
    #endregion

    #region Create Hotel
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
    {
        _logger.LogInformation($"attempting to Create Hotel: {hotelDTO}", hotelDTO);

        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
            return BadRequest(ModelState);
        }

        var hotel = _mapper.Map<Hotel>(hotelDTO);
        await _unitOfWork.Hotels.Insert(hotel);
        await _unitOfWork.Save();

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }
    #endregion

    #region Update Hotel
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
    {
        _logger.LogInformation($"attempting to Update Hotel with {id} and {hotelDTO}", hotelDTO);

        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
            return BadRequest(ModelState);
        }

        var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
        if (hotel == null)
        {
            _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
            return BadRequest("Submitted data is invalid!");
        }

        _mapper.Map(hotelDTO, hotel);
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.Save();

        return NoContent();
    }
    #endregion

    #region Delete Hotel
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        _logger.LogInformation($"attempting to DELETE hotel with id: {id}");

        if (id < 1)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
            return BadRequest();
        }

        var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
        if (hotel == null)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
            return BadRequest("Submitted data is invalid!");
        }

        await _unitOfWork.Hotels.Delete(id);
        await _unitOfWork.Save();

        return NoContent();
    } 
    #endregion
}
