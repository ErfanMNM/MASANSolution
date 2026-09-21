using System;
using System.IO;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core;

/// <summary>
/// 所有数据转换类的静态辅助方法<br />
/// Static helper method for all data conversion classes
/// </summary>
public static class ByteTransformHelper
{
	/// <summary>
	/// 结果转换操作的基础方法，需要支持类型，及转换的委托，并捕获转换时的异常方法
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <param name="result">源</param>
	/// <param name="translator">实际转换的委托</param>
	/// <returns>转换结果</returns>
	public static OperateResult<TResult> GetResultFromBytes<TResult>(OperateResult<byte[]> result, Func<byte[], TResult> translator)
	{
		try
		{
			if (result.IsSuccess)
			{
				return OperateResult.CreateSuccessResult(translator(result.Content));
			}
			return OperateResult.CreateFailedResult<TResult>(result);
		}
		catch (Exception ex)
		{
			OperateResult<TResult> operateResult = new OperateResult<TResult>();
			operateResult.Message = $"{StringResources.Language.DataTransformError} {SoftBasic.ByteToHexString(result.Content)} : Length({result.Content.Length}) {ex.Message}";
			return operateResult;
		}
	}

	/// <summary>
	/// 结果转换操作的基础方法，需要支持类型，及转换的委托
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <param name="result">源结果</param>
	/// <returns>转换结果</returns>
	public static OperateResult<TResult> GetResultFromArray<TResult>(OperateResult<TResult[]> result)
	{
		return GetSuccessResultFromOther(result, (TResult[] m) => m[0]);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容，所转换的规则
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn">输入类型</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans">转换方法，从类型TIn转换拿到TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetSuccessResultFromOther<TResult, TIn>(OperateResult<TIn> result, Func<TIn, TResult> trans)
	{
		if (!result.IsSuccess)
		{
			return OperateResult.CreateFailedResult<TResult>(result);
		}
		try
		{
			TResult value = trans(result.Content);
			return OperateResult.CreateSuccessResult(value);
		}
		catch (Exception ex)
		{
			return new OperateResult<TResult>(StringResources.Language.DataTransformError + " " + ex.Message);
		}
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TIn">输入类型</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans">转换方法，从类型TIn转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult GetResultFromOther<TIn>(OperateResult<TIn> result, Func<TIn, OperateResult> trans)
	{
		if (!result.IsSuccess)
		{
			return result;
		}
		return trans(result.Content);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn">输入类型</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans">转换方法，从类型TIn转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn>(OperateResult<TIn> result, Func<TIn, OperateResult<TResult>> trans)
	{
		return result.Then(trans);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TResult>> trans2)
	{
		return result.Then(trans1).Then(trans2);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TResult>> trans3)
	{
		return result.Then(trans1).Then(trans2).Then(trans3);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TResult>> trans4)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TResult>> trans5)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <typeparam name="TIn6">输入类型6</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
	/// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TResult>> trans6)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <typeparam name="TIn6">输入类型6</typeparam>
	/// <typeparam name="TIn7">输入类型7</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
	/// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
	/// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TResult>> trans7)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <typeparam name="TIn6">输入类型6</typeparam>
	/// <typeparam name="TIn7">输入类型7</typeparam>
	/// <typeparam name="TIn8">输入类型8</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
	/// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
	/// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
	/// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TResult>> trans8)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <typeparam name="TIn6">输入类型6</typeparam>
	/// <typeparam name="TIn7">输入类型7</typeparam>
	/// <typeparam name="TIn8">输入类型8</typeparam>
	/// <typeparam name="TIn9">输入类型9</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
	/// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
	/// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
	/// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TIn9的泛型委托</param>
	/// <param name="trans9">转换方法9，从类型TIn9转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TIn9>> trans8, Func<TIn9, OperateResult<TResult>> trans9)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8)
			.Then(trans9);
	}

	/// <summary>
	/// 使用指定的转换方法，来获取到实际的结果对象内容
	/// </summary>
	/// <typeparam name="TResult">结果类型</typeparam>
	/// <typeparam name="TIn1">输入类型1</typeparam>
	/// <typeparam name="TIn2">输入类型2</typeparam>
	/// <typeparam name="TIn3">输入类型3</typeparam>
	/// <typeparam name="TIn4">输入类型4</typeparam>
	/// <typeparam name="TIn5">输入类型5</typeparam>
	/// <typeparam name="TIn6">输入类型6</typeparam>
	/// <typeparam name="TIn7">输入类型7</typeparam>
	/// <typeparam name="TIn8">输入类型8</typeparam>
	/// <typeparam name="TIn9">输入类型9</typeparam>
	/// <typeparam name="TIn10">输入类型10</typeparam>
	/// <param name="result">原始的结果对象</param>
	/// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
	/// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
	/// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
	/// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
	/// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
	/// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
	/// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
	/// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TIn9的泛型委托</param>
	/// <param name="trans9">转换方法9，从类型TIn9转换拿到OperateResult的TIn10的泛型委托</param>
	/// <param name="trans10">转换方法10，从类型TIn10转换拿到OperateResult的TResult的泛型委托</param>
	/// <returns>类型为TResult的对象</returns>
	public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9, TIn10>(OperateResult<TIn1> result, Func<TIn1, OperateResult<TIn2>> trans1, Func<TIn2, OperateResult<TIn3>> trans2, Func<TIn3, OperateResult<TIn4>> trans3, Func<TIn4, OperateResult<TIn5>> trans4, Func<TIn5, OperateResult<TIn6>> trans5, Func<TIn6, OperateResult<TIn7>> trans6, Func<TIn7, OperateResult<TIn8>> trans7, Func<TIn8, OperateResult<TIn9>> trans8, Func<TIn9, OperateResult<TIn10>> trans9, Func<TIn10, OperateResult<TResult>> trans10)
	{
		return result.Then(trans1).Then(trans2).Then(trans3)
			.Then(trans4)
			.Then(trans5)
			.Then(trans6)
			.Then(trans7)
			.Then(trans8)
			.Then(trans9)
			.Then(trans10);
	}

	/// <summary>
	/// 将多个SoftBuffer实例中的字节数组合并成一个MemoryStream<br />
	/// Combines the byte arrays from multiple SoftBuffer instances into a single MemoryStream.
	/// </summary>
	/// <param name="softBuffers">An array of SoftBuffer instances whose bytes will be combined.</param>
	/// <returns>A MemoryStream containing the concatenated bytes from all provided SoftBuffer instances.</returns>
	public static MemoryStream GetByteFromSoftBuffer(params SoftBuffer[] softBuffers)
	{
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < softBuffers.Length; i++)
		{
			memoryStream.Write(softBuffers[i].GetBytes());
		}
		return memoryStream;
	}

	/// <summary>
	/// 将一个字节数组的内容依次分配到多个SoftBuffer实例中<br />
	/// Distributes the contents of a byte array into multiple SoftBuffer instances sequentially.
	/// </summary>
	/// <param name="buffer">The source byte array to be distributed.</param>
	/// <param name="startIndex">起始偏移的字节索引</param>
	/// <param name="softBuffers">The target SoftBuffer instances to receive the byte data.</param>
	public static void SetByteToSoftBuffer(byte[] buffer, int startIndex, params SoftBuffer[] softBuffers)
	{
		if (buffer != null && buffer.Length != 0)
		{
			int num = startIndex;
			for (int i = 0; i < softBuffers.Length && num + softBuffers[i].Capacity <= buffer.Length; i++)
			{
				softBuffers[i].SetBytes(buffer, num, 0, softBuffers[i].Capacity);
				num += softBuffers[i].Capacity;
			}
		}
	}
}
