namespace Olav.Sanity.Client.Mutators
{
    public class MutationOperation
    {
        public string Operation { get; set; }
        public string Id { get; set; }
        public object Document { get; set; }
    }

    public class MutationResult
    {        
        public string TransactionId { get; set; }
        public MutationOperation[] Results { get; set; }
    }
}