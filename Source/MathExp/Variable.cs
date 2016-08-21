/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Xml;
using ProgElements;
using System.Collections.Specialized;

namespace MathExp
{
	public interface IVariable : IPortOwner, IParameter
	{
		RaisDataType VariableType { get; set; }
		string VariableName { get; set; }
		string SubscriptName { get; set; }
		IMathExpression Scope { get; set; }
		/// <summary>
		/// a local variable used by the math expression (MathNode)
		/// </summary>
		bool IsLocal { get; set; }
		/// <summary>
		/// an element not used as a variable, for example, dx in an integral operation.
		/// </summary>
		bool IsParam { get; set; }
		/// <summary>
		/// used to represent the calculation result
		/// </summary>
		bool IsReturn { get; set; }
		/// <summary>
		/// in the same scope and same KeyName, this flag indicates whether this variable's InPort is used as the input-port
		/// </summary>
		bool IsInPort { get; set; }
		bool IsConst { get; }
		bool NoAutoDeclare { get; set; }
		/// <summary>
		/// true: a variable will be generated; false: no variable will be generated
		/// </summary>
		bool IsCodeVariable { get; }
		bool IsMathRootReturn { get; }
		bool ClonePorts { get; set; }
		string KeyName { get; }//=VariableName+"_"+SubscriptName
		string DisplayName { get; }
		/// <summary>
		/// variable name for code compiling
		/// </summary>
		string CodeVariableName { get; }
		CodeExpression ExportCode(IMethodCompile method);
		string ExportJavaScriptCode(StringCollection methodCode);
		string ExportPhpScriptCode(StringCollection methodCode);
		void AssignCode(CodeExpression code);
		void AssignJavaScriptCode(string code);
		void AssignPhpScriptCode(string code);
		/// <summary>
		/// unique ID, even KeyName is the same for two variables their ID values are not.
		/// </summary>
		UInt32 ID { get; }
		UInt32 RawId { get; }
		void ResetID(UInt32 id);
		object CloneExp(MathNode parent);
		LinkLineNodeInPort InPort { get; set; }
		LinkLineNodeOutPort[] OutPorts { get; set; }
		MathNodeRoot MathExpression { get; }
	}
	public class VariableList : List<IVariable>
	{
		public VariableList()
		{
		}
		public IVariable ContainsVariable(IVariable v)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].VariableName == v.VariableName && this[i].SubscriptName == v.SubscriptName)
				{
					return this[i];
				}
			}
			return null;
		}
		public IVariable FindMatchingPublicVariable(IVariable v)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (!this[i].IsLocal && this[i].VariableName == v.VariableName && this[i].SubscriptName == v.SubscriptName)
				{
					return this[i];
				}
			}
			return null;
		}
		public IVariable GetVariableById(UInt32 id)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ID == id)
				{
					return this[i];
				}
			}
			return null;
		}
		public string GetCodeVariableNameByKey(string key)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.CompareOrdinal(this[i].KeyName, key) == 0)
				{
					return this[i].CodeVariableName;
				}
			}
			return string.Empty;
		}
		public string GetKeyByCodeVariableName(string codeVarName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.CompareOrdinal(this[i].CodeVariableName, codeVarName) == 0)
				{
					return this[i].KeyName;
				}
			}
			return string.Empty;
		}
	}
}
