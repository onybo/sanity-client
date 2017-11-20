namespace Olav.Sanity.Client.Mutators
{
    public class PatchMutator : Mutator
    {
        public ISanityType Patch 
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