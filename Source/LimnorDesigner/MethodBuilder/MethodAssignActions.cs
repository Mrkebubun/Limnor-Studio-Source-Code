/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;
using LimnorDesigner.Event;
using System.ComponentModel;

namespace LimnorDesigner.MethodBuilder
{
	public class MethodAssignActions : IMethod, IActionCompiler
	{
		private EventAction _eventAction;
		public MethodAssignActions()
		{
		}
		public IEvent Event
		{
			get
			{
				if (_eventAction != null)
					return _eventAction.Event;
				return null;
			}
			set
			{
				if (_eventAction == null)
					_eventAction = new EventAction();
				_eventAction.Event = value;
			}
		}

		#region IMethod Members
		[Browsable(false)]
		[ReadOnly(true)]
		public IParameter MethodReturnType
		{
			get
			{
				return new ParameterClass(typeof(void), "Ret", this);
			}
			set
			{

			}
		}
		[Browsable(false)]
		public bool NoReturn
		{
			get { return true; }
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
			get { return true; }
		}

		public bool HasReturn
		{
			get { return false; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
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

		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			throw new NotImplementedException();
		}

		public object GetParameterType(uint id)
		{
			throw new NotImplementedException();
		}

		public Type ActionBranchType
		{
			get { throw new NotImplementedException(); }
		}

		public Type ActionType
		{
			get { throw new NotImplementedException(); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ModuleProject
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

		public bool IsParameterFilePath(string parameterName)
		{
			throw new NotImplementedException();
		}

		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IMethod0 Members

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

		public int ParameterCount
		{
			get { throw new NotImplementedException(); }
		}

		public IList<IParameter> MethodParameterTypes
		{
			get { throw new NotImplementedException(); }
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

		#region IActionCompiler Members

		public void Compile(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, System.CodeDom.CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			throw new NotImplementedException();
		}

		public void CreateJavaScript(System.Collections.Specialized.StringCollection sb, System.Collections.Specialized.StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		public void CreatePhpScript(System.Collections.Specialized.StringCollection sb, System.Collections.Specialized.StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
