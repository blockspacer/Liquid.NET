﻿using System;
using System.Collections.Generic;
using System.Linq;
using Liquid.NET.Constants;

namespace Liquid.NET.Filters.Array
{
    public class JoinFilter : FilterExpression<ExpressionConstant, StringValue>
    {
        private readonly StringValue _separator;

        public JoinFilter(StringValue separator)
        {
            _separator = separator;
        }

//        public override StringValue Apply(ExpressionConstant objectExpression)
//        {
//            return ApplyTo((dynamic)objectExpression);
//        }

        public override StringValue ApplyTo(ArrayValue objectExpression)
        {
            var vals = objectExpression
                .ArrValue
                .Select(ValueCaster.Cast<IExpressionConstant, StringValue>)
                .Select(x => x.Value);
            return new StringValue(String.Join(_separator.StringVal, vals));
        }

        public override StringValue ApplyTo(StringValue objectExpression)
        {
            if (String.IsNullOrEmpty(objectExpression.StringVal))
            {
                return new StringValue(null);
            }
            return new StringValue(String.Join(_separator.StringVal, 
                objectExpression.StringVal.ToCharArray().Select(c => c.ToString())));

        }
    }
}
