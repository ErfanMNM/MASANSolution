using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HslCommunication.Profinet.Siemens.S7PlusHelper;

/// <summary>
/// 节点类对象
/// </summary>
public class S7Tag : IS7Object
{
	/// <summary>
	/// 数据名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 数据ID信息，第一个为DB块的信息，第二个是数据信息，如果是结构体，则继续往后面拓展
	/// </summary>
	public List<uint> LID { get; set; }

	/// <summary>
	/// 类型代号
	/// </summary>
	public byte TypeCode { get; set; }

	/// <summary>
	/// 数组长度，如果小于0则表示标量的数据
	/// </summary>
	public int ArrayLength { get; set; } = -1;

	/// <summary>
	/// 当类型为结构体的时候，关联的其他结构体的ID信息
	/// </summary>
	public uint StructID { get; set; }

	/// <summary>
	/// 当类型为结构体的时候，标签的偏移信息
	/// </summary>
	public int StructOffset { get; set; }

	/// <summary>
	/// 用户自定义的标签信息，可以用来存储一些额外的信息，方便用户使用
	/// </summary>
	public string Tag { get; set; }

	/// <summary>
	/// 获取类型的文本描述
	/// </summary>
	/// <returns>string info</returns>
	public string GetTypeText()
	{
		string text = null;
		text = TypeCode switch
		{
			0 => "Void", 
			1 => "Bool", 
			2 => "Byte", 
			3 => "Char", 
			4 => "Word(ushort)", 
			5 => "Int(short)", 
			6 => "DWord(uint)", 
			7 => "DInt(int)", 
			8 => "Real(float)", 
			9 => "Date", 
			10 => "TimeOfDay", 
			11 => "Time", 
			12 => "S5Time", 
			13 => "S5Count", 
			14 => "DateAndTime", 
			15 => "InteretTime", 
			16 => "Array", 
			17 => "Struct", 
			18 => "EndStruct", 
			19 => "String", 
			20 => "Pointer", 
			21 => "Multi_FB", 
			22 => "Any", 
			23 => "Block_FB", 
			24 => "Block_FC", 
			25 => "Block_DB", 
			26 => "Block_SDB", 
			27 => "Multi_SFB", 
			28 => "Counter", 
			29 => "Timer", 
			30 => "IEC_COUNTER", 
			31 => "IEC_TIMER", 
			32 => "Block_SFB", 
			33 => "Block_SFC", 
			34 => "Block_CB", 
			35 => "Block_SCB", 
			36 => "Block_OB", 
			37 => "Block_UDT", 
			38 => "Offset", 
			39 => "Block_SDT", 
			40 => "BBOOL", 
			41 => "BLOCK_EXT", 
			48 => "LReal(double)", 
			49 => "ULInt(ulong)", 
			50 => "LInt(long)", 
			51 => "LWord(ulong)", 
			52 => "USInt", 
			53 => "UInt(ushort)", 
			54 => "UDInt(uint)", 
			55 => "SInt(sbyte)", 
			56 => "Bcd8", 
			57 => "Bcd16", 
			58 => "Bcd32", 
			59 => "Bcd64", 
			60 => "ARef", 
			61 => "WChar", 
			62 => "WString", 
			63 => "Variant", 
			64 => "LTime", 
			65 => "LTOD", 
			66 => "LDT", 
			67 => "DTL", 
			68 => "IEC_LTIMER", 
			97 => "Error_Struct", 
			144 => "HW_ANY", 
			145 => "HW_IOSYSTEM", 
			149 => "HW_IO", 
			170 => "CONN_OUC", 
			254 => "Label", 
			byte.MaxValue => "Undefined", 
			_ => $"Unknown({TypeCode})", 
		};
		return (ArrayLength >= 0) ? $"{text}[{ArrayLength}]" : text;
	}

	/// <summary>
	/// 获取LID的文本信息
	/// </summary>
	/// <returns></returns>
	public string GetLIDText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (LID != null)
		{
			for (int i = 0; i < LID.Count; i++)
			{
				stringBuilder.Append(LID[i].ToString("X"));
				if (i != LID.Count - 1)
				{
					stringBuilder.Append(".");
				}
			}
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 获取LID的文本信息
	/// </summary>
	/// <param name="relationId">关联的ID信息</param>
	/// <returns>地址文本</returns>
	public string GetLIDText(uint relationId)
	{
		StringBuilder stringBuilder = new StringBuilder(relationId.ToString("X"));
		stringBuilder.Append(".");
		if (LID != null)
		{
			for (int i = 0; i < LID.Count; i++)
			{
				stringBuilder.Append(LID[i].ToString("X"));
				if (i != LID.Count - 1)
				{
					stringBuilder.Append(".");
				}
			}
		}
		return stringBuilder.ToString();
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Siemens.S7PlusHelper.IS7Object.WriteMessgae(System.IO.MemoryStream)" />
	public void WriteMessgae(MemoryStream ms)
	{
		S7Object.WriteUint32(ms, 0u);
		S7Object.WriteUint32(ms, LID[0]);
		S7Object.WriteUint32(ms, (uint)LID.Count);
		if (LID[0] >= 2316173312u)
		{
			S7Object.WriteUint32(ms, 2550u);
		}
		else
		{
			S7Object.WriteUint32(ms, 3736u);
		}
		for (int i = 1; i < LID.Count; i++)
		{
			S7Object.WriteUint32(ms, LID[i]);
		}
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Siemens.S7PlusHelper.IS7Object.GetNumberOfFields" />
	public int GetNumberOfFields()
	{
		return 3 + LID.Count;
	}

	/// <summary>
	/// 深度克隆一个对象
	/// </summary>
	/// <returns></returns>
	public S7Tag Clone()
	{
		S7Tag s7Tag = new S7Tag();
		s7Tag.Name = Name;
		if (LID != null)
		{
			s7Tag.LID = new List<uint>(LID.ToArray());
		}
		s7Tag.TypeCode = TypeCode;
		s7Tag.ArrayLength = ArrayLength;
		s7Tag.StructID = StructID;
		s7Tag.StructOffset = StructOffset;
		s7Tag.Tag = Tag;
		return s7Tag;
	}
}
