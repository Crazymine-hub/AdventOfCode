using AdventOfCode.Tools.SpecificBitwise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools;

// [Obsolete("Consider using the specialized class.")]
// public static class Bitwise
// {
//     private static Dictionary<Type, ImplementerDetail> Implementers = null;

//     public static T SetBit<T>(T data, int bitNr, bool value)
//     {
//         return (T)InvokeImplementer<T>(nameof(SetBit), new object[] { data, bitNr, value });
//     }

//     public static bool IsBitSet<T>(T data, int bitNr)
//     {
//         return (bool)InvokeImplementer<T>(nameof(IsBitSet), new object[] { data, bitNr});
//     }

//     public static T GetBitMask<T>(int length)
//     {
//         return (T)InvokeImplementer<T>(nameof(GetBitMask), new object[] { length});
//     }

//     public static int CountSetBits<T>(T data)
//     {
//         return (int)InvokeImplementer<T>(nameof(CountSetBits), new object[] {data});
//     }

//     public static T GetValue<T>(IEnumerable<bool> values)
//     {
//         return (T)InvokeImplementer<T>(nameof(GetValue), new object[] {values});
//     }

//     public static List<bool> GetBits<T>(T bits)
//     {
//         return (List<bool>)InvokeImplementer<T>(nameof(GetBits), new object[] {bits});
//     }

//     private static object InvokeImplementer<T>(string methodName, object[] arguments)
//     {
//         List<Type> argumentTypes = new List<Type>();
//         foreach (var argument in arguments)
//             argumentTypes.Add(argument.GetType());
//         var targetMethod = GetImplementer(typeof(T)).GetMethod(methodName, argumentTypes.ToArray());
//         if (targetMethod == null) throw new MissingMethodException($"The method {methodName}({string.Join(", ", argumentTypes.Select(x => x.Name))}) is not available in the handler associated with the type {typeof(T)}.");
//         return targetMethod.Invoke(null, arguments);
//     }

//     private static Type GetImplementer(Type targetType)
//     {
//         Implementers ??= ListImplementers();
//         if (!Implementers.TryGetValue(targetType, out var implementerDetail)) throw new NotSupportedException($"There is no known handler associated with the type '{targetType}'!");
//         return implementerDetail.Implementer;
//     }

//     private static Dictionary<Type, ImplementerDetail> ListImplementers()
//     {
//         var implementers = new Dictionary<Type,ImplementerDetail>();
//         foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
//         {
//             var bitwiseAttribute = type.GetCustomAttributes().FirstOrDefault(attribute => attribute is BitwiseHandlerAttribute) as BitwiseHandlerAttribute;
//             if (bitwiseAttribute == null) continue;
//             Type handled = bitwiseAttribute.HandledType;
//             if (!implementers.ContainsKey(handled))
//                 implementers.Add(handled, new ImplementerDetail(type, bitwiseAttribute.Priority));
//             if(implementers[handled].Priority < bitwiseAttribute.Priority)
//                 implementers[handled] = new ImplementerDetail(type, bitwiseAttribute.Priority);
//         }
//         return implementers;
//     }
// }
