using System;
using Olav.Sanity.Client.Mutators;
using Xunit;

namespace Olav.Sanity.Client.Test
{
    public class NoId : ISanityType
    {
        public string SanityTypeName => throw new NotImplementedException();
    }

    public class WithId : ISanityType
    {
        public string SanityTypeName => throw new NotImplementedException();
        public string Id { get; private set; }
    }

    public class MutationTests
    {
        [Fact]
        public void CreateOrReplace_Should_Throw_If_Id_Is_Missing()
        {
            var createOrReplace = new CreateOrReplaceMutator();
            Assert.Throws<Exception>(() => createOrReplace.CreateOrReplace = new NoId());
        }

        [Fact]
        public void CreateOrReplace_Should_Not_Throw_If_Id_Is_Present()
        {
            var createOrReplace = new CreateOrReplaceMutator();
            createOrReplace.CreateOrReplace = new WithId();
        }
    }
}
