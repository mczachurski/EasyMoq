using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
	public class CallInfo
	{
		public Expression Object { get; set; }
		public MethodInfo Method { get; set; }
		public IEnumerable<Expression> Arguments { get; set; }
	}
}