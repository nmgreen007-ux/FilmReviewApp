export interface ActorDto {
  actorId: number;
  name: string;
}

export interface FilmDetailDto {
  filmId: number;
  title: string;
  posterUrl: string;
  plotSummary: string;
  averageRanking: number;
  aiSummary: string | null;
  castMembers: ActorDto[];
}

export interface ReviewDto {
  reviewId: number;
  note: string;
  ranking: number;
  displayName: string | null;
  submittedAt: string;
}

export interface ReviewsListDto {
  reviews: ReviewDto[];
  totalCount: number;
  page: number;
  totalPages: number;
}

export interface CreateReviewDto {
  note: string;
  ranking: number;
  displayName?: string;
}
