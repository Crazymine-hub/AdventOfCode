using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Exceptions;


[Serializable]
public class ResultValidationException: Exception
{
	public ResultValidationException() { }
	public ResultValidationException(string message) : base(message) { }
	public ResultValidationException(string message, Exception inner) : base(message, inner) { }
	protected ResultValidationException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
