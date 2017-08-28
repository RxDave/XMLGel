using System;
using System.Globalization;
using System.Windows.Controls;
using System.Xml.Schema;

namespace DaveSexton.XmlGel
{
	internal sealed class TypeCodeValidationRule : ValidationRule
	{
		private readonly XmlTypeCode typeCode;

		public TypeCodeValidationRule(XmlTypeCode typeCode)
		{
			this.typeCode = typeCode;
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var v = value as string;

			switch (typeCode)
			{
				case XmlTypeCode.Boolean:
					bool b;
					return new ValidationResult(bool.TryParse(v, out b), "The specified value is not a valid boolean.");
				case XmlTypeCode.Byte:
					sbyte sb;
					return new ValidationResult(sbyte.TryParse(v, out sb), "The specified value is not a valid signed byte.");
				case XmlTypeCode.Decimal:
					decimal dm;
					return new ValidationResult(decimal.TryParse(v, out dm), "The specified value is not a valid decimal number.");
				case XmlTypeCode.Double:
					double d;
					return new ValidationResult(double.TryParse(v, out d), "The specified value is not a valid double-precision floating point number.");
				case XmlTypeCode.Float:
					float f;
					return new ValidationResult(float.TryParse(v, out f), "The specified value is not a valid floating point number.");
				case XmlTypeCode.Int:
				case XmlTypeCode.Integer:
					int i;
					return new ValidationResult(int.TryParse(v, out i), "The specified value is not a valid integer.");
				case XmlTypeCode.Long:
					long l;
					return new ValidationResult(long.TryParse(v, out l), "The specified value is not a valid long integer.");
				case XmlTypeCode.NonNegativeInteger:
					int nni;
					return new ValidationResult(int.TryParse(v, out nni) && nni >= 0, "The specified value is not a valid non-negative integer.");
				case XmlTypeCode.NonPositiveInteger:
					int npi;
					return new ValidationResult(int.TryParse(v, out npi) && npi <= 0, "The specified value is not a valid non-positive integer.");
				case XmlTypeCode.PositiveInteger:
					int pi;
					return new ValidationResult(int.TryParse(v, out pi) && pi > 0, "The specified value is not a valid positive integer.");
				case XmlTypeCode.Short:
					short s;
					return new ValidationResult(short.TryParse(v, out s), "The specified value is not a valid short integer.");
				case XmlTypeCode.UnsignedByte:
					byte ub;
					return new ValidationResult(byte.TryParse(v, out ub), "The specified value is not a valid byte.");
				case XmlTypeCode.UnsignedInt:
					uint ui;
					return new ValidationResult(uint.TryParse(v, out ui), "The specified value is not a valid unsigned integer.");
				case XmlTypeCode.UnsignedLong:
					ulong ul;
					return new ValidationResult(ulong.TryParse(v, out ul), "The specified value is not a valid unsigned long integer.");
				case XmlTypeCode.UnsignedShort:
					ushort us;
					return new ValidationResult(ushort.TryParse(v, out us), "The specified value is not a valid unsigned short integer.");
				case XmlTypeCode.Date:
				case XmlTypeCode.DateTime:
					DateTime dt;
					return new ValidationResult(DateTime.TryParse(v, out dt), "The specified value is not a valid data or date/time.");
				case XmlTypeCode.Duration:
				case XmlTypeCode.Time:
					TimeSpan ts;
					return new ValidationResult(TimeSpan.TryParse(v, out ts), "The specified value is not a valid time or duration.");
				default:
					return new ValidationResult(true, null);
			}
		}
	}
}