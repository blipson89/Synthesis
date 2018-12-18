using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Synthesis.Utility
{
    public delegate T ObjectActivator<T>(params object[] args);
    /// <summary>
    /// Dynamic Activator using compiled lambda expressions. Performs better than Activator.CreateInstance
    /// </summary>
    /// <remarks>
    /// Credit: Roger Johansson (https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects)
    /// </remarks>
    /// 
    public static class ObjectActivator
    {
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }
    }
}
