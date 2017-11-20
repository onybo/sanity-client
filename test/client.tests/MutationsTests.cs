using System;
using Newtonsoft.Json;
using Olav.Sanity.Client.Mutators;
using Xunit;

namespace Olav.Sanity.Client.Test
{
    public class CreateTest : ISanityType
    {
        public string SanityTypeName => "create_test";
    }

    public class MutationsTests
    {
        [Fact]
        public void Mutations_Serialize_Should_Generate_When_Create_Added()
        {
            var mutations = new Mutations();
            var result = mutations.AddCreate(new CreateTest()).Serialize();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
