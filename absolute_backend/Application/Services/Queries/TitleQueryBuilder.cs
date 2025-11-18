using Core;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries
{
    public static class TitleQueryBuilder
    {
        //for now its basic  there will be  credits cast crew etc later.
        public static IQueryable<Title> WithGenres(this IQueryable<Title> query)
        {
            return query.Include(t => t.TitleGenres)
                        .ThenInclude(tg => tg.Genre);
        }

        public static IQueryable<Title> WithFullDetails(this IQueryable<Title> query)
        {
            return query
                .Include(t => t.TitleGenres)
                    .ThenInclude(tg => tg.Genre);
        }
    }
}