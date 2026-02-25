import type { FilmDetailDto } from "../types/index";

interface OverviewProps {
  film: FilmDetailDto;
}

function Overview({ film }: OverviewProps) {
  return (
    <div className="overview-container">
      <div className="film-header">
        <img src={film.posterUrl} alt={film.title} className="film-poster" />
        <div className="film-info">
          <h1>{film.title}</h1>

          <div className="cast-section">
            <h3>Cast</h3>
            <ul className="cast-list">
              {film.castMembers.slice(0, 2).map((actor) => (
                <li key={actor.actorId}>{actor.name}</li>
              ))}
            </ul>
          </div>
        </div>
      </div>
      <div className="plot-summary">
        <h3>Plot</h3>
        <p>{film.plotSummary}</p>
      </div>

      <hr className="section-separator" />

      {/* Show average ranking and AI summary if available */}
      <div
        className={`rating-summary-row ${film.aiSummary ? "has-ai-summary" : ""}`}
      >
        {/* Show "Average Ranking" label only if AI summary is available to avoid confusion */}
        <p className="rating-label">Average Ranking</p>
        {film.aiSummary && <p className="summary-label">What reviews say:</p>}

        {/* Show average ranking value only if AI summary is available to avoid confusion 
        As the ranking is stored as decimal, convert it to a fixed-point number with one decimal place */}
        <p className="rating-value">{film.averageRanking.toFixed(1)}/10</p>
        {film.aiSummary && <p className="summary-text">{film.aiSummary}</p>}
      </div>
    </div>
  );
}
export default Overview;
