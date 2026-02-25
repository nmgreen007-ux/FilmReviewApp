using FileReview.Data.Entities;

namespace FileReview.Data.Repositories;

public interface IFilmRepository
{
    Task<Film?> GetFilmByIdAsync(int filmId);

    /// <summary>
    /// Update a film with new statistics
    /// </summary>
    Task<Film?> UpdateFilmAsync(Film film);
}

