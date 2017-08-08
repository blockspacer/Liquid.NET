﻿using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting;
using Liquid.NET.Constants;
using Liquid.NET.Filters;
using Liquid.NET.Utils;
using NUnit.Framework;

namespace Liquid.NET.Tests.Examples
{
    /// <summary>
    /// Examples in the Wiki
    /// </summary>
    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void Test_Simple_Template()
        {
            ITemplateContext ctx = new TemplateContext();

            ctx.DefineLocalVariable("myvariable", LiquidString.Create("Hello World"));

            var parsingResult = LiquidTemplate.Create("<div>{{myvariable}}</div>");

            Assert.That(parsingResult.LiquidTemplate.Render(ctx).Result, Is.EqualTo("<div>Hello World</div>"));

        }

        [Test]
        public void Test_Simple_Collection()
        {
            ITemplateContext ctx = new TemplateContext();

            ctx.DefineLocalVariable("items", new LiquidCollection
            {
                LiquidNumeric.Create(2), 
                LiquidNumeric.Create(4),
                LiquidNumeric.Create(6)
            });

            var parsingResult = LiquidTemplate.Create("<ul>{% for item in items %}<li>{{item}}</li>{% endfor %}</ul>");

            Assert.That(parsingResult.LiquidTemplate.Render(ctx).Result, Is.EqualTo("<ul><li>2</li><li>4</li><li>6</li></ul>"));

        }

        [Test]
        public void Test_Simple_Hash()
        {
            ITemplateContext ctx = new TemplateContext();
            var nameHash = new LiquidHash
            {
                {"first", LiquidString.Create("Tobias")},
                {"last", LiquidString.Create("Lütke")}
            };

            ctx.DefineLocalVariable("greeting", new LiquidHash
            {
                {"address", LiquidString.Create("Hello")},
                {"name", nameHash}
            });

            var parsingResult = LiquidTemplate.Create("You said '{{ greeting.address }} {{ greeting.name.first }} {{ greeting.name.last }}'");

            Assert.That(parsingResult.LiquidTemplate.Render(ctx).Result, Is.EqualTo("You said 'Hello Tobias Lütke'"));

        }

        [Test]
        public void Test_Filter()
        {
            ITemplateContext ctx = new TemplateContext()
                .WithAllFilters()
                .DefineLocalVariable("resultcount", LiquidNumeric.Create(42))
                .DefineLocalVariable("searchterm", LiquidString.Create("MiXeDcAsE"));

            var parsingResult = LiquidTemplate.Create("{{ resultcount }} {{ resultcount | pluralize: 'item', 'items' }} were found for '{{searchterm | downcase}}'.");

            Assert.That(parsingResult.LiquidTemplate.Render(ctx).Result, Is.EqualTo("42 items were found for 'mixedcase'."));

        }

        [Test]
        public void Test_Parsing_Error()
        {
            var parsingResult = LiquidTemplate.Create("This filter delimiter is not terminated: {{ myfilter");            
            String error = String.Join(",", parsingResult.ParsingErrors.Select(x => x.ToString()));
            Assert.That(error, Does.Contain("line 1:52 at <EOF>: Missing '}}'"));            
        }

