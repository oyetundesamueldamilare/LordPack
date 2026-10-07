using LordPack.Api.Interfaces;
using LordPack.Shared.DTOs;
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

    // GET /api/audiobooks?testament=OldTestament&version=KJV
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AudioBookDto>>> GetBooks(
        [FromQuery] Testament? testament,
        [FromQuery] string version = "KJV")
    {
        var books = await _audioBookService.GetAudioBookDtosAsync(testament, version);
        return Ok(books);
    }

    // GET /api/audiobooks/{id}/chapters
    [HttpGet("{id:int}/chapters")]
    public async Task<IActionResult> GetChapters(int id)
    {
        var chapters = await _audioBookService.GetChaptersByBookIdAsync(id);
        if (chapters == null) return NotFound("Book not found.");
        return Ok(chapters);
    }

    // GET /api/audiobooks/chapters/{chapterId}/details  (matches mobile client)
    [HttpGet("chapters/{chapterId:int}/details")]
    public async Task<IActionResult> GetChapterDetails(int chapterId)
    {
        var chapter = await _audioBookService.GetChapterDetailsAsync(chapterId);
        if (chapter == null) return NotFound("Chapter not found.");
        return Ok(chapter);
    }

    // GET /api/audiobooks/books?testament=OldTestament  (Text Bible book list)
    [HttpGet("books")]
    public async Task<IActionResult> GetBibleBooks([FromQuery] Testament? testament)
    {
        var books = await _audioBookService.GetBibleBooksAsync(testament);
        return Ok(books);
    }
}