using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesis.Mvc
{
	public class RenderingErrorModel
	{
		public string ViewName { get; set; }
		public string DataSource { get; set; }
		public Type ModelType { get; set; }
		public object Model { get; set; }
		public Exception Exception { get; set; }

		public RenderingErrorModel(string viewName, string dataSource, Type modelType, object model, Exception exception)
		{
			ViewName = viewName;
			DataSource = dataSource;
			ModelType = modelType;
			Model = model;
			Exception = exception;
		}
	}
}
