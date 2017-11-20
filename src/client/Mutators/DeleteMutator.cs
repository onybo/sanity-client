namespace Olav.Sanity.Client.Mutators
{
    public class DeleteMutator : Mutator
    {
        private IHaveId _idObj;
        public IHaveId Delete 
        {
            get => _idObj;
            set 
            {
                RequireId(value);
                _idObj = value;
            } 
        }
    }
}