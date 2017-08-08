﻿using System;
using Liquid.NET.Constants;
using Liquid.NET.Utils;

namespace Liquid.NET.Filters.Strings
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/string-filters#capitalize
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CapitalizeFilter : FilterExpression<ILiquidValue, LiquidString>
    {
        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, ILiquidValue liquidExpression)
        {
            String before = ValueCaster.RenderAsString(liquidExpression);
            if (String.IsNullOrWhiteSpace(before))
            {
                return LiquidExpressionResult.Success(LiquidString.Create(""));
            }
            var nospaces = before.TrimStart();
            String trimmed = before.Substring(0, before.Length - nospaces.Length);
            return LiquidExpressionResult.Success(LiquidString.Create(trimmed + char.ToUpper(nospaces[0]) + nospaces.Substring(1)));
        }
    }
}
