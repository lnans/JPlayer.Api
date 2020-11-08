using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JPlayer.Lib.Mapper
{
    public class ObjectMapper
    {
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Delegate> _mapCache =
            new ConcurrentDictionary<Tuple<Type, Type>, Delegate>();

        public TDest Map<TDest, TSource>(TSource source) where TDest : new()
        {
            return this.Map(source, new TDest());
        }

        public TDest Map<TSource, TDest>(TSource source, TDest dest)
        {
            if (source == null)
                return default;

            Func<TSource, TDest, TDest> map =
                (Func<TSource, TDest, TDest>) this._mapCache.GetOrAdd(Tuple.Create(typeof(TSource), typeof(TDest)),
                    _ => MakeMapMethod<TSource, TDest>());
            return map(source, dest);
        }

        private static Func<TSource, TDest, TDest> MakeMapMethod<TSource, TDest>()
        {
            ParameterExpression destArg = Expression.Parameter(typeof(TDest), "dest");
            ParameterExpression srcArg = Expression.Parameter(typeof(TSource), "src");

            IEnumerable<Expression> assignments =
                MakeAssignments(typeof(TSource), typeof(TDest), srcArg, destArg).ToList();

            if (!assignments.Any())
            {
                throw new InvalidOperationException(string.Format(
                    "No matching properties were found between the types {0} and {1}", typeof(TSource), typeof(TDest)));
            }

            BlockExpression assignmentsBlock = Expression.Block(assignments);
            BlockExpression blockExp = Expression.Block(typeof(TDest), assignmentsBlock, destArg);
            Expression<Func<TSource, TDest, TDest>> map =
                Expression.Lambda<Func<TSource, TDest, TDest>>(blockExp, srcArg, destArg);

            return map.Compile();
        }

        private static IEnumerable<Expression> MakeAssignments(Type sourceType,
            Type destType,
            Expression sourcePropertyExp,
            Expression destPropertyExp)
        {
            Dictionary<string, PropertyInfo> sourceProps = sourceType.GetRuntimeProperties().ToDictionary(p => p.Name);
            Dictionary<string, PropertyInfo> destProps = destType.GetRuntimeProperties().ToDictionary(p => p.Name);
            List<Expression> assignments = new List<Expression>();

            foreach (KeyValuePair<string, PropertyInfo> srcProp in sourceProps)
            {
                if (!srcProp.Value.GetMethod.IsPublic)
                    continue;

                PropertyInfo destProp = destProps.ContainsKey(srcProp.Key) ? destProps[srcProp.Key] : null;

                if (destProp == null || destProp.SetMethod == null)
                    continue;

                Type destPropType = destProp.PropertyType;
                Type srcPropType = srcProp.Value.PropertyType;

                MemberExpression srcPropExp = Expression.Property(sourcePropertyExp, srcProp.Value);
                MemberExpression destPropExp = Expression.Property(destPropertyExp, destProp);

                if (destPropType.GetTypeInfo().IsAssignableFrom(srcPropType.GetTypeInfo()) &&
                    destProp.SetMethod.IsPublic)
                {
                    BinaryExpression assignmentExp = Expression.Assign(destPropExp, srcPropExp);
                    assignments.Add(assignmentExp);
                }
                else if (destProp.GetMethod.IsPublic)
                {
                    // The properties aren't assignable but they may have members that are
                    IEnumerable<Expression> deepAssignmentExp =
                        MakeAssignments(srcPropType, destPropType, srcPropExp, destPropExp).ToList();

                    if (!deepAssignmentExp.Any())
                        continue;

                    BinaryExpression nullCheckExp = Expression.And(
                        Expression.NotEqual(destPropExp, Expression.Constant(null)),
                        Expression.NotEqual(srcPropExp, Expression.Constant(null)));
                    ConditionalExpression nullCheckExpBlock =
                        Expression.IfThen(nullCheckExp, Expression.Block(deepAssignmentExp));
                    assignments.Add(nullCheckExpBlock);
                }
            }

            return assignments;
        }
    }
}