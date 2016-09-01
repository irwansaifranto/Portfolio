using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProjectLog.Linq;

namespace ProjectLog.Infrastructure
{
    public static class GridHelper
    {
        public static void ProcessFilters<T>(FilterInfo filter, ref IQueryable<T> queryable)
        {
            var whereClause = string.Empty;
            var filters = filter.Filters;
            var parameters = new List<object>();
            for (int i = 0; i < filters.Count; i++)
            {
                var f = filters[i];

                if (f.Filters == null)
                {
                    if (i == 0)
                        whereClause += BuildWhereClause<T>(f, i, parameters) + " ";
                    if (i != 0)
                        whereClause += ToLinqOperator(filter.Logic) + BuildWhereClause<T>(f, i, parameters) + " ";
                    if (i == (filters.Count - 1))
                    {
                        CleanUp(ref whereClause);
                        queryable = queryable.Where(whereClause, parameters.ToArray());
                    }
                }
                else
                    ProcessFilters(f, ref queryable);
            }
        }

        public static string CleanUp(ref string whereClause)
        {
            switch (whereClause.Trim().Substring(0, 2).ToLower())
            {
                case "&&":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
                case "||":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
            }

            return whereClause;
        }

        public static string BuildWhereClause<T>(FilterInfo filter, int index, List<object> parameters)
        {
            var entityType = (typeof(T));
            var property = entityType.GetProperty(filter.Field.ToLower());

            string[] filterArray = filter.Field.ToLower().Split('.');
            if (filterArray.Length > 1)
            {
                foreach (var str in filterArray)
                {
                    property = entityType.GetProperty(str);
                    entityType = property.PropertyType;
                }
            }

            var parameterIndex = parameters.Count;

            switch (filter.Operator.ToLower())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    //penanganan untuk atribut yang tidak terdapat di kelas (dari foreign key), misalnya contractor.name di model project
                    //kemungkinan akan bermasalah kalau atribut bukan string, misalnya contractor.id
                    if (property != null)
                    {
                        if (typeof(DateTime?).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(DateTime.Parse(filter.Value).Date);
                            return string.Format("EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(DateTime.Parse(filter.Value).Date);
                            return string.Format("EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(int).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(int.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        //belum ditangani di source code asli
                        if (typeof(Nullable<Byte>).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(int.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(Nullable<int>).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(int.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(Boolean).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(Boolean.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(long).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(Int64.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if (typeof(Guid).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(Guid.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                    }
                    parameters.Add(filter.Value);
                    return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                case "startswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".StartsWith(" + "@" + parameterIndex + ")";
                case "endswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".EndsWith(" + "@" + parameterIndex + ")";
                case "contains":
                    parameters.Add(filter.Value);
                    return filter.Field + ".Contains(" + "@" + parameterIndex + ")";
                default:
                    throw new ArgumentException("This operator is not yet supported for this Grid", filter.Operator);
            }
        }

        public static string ToLinqOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq": return " == ";
                case "neq": return " != ";
                case "gte": return " >= ";
                case "gt": return " > ";
                case "lte": return " <= ";
                case "lt": return " < ";
                case "or": return " || ";
                case "and": return " && ";
                default: return null;
            }
        }
    }
}
