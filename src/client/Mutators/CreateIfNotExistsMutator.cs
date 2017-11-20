namespace Olav.Sanity.Client.Mutators
{
    public class CreateIfNotExistsMutator : Mutator
    {
        public ISanityType CreateIfNotExists 
        {
            get => _obj;
            set 
            {
                RequireId(value);
                _obj =value;
            } 
        }
    }
}