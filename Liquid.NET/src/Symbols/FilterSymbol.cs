﻿using System;
using System.Collections.Generic;
using Liquid.NET.Utils;

namespace Liquid.NET.Symbols
{
    public class FilterSymbol
    {

        public String Name { get; private set; }

        public String RawArgs { get; set; }

        //public readonly List<IExpressionDescription> Args = new List<IExpressionDescription>();
        public readonly IList<TreeNode<LiquidExpression>> Args = new List<TreeNode<LiquidExpression>>();

        public FilterSymbol(String name)
        {
            Name = name;
        }

        public void AddArg(TreeNode<LiquidExpression> obj)
        {
            Args.Add(obj);
        }

    }
}
