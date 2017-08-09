﻿using Xunit;

namespace Liquid.NET.Tests.Filters.Strings
{
        
    public class DownCaseFilterTests
    {
        [Fact]
        public void It_Should_Put_A_String_In_Local_Case()
        {
            // Arrange
            var result = RenderingHelper.RenderTemplate("Result : {{ \"TeSt\" | downcase }}");

            // Assert
            Assert.Equal("Result : test", result);

        }
    }
}
