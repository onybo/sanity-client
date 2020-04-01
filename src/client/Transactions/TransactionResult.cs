using System;
using System.Collections.Generic;

namespace sanity
{
    public class TransactionResult
    {
        public TransactionResult(IList<Transaction> transactions)
        {
            Transactions = transactions;
        }

        public IList<Transaction> Transactions { get; }
    }

    public class Transaction
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
        public IList<KeyValuePair<string, MutationDocument>> Mutations { get; set; }
        public IList<string> DocumentIds { get; set; }
    }

    public class MutationDocument
    {
        public string Id { get; set; }
        public bool Purge { get; set; }
    }
}
