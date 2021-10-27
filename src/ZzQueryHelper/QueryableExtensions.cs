using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ZzSoft.QueryHelper.Parser;

namespace ZzSoft.QueryHelper
{
    public static class QueryableExtensions
    {
        private const string Orderby_Separator = ",";

        /*
        private static MethodInfo MethodContains = typeof(Enumerable).GetMethods(
          BindingFlags.Static | BindingFlags.Public)
          .Single(m => m.Name == nameof(Enumerable.Contains)
              && m.GetParameters().Length == 2);
        */

        // VErificare se si riesce ad otterne un filtro funzionante
        // dopo aver e un filtro funzionante
        // verificare se il "trucco" della proiezione funziona anche sul dB
        // https://blog.jeremylikness.com/blog/dynamically-build-linq-expressions/
        /*
public static IQueryable<TEntity> ApplyFilter<TEntity, TProperty>(
    this IQueryable<TEntity> query, 
    Expression<Func<TEntity, TProperty>> expr, TProperty value)
{
    Expression<Func<TEntity, bool>> predicate = param => true;

    var filterExpression = Expression.Equal(expr, Expression.Constant(value));
    var lambda = Expression.Lambda<Func<TEntity, bool>>(filterExpression);

    predicate = predicate.And(lambda);
    return query.Where(predicate);
} 
        */

        public static IOrderedQueryable<T> OrderBy<T>(
            this IQueryable<T> source,
            string orderByClouse)
        {
            var orderedSource = (IOrderedQueryable<T>)source;

            if (string.IsNullOrEmpty(orderByClouse) == false)
            {
                var orderPart = orderByClouse.Split(Orderby_Separator);
                bool anotherLevel = false;
                
                foreach (string part in orderPart)
                {
                    if (string.IsNullOrEmpty(part?.Trim()) == false)
                    {
                        int descending = 0;
                        string field = part;
                        if (field.EndsWith("-") == true)
                        {
                            field = string.Concat(part.Take(field.Length - 1));
                            descending = -1;
                        }
                        else if (field.EndsWith("+") == true)
                        {
                            field = string.Concat(part.Take(field.Length - 1));
                        }
                        orderedSource = orderedSource.Order<T>(
                            field.Trim(),
                            descending,
                            anotherLevel);

                        anotherLevel = true;
                    }
                }
            }
            
            return orderedSource;
        }

        // esempio di filterQuery
        // $and($eq(campo,"aaa"),$eq(campo2,"bbb"))
        public static IQueryable<T> Where<T>(
            this IQueryable<T> source,
            string filterQuery)
        {
            // crea il paratro del predicate da usare nella where condition
            var paramExp = Expression.Parameter(typeof(T));

            var tokenizer = new Tokenizer<T>();

            var tokenEnum = tokenizer.Tokenize(filterQuery).GetEnumerator();
            if (tokenEnum.MoveNext() == false) return source;

            var conditions = GetLogicalExpression(paramExp, tokenEnum);
            if (conditions.CanReduce)
            {
                conditions = conditions.ReduceAndCheck();
            }
            var query = Expression.Lambda<Func<T, bool>>(conditions, paramExp);

            return source.Where<T>(
                query.Compile())
                .AsQueryable<T>();
        }

        #region private helper funcion
        private static IOrderedQueryable<T> Order<T>(
            this IQueryable<T> source,
            string propertyName,
            int descending,
            bool anotherLevel = false)
        {
            var param = Expression.Parameter(typeof(T), string.Empty);
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") +
                (descending == -1 ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        private static Expression GetLogicalExpression (
            ParameterExpression paramExp,
            IEnumerator<DslToken> tokenEnum)
        {
            Expression exp;
            var token = tokenEnum.Current;

            if (token.TokenType == TokenType.And
                || token.TokenType == TokenType.Or)
            {
                if (tokenEnum.MoveNext() == false) return null;
                var expLeft = GetLogicalExpression(paramExp, tokenEnum);
                var expRight = GetLogicalExpression(paramExp, tokenEnum);
                if (token.TokenType != TokenType.CloseParenthesis)
                {
                    throw new ApplicationException("stringa do query non crretta");
                }
                if (token.TokenType == TokenType.And)
                {
                    exp = Expression.And(expLeft, expRight);
                }
                else
                {
                    exp = Expression.Or(expLeft, expRight);
                }
            }
            else
            {
                exp = GetBinaryExpression(paramExp, tokenEnum);
            }
            return exp;
        }
        private static Expression GetBinaryExpression (
            ParameterExpression paramExp,
            IEnumerator<DslToken> tokenEnum)
        {
            Expression exp = null;
            var token = tokenEnum.Current;
            Func<Expression, Expression, Expression> func = null;
            switch (token.TokenType)
            {
                case TokenType.Equals:
                    func = (l, r) => Expression.Equal(l, r);
                    break;
                case TokenType.NotEquals:
                    func = (l, r) => Expression.NotEqual(l, r);
                    break;

                case TokenType.LessThan:
                    func = (l, r) => Expression.LessThan(l, r);
                    break;
                case TokenType.LessOrEqual:
                    func = (l, r) => Expression.LessThanOrEqual(l, r);
                    break;

                case TokenType.GreaterThan:
                    func = (l, r) => Expression.GreaterThan(l, r);
                    break;
                case TokenType.GreaterOrEqual:
                    func = (l, r) => Expression.GreaterThanOrEqual(l, r);
                    break;
            }
            if (func != null)
            {
                // Attributo da verificare
                var expAttr = GetAttributeName (paramExp, tokenEnum);
                if (tokenEnum.MoveNext() == false) return null;
                if (tokenEnum.Current.TokenType == TokenType.Comma)
                {
                    // valore di confronto
                    Expression expRight = GetValues(tokenEnum, expAttr.Type);
                    exp = func(expAttr, expRight);
                }
            }
            return exp;
        }
        private static MemberExpression GetAttributeName (
            ParameterExpression paramExp,
            IEnumerator<DslToken> tokenEnum)
        {
            if (tokenEnum.MoveNext() == false) return null;
            MemberExpression exp = null;
            var toke = tokenEnum.Current;
            if (toke.TokenType== TokenType.AttributeName)
            {
                exp = Expression.Property(paramExp, toke.Value);
            }
            return exp;
        }

        private static Expression GetValues(
            IEnumerator<DslToken> tokenEnum,
            Type dsiredType)
        {
            var list = new List<object>();
            while (tokenEnum.MoveNext() == true)
            {
                if (tokenEnum.Current.TokenType == TokenType.CloseParenthesis) break;
                if (tokenEnum.Current.TokenType == TokenType.Comma) continue;
                list.Add(GetTokenValue(tokenEnum.Current, dsiredType));
            }
            Expression exp = null;
            if (list.Count == 1)
            {
                exp = Expression.Constant(list[0]);
            }
            else if (list.Count > 1)
            {
                exp = Expression.Constant(list);
            }
            return exp;
        }
        private static object GetTokenValue (
            DslToken token,
            Type dsiredType)
        {
            return token.TokenType switch
            {
                TokenType.Number => token.GetNumericValue(dsiredType),
                TokenType.DateTimeValue => token.GetDatetTimeValue(),
                TokenType.StringValue => token.GetStringValue(),
                TokenType.TrueValue => true,
                TokenType.FalseValue => false,
                _ => null
            };
        }
        #endregion
    }
}
