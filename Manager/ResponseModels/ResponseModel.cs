
namespace Manager.Model;

public class ResponseModel<T> where T : class
{
    public int total { get; set; }
    public int page { get; set; }
    public dynamic data { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
