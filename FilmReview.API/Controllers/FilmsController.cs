using FileReview.Core.Dtos;
using FileReview.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmReview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    /// <summary>
    /// Get film details including cast and reviews
    /// </summary>
    [HttpGet("{filmId}")]
    public async Task<ActionResult<FilmDetailDto>> GetFilm(int filmId)
    {
        var film = await _filmService.GetFilmAsync(filmId);

        if (film == null)
            return NotFound();

        return Ok(film);
    }
}
