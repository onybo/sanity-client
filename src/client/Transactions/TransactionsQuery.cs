using System;
using System.Collections.Generic;

namespace sanity
{
    public class TransactionsQuery
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public string FromTransaction { get; set; }
        public string ToTransaction { get; set; }
        public IList<string> Authors { get; set; } = new List<string>();
        public bool Reverse { get; set; }
        public int? Limit { get; set; }
    }
}
