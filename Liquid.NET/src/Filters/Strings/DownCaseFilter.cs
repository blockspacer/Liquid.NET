﻿using Liquid.NET.Constants;
using Liquid.NET.Utils;

namespace Liquid.NET.Filters.Strings
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/string-filters#downcase
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DownCaseFilter: FilterExpression<ILiquidValue, LiquidString>
    {
        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, ILiquidValue liquidExpression)
        {
            return LiquidExpressionResult.Success(StringUtils.Eval(liquidExpression, x => x.ToLower()));
        }
    }
}
