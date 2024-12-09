using Newtonsoft.Json;

namespace Provider.ViewModel
{
    public class PagedResponseModel<T> where T : class
    {
        public int TotalPage { get; set; }

        public int Page { get; set; }

        public Dictionary<string, int> Totals { get; set; } = new Dictionary<string, int>();

        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    }
}
