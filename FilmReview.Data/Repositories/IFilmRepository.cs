using FilmReview.Data.Entities;

namespace FilmReview.Data.Repositories;

public interface IFilmRepository
{
    Task<Film?> GetFilmByIdAsync(int filmId);

    /// <summary>
    /// Update a film with new statistics
    /// </summary>
    Task<Film?> UpdateFilmAsync(Film film);
}

