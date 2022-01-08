﻿using System;
using System.Linq.Expressions;

namespace SimpleSplit.Domain.Base
{
    // Based mostly on
    // https://enterprisecraftsmanship.com/posts/specification-pattern-c-implementation/
    public class Specification<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public Specification(Expression<Func<T, bool>> expression)
            => _expression = expression;

        public virtual Expression<Func<T, bool>> ToExpression()
            => _expression;

        public bool IsSatisfiedBy(T entity)
        {
            var predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public static readonly Specification<T> True = new Specification<T>(i => true);
        public static readonly Specification<T> False = new Specification<T>(i => false);
    }

    public static class SpecificationExtensions
    {
        public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
        {
            var leftExpression = left.ToExpression();
            var rightExpression = right.ToExpression();

            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
            var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

            return new Specification<T>(finalExpr);
        }

        public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
        {
            var leftExpression = left.ToExpression();
            var rightExpression = right.ToExpression();

            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.Or(leftExpression.Body, rightExpression.Body);
            var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
            var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

            return new Specification<T>(finalExpr);
        }

        public static Specification<T> Not<T>(this Specification<T> left)
        {
            var leftExpression = left.ToExpression();

            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.Not(leftExpression.Body);
            var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
            var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

            return new Specification<T>(finalExpr);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
                => base.VisitParameter(_parameter);

            internal ParameterReplacer(ParameterExpression parameter) => _parameter = parameter;
        }
    }
}