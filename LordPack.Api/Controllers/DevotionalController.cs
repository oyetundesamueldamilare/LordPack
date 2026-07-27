using LordPack.Api.Interfaces;
using LordPack.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace LordPack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevotionalsController : ControllerBase
{
    private readonly IDevotionalService _devotionalService;

    public DevotionalsController(IDevotionalService devotionalService)
    {
        _devotionalService = devotionalService;
    }

    // READ ALL: GET api/devotionals
    [HttpGet]
    public async Task<IActionResult> GetAllDevotionals()
    {
        var devotionals = await _devotionalService.GetAllDevotionalsAsync();
        return Ok(devotionals);
    }

    // READ TODAY: GET api/devotionals/today
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayDevotional()
    {
        var devotional = await _devotionalService.GetTodayDevotionalAsync();
        if (devotional == null) return NotFound("No devotionals available.");

        return Ok(devotional);
    }

    // READ BY ID: GET api/devotionals/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDevotionalById(int id)
    {
        var devotional = await _devotionalService.GetDevotionalByIdAsync(id);
        if (devotional == null) return NotFound($"Devotional with ID {id} not found.");

        return Ok(devotional);
    }

    // CREATE: POST api/devotionals
    [HttpPost]
    public async Task<IActionResult> CreateDevotional([FromBody] Devotional devotional)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _devotionalService.CreateDevotionalAsync(devotional);
        return CreatedAtAction(nameof(GetDevotionalById), new { id = created.Id }, created);
    }

    // UPDATE: PUT api/devotionals/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDevotional(int id, [FromBody] Devotional devotional)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _devotionalService.UpdateDevotionalAsync(id, devotional);
        if (!updated) return NotFound($"Devotional with ID {id} not found.");

        return NoContent();
    }

    // DELETE: DELETE api/devotionals/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDevotional(int id)
    {
        var deleted = await _devotionalService.DeleteDevotionalAsync(id);
        if (!deleted) return NotFound($"Devotional with ID {id} not found.");

        return NoContent();
    }
}