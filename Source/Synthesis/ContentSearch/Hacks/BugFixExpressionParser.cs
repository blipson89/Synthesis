using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Extensions;
using Sitecore.ContentSearch.Linq.Helpers;
using Sitecore.ContentSearch.Linq.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sitecore.ContentSearch.Linq.Parsing;

namespace Synthesis.ContentSearch.Hacks
{
	/// <summary>
	/// This class patches the default expression parser class to fix some bugs around interface queries
	/// </summary>
	public class BugFixExpressionParser : ExpressionParser
	{
		public BugFixExpressionParser(Type elementType, Type itemType, FieldNameTranslator fieldNameTranslator) : base(elementType, itemType, fieldNameTranslator)
		{
		}

		protected override QueryNode VisitMemberAccess(MemberExpression expression)
		{
			if (expression.Expression == null)
				return base.VisitMemberAccess(expression);

			if (expression.Expression.NodeType == ExpressionType.Constant ||
				expression.Expression.NodeType == ExpressionType.Parameter ||
				expression.Expression.NodeType == ExpressionType.Convert)
				return base.VisitMemberAccess(expression);
		
			if (!(expression.Expression is MemberExpression))
				throw new NotSupportedException(string.Format("Unsupported member access. Expression type: {0}. Member name: {1}.", expression.Expression.NodeType, expression.Member.Name));

			QueryNode queryNode = VisitMemberAccess((MemberExpression)expression.Expression);

			// PATCH: this fixes querying on subproperties of result items (e.g. x=> x.Foo.Bar)
			var constant = queryNode as ConstantNode;

			if (constant != null) return new ConstantNode(expression.Member.GetValue(constant.Value), expression.Member.GetValueType());

			var field = queryNode as FieldNode;

			if (field != null)
			{
				// NOTE: later releases of SC7 change this behavior to include the property name in the field by default (e.g. x=>x.Foo.Bar would map to "foo.bar" in the index)
				// there may be some compatibility changes that go here later
				return new FieldNode(field.FieldKey, expression.Member.GetValueType()); // return a new FIELD, but set its effective type to the property's so there aren't comparison issues later
			}

			throw new NotSupportedException(string.Format("Unsupported member access. Expression type: {0}. Member name: {1}. Member Value Type: {2}", expression.Expression.NodeType, expression.Member.Name, queryNode.NodeType));
			// END PATCH	
		}

		protected override QueryNode VisitMethodCall(MethodCallExpression methodCall)
		{
			MethodInfo method = methodCall.Method;
			if (method.DeclaringType == typeof (Queryable) || method.DeclaringType == typeof (QueryableExtensions))
				return base.VisitMethodCall(methodCall);

			// PATCH: this fixes querying on interfaces using indexers
			if (method.DeclaringType == ItemType || (method.DeclaringType != null && method.DeclaringType.IsAssignableFrom(ItemType)))
				return VisitItemMethod(methodCall);
			// END PATCH 

			if (method.DeclaringType == typeof (string) || method.DeclaringType == typeof (MethodExtensions))
				return base.VisitMethodCall(methodCall);

			// PATCH: this fixes querying using ICollections on interfaces
			if (method.DeclaringType != null && method.DeclaringType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))
			{
				return VisitICollectionMethod(methodCall);
			}
			// END PATCH

			if (method.DeclaringType == typeof(Enumerable) && methodCall.Arguments.Count > 0 && methodCall.Arguments.First() is MemberExpression)
			{
				Type declaringType = ((MemberExpression)methodCall.Arguments.First()).Member.DeclaringType;

				// PATCH: fixes querying on interfaces using LINQ Enumerable extension methods
				if (declaringType == ItemType || (declaringType != null && declaringType.IsAssignableFrom(ItemType)))
					return VisitLinqEnumerableExtensionMethod(methodCall);
			}

			return EvaluateMethodCall(methodCall);
		}

		private QueryNode VisitLinqEnumerableExtensionMethod(MethodCallExpression methodCall)
		{
			MethodInfo method = methodCall.Method;
			switch (method.Name)
			{
				case "Contains":
					return VisitLinqEnumerableContainsMethod(methodCall);
				default:
					throw new NotSupportedException(string.Format("The method '{0}' is not supported. Declaring type: {1}", method.Name, method.DeclaringType != (Type)null ? method.DeclaringType.FullName : "[unknown]"));
			}
		}

		private QueryNode VisitLinqEnumerableContainsMethod(MethodCallExpression methodCall)
		{
			if (methodCall.Arguments.Count != 2)
				throw new NotSupportedException(string.Format("The method '{0}' is not supported. Declaring type: {1}", methodCall.Method.Name, methodCall.Method.DeclaringType != (Type)null ? methodCall.Method.DeclaringType.FullName : "[unknown]"));
			
			return new EqualNode(GetMethodArgument(methodCall, 0), GetMethodArgument(methodCall, 1));
		}

		private QueryNode VisitICollectionMethod(MethodCallExpression methodCall)
		{
			MethodInfo method = methodCall.Method;
			switch (method.Name)
			{
				case "Contains":
					return VisitICollectionContainsMethod(methodCall);
				default:
					throw new NotSupportedException(string.Format("The method '{0}' is not supported. Declaring type: {1}", method.Name, method.DeclaringType != (Type)null ? method.DeclaringType.FullName : "[unknown]"));
			}
		}

		private Expression GetArgument(ReadOnlyCollection<Expression> arguments, int index)
		{
			if (arguments.Count < index)
				throw new InvalidOperationException(string.Format("Too few arguments ({0}). Tried to access index: {1}", arguments.Count, index));
			
			return arguments[index];
		}

		private QueryNode GetMethodArgument(MethodCallExpression methodCall, int index)
		{
			if (methodCall.Object != null)
				--index;
			if (index < 0)
				return Visit(methodCall.Object);
			
			return Visit(GetArgument(methodCall.Arguments, index));
		}
	}
}
