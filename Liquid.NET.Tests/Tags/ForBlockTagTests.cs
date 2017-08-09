﻿using System;

using System.Linq;
using Liquid.NET.Constants;
using Liquid.NET.Tests.Ruby;
using Liquid.NET.Utils;
using Xunit;

namespace Liquid.NET.Tests.Tags
{
    
    public class ForBlockTagTests
    {
        [Fact]
        public void It_Should_Iterate_Through_A_Collection()
        {
            // Arrange
            const string templateString = "Result : {% for item in array %}<li>{{ item }}</li>{% endfor %}";            
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>a string</li><li>123</li><li>456.0</li><li>false</li>", result);
        }

        [Fact]
        public void It_Should_Iterate_Through_A_Collection_Backward()
        {
            // Arrange
            const string templateString = "Result : {% for item in array reversed %}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>false</li><li>456.0</li><li>123</li><li>a string</li>", result);
        }

        [Fact]
        public void It_Should_Iterate_Through_A_Collection_Offset()
        {
            // Arrange
            const string templateString = "Result : {% for item in array offset: 2%}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>456.0</li><li>false</li>", result);
        }

        [Fact]
        public void It_Should_Iterate_Through_A_Collection_Limited()
        {
            // Arrange
            const string templateString = "Result : {% for item in array limit: 2%}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>a string</li><li>123</li>", result);
        }


        [Fact]
        public void It_Should_Iterate_Through_A_Collection_Limit_Offset()
        {
            // Arrange
            const string templateString = "Result : {% for item in array limit: 1 offset: 2 %}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>456.0</li>", result);
        }

        [Fact]
        public void It_Should_Not_Fail_If_Length_Off_End_Of_Array()
        {
            // Arrange
            const string templateString = "Result : {% for item in array limit: 6 offset: 2 %}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>456.0</li><li>false</li>", result);
        }

        [Fact]
        public void It_Should_Not_Fail_If_Offset_Off_End_Of_Array()
        {
            // Arrange
            const string templateString = "Result : {% for item in array offset: 10 %}<li>{{ item }}</li>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : ", result);
        }

        [Fact]
        public void It_Should_Allow_Variables_In_Args()
        {
            // Arrange
            const string templateString = "Result : {%for i in array limit: x offset: y %}{{ i }}{%endfor%}";
            TemplateContext ctx = new TemplateContext();
            var arr = new LiquidCollection();
            for (var index = 1; index < 10; index++)
            {
                arr.Add(LiquidNumeric.Create(index));
            }
            ctx.DefineLocalVariable("array", arr);
               
            ctx.DefineLocalVariable("x", LiquidNumeric.Create(2));
            ctx.DefineLocalVariable("y", LiquidNumeric.Create(2));
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : 34", result);
        }

        [Fact]
        public void It_Should_Not_Let_Local_Variable_Outside_Scope()
        {
            // Arrange
            const string templateString = "Result :{% for item in array %} Inside: {{ item }}{% endfor %} Outside: {{ item }}";
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : Inside: a string Inside: 123 Inside: 456.0 Inside: false Outside: ", result); // undefined is blank.
        }

        [Fact]
        public void It_Should_Iterate_Through_A_Nested_Collection()
        {
            // Arrange
            const string templateString = "Result : {% for subarray in array1 %}"
                                        + "<tr>{% for item in subarray %}"
                                        + "<td>{{item}}</td>"
                                        + "{% endfor %}"
                                        + "</tr>{% endfor %}";
            TemplateContext ctx = new TemplateContext();
            var liquidCollection = new LiquidCollection();

            var numericValues = Enumerable.Range(0, 3).Select(x => (ILiquidValue) LiquidNumeric.Create(x)).ToList();
            foreach (var item in numericValues)
            {
                liquidCollection.Add(Option<ILiquidValue>.Create(item));
            }
            //var array1 = Enumerable.Range(0, 3).Select(x => new LiquidCollection(array2);
            var array1 = new LiquidCollection { liquidCollection, liquidCollection, liquidCollection };
            ctx.DefineLocalVariable("array1", array1);
            
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            String row = "<tr><td>0</td><td>1</td><td>2</td></tr>";
            Assert.Equal("Result : "+row + row + row, result);
        }

        [Fact]
        public void It_Should_Iterate_Through_An_Array_Inside_An_Array()
        {
            // Arrange
            const string templateString = "Result : {% for item in array[1] %}<li>{{ item }}</li>{% endfor %}";
            Logger.Log(templateString);
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", new LiquidCollection{LiquidString.Create("HELLO"), CreateArrayValues()});
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : <li>a string</li><li>123</li><li>456.0</li><li>false</li>", result);

        }

        [Fact]
        public void It_Should_Iterate_Through_A_String()
        {
            // Arrange

            var template = LiquidTemplate.Create("Result : {% for item in \"Hello\" %}<li>{{ item }}</li>{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(new TemplateContext()).Result;

            // Assert
            //Assert.Equal("Result : <li>H</li><li>e</li><li>l</li><li>l</li><li>o</li>", result);
            Assert.Equal("Result : <li>Hello</li>", result);

        }



        [Theory]
        [InlineData("(0..3)","0123")]
        public void It_Should_Iterate_Through_A_Generator(String generator, String expected)
        {
            // Arrange
            var template = LiquidTemplate.Create("Result : {% for item in "+generator+" %}{{ item }}{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(new TemplateContext()).Result;

            // Assert
            Assert.Equal("Result : " + expected, result);

        }


        [Fact]
        public void It_Should_Iterate_Through_A_Generator_With_Vars()
        {
            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", new LiquidCollection{ LiquidNumeric.Create(10), LiquidNumeric.Create(13) });

            var template = LiquidTemplate.Create("Result : {% for item in (array[0]..array[1]) %}{{ item }}{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : 10111213", result);

        }



        [Fact]
        public void It_Should_Iterate_Through_A_Generator_Backwards_With_Vars()
        {
            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", new LiquidCollection { LiquidNumeric.Create(5), LiquidNumeric.Create(3) });

            var template = LiquidTemplate.Create("Result : {% for item in (array[0]..array[1]) %}{{ item }}{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : 543", result);

        }

        [Fact]
        public void It_Should_Find_The_Parent_Loop()
        {
            
            TemplateContext ctx = new TemplateContext();
            //ctx.Define("outer", new LiquidCollection(new List<ILiquidValue> { new LiquidNumeric(1), new LiquidNumeric(1), new LiquidNumeric(1) }));
            ctx.DefineLocalVariable("outer", DictionaryFactory.CreateArrayFromJson("[[1, 1, 1], [1, 1, 1]]"));

            var template = LiquidTemplate.Create("Result :{% for inner in outer %}{% for k in inner %} {{ forloop.parentloop.index }}.{{ forloop.index }}{% endfor %}{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : 1.1 1.2 1.3 2.1 2.2 2.3", result);

        }

        [Fact(Skip="Can't do this yet")]
        public void It_Can_Use_Reserved_Words()
        {
            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("limit", LiquidNumeric.Create(3));
            ctx.DefineLocalVariable("offset", LiquidNumeric.Create(1));
            var template = LiquidTemplate.Create("Result : {% for wef in \"Hello\" limit:limit offset:offset %}<li>{{ for }}</li>{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(new TemplateContext()).Result;

            // Assert
            Assert.Equal("Result : <li>e</li><li>l</li><li>l</li>", result);

        }

        [Fact]
        public void It_Should_Print_Loop_Variables_From_A_String()
        {

            // Arrange
            String input = @"{%for val in string%}{{forloop.name}}-{{forloop.index}}-{{forloop.length}}-{{forloop.index0}}-{{forloop.rindex}}-{{forloop.rindex0}}-{{forloop.first}}-{{forloop.last}}-{{val}}{%endfor%}";
            ITemplateContext ctx = new TemplateContext();

            ctx.DefineLocalVariable("string", LiquidString.Create("test string"));
            var template = LiquidTemplate.Create(input);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;
            Logger.Log(result);
            // Assert
            Assert.Equal(@"val-string-1-1-0-1-0-true-true-test string", result.Trim());
        }

        [Fact]
        public void It_Should_Allow_Logic_on_For_Loop()
        {

            // Arrange
            String input = @"{% assign array = ""1,2,3,4"" | split : "","" %}{%for val in array%}{% if forloop.index | modulo: 2 == 0 %}even{% else %}odd{%endif%}{%endfor%}";
            ITemplateContext ctx = new TemplateContext().WithAllFilters();

            //ctx.DefineLocalVariable("string", LiquidString.Create("test"));
            var template = LiquidTemplate.Create(input);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;
            Logger.Log(result);
            // Assert
            Assert.Equal(@"oddevenoddeven", result.Trim());
        }



        /// <summary>
        /// forloop.length      # => length of the entire for loop
        /// forloop.index       # => index of the current iteration
        /// forloop.index0      # => index of the current iteration (zero based)
        /// forloop.rindex      # => how many items are still left?
        /// forloop.rindex0     # => how many items are still left? (zero based)
        /// forloop.first       # => is this the first iteration?
        /// forloop.last        # => is this the last iteration?
        /// </summary>
        [Theory]
        [InlineData("forloop.first", "", "true false false false")]
        [InlineData("forloop.index", "", "1 2 3 4")]
        [InlineData("forloop.index0", "", "0 1 2 3")]
        [InlineData("forloop.rindex", "", "4 3 2 1")]
        [InlineData("forloop.rindex0", "", "3 2 1 0")]
        [InlineData("forloop.last", "", "false false false true")]
        [InlineData("forloop.length", "", "4 4 4 4")]

        public void It_Should_Insert_ForLoop_First(String varname, String parms, String expected)
        {
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", CreateArrayValues());

            var template = LiquidTemplate.Create("Result : {% for item in array "+parms +"%}{{ "+varname+" }} {% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;
            Logger.Log(result);

            // Assert
            Assert.Equal("Result : " + expected, result.Trim());

        }

        [Theory]
        [InlineData("(dict.start .. dict.end)")]
        [InlineData("(dict.start..5)")]
        [InlineData("(1..dict.end)")]
        public void It_Should_Iterate_Through_A_Generator_From_A_Dictionary(String generator)
        {
            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("dict", new LiquidHash
            {
                {"start", LiquidNumeric.Create(1)},
                {"end", LiquidNumeric.Create(5)}
            });
            var template = LiquidTemplate.Create("Result : {% for item in " + generator + " %}{{ item }}{% endfor %}");

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : 12345", result);

        }


        [Theory]
        [InlineData("one : ONE")]
        [InlineData("two : TWO")]
        [InlineData("three : THREE")]
        [InlineData("four : FOUR")]
        public void It_Should_Iterate_Through_A_Dictionary(String expected)
        {
            // SEE: https://github.com/Shopify/liquid/wiki/Liquid-for-Designers

            // Arrange
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("dict", new LiquidHash
            {
                {"one", LiquidString.Create("ONE")},
                {"two", LiquidString.Create("TWO")},
                {"three", LiquidString.Create("THREE")},
                {"four", LiquidString.Create("FOUR")}

            });
            var template =
                LiquidTemplate.Create(
                    "Result : {% for item in dict %}<li>{{ item[0] }} : {{ item[1] }}</li>{% endfor %}");


            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Contains(expected, result);

        }

        [Fact]
        public void It_Should_Break_Out_Of_A_Loop()
        {
            // Arrange
            var tmpl = GetForLoop("{% break %}");

            // Act
            var result = RenderingHelper.RenderTemplate(tmpl);

            // Assert
            Assert.Equal("Result : loop1loop2loop", result);
        }

        [Fact]
        public void It_Should_Skip_Part_Of_A_Loop()
        {
            // Arrange
            var tmpl = GetForLoop("{% continue %}");

            // Act 
            var result = RenderingHelper.RenderTemplate(tmpl);

            // Assert
            Assert.Equal("Result : loop1loop2looploop", result);
        }

        [Fact]
        // SEE: https://github.com/Shopify/liquid/commit/410cce97407735b02dc265ba60a893efe7c1165e
        public void It_Should_Use_Else_When_For_Loop_Has_Empty_Array()
        {
            // Arrange
            const string emptystr = "There is nothing in the collection";
            const string templateString = "Result : {% for item in array  %}<li>{{ item }}</li>{% else %}"+emptystr+"{% endfor %}";
            Logger.Log(templateString);
            TemplateContext ctx = new TemplateContext();
            ctx.DefineLocalVariable("array", new LiquidCollection());
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal("Result : "+emptystr, result);

        }

        [Theory]
        [InlineData("\'\'", "char:")]
        //[InlineData("\'abc\'", "char:a char:b char:c ")]// these don't work this way
        [InlineData("\'abc\'", "char:abc")]
        public void It_Should_Iterate_Over_A_Strings_Characters(String str, String expected)
        {
            //[InlineData(@"{% for char in characters %}I WILL NOT BE OUTPUT{% endfor %}", @"{""characters"":""""}", @"")]
            TemplateContext ctx = new TemplateContext();
            var template = LiquidTemplate.Create(@"{% for char in "+str+" %}char:{{char}}{% endfor %}");
            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void It_Should_Remove_The_Upper_For_Limit()
        {
            // Arrange
            const string templateString = "Result : {% for item in array%}<li>{{ item }}</li>{% endfor %}";
            ITemplateContext ctx = new TemplateContext()
                .WithNoForLimit();
            var list = new LiquidCollection();

            var items = 
                Enumerable.Range(1, 100)
                .Select(LiquidNumeric.Create)
                .Cast<ILiquidValue>().ToList();
            foreach (var item in items)
            {
                //list.Add((Option<ILiquidValue>)item);
                list.Add(Option<ILiquidValue>.Create(item));
            }

            ctx.DefineLocalVariable("array", list);
            var template = LiquidTemplate.Create(templateString);

            // Act
            String result = template.LiquidTemplate.Render(ctx).Result;

            // Assert
            Assert.Contains("<li>100</li>", result);
        }

        private static string GetForLoop(string txt)
        {
            return @"Result : {%assign coll = ""1,2,3,4"" | split: ','%}"
                   + "{% for item in coll %}"
                   + "loop"
                   + "{% if item > 2 %}"
                   + txt
                   + "{% endif %}"
                   + "{{item}}"
                   + "{% endfor %}";
        }

      

        private LiquidCollection CreateArrayValues()
        {
           return new LiquidCollection {
                LiquidString.Create("a string"),
                LiquidNumeric.Create(123),
                LiquidNumeric.Create(456m),
                new LiquidBoolean(false)
            };            
        }
    }
}
