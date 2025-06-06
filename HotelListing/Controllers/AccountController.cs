﻿using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthManager _authManager;

    public AccountController(UserManager<ApiUser> userManager,
        ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
    {
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
        _authManager = authManager;
    }

    #region Register
    [HttpPost]
    [Route("Register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _mapper.Map<ApiUser>(userDTO);
        user.UserName = userDTO.Email;
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }
        await _userManager.AddToRolesAsync(user, userDTO.Roles);
        return Accepted();
    }
    #endregion

    #region Login
    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
    {

        _logger.LogInformation($"Login Attempt for {userDTO.Email}");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (!await _authManager.ValidateUser(userDTO))
        {
            return Unauthorized();
        }

        return Accepted(new { Token = await _authManager.CreateToken() });
    } 
    #endregion
}
