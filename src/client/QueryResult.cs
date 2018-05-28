namespace Olav.Sanity.Client
{
    public class QueryResult<T>
    {
        public int Ms { get; set; }
        public string Query { get; set; }
        public T Result { get; set; }
    }
}