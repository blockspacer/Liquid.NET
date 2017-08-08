﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Liquid.NET.Constants;
using Liquid.NET.Expressions;
using Liquid.NET.Symbols;
using Liquid.NET.Tags;
using Liquid.NET.Utils;
using NUnit.Framework;

namespace Liquid.NET.Tests
{
    [TestFixture]
    public class LiquidASTGeneratorTests
    {
        [Test]
        public void It_Should_Parse_An_Object_Expression()
        {
            // Arrange
            var ast = CreateAST("Result : {{ 123 }}");

            // Assert

            var liquidExpressions = FindNodesWithType(ast, typeof (LiquidExpressionTree));
            Logger.Log("There are " + ast.RootNode.Children.Count+" Nodes");
            Logger.Log("It is " + ast.RootNode.Children[0].Data);
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));
        }

        private static LiquidAST CreateAST(string template)
        {
            LiquidASTGenerator generator = new LiquidASTGenerator();

            // Act
           return generator.Generate(template).LiquidAST;

        }

        [Test]
        public void It_Should_Parse_An_Object_Expression_With_A_Variable()
        {
            // Arrange
            var ast = CreateAST("Result : {{ a }}");

            // Assert

            var liquidExpressions = FindNodesWithType(ast, typeof(LiquidExpressionTree));
            Logger.Log("There are " + ast.RootNode.Children.Count + " Nodes");
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));

        }


        [Test]
        public void It_Should_Parse_An_Object_Expression_With_An_Propertied_Variable()
        {
            // Arrange
            var ast = CreateAST("Result : {{ a.b }}");

            // Assert

            var liquidExpressions = FindNodesWithType(ast, typeof(LiquidExpressionTree));
            Logger.Log("There are " + ast.RootNode.Children.Count + " Nodes");
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));

        }



        [Test]
        public void It_Should_Find_A_Filter()
        {
            // Arrange
            var ast = CreateAST("Result : {{ 123 | plus: 3}}");

            // Assert
            var liquidExpressions = FindNodesWithType(ast, typeof(LiquidExpressionTree)).FirstOrDefault();
            Assert.That(liquidExpressions, Is.Not.Null);
            // ReSharper disable once PossibleNullReferenceException
            var liquidExpression = ((LiquidExpressionTree)liquidExpressions.Data);

            Assert.That(liquidExpression.ExpressionTree.Data.FilterSymbols.Count, Is.EqualTo(1));

        }

        [Test]
        public void It_Should_Find_A_Filter_Argument()
        {
            // Arrange
            var ast = CreateAST("Result : {{ 123 | add: 3}}");

            // Assert
            var liquidExpressions = FindNodesWithType(ast, typeof (LiquidExpressionTree)).FirstOrDefault();
            Assert.That(liquidExpressions, Is.Not.Null);
            // ReSharper disable once PossibleNullReferenceException
            var liquidExpression = (LiquidExpressionTree) liquidExpressions.Data;
            // ReSharper disable once PossibleNullReferenceException
            Assert.That(liquidExpression.ExpressionTree.Data.FilterSymbols.FirstOrDefault().Args.Count, Is.EqualTo(1));

        }

        [Test]
        public void It_Can_Find_Two_Object_Expressions()
        {
            // Arrange
            var ast = CreateAST("Result : {{ 123 }} {{ 456 }}");

            // Assert
            var liquidExpressions = FindNodesWithType(ast, typeof(LiquidExpressionTree));

            Assert.That(liquidExpressions.Count(), Is.EqualTo(2));
            //Assert.That(generator.GetNonEmptyStackErrors(), Is.Empty);
        }

        [Test]
        public void It_Should_Capture_The_Raw_Text()
        {
            // Arrange
            var ast = CreateAST("Result : {{ 123 | add: 3}} More Text.");

            // Assert
            var liquidExpressions = FindNodesWithType(ast, typeof(RawBlockTag));
            Assert.That(liquidExpressions.Count(), Is.EqualTo(2));

        }

        [Test]
        public void It_Should_Find_An_If_Tag()
        {
            // Arrange
            var ast = CreateAST("Result : {% if true %} abcd {% endif %}");

            // Assert
            var tagExpressions = FindNodesWithType(ast, typeof(IfThenElseBlockTag));
            Assert.That(tagExpressions.Count(), Is.EqualTo(1));
            //Assert.That(liquidExpressions, Is.Not.Null);
            //Assert.That(((LiquidExpression)liquidExpressions.Data).FilterSymbols.Count(), Is.EqualTo(1));

        }

        [Test]
        public void It_Should_Find_An_If_Tag_With_ElsIfs_And_Else()
        {
            // Arrange
            var ast = CreateAST("Result : {% if true %} aaaa {% elsif false %} test 2 {% elsif true %} test 3 {% else %} ELSE {% endif %}");

            // Assert
            var tagExpressions = FindNodesWithType(ast, typeof(IfThenElseBlockTag));
            Assert.That(tagExpressions.Count(), Is.EqualTo(1));
            //Assert.That(liquidExpressions, Is.Not.Null);
            //Assert.That(((LiquidExpression)liquidExpressions.Data).FilterSymbols.Count(), Is.EqualTo(1));

        }

        [Test]
        public void It_Should_Find_An_Object_Expression_Inside_A_Block_ElsIfs_And_Else()
        {
            // Arrange
            var ast = CreateAST("Result : {% if true %} 33 + 4 = {{ 33 | add: 4}} {% else %} hello {% endif %}");
            var tagExpressions = FindNodesWithType(ast, typeof(IfThenElseBlockTag)).FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            var ifThenElseTag = (IfThenElseBlockTag) tagExpressions.Data;
            var liquidExpressions = FindWhere(ifThenElseTag.IfElseClauses[0].LiquidBlock.Children, typeof(LiquidExpressionTree));
            
            // Assert
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));
        }

        [Test]
        public void It_Should_Nest_Expressions_Inside_Else()
        {
            // Arrange
            var ast = CreateAST("Result : {% if true %} 33 + 4 = {% if true %} {{ 33 | add: 4}} {% endif %}{% else %} hello {% endif %}");

            var tagExpressions = FindNodesWithType(ast, typeof(IfThenElseBlockTag)).FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            var ifThenElseTag = (IfThenElseBlockTag)tagExpressions.Data;
            var blockTags = ifThenElseTag.IfElseClauses[0].LiquidBlock.Children;
            var liquidExpressions = FindWhere(blockTags, typeof(IfThenElseBlockTag));

            // Assert
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));

        }

        public static void StartVisiting(IASTVisitor visitor, TreeNode<IASTNode> rootNode)
        {
            rootNode.Data.Accept(visitor);
            rootNode.Children.ForEach(child => StartVisiting(visitor, child));
        }

        [Test]
        public void It_Should_Group_Expressions_In_Parens()
        {
            // Arrange
            var ast = CreateAST("Result : {% if true and (false or false) %}FALSE{% endif %}");

            var visitor= new DebuggingVisitor();

            StartVisiting(visitor,ast.RootNode);
             
            // Assert
            var tagExpressions = FindNodesWithType(ast, typeof(IfThenElseBlockTag)).ToList();
            var ifThenElseTag = (IfThenElseBlockTag) tagExpressions[0].Data;

            Assert.That(tagExpressions.Count, Is.EqualTo(1));
            //Assert.That(ifThenElseTag.IfElseClauses[0].RootNode.Data, Is.TypeOf<AndExpression>());            
            //TODO: TextMessageWriter otu the tree
            //Assert.That(ifThenElseTag.IfElseClauses[0].RootNode[0].Data, Is.TypeOf<AndExpression>());
            var ifTagSymbol = ifThenElseTag.IfElseClauses[0];
            //Assert.That(ifTagSymbol.RootNode.Data, Is.TypeOf<AndExpression>());
            var expressionSymbolTree = ifTagSymbol.LiquidExpressionTree;
            Assert.That(expressionSymbolTree.Data.Expression, Is.TypeOf<AndExpression>());
            Assert.That(expressionSymbolTree.Children.Count, Is.EqualTo(2));
            Assert.That(expressionSymbolTree[0].Data.Expression, Is.TypeOf<LiquidBoolean>());
            Assert.That(expressionSymbolTree[1].Data.Expression, Is.TypeOf<GroupedExpression>());
            Assert.That(expressionSymbolTree[1].Children.Count, Is.EqualTo(1));
            Assert.That(expressionSymbolTree[1][0].Data.Expression, Is.TypeOf<OrExpression>());
            Assert.That(expressionSymbolTree[1][0][0].Data.Expression, Is.TypeOf<LiquidBoolean>());
            Assert.That(expressionSymbolTree[1][0][1].Data.Expression, Is.TypeOf<LiquidBoolean>());
            //Assert.That(ifThenElseTag.IfElseClauses[0].LiquidExpression[2].Data, Is.TypeOf<GroupedExpression>());
            
            //Assert.That(liquidExpressions, Is.Not.Null);
            //Assert.That(((LiquidExpression)liquidExpressions.Data).FilterSymbols.Count(), Is.EqualTo(1));

        }

        [Test]
        [TestCase("{{ a[b[c[d][e]][f][g[h]]] }}")]
        [TestCase("{{ a[b][c] }}")]
        [TestCase("{{ a[b] }}")]
        public void It_Should_Parse_An_Indexed_Object_Reference(String tmpl)
        {
            // Arrange
            var ast = CreateAST("Result : " + tmpl);

            // Assert

            var liquidExpressions = FindNodesWithType(ast, typeof(LiquidExpressionTree));
            
            Assert.That(liquidExpressions, Is.Not.Null);

            //Logger.Log("There are " + ast.RootNode.Children.Count + " Nodes");
            //Logger.Log("It is " + ast.RootNode.Children[0].Data);
            //Assert.That(liquidExpressions.Count(), Is.EqualTo(1));
            //Assert.That(generator.GetNonEmptyStackErrors(), Is.Empty);
            Assert.That(liquidExpressions.Count(), Is.EqualTo(1));
            //String result = VariableReferenceTreeBuilderTests.VariableReferenceTreePrinter.Print(liquidExpressions);
        }

        [Test]
        public void It_Should_Parse_A_Regular_Variable()
        {
            String input = @"{{ v | plus: v }}";
            var template = LiquidTemplate.Create(input);
            
            // Act
            String result = template.LiquidTemplate.Render(new TemplateContext()
                .DefineLocalVariable("v", LiquidNumeric.Create(3))
                .WithAllFilters()).Result;
        
            // Assert
            Assert.That(result.Trim(), Is.EqualTo("6"));
        }

        [Test]
        public void It_Should_Generate_One_Error() // bug
        {
            // Arrange
            var templateResult = LiquidTemplate.Create("This tag delimiter is not terminated: {% .");

            // Assert
            Assert.That(templateResult.ParsingErrors.Count, Is.EqualTo(1));
        }

        [Test]
        public void It_Should_Generate_An_AST_Even_When_Parsing_Errors_Exist()
        {
            // Arrange
            ITemplateContext ctx = new TemplateContext();
            var templateResult = LiquidTemplate.Create("This filter is not terminated: {{ test" +
                                                       "Some more text");
            String error = String.Join(",", templateResult.ParsingErrors.Select(x => x.Message));

            Assert.That(error, Does.Contain("Missing '}}'"));
            Assert.That(templateResult.LiquidTemplate, Is.Not.Null);

            // Act
            var result = templateResult.LiquidTemplate.Render(ctx);

            // Assert
            //Console.WriteLine(result.Result);
            Assert.That(result.Result, Does.Contain("This filter is not terminated"));
            //Assert.That(result.Result, Does.Contain("Some more text")); // this seems to terminate here...


        }

        public static IEnumerable<TreeNode<IASTNode>> FindNodesWithType(LiquidAST ast, Type type)
        {
            return FindWhere(ast.RootNode.Children, type);
        }

        // ReSharper disable UnusedParameter.Local
        private static IEnumerable<TreeNode<IASTNode>> FindWhere(IEnumerable<TreeNode<IASTNode>> nodes, Type type)
        // ReSharper restore UnusedParameter.Local
        {
            return TreeNode<IASTNode>.FindWhere(nodes, x => x.GetType() == type);
        }
    }
}
