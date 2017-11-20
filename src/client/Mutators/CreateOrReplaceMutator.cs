namespace Olav.Sanity.Client.Mutators
{
    public class CreateOrReplaceMutator : Mutator
    {
        public ISanityType CreateOrReplace 
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