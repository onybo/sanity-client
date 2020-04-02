using System;
using System.Collections.Generic;
using Olav.Sanity.Client.Transactions;
using Xunit;

namespace Olav.Sanity.Client.Test
{
    public class TransactionsQueryStringBuilderTests
    {
        [Fact]
        public void BuildQueryString_EmptyQueryObject_AddsDefaultFieldsOnly()
        {
            var query = new TransactionsQuery();
            Assert.Equal("reverse=false&excludeContent=true", query.BuildQueryString());
        }

        [Fact]
        public void BuildQueryString_FullyPopulatedQueryObject_AddsAllFields()
        {
            var query = new TransactionsQuery
            {
                Limit = 10,
                FromTime = new DateTime(2020, 03, 01),
                ToTime = new DateTime(2020, 04, 01),
                FromTransaction = "123",
                ToTransaction = "456",
                Authors = new List<string> { "Author1", "Author2"},
                Reverse = true
            };
            Assert.Equal("authors=Author1%2cAuthor2" +
                         "&fromTime=2020-03-01T00%3a00%3a00.0000000" +
                         "&toTime=2020-04-01T00%3a00%3a00.0000000" +
                         "&fromTransaction=123" +
                         "&toTransaction=456" +
                         "&limit=10" +
                         "&reverse=true" +
                         "&excludeContent=true", query.BuildQueryString());
        }
    }
}