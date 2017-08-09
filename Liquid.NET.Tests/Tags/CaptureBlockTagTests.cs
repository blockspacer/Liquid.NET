﻿using System;
using System.Collections.Generic;
using Liquid.NET.Constants;
using Xunit;

namespace Liquid.NET.Tests.Tags
{
    
    public class CaptureBlockTagTests
    {
        [Fact]
        public void It_Should_Put_A_Block_In_A_Variable()
        {
            // Arrange
            ITemplateContext ctx = new TemplateContext().WithAllFilters();
            var template = LiquidTemplate.Create("{% assign foo = \"test\" | upcase %}{% capture cap %}Result: {{ foo }}{% endcapture %}Captured: {{ cap }}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Captured: Result: TEST", result);

        }

        [Fact]
        public void It_Should_Assign_A_Cycle_To_A_Capture()
        {
            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());

            var template =
                LiquidTemplate.Create(
                    "Result : {% for item in array %}{% capture thecycle %}{% cycle 'odd', 'even' %}{% endcapture %}{{ thecycle }} {% endfor %}");

            String result = template.LiquidTemplate.Render(ctx).Result;
            Logger.Log(result);

            // Assert
            Assert.Equal("Result : odd even odd even", result.TrimEnd());

        }

        private LiquidCollection CreateArrayValues()
        {
            return new LiquidCollection
            {
                LiquidString.Create("a string"),
                LiquidNumeric.Create(123),
                LiquidNumeric.Create(456m),
                new LiquidBoolean(false)
            };
        }

    }
}
