using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Synthesis.Utility
{
	/// <summary>
	/// Methods to help in matching types (by interface, attribute, etc)
	/// </summary>
	public static class TypeCollectionUtility
	{
		/// <summary>
		/// Matches types that are tagged with a specified attribute type
		/// </summary>
		public static AttributeResultCollection<TAttribute> WithAttribute<TAttribute>(this IEnumerable<Type> types)
			where TAttribute : Attribute
		{
			var matchingTypes = new AttributeResultCollection<TAttribute>();

			foreach (Type type in types)
			{
				var attribute = (TAttribute)type.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
				if (attribute != null)
				{
					matchingTypes.Add(new KeyValuePair<Type, TAttribute>(type, attribute));
				}
			}

			return matchingTypes;
		}

		/// <summary>
		/// Matches types that implement an interface type
		/// </summary>
		public static IEnumerable<Type> ImplementingInterface(this IEnumerable<Type> types, params Type[] interfaceTypes)
		{
			var matchingTypes = new Collection<Type>();
			foreach (Type type in types)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type interfaceType in interfaceTypes)
				{
					if (interfaces.Contains(interfaceType))
					{
						matchingTypes.Add(type);
						break;
					}
				}
			}

			return matchingTypes;
		}

		/// <summary>
		/// Checks if a type implements an open generic interface (i.e. "typeof(IEnumerable&lt;&gt;)")
		/// </summary>
		/// <param name="toCheck">Type to test</param>
		/// <param name="openGenericInterface">Open generic interface to check for</param>
		public static bool ImplementsOpenGenericInterface(this Type toCheck, Type openGenericInterface)
		{
			while (toCheck != typeof(object) && toCheck != null)
			{
				if (toCheck.IsGenericType && toCheck.GetGenericTypeDefinition() == openGenericInterface)
					return true;

				if (toCheck.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericInterface))
					return true;

				toCheck = toCheck.BaseType;
			}

			return false;
		}
	}

	public class AttributeResultCollection<T> : Collection<KeyValuePair<Type, T>>
			where T : Attribute
	{

	}
}
