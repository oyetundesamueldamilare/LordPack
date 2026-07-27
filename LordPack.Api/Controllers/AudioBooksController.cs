using LordPack.Api.Interfaces;
using LordPack.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace LordPack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioBooksController : ControllerBase
{
    private readonly IAudioBookService _audioBookService;

    public AudioBooksController(IAudioBookService audioBookService)
    {
        _audioBookService = audioBookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] Testament? testament, [FromQuery] string version = "KJV")
    {
        var books = await _audioBookService.GetBooksAsync(testament, version);
        return Ok(books);
    }

    [HttpGet("{id:int}/chapters")]
    public async Task<IActionResult> GetChapters(int id)
    {
        var chapters = await _audioBookService.GetChaptersByBookIdAsync(id);
        if (chapters == null) return NotFound("Book not found.");

        return Ok(chapters);
    }

    [HttpGet("chapters/{chapterId:int}")]
    public async Task<IActionResult> GetChapterDetails(int chapterId)
    {
        var chapter = await _audioBookService.GetChapterDetailsAsync(chapterId);
        if (chapter == null) return NotFound("Chapter not found.");

        return Ok(chapter);
    }
}