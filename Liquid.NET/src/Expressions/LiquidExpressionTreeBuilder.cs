﻿//using System;

using System.Collections.Generic;
using System.Linq;
using Liquid.NET.Symbols;
using Liquid.NET.Utils;

namespace Liquid.NET.Expressions
{

    public delegate void OnExpressionCompleteEventHandler(TreeNode<IExpressionDescription> expressionTree);

    public class LiquidExpressionTreeBuilder
    {

        public event OnExpressionCompleteEventHandler ExpressionCompleteEvent;

        //private TreeNode<LiquidExpression> _liquidExpressionTree;
        private TreeNode<LiquidExpression> _lastExpression; // we need to keep track of the previous one because we don't know if we're completely done or if there are filters.
        private readonly Stack<TreeNode<LiquidExpression>> _liquidExpressionStack = new Stack<TreeNode<LiquidExpression>>(); 

        public void StartLiquidExpression(IExpressionDescription expressionDescription)
        {
            //Console.WriteLine("LiquidExpressionBuilder >>> PUSH");
            LiquidExpression liquidExpression = new LiquidExpression { Expression = expressionDescription };
            var child = new TreeNode<LiquidExpression>(liquidExpression);
          
            if (_liquidExpressionStack.Any())
            {
                //Console.WriteLine(" to " + _liquidExpressionStack.Peek());
                _liquidExpressionStack.Peek().AddChild(child);
            }
//            else
//            {
//                //Console.WriteLine(" to ROOT ");
//                _liquidExpressionTree = child; 
//            }
            _liquidExpressionStack.Push(child);
        }

//        public void AddFilter(FilterSymbol filter)
//        {
//            _liquidExpressionStack.Peek().Data.FilterSymbols.Add(filter);
//        }

//        public void AddFilterSymbolToLastExpression(FilterSymbol filter)
//        {
//            _lastExpression.Data.AddFilterSymbol(filter);
//        }
//
//        public void AddFilterArgToLastExpressionsFilter(TreeNode<IExpressionDescription> filterArg)        
//        {
//            _lastExpression.Data.FilterSymbols.Last().AddArg(filterArg);
//        }
//
//        public void SetRawArgsForLastExpressionsFilter(string argstring)
//        {
//            _lastExpression.Data.FilterSymbols.Last().RawArgs = argstring;
//        }

//        public void AddFilterSymbolToCurrentExpression(FilterSymbol filter)
//        {
//            _liquidExpressionStack.Peek().Data.AddFilterSymbol(filter);
//        }
//
//        public void AddFilterArgToCurrentExpressionsFilter(TreeNode<LiquidExpression> filterArg)
//        {
//            _liquidExpressionStack.Peek().Data.FilterSymbols.Last().AddArg(filterArg);
//        }
//
//        public void SetRawArgsForCurrentExpressionsFilter(string argstring)
//        {
//            _liquidExpressionStack.Peek().Data.FilterSymbols.Last().RawArgs = argstring;
//        }


        public void EndLiquidExpression()
        {
            //Console.WriteLine("LiquidExpressionBuilder >>> POP");
            _lastExpression = _liquidExpressionStack.Pop();
            //Console.WriteLine("invoking end event...");
            // TODO maybe make this contravariant
            
            //expr.AddChildren(_lastExpression.Children);

            InvokeExpressionCompleteEvent(_lastExpression);
        }

        private void InvokeExpressionCompleteEvent(TreeNode<IExpressionDescription> expressionTree)
        {
            OnExpressionCompleteEventHandler handler = ExpressionCompleteEvent;
            if (handler != null)
            {
                handler(expressionTree);
            }
        }

         
    }

}
