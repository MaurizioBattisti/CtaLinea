using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class RepositoryBase
    {
        #region funzioni generiche di lavoro
        protected async Task InsertTableAsync<T>(
            string tableName,
            IDbConnection conn,
            IDbTransaction tran,
            T data,
            string[] exclude = null
            )
            where T : class
        {
            var sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})",
                tableName,
                this.GetFieldListString(data, exclude),
                this.GetArgsListString(data), exclude);
            await conn.ExecuteAsync(
                sql,
                data,
                tran);
        }

        protected async Task UpdateTableAsync<TData, TKey>(
            string tableName,
            IDbConnection conn,
            IDbTransaction tran,
            TKey key,
            TData data,
            string[] exclude = null
            )
            where TData : class
            where TKey : class
        {
            var sql = string.Format(
                "UPDATE {0} SET {1} WHERE {2}",
                tableName,
                this.GetFieldEqualArgString(data, ","),
                this.GetFieldEqualArgString(key, "AND", exclude));

            // fonde i dati con le chiavi
            var newData = this.JoinObjects(data, key);

            await conn.ExecuteAsync(
                sql,
                newData,
                tran);
        }
        protected async Task DeleteTableAsync<TKey>(
            string tableName,
            IDbConnection conn,
            IDbTransaction tran,
            TKey key
            )
            where TKey : class
        {
            var sql = string.Format(
                "DELETE FROM {0} WHERE {1}",
                tableName,
                this.GetFieldEqualArgString(key, "AND"));

            await conn.ExecuteAsync(
                sql,
                key,
                tran);
        }

        protected IEnumerable<string> GetPropertiesList(
            object value,
            string preFix,
            string postFix,
            string[] exclude =null)
        {
            var dict = value as IDictionary<string, object>;
            if (dict == null)
            {
                Type type = value.GetType();
                var proprs = type.GetProperties();
                foreach (var p in proprs)
                {
                    if (exclude != null && exclude.Contains(p.Name)) continue;
                    yield return preFix + p.Name + postFix;
                }
            }
            else
            {
                foreach (var k in dict.Keys)
                {
                    if (exclude != null && exclude.Contains(k)) continue;
                    yield return preFix + k + postFix;
                }
            }
        }
        protected string GetFieldListString(
            object value,
            string[] exclude = null)
        {
            return string.Join(
                ",",
                this.GetPropertiesList(value, "[", "]", exclude)
                ); ;
        }
        protected string GetArgsListString(
            object value,
            string[] exclude = null)
        {
            return string.Join(
                ",",
                this.GetPropertiesList(value, "@", string.Empty, exclude)
                ); ;
        }
        protected string GetFieldEqualArgString(
            object value,
            string separator,
            string[] exclude = null)
        {
            var items = (from p in this.GetPropertiesList(value, string.Empty, string.Empty, exclude)
                            select " [" + p + "] = @" + p + " ");

            return string.Join(
                separator,
                items
                ); ;
        }
        protected object JoinObjects(
            params object[] items)
        {
            var newData = new ExpandoObject();
            var dict = newData as IDictionary<string, object>;
            foreach (var item in items)
            {
                var type = item.GetType();
                var props = type.GetProperties();
                foreach (var p in props)
                {
                    dict.TryAdd(p.Name, p.GetValue(item));
                }
            }
            return newData;
        }

        #endregion

        #region funzioni per l'update di gerarchie
        protected async Task FindDeletedAsync<T, TKey>(
            IEnumerable<T> oldList,
            IEnumerable<T> newList,
            Func<T, TKey> selectKey,
            Func<TKey, Task> action
            )
        {
            if (oldList == null) return;
            if (newList == null) newList = new List<T>();

            foreach (var key in oldList.Where(ov => newList.Select(v => selectKey(v)).ToList().Contains(selectKey(ov)) == false).Select(ov => selectKey(ov)))
            {
                await action(key);
            }
        }
        protected async Task FindNewAsync<T, TKey>(
            IEnumerable<T> oldList,
            IEnumerable<T> newList,
            Func<T, TKey> selectKey,
            Func<T, Task> action
            )
        {
            if (newList == null) return;
            if (oldList == null) oldList = new List<T>();

            foreach (var data in newList.Where(v => oldList.Select(ov => selectKey(ov)).ToList().Contains(selectKey(v)) == false))
            {
                await action(data);
            }
        }
        protected async Task FindUpdatedAsync<T, TKey>(
            IEnumerable<T> oldList,
            IEnumerable<T> newList,
            Func<T, TKey> selectKey,
            Func<T, T, bool> matchFunc,
            Func<T, T, Task> action
            )
            where T : class
        {
            if (newList == null && oldList == null) return;
            if (newList == null) newList= new List<T>(); ; ;
            if (oldList == null) oldList=  new List<T>(); ;

            foreach (var data in newList.Where(v => oldList.Select(ov => selectKey(ov)).ToList().Contains(selectKey(v))))
            {
                var oldData = oldList.Where(ov => matchFunc(data, ov)).SingleOrDefault();
                await action(data, oldData);
            }
        }
        #endregion
    }
}
