/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgElements;
using LimnorDesigner.Property;
using System.ComponentModel;
using System.Xml.Serialization;
using VSPrj;

namespace LimnorDesigner.MethodBuilder
{
	public interface IMethodWrapper : IMethod
	{
	}
	public interface IPropertyWrapper : IProperty
	{
	}
	public class MethodWrapper : IMethodWrapper, IMethodPointer
	{
		private IActionContext _action;
		public MethodWrapper()
		{
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionContext Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}
		#region IMethod Members
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MethodWrapper obj = (MethodWrapper)this.Clone();
			obj._action = action;
			return obj;
		}
		private LimnorProject _prj;
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public object ModuleProject
		{
			get
			{
				return _prj;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return null;
			}
		}
		public string MethodName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get
			{
				return this.MethodName;
			}
		}
		public IParameter MethodReturnType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IObjectIdentity ReturnPointer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get { throw new NotImplementedException(); }
		}
		public IList<IParameterValue> MethodParameterValues
		{
			get { throw new NotImplementedException(); }
		}
		public int ParameterCount
		{
			get { throw new NotImplementedException(); }
		}

		public string ObjectKey
		{
			get { throw new NotImplementedException(); }
		}

		public string MethodSignature
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsForLocalAction
		{
			get { throw new NotImplementedException(); }
		}
		public bool NoReturn
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public bool HasReturn
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSameMethod(IMethod method)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			throw new NotImplementedException();
		}

		public string ParameterName(int i)
		{
			throw new NotImplementedException();
		}

		public object GetParameterValue(string name)
		{
			throw new NotImplementedException();
		}
		public object GetParameterType(UInt32 id)
		{
			throw new NotImplementedException();
		}
		public void SetParameterValue(string name, object value)
		{
			throw new NotImplementedException();
		}

		public void SetParameterValueChangeEvent(EventHandler h)
		{
			throw new NotImplementedException();
		}
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			throw new NotImplementedException();
		}

		public IObjectIdentity IdentityOwner
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsStatic
		{
			get { throw new NotImplementedException(); }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { throw new NotImplementedException(); }
		}

		public EnumPointerType PointerType
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			IObjectIdentity o = pointer as IObjectIdentity;
			if (o != null)
			{
				return IsSameObjectRef(o);
			}
			return false;
		}

		#endregion
	}
}
