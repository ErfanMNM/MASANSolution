using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HslCommunication.Core.Plugin;

/// <summary>
/// 插件相关的辅助方法
/// </summary>
public class PluginsHelper
{
	/// <summary>
	/// 根据类型名称，示例对象，属性名来获取实际对象值
	/// </summary>
	/// <typeparam name="T">属性类型信息</typeparam>
	/// <param name="pluginType">插件类型</param>
	/// <param name="obj">插件对象</param>
	/// <param name="name">属性名</param>
	/// <returns>实际的属性信息</returns>
	public static T GetObjectProperty<T>(Type pluginType, object obj, string name)
	{
		PropertyInfo property = pluginType.GetProperty(name);
		if (property == null)
		{
			return default(T);
		}
		object value = property.GetValue(obj, null);
		if (value == null)
		{
			return default(T);
		}
		try
		{
			return (T)value;
		}
		catch
		{
			return default(T);
		}
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Plugin.PluginsHelper.GetObjectProperty``1(System.Type,System.Object,System.String)" />
	public static T GetObjectProperty<T>(object obj, string name)
	{
		Type type = obj.GetType();
		return GetObjectProperty<T>(type, obj, name);
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Plugin.PluginsHelper.ExecuteFuncFour``5(System.Type,System.Object,System.String,System.String[])" />
	public static Func<TResult> ExecuteFuncZero<TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[0]);
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		Expression body = Expression.Call(instance, method);
		Expression<Func<TResult>> expression = Expression.Lambda<Func<TResult>>(body, new ParameterExpression[0]);
		return expression.Compile();
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Plugin.PluginsHelper.ExecuteFuncFour``5(System.Type,System.Object,System.String,System.String[])" />
	public static Func<T1, TResult> ExecuteFuncOne<T1, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[1] { typeof(T1) });
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		Expression body = Expression.Call(instance, method, parameterExpression);
		Expression<Func<T1, TResult>> expression = Expression.Lambda<Func<T1, TResult>>(body, new ParameterExpression[1] { parameterExpression });
		return expression.Compile();
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Plugin.PluginsHelper.ExecuteFuncFour``5(System.Type,System.Object,System.String,System.String[])" />
	public static Func<T1, T2, TResult> ExecuteFuncTwo<T1, T2, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[2]
		{
			typeof(T1),
			typeof(T2)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2);
		Expression<Func<T1, T2, TResult>> expression = Expression.Lambda<Func<T1, T2, TResult>>(body, new ParameterExpression[2] { parameterExpression, parameterExpression2 });
		return expression.Compile();
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Plugin.PluginsHelper.ExecuteFuncFour``5(System.Type,System.Object,System.String,System.String[])" />
	public static Func<T1, T2, T3, TResult> ExecuteFuncThree<T1, T2, T3, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[3]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T3), paraNames[2]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2, parameterExpression3);
		Expression<Func<T1, T2, T3, TResult>> expression = Expression.Lambda<Func<T1, T2, T3, TResult>>(body, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 });
		return expression.Compile();
	}

	/// <summary>
	/// 四个参数的表达式数据解析
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="type"></param>
	/// <param name="deviceObject"></param>
	/// <param name="methodName"></param>
	/// <param name="paraNames"></param>
	/// <returns></returns>
	public static Func<T1, T2, T3, T4, TResult> ExecuteFuncFour<T1, T2, T3, T4, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[4]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T3), paraNames[2]);
		ParameterExpression parameterExpression4 = Expression.Parameter(typeof(T4), paraNames[3]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4);
		Expression<Func<T1, T2, T3, T4, TResult>> expression = Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(body, new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 });
		return expression.Compile();
	}
}
