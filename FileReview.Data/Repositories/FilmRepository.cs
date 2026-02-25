using FileReview.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileReview.Data.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly ApplicationDbContext _context;

    public FilmRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Film?> GetFilmByIdAsync(int filmId)
    {
        return await _context.Films
            .Include(f => f.CastMembers)
            .ThenInclude(cm => cm.Actor)
            .FirstOrDefaultAsync(f => f.FilmId == filmId);
    }

    public async Task<Film?> UpdateFilmAsync(Film film)
    {
        _context.Films.Update(film);
        await _context.SaveChangesAsync();
        return film;
    }
}