        [Test]
        public void Test_Rendering_Error()
        {
            ITemplateContext ctx = new TemplateContext().WithAllFilters();
            var parsingResult = LiquidTemplate.Create("Divide by zero result in: {{ 1 | divided_by: 0}}");
            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);
            String error = String.Join(",", renderingResult.RenderingErrors.Select(x => x.Message));
            //Console.WriteLine("The ERROR was : " + error);
            //Console.WriteLine("The RESULT was : " + renderingResult.Result);
            Assert.That(error, Does.Contain("Liquid error: divided by 0"));
        }

        [Test]
        public void Poco_Object_Should_Be_Serialized()
        {
            ITemplateContext ctx = new TemplateContext()
                .DefineLocalVariable("poco", new MyPoco
                {
                    MyStringField = "A string field",
                    MyNullableIntField = 123,
                    MyOtherField = "Some Other Field",
                    MyIgnoredField = "This Shouldn't Show Up",
                    MyIgnoreIfNotNullField = null,
                    NestedPoco = new MyPoco { MyStringField = "Nested Poco"}

                }.ToLiquid());

            var parsingResult = LiquidTemplate.Create("Poco Result: {{ poco }}");
            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);

            Assert.That(renderingResult.Result, Does.Contain(@"Poco Result: { ""mystringfield"" : ""A string field"", ""mynullableintfield"" : 123, ""myrenamedfield"" : ""Some Other Field"", ""nestedpoco"" : { ""mystringfield"" : ""Nested Poco"", ""mynullableintfield"" : null, ""myrenamedfield"" : null } }"));
        }

        public class MyPoco
        {
            public String MyStringField { get; set; }

            public int? MyNullableIntField { get; set; }

            [LiquidName("myrenamedfield")]
            public String MyOtherField { get; set; }

            [LiquidIgnoreIfNull]
            public String MyIgnoreIfNotNullField { get; set; }

            [LiquidIgnore]
            public String MyIgnoredField { get; set; }

            [LiquidIgnoreIfNull]
            public MyPoco NestedPoco { get; set; }

        }



        public class MyUpCaseFilter : FilterExpression<LiquidString, LiquidString>
        {
            public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, LiquidString liquidExpression)
            {
                return LiquidExpressionResult.Success(liquidExpression.ToString().ToUpper());
            }
        }

        [Test]
        public void It_Should_Render_In_Upper_Case()
        {
            // Arrange
            var ctx = new TemplateContext()
                .WithFilter<MyUpCaseFilter>("myupcase");
            var parsingResult = LiquidTemplate.Create("Result : {{ \"test\" | myupcase }}");

            // Act
            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);
           
            // Assert
            Assert.That(renderingResult.Result, Is.EqualTo("Result : TEST"));
        }

        [Test]
        public void Test_Introductory_Example()
        {
            // create a template context that knows about the standard filters,
            // and define a string variable "myvariable"
            ITemplateContext ctx = new TemplateContext()
                .WithAllFilters()
                .DefineLocalVariable("myvariable", LiquidString.Create("Hello World"));

            // parse the template and check for errors
            var parsingResult = LiquidTemplate.Create("<div>{{myvariable}}</div>");

            if (parsingResult.HasParsingErrors)
            {
                HandleErrors(parsingResult.ParsingErrors);
                return;
            }

            // merge the variables from the context into the template and check for errors
            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);
            if (renderingResult.HasParsingErrors)
            {
                HandleErrors(renderingResult.ParsingErrors);
                return;
            }
            if (renderingResult.HasRenderingErrors)
            {
                HandleErrors(renderingResult.RenderingErrors);
                return;
            }

            Assert.That(renderingResult.Result, Is.EqualTo("<div>Hello World</div>"));

        }

        [Test]
        public void Test_Introductory_Example_With_Syntactic_Sugar()
        {
            // create a place to accumulate parsing and rendering errors.
            var errors = new List<LiquidError>();

            // Note that you will still get a best-guess LiquidTemplate, even if you encounter errors.
            var liquidTemplate = LiquidTemplate.Create("<div>{{myvariable}}</div>")
                .OnParsingError(errors.Add)
                .LiquidTemplate;

            // [add code here to handle the parsing errors, return]
            Assert.That(errors.Any(), Is.False);

            var ctx = new TemplateContext()
                .WithAllFilters()
                .DefineLocalVariable("myvariable", LiquidString.Create("Hello World"));


            // The final String output will still be available in .Result, 
            // even when parsing or rendering errors are encountered.
            var result = liquidTemplate.Render(ctx)
                .OnAnyError(errors.Add) // also available: .OnParsingError, .OnRenderingError
                .Result;

            // [add code here to handle the parsing and rendering errors]
            Assert.That(errors.Any(), Is.False);

            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo("<div>Hello World</div>"));

        }

//        [Test]
//        public void It_Should_Throw_An_Error()
//        {
//            LiquidHash hash = new LiquidHash();
//            ITemplateContext ctx = new TemplateContext()
//                .ErrorWhenValueMissing()
//                .DefineLocalVariable("myvariable", hash);
//
//            var parsingResult = LiquidTemplate.Create("<div>{{myvariable.ss}}</div>");
//            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);
//            
//            Console.WriteLine("ERROR: " + String.Join("; ", renderingResult.RenderingErrors.Select(x => x.Message)));
//            Console.WriteLine(renderingResult.Result, "<div>ui</div>");
//        }

        private void HandleErrors(IList<LiquidError> errors)
        {
            // ...
        }
    }
}
