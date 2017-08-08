﻿using System;
using System.Collections.Generic;
using System.Linq;
using Liquid.NET.Constants;
using Liquid.NET.Utils;

namespace Liquid.NET.Filters.Strings
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/string-filters#slice
    /// Note: documentation is wrong: negative start index starts at -1.
    /// (as per ruby http://ruby-doc.org/core-2.2.0/Array.html)
    /// Verified in liquid.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SliceFilter : FilterExpression<ILiquidValue, ILiquidValue>
    {
        private readonly LiquidNumeric _start;
        private readonly LiquidNumeric _length;

        public SliceFilter(LiquidNumeric start, LiquidNumeric length)
        {
            _start = start;
            _length = length;
        }

        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, ILiquidValue liquidExpression)
        {
            return LiquidExpressionResult.Error("Can't slice a object of type '"+liquidExpression.LiquidTypeName+"'.  It is not an array or a string.");
        }

        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, LiquidCollection liquidArrayExpression)
        {
            if (_start == null)
            {
                return LiquidExpressionResult.Error("Please pass a start parameter.");
            }

            return LiquidExpressionResult.Success(new LiquidCollection(SliceList(liquidArrayExpression)));
        }

        public override LiquidExpressionResult ApplyTo(ITemplateContext ctx, LiquidString liquidString)
        {
            var list = liquidString.StringVal.ToCharArray().ToList();

            if (_start == null)
            {
                return LiquidExpressionResult.Error("Please pass a start parameter.");
            }

            return LiquidExpressionResult.Success(LiquidString.Create(String.Concat(SliceList(list))));
        }

        private IList<T> SliceList<T>(IList<T> list)
        {
            var take = 1;
            var skip = _start.IntValue;
            skip = FindPosition(list, skip);

            if (_length != null)
            {
                take = _length.IntValue;
            }
            return take < 0 ? new List<T>() : Slice(list, skip, take);
        }

        /// <summary>
        /// Negative counts frome end.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        private static int FindPosition<T>(ICollection<T> list, int skip)
        {
            if (skip < 0)
            {
                skip = list.Count + skip;
            }
            return skip;
        }


        private static IList<T> Slice<T>(IEnumerable<T> list, int skip, int take)
        {            
            return new List<T>(list.Skip(skip).Take(take));            
        }

    }
}
