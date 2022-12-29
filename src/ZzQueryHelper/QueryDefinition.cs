using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ZzSoft.QueryHelper.Parser;

namespace ZzSoft.QueryHelper
{
    public class QueryDefinition<T> 
        : IQueryDefinition<T>
    {
        #region costanti
        private const string InputORderBy_Separator = ",";
        private const string InputOrderAscending_Operator = "+";
        private const string InputOrderDescending_Operator = "-";

        private const string SqlField_Separator = ",";
        private const string SqlPart_Separator = ".";
        private const string SqlColumnAlias_Separator = " AS ";
        private const string SqlSorteAscending = " ASC";
        private const string SqlSorteDescending = " DESC";

        private const string SqlPaging_Fmt = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";

        private const string PARENTESIS_OPEN = "(";
        private const string PARENTESIS_CLOSE = ")";

        private const string SELECT_Name = "SELECT ";
        private const string FROM_Name = " FROM ";
        private const string WHYERE_Name = " WHERE ";
        private const string COUNT_Name = "COUNT(*) AS zz_Num";
        private const string ORDERBY_NAME = " ORDER BY ";
        private const string SQL_LIKEALLPLACEHOLDER = "%";

        private const string LOGICAL_AND = " AND ";
        private const string LOGICAL_OR = " OR ";

        private const string COmpareOpFmt_Equal = "{0} = {1}";
        private const string COmpareOpFmt_NotEqual = "{0} <> {1}";
        private const string COmpareOpFmt_Gt = "{0} > {1}";
        private const string COmpareOpFmt_Ge = "{0} >= {1}";
        private const string COmpareOpFmt_Lt = "{0} < {1}";
        private const string COmpareOpFmt_Le = "{0} <= {1}";

        private const string COmpareOpFmt_Like = "{0} LIKE {1}";
        private const string COmpareOpFmt_NotLike = "{0} NOT LIKE {1}";

        private const string SqlParameterPrefix = "@P_";

        #endregion

        private readonly IList<object> _arguments = null;
        private readonly string _tablesAndJoins;
        private readonly IFilteringContext _filterContext;
        private readonly string _filterClouse;
        private readonly object _queryArguments;

        public QueryDefinition(
            string tablesAndJoins,
            IFilteringContext filterContext = null,
            string customFilter = null,
            object customArgs = null
            )
        {
            if (filterContext == null)
            {
                filterContext = new FilteringContext(null, null, null, null, 0, int.MaxValue, false);
            }
            this._filterContext = filterContext;
            this._tablesAndJoins = tablesAndJoins;
            this._arguments = new List<object>();

            this._filterClouse = this.GetWhereClause(customFilter);

            this._queryArguments = this.CreateQeryArguments(customArgs);
            this.RequestedPage = this._filterContext.Page;
        }

        public int RequestedPage { get; private set; }

        public string GetSelectQuery()
        {
            var sb = new StringBuilder(1024);

            sb.Append(SELECT_Name);
            sb.Append(this.GetSqlFieldList());
            sb.Append(FROM_Name);
            sb.Append(this._tablesAndJoins);

            // il filtro va qui
            if (string.IsNullOrEmpty(this._filterClouse) == false)
            {
                sb.Append(WHYERE_Name);
                sb.Append(this._filterClouse);
            }
            sb.Append(ORDERBY_NAME);
            sb.Append (
                this.GetSortClause(
                    this._filterContext.Sort, 
                   string.IsNullOrWhiteSpace(this._filterContext.Sort)
                   )
                );
      
            sb.Append(this.GetPaginationWhereClouse(
                this._filterContext.Page,
                this._filterContext.PageSize));

            return sb.ToString();
        }
        public string GetCountQuery()
        {
            if (this._filterContext.Count == false) return null;

            var sb = new StringBuilder(1024);

            sb.Append(SELECT_Name);
            sb.Append(COUNT_Name);

            sb.Append(FROM_Name);
            sb.Append(this._tablesAndJoins);

            // il filtro va qui
            if (string.IsNullOrEmpty(this._filterClouse) == false)
            {
                sb.Append(WHYERE_Name);
                sb.Append(this._filterClouse);
            }

            return sb.ToString();
        }
        public object Arguments => this._queryArguments;

        private string GetSqlFieldList()
        {
            var sb = new StringBuilder(1024);

            var type = typeof(T);
            var defaultAliasPart = this.GetDefaultAlias();

            foreach (var prop in type.GetProperties())
            {
                if (sb.Length > 0) sb.Append(SqlField_Separator);

                var fieldName = this.GetDbFieldName(prop, true, defaultAliasPart);
                sb.Append(fieldName);
            }
            return sb.ToString();
        }
        private string GetDefaultSort()
        {
            var sb = new StringBuilder(1024);

            var type = typeof(T);
            var defaultAliasPart = this.GetDefaultAlias();
            PropertyInfo firstProp = type.GetProperties().FirstOrDefault();

            var list = new List<Tuple<PropertyInfo, SqlFieldAttribute>>();
            foreach (var prop in type.GetProperties())
            {
                if (firstProp == null) firstProp = prop;
                var attr = (from a in prop.GetCustomAttributes<SqlFieldAttribute>()
                            where a.SortPosition >= 0
                            select a)
                           .SingleOrDefault();
                    
                if (attr != null)
                {
                    list.Add(new Tuple<PropertyInfo, SqlFieldAttribute>(prop, attr));
                }
            }
            if (list.Count > 0)
            {
                var props = (from l in list
                             orderby l.Item2.SortPosition ascending
                             select l.Item1);

                foreach (var prop in props)
                {
                    if (sb.Length > 0) sb.Append(SqlField_Separator);

                    var fieldName = this.GetDbFieldName(prop, false, defaultAliasPart);
                    sb.Append(fieldName);
                }
            }
            else
            {
                var fieldName = this.GetDbFieldName(firstProp, false, defaultAliasPart);
                sb.Append(fieldName);
            }
            return sb.ToString();
        }

        private string GetSortClause(
            string filterParam,
            bool addDefaultSort = false)
        {
            var sb = new StringBuilder(1024);

            if (string.IsNullOrEmpty(filterParam) == false)
            {
                var orderPart = filterParam.Split(InputORderBy_Separator);
                var defaultAlias = this.GetDefaultAlias();

                foreach (string part in orderPart)
                {
                    if (string.IsNullOrEmpty(part?.Trim()) == false)
                    {
                        string direction = SqlSorteAscending;

                        string field = part;

                        if (field.EndsWith(InputOrderDescending_Operator) == true)
                        {
                            field = string.Concat(part.Take(field.Length - 1));
                            direction = SqlSorteDescending;
                        }
                        else if (field.EndsWith(InputOrderAscending_Operator) == true)
                        {
                            field = string.Concat(part.Take(field.Length - 1));
                        }
                        var property = this.GetPropertyByName(field);
                        var fieldName = this.GetDbFieldName(property, false, defaultAlias);
                        if (sb.Length > 0) sb.Append(SqlField_Separator);
                        sb.Append(fieldName);
                        sb.Append(direction);
                    }
                }
            }
            if (addDefaultSort == true)
            {
                if (sb.Length > 0) sb.Append(SqlField_Separator);
                sb.Append(
                    this.GetDefaultSort()
                    );
            }

            return sb.ToString();
        }

        private string GetPaginationWhereClouse(
            int page,
            int pageSize
            )
        {
            int startRow = ((page - 1) * pageSize);

            return string.Format(
                SqlPaging_Fmt,
                startRow,
                pageSize
                );
        }


        private string GetWhereClause(
            string customFilter = null)
        {
            var sb = new StringBuilder(1024);

            // aggiunge i filtri custom
            if (string.IsNullOrEmpty(customFilter) == false)
            {
                if (sb.Length > 0) sb.Append(LOGICAL_AND);
                sb.Append(PARENTESIS_OPEN);
                sb.Append(customFilter);
                sb.Append(PARENTESIS_CLOSE);
            }

            // aggiunge i filtri da  parametro
            string filter = this.GetFilterClause(this._filterContext.Filter);
            if (string.IsNullOrEmpty(filter) == false)
            {
                if (sb.Length > 0) sb.Append(LOGICAL_AND);
                sb.Append(PARENTESIS_OPEN);
                sb.Append(filter);
                sb.Append(PARENTESIS_CLOSE);
            }
            // aggiunge i filtri per il full text
            filter = this.GetFullTextWhereClause(this._filterContext.FullText);
            if (string.IsNullOrEmpty(filter) == false)
            {
                if (sb.Length > 0) sb.Append(LOGICAL_AND);
                sb.Append(PARENTESIS_OPEN);
                sb.Append(filter);
                sb.Append(PARENTESIS_CLOSE);
            }

            return sb.ToString();
        }

        private string GetFilterClause(
            string filterQuery)
        {
            // crea il tokenizzatore
            var tokenizer = new Tokenizer<T>();

            var tokenEnum = tokenizer.Tokenize(filterQuery).GetEnumerator();
            if (tokenEnum.MoveNext() == false) return string.Empty;
            return this.GetLogicalExpression(tokenEnum);
        }

        private string GetFullTextWhereClause(
            string fulltextSearch)
        {
            var sb = new StringBuilder(1024);
            if (string.IsNullOrWhiteSpace(fulltextSearch) == false)
            {
                var words = fulltextSearch.Split(",");
                foreach (var word in words)
                {
                    var w = word.Trim();
                    if (string.IsNullOrWhiteSpace(w) == false)
                    {
                        if (sb.Length > 0) sb.Append(LOGICAL_AND);
                        sb.Append(PARENTESIS_OPEN);

                        var fileter = this.GetFullTextWhereClauseSingleWord(w);
                        sb.Append(fileter);

                        sb.Append(PARENTESIS_CLOSE);
                    }
                }
            }
            return sb.ToString();
        }
        private string GetFullTextWhereClauseSingleWord(
            string fulltextSearch)
        {
            var sb = new StringBuilder(1024);
            if (string.IsNullOrEmpty(fulltextSearch) == false)
            {
                var type = typeof(T);
                var alias = this.GetDefaultAlias();
                fulltextSearch = SQL_LIKEALLPLACEHOLDER + fulltextSearch + SQL_LIKEALLPLACEHOLDER;
                var ft_paramName = this.CreateParameter(fulltextSearch);

                foreach (var p in type.GetProperties())
                {
                    // controla se la prorietà ha l'attributo per il full text
                    var attr = (from a in p.GetCustomAttributes<SqlFieldAttribute>(true)
                                where a.FullText == true
                                select a).SingleOrDefault();
                    if (attr != null)
                    {
                        // recupera il nome del campo di database
                        var fieldName = this.GetDbFieldName(p, false, alias);
                        if (sb.Length > 0) sb.Append(LOGICAL_OR);
                        sb.AppendFormat(COmpareOpFmt_Like,
                            fieldName,
                            ft_paramName);
                    }
                }
            }
            return sb.ToString();
        }

        private object CreateQeryArguments(
            object customArgs = null)
        {
            var initialized = false;
            var args = new System.Dynamic.ExpandoObject();
            var dict = args as IDictionary<string, object>;

            // aggiunge gli arbomenti custom,
            if (customArgs != null)
            {
                var type = customArgs.GetType();
                foreach (var p in type.GetProperties())
                {
                    initialized = true;
                    dict.Add(p.Name, p.GetValue(customArgs));
                }
            }

            for (int index = 0; index < this._arguments.Count; ++index)
            {
                initialized = true;
                var value = this._arguments[index];
                var argName = SqlParameterPrefix + (index + 1).ToString().Trim();
                dict.Add(argName, value);
            }

            object arguments = null;
            if (initialized == true)
            {
                arguments = (object)args;
            }
            return arguments;
        }

        #region helper
        private string GetDefaultAlias()
        {
            var type = typeof(T);
            SqlAliasAttribute defaultAlias = type.GetCustomAttributes(true)
                .Where(a => a is SqlAliasAttribute)
                .Select(a => a as SqlAliasAttribute)
                .FirstOrDefault();
            return (defaultAlias == null ? string.Empty : defaultAlias.Alias + SqlPart_Separator);
        }

        private string GetDbFieldName(
            PropertyInfo property,
            bool rename = false,
            string defaultAliasPart = null)
        {
            if (defaultAliasPart == null)
            {
                defaultAliasPart = this.GetDefaultAlias();
            }

            var fieldName = this.EscapeFieldName(property.Name);
            var fieldAlias = defaultAliasPart;

            // calcola l'alias
            var sqlAliasAttr = (from a in property.GetCustomAttributes(true)
                                where a is SqlAliasAttribute
                                select a as SqlAliasAttribute)
                                .FirstOrDefault();
            if (sqlAliasAttr != null)
            {
                fieldAlias = sqlAliasAttr.Alias + SqlPart_Separator;
            }

            // calcola il nome del campo
            var sqlFieldAttr = (from a in property.GetCustomAttributes(true)
                                where a is SqlFieldAttribute
                                select a as SqlFieldAttribute)
                                .FirstOrDefault();
            if (sqlFieldAttr != null)
            {
                if (string.IsNullOrEmpty(sqlFieldAttr.Name) == false)
                {
                    fieldName = this.EscapeFieldName(sqlFieldAttr.Name);
                    if (rename == true)
                    {
                        fieldName += SqlColumnAlias_Separator + this.EscapeFieldName (property.Name);
                    }
                }
            }
            return fieldAlias + fieldName;
        }
        private string EscapeFieldName (string filedName)
        {
            return "[" + filedName + "]";
        }

        private PropertyInfo GetPropertyByName(
            string propertyName)
        {
            var type = typeof(T);
            return type.GetProperties()
                .AsEnumerable()
                .Where(p => string.Compare(p.Name,
                   propertyName,
                   StringComparison.OrdinalIgnoreCase) == 0
                )
                .FirstOrDefault();
        }
        #endregion
        #region filter clouse helper
        private string GetLogicalExpression(
            IEnumerator<DslToken> tokenEnum)
        {
            var sb = new StringBuilder(1024);

            var token = tokenEnum.Current;
            if (token.TokenType == TokenType.And
                || token.TokenType == TokenType.Or)
            {
                if (tokenEnum.MoveNext() == false) return null;
                var ExpressionList = new List<string>();
                while (tokenEnum.Current.TokenType != TokenType.CloseParenthesis
                    // && ExpressionList.Count <= 2
                    )
                {
                    var expr = this.GetLogicalExpression(tokenEnum);
                    tokenEnum.MoveNext();
                    if (string.IsNullOrEmpty(expr) == true) break;

                    if (tokenEnum.Current.TokenType == TokenType.Comma)
                    { 
                        tokenEnum.MoveNext();
                    }
                    ExpressionList.Add (expr);
                }
                if (ExpressionList.Count < 2) throw new ApplicationException("Sintasssi di query non corretta");
                
                string logicalOp = token.TokenType switch
                {
                    TokenType.And => LOGICAL_AND,
                    TokenType.Or => LOGICAL_OR,
                    _ => string.Empty
                };

                sb.Append("(");
                var first = true;
                foreach (var expr in ExpressionList)
                {
                    if (first == false)  sb.Append(logicalOp);
                    first = false;
                    sb.Append(expr);
                }
                sb.Append(")");
            }
            else
            {
                sb.Append(this.GetBinaryExpression(tokenEnum));
            }
            return sb.ToString();
        }
        private string GetBinaryExpression(
            IEnumerator<DslToken> tokenEnum)
        {
            var comparison = string.Empty;

            var token = tokenEnum.Current;
            string compareOpFmt = null;
            bool onlyStart = false;
            string likeOpFmt = null;

            switch (token.TokenType)
            {
                case TokenType.Equals:
                    compareOpFmt = COmpareOpFmt_Equal;
                    break;
                case TokenType.NotEquals:
                    compareOpFmt = COmpareOpFmt_NotEqual;
                    break;

                case TokenType.LessThan:
                    compareOpFmt = COmpareOpFmt_Lt;
                    break;
                case TokenType.LessOrEqual:
                    compareOpFmt = COmpareOpFmt_Le;
                    break;

                case TokenType.GreaterThan:
                    compareOpFmt = COmpareOpFmt_Gt;
                    break;
                case TokenType.GreaterOrEqual:
                    compareOpFmt = COmpareOpFmt_Ge;
                    break;

                // start and not start
                case TokenType.StartWith:
                    likeOpFmt = COmpareOpFmt_Like;
                    onlyStart = true;
                    break;
                case TokenType.NntStartWith:
                    likeOpFmt = COmpareOpFmt_NotLike;
                    onlyStart = true;
                    break;

                // contains and not contains
                case TokenType.Contains:
                    likeOpFmt = COmpareOpFmt_Like;
                    break;
                case TokenType.NotContains:
                    likeOpFmt = COmpareOpFmt_NotLike;
                    break;
            }
            if (compareOpFmt != null)
            {
                // Attributo da verificare
                var expAttr = this.GetAttributeName(tokenEnum);
                if (tokenEnum.MoveNext() == false) return null;
                if (tokenEnum.Current.TokenType == TokenType.Comma)
                {
                    // valore di confronto
                    var value = this.GetValues(tokenEnum);
                    var param = this.CreateParameter(value);
                    comparison = string.Format(compareOpFmt, expAttr, param);
                }
            }
            else if (likeOpFmt != null)
            {
                // Attributo da verificare
                var expAttr = this.GetAttributeName(tokenEnum);
                if (tokenEnum.MoveNext() == false) return null;
                if (tokenEnum.Current.TokenType == TokenType.Comma)
                {
                    // valore di confronto
                    var value = (onlyStart == false ? SQL_LIKEALLPLACEHOLDER : string.Empty)
                        + this.GetValues(tokenEnum)
                        + SQL_LIKEALLPLACEHOLDER;
                    var param = this.CreateParameter(value);
                    comparison = string.Format(likeOpFmt, expAttr, param);
                }
            }
            return comparison;
        }
        private string GetAttributeName(
            IEnumerator<DslToken> tokenEnum)
        {
            string field = string.Empty;
            if (tokenEnum.MoveNext() == false) return null;
            var toke = tokenEnum.Current;
            if (toke.TokenType == TokenType.AttributeName)
            {
                var pName = toke.Value;
                var prop = this.GetPropertyByName(pName);
                field = this.GetDbFieldName(prop);
            }
            return field;
        }

        private object GetValues(
            IEnumerator<DslToken> tokenEnum)
        {
            var list = new List<object>();
            while (tokenEnum.MoveNext() == true)
            {
                if (tokenEnum.Current.TokenType == TokenType.CloseParenthesis) break;
                if (tokenEnum.Current.TokenType == TokenType.Comma) continue;
                list.Add(this.GetTokenValue(tokenEnum.Current));
            }
            if (list.Count == 0)
            {
                return null;
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            return list;
        }
        private string CreateParameter(object argumentValue)
        {
            this._arguments.Add(argumentValue);
            return SqlParameterPrefix + this._arguments.Count().ToString().Trim();
        }
        private object GetTokenValue(
            DslToken token)
        {
            return token.TokenType switch
            {
                TokenType.Number => token.GetDecimalNumericValue(),
                TokenType.DateTimeValue => token.GetDatetTimeValue(),
                TokenType.TimeValue => token.GetTimeValue(),
                TokenType.StringValue => token.GetStringValue(),
                TokenType.TrueValue => true,
                TokenType.FalseValue => false,
                _ => null
            };
        }
        #endregion
    }
}
