using System;
using System.Collections.Generic;

namespace Synthesis.Configuration
{
	public interface ITypeListProvider
	{
		IEnumerable<Type> CreateTypeList();
	}
}
