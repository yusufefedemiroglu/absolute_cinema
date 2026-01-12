using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET api/users
    [Authorize] //for now we authorize all authenticated users later we can restrict to admin role
    [HttpGet]
    [ProducesResponseType(typeof(List<UserReadDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllReadAsync();

        if (users.Count == 0)
            return NotFound(new { Message = "No users found." });

        return Ok(users);
    }

    // GET api/users/5
    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetReadByIdAsync(id);

        if (user == null)
            return NotFound(new { Message = "User not found." });

        return Ok(user);
    }

    // POST api/users
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] UserCreateDto request)
    {
        if (request == null)
            return BadRequest(new { Message = "Request body is required." });

        var newId = await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = newId },
            new { Id = newId }
        );
    }
}