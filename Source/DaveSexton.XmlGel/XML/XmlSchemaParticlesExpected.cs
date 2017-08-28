using System.Collections.Generic;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XmlSchemaParticlesExpected : List<XmlSchemaParticle>
	{
		public Part TargetPart
		{
			get
			{
				return targetPart;
			}
		}

		public bool IsExpectedAfterTargetPart
		{
			get
			{
				return isExpectedAfterTargetPart;
			}
		}

		public bool IsRequired
		{
			get;
			set;
		}

		private Part targetPart;
		private bool isExpectedAfterTargetPart;

		public XmlSchemaParticlesExpected()
		{
		}

		public XmlSchemaParticlesExpected(IEnumerable<XmlSchemaParticle> particles)
			: base(particles)
		{
		}

		internal void DeleteTextElements()
		{
			targetPart.Element.RemoveFromParent();

			// TODO: Consider deleting other related required elements; e.g., MinOccurs="2"
		}

		internal void SetTargetPart(Part targetPart, bool isExpectedAfter)
		{
			this.targetPart = targetPart;
			this.isExpectedAfterTargetPart = isExpectedAfter;
		}
	}
}