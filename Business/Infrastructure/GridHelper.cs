using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Business.Linq;

namespace Business.Infrastructure
{
    public static class GridHelper
    {

        public static void ProcessFilters<T>(FilterInfo filter, ref IQueryable<T> queryable)
        {
            var whereClause = string.Empty;
            var filters = filter.Filters;
            var parameters = new List<object>();
            //for (int i = 0; i < filters.Count; i++)
            //{
            //    var f = filters[i];

            //    if (f.Filters == null)
            //    {
            //        if (i == 0)
            //            whereClause += BuildWhereClause<T>(f, i, parameters) + " ";
            //        if (i != 0)
            //            whereClause += ToLinqOperator(filter.Logic) + BuildWhereClause<T>(f, i, parameters) + " ";
            //        if (i == (filters.Count - 1))
            //        {
            //            CleanUp(ref whereClause);
            //            queryable = queryable.Where(whereClause, parameters.ToArray());
            //        }
            //    }
            //    else
            //        ProcessFilters(f, ref queryable);
            //}
            CreateWhereClause<T>(filter, ref parameters, ref whereClause);
            if (whereClause.Length > 2)
            {
                for (int i = 0; i < parameters.Count(); i++)
                {
                    if (parameters[i].GetType() == typeof(string))
                        parameters[i] = ((string)parameters[i]).ToLower();
                }

                queryable = queryable.Where(whereClause, parameters.ToArray());
            }
        }

        public static void CreateWhereClause<T>(FilterInfo filter, ref List<object> parameters, ref string whereClause)
        {
            string clauses = string.Empty;
            if (filter.Filters == null)
            {
                whereClause += "(";
                clauses += BuildWhereClause<T>(filter, 0, parameters) + " ";
                whereClause += clauses;
            }
            else
            {

                var filters = filter.Filters;
                if (filter.Logic == null || filter.Logic == "")
                {
                    filter.Logic = "and";
                }
                whereClause += "(";
                for (int i = 0; i < filters.Count; i++)
                {
                    var f = filters[i];

                    if (f.Filters == null)
                    {
                        if (i == 0)
                            clauses += BuildWhereClause<T>(f, i, parameters) + " ";
                        if (i != 0)
                            clauses += ToLinqOperator(filter.Logic) + BuildWhereClause<T>(f, i, parameters) + " ";
                        if (i == (filters.Count - 1))
                        {
                            //CleanUp(ref clauses);
                            whereClause += clauses;
                        }
                    }
                    else
                    {
                        if (i != 0)
                        {
                            whereClause += clauses + " " + ToLinqOperator(filter.Logic) + " ";
                        }
                        CreateWhereClause<T>(f, ref parameters, ref whereClause);
                        //if (i == (filters.Count - 1))
                        //{
                        //    whereClause += ")";
                        //}
                    }
                }
            }
            whereClause += ")";
        }

        public static void ProcessSorts<T>(List<SortingInfo> sorts, ref IQueryable<T> queryable)
        {
            var sortClause = string.Empty;
            var parameters = new List<object>();
            for (int i = 0; i < sorts.Count; i++)
            {
                var s = sorts[i];
                s.FormatSortOnToUnderscore();
                if (i != 0)
                {
                    sortClause += ", ";
                }
                sortClause += s.SortOn + " " + s.SortOrder;
            }
            queryable = queryable.OrderBy<T>(sortClause);
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
                        if ((typeof(Nullable<DateTime>).IsAssignableFrom(property.PropertyType)) || (typeof(DateTime).IsAssignableFrom(property.PropertyType)))
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
                        if (typeof(Nullable<Int16>).IsAssignableFrom(property.PropertyType))
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
                        if (typeof(Nullable<Guid>).IsAssignableFrom(property.PropertyType))
                        {
                            parameters.Add(Guid.Parse(filter.Value));
                            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }
                        if ((typeof(Nullable<DateTimeOffset>).IsAssignableFrom(property.PropertyType)) || (typeof(DateTime).IsAssignableFrom(property.PropertyType)))
                        {
                            DateTimeOffset dto = DateTimeOffset.Parse(filter.Value);

                            parameters.Add(dto.ToLocalTime());
                            return string.Format("EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        }

                    }
                    parameters.Add(filter.Value);
                    return string.Format(filter.Field + ".ToLower()" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                case "startswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".StartsWith(" + "@" + parameterIndex + ")";
                case "endswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".EndsWith(" + "@" + parameterIndex + ")";
                case "contains":
                    if (typeof(Nullable<Guid>).IsAssignableFrom(property.PropertyType))
                    {
                        parameters.Add(Guid.Parse(filter.Value));
                        return filter.Field + ".Contains(" + "@" + parameterIndex + ")";
                    }

                    parameters.Add(filter.Value);
                    return filter.Field + ".ToLower().Contains(" + "@" + parameterIndex + ")";
                case "notcontains":
                    parameters.Add(filter.Value);
                    return "!" + filter.Field + ".Contains(" + "@" + parameterIndex + ")";
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
