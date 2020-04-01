using System.Linq;
using System.Web;

namespace Olav.Sanity.Client.Transactions
{
    public static class TransactionsQueryStringBuilder
    {
        public static string BuildQueryString(this TransactionsQuery query)
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (query.Authors != null && query.Authors.Any()) queryParams.Add("authors", string.Join(",", query.Authors));
            if (query.FromTime.HasValue) queryParams.Add("fromTime", query.FromTime.Value.ToString("O"));
            if (query.ToTime.HasValue) queryParams.Add("toTime", query.ToTime.Value.ToString("O"));
            if (query.FromTransaction != null) queryParams.Add("fromTransaction", query.FromTransaction);
            if (query.ToTransaction != null) queryParams.Add("toTransaction", query.ToTransaction);
            if (query.Limit.HasValue) queryParams.Add("limit", query.Limit.Value.ToString());

            queryParams.Add("reverse", query.Reverse.ToString().ToLower());
            queryParams.Add("excludeContent", "true"); // Required by santiy

            return queryParams.ToString();
        }
    }
}
