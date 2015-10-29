﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liquid.NET.Constants;
using Liquid.NET.Utils;

namespace Liquid.NET.Filters.Strings
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReplaceFilter : FilterExpression<IExpressionConstant, StringValue>
    {
        private readonly StringValue _stringToRemove;
        private readonly StringValue _replacementString;

        public ReplaceFilter(StringValue stringToRemove, StringValue replacementString)
        {
            _stringToRemove = stringToRemove;
            _replacementString = replacementString;
        }


        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, IExpressionConstant liquidExpression)
        {
            return LiquidExpressionResult.Success(StringUtils.Eval(liquidExpression, x => x.Replace(_stringToRemove.StringVal, _replacementString.StringVal)));
        }

    }
}
