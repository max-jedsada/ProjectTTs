using Provider.ViewModel;

namespace Project.Provider.Extensions
{
    public static class QueryableExtensions
    {
        public static PagedResponseModel<T> GetPaged<T>(this IQueryable<T> input, int page, int pageSize) where T : class
        {
            if (input is null || page < 1)
            {
                return new PagedResponseModel<T>
                {
                    TotalPage = 0,
                    Page = page,
                    Items = Enumerable.Empty<T>()
                };
            }

            var totalItem = input.Count();
            var totalPage = Convert.ToInt32(Math.Ceiling(totalItem / (double)pageSize));
            var skipCount = (page - 1) * pageSize;

            var response = new PagedResponseModel<T>
            {
                TotalPage = totalPage,
                Page = page,
                Items = input.Skip(skipCount)
                             .Take(pageSize)
                             .ToList()
            };
            return response;
        }

    }
}
