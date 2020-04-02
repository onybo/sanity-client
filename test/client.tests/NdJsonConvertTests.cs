using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Olav.Sanity.Client.Transactions;
using Xunit;

namespace Olav.Sanity.Client.Test
{
    public class NdJsonConvertTests
    {
        [Fact]
        public void Deserialize_SingleLineNdJson_ProducesListWithSingleItem()
        {
            var ndJson = "{\"oneProp\": \"value1\", \"twoProp\": \"value2\"}\n";

            var result = NdJsonConvert.Deserialize<SomeJsonData>(ndJson);

            Assert.Single(result);
            Assert.Equal("value1", result.First().OneProp);
            Assert.Equal("value2", result.First().TwoProp);
        }


        [Fact]
        public void Deserialize_MultilineNdJson_ProducesListItemForeachLine()
        {
            var ndJson = "{\"oneProp\": \"value1\", \"twoProp\": \"value2\"}\n" +
                         "{\"oneProp\": \"value3\", \"twoProp\": \"value4\"}";

            var result = NdJsonConvert.Deserialize<SomeJsonData>(ndJson);

            Assert.Equal(2, result.Count);
            Assert.Equal("value1", result.First().OneProp);
            Assert.Equal("value2", result.First().TwoProp);
            Assert.Equal("value3", result.Last().OneProp);
            Assert.Equal("value4", result.Last().TwoProp);
        }
    }


    public class SomeJsonData
    {
        public string OneProp { get; set; }
        public string TwoProp { get; set; }
    }
}