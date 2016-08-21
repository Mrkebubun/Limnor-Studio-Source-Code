using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
//using System.Windows.Forms;

namespace DynamicEventLinker
{
	/// <summary>
	/// generic event handler to handler all events
	/// </summary>
	/// <param name="sinker">event handler owner</param>
	/// <param name="eventParameters"></param>
	public delegate void fnGenericHandler(IEventPointer eventPointer, object[] eventParameters);
	//
	/// <summary>
	/// used by DynamicLink
	/// </summary>
	public interface IEventPointer
	{
		object ObjectInstance { get; }
		EventInfo Info { get; }
	}
	// this is the dynamic class to be created for event linking
	/*
	class DLink1
	{
		IEventPointer _eventId;
		fnGenericHandler _handler;
		public DLink1(IEventPointer eventId, fnGenericHandler handler)
		{
			_eventId = eventId;
			_handler = handler;
		}
		//the parameters match the event handler signature
		public void HookEvent(object sender, EventArgs e)
		{
			object[] paramArray = new object[] { sender, e };
			_handler(_eventId, paramArray);
		}
		//EventHandler should be the type of the event handler
		public EventHandler GetHookEvent()
		{
			return new EventHandler(this.HookEvent);
		}
	}
	*/
	public class EventLinkerCollection : Dictionary<IEventPointer, object>
	{
		private Type _dynaType;
		public EventLinkerCollection(Type dynamicClassType)
		{
			_dynaType = dynamicClassType;
		}
		public Type DynamicClassType
		{
			get
			{
				return _dynaType;
			}
		}
	}
	//
	public sealed class DynamicLink
	{
		//1<=>1 mapping: this mapping does the dynamic link
		//object+event (event pointer) <=> dynamic object holding the handler
		//1<=>1 mapping: this mapping does the generic link
		//object+event (event pointer) <=> Action ID
		//EventHandlerType, DynamicObjectType, EventPointer, DynamicObject
		static Dictionary<Type, EventLinkerCollection> eventLinks = new Dictionary<Type, EventLinkerCollection>();
		private DynamicLink()
		{
		}
		/// <summary>
		/// hook event to a generic event handler
		/// </summary>
		/// <param name="ep">event sender</param>
		/// <param name="gh">the generic event handler</param>
		/// <returns></returns>
		static public CompilerErrorCollection LinkEvent(IEventPointer ep, fnGenericHandler gh)
		{
			EventLinkerCollection ec = null;
			if (eventLinks.ContainsKey(ep.Info.EventHandlerType))
			{
				ec = eventLinks[ep.Info.EventHandlerType];
			}
			else
			{
				string nsName = "DynamicEventLinker";
				// Create a code compile unit and a namespace
				CodeCompileUnit ccu = new CodeCompileUnit();
				CodeNamespace ns = new CodeNamespace(nsName);

				// Add some imports statements to the namespace
				ns.Imports.Add(new CodeNamespaceImport("System"));
				ns.Imports.Add(new CodeNamespaceImport("DynamicEventLinker"));
				//ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));
				//ns.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));

				// Add the namespace to the code compile unit
				ccu.Namespaces.Add(ns);
				//
				//Make class name unique
				string className = "DLink1";
				int n = 1;
				while (true)
				{
					bool bFound = false;
					foreach (KeyValuePair<Type, EventLinkerCollection> kv in eventLinks)
					{
						if (kv.Value.DynamicClassType.Name == className)
						{
							bFound = true;
							n++;
							className = "DLink" + n.ToString();
						}
					}
					if (!bFound)
						break;
				}
				CodeTypeDeclaration ctd = new CodeTypeDeclaration(className);
				ctd.BaseTypes.Add(typeof(object));
				ns.Types.Add(ctd);
				//
				CodeConstructor constructor = new CodeConstructor();
				constructor.Attributes = MemberAttributes.Public;
				ctd.Members.Add(constructor);
				constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IEventPointer), "eventId"));
				constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(fnGenericHandler), "handler"));
				ctd.Members.Add(new CodeMemberField(typeof(IEventPointer), "_eventId"));
				ctd.Members.Add(new CodeMemberField(typeof(fnGenericHandler), "_handler"));
				constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_eventId"),
					new CodeVariableReferenceExpression("eventId")));
				constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_handler"),
					new CodeVariableReferenceExpression("handler")));
				//
				//payload
				CodeMemberMethod method = new CodeMemberMethod();
				MethodInfo mif = ep.Info.EventHandlerType.GetMethod("Invoke");
				ParameterInfo[] pifs = mif.GetParameters();
				method.Name = "HookEvent";
				method.Attributes = MemberAttributes.Public;
				method.ReturnType = new CodeTypeReference(typeof(void));
				for (int i = 0; i < pifs.Length; i++)
				{
					method.Parameters.Add(new CodeParameterDeclarationExpression(pifs[i].ParameterType, pifs[i].Name));
				}
				//call _handler passing eventId and parameters to it
				//create parameter array
				CodeExpression[] paramArray = new CodeExpression[pifs.Length];
				for (int i = 0; i < pifs.Length; i++)
				{
					paramArray[i] = new CodeVariableReferenceExpression(pifs[i].Name);
				}
				//make the array variable name unique
				string arrayName = "paramArray";
				n = 1;
				while (true)
				{
					bool bFound = false;
					for (int i = 0; i < pifs.Length; i++)
					{
						if (pifs[i].Name == arrayName)
						{
							bFound = true;
							n++;
							arrayName = "paramArray" + n.ToString();
						}
					}
					if (!bFound)
						break;
				}

				method.Statements.Add(new CodeVariableDeclarationStatement(typeof(object[]), arrayName,
					new CodeArrayCreateExpression(typeof(object), paramArray)));
				//call _handler
				method.Statements.Add(new CodeDelegateInvokeExpression(new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), "_handler"), new CodeExpression[]{
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_eventId"),
                        new CodeVariableReferenceExpression(arrayName)}));
				//
				ctd.Members.Add(method);
				//method returns a delegate for the event-specific handler to be added to the event's handler
				method = new CodeMemberMethod();
				method.Name = "GetHookEvent";
				method.Attributes = MemberAttributes.Public;
				method.ReturnType = new CodeTypeReference(ep.Info.EventHandlerType);
				method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(
					ep.Info.EventHandlerType, new CodeExpression[]{
                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "HookEvent")
                }
					)));
				ctd.Members.Add(method);
				//compile it
				CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
				//ICodeCompiler compiler = provider.CreateGenerator() as ICodeCompiler;
				string linker = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicEventLinker.dll");
				if (!System.IO.File.Exists(linker))
				{
					CompilerErrorCollection errs = new CompilerErrorCollection();
					errs.Add(new CompilerError("DynamicLinker", 3, 3, "3", "Linker not found:" + linker));
					return errs;
				}
				CompilerParameters cp = new CompilerParameters(new string[] { 
                "System.dll",linker /*"System.Windows.Forms.dll", "System.Drawing.dll"*/ });
				cp.GenerateInMemory = true;
				cp.OutputAssembly = "AutoGenerated";
				CompilerResults results = provider.CompileAssemblyFromDom(cp, ccu);
				if (results.Errors == null || results.Errors.Count == 0)
				{
					Type[] tps = results.CompiledAssembly.GetExportedTypes();
					for (int i = 0; i < tps.Length; i++)
					{
						if (tps[i].Name == className)
						{
							ec = new EventLinkerCollection(tps[i]);
							eventLinks.Add(ep.Info.EventHandlerType, ec);
							break;
						}
					}
				}
				else
				{
					return results.Errors;
				}
			}
			if (ec != null)
			{
				try
				{
					object ev = Activator.CreateInstance(ec.DynamicClassType, new object[] { ep, gh });
					MethodInfo mi = ec.DynamicClassType.GetMethod("GetHookEvent");
					Delegate eh = (Delegate)mi.Invoke(ev, new object[] { });
					ep.Info.AddEventHandler(ep.ObjectInstance, eh);
					if (ec.ContainsKey(ep))
					{
						ec[ep] = ev;
					}
					else
					{
						ec.Add(ep, ev);
					}
				}
				catch (Exception er)
				{
					CompilerErrorCollection errs = new CompilerErrorCollection();
					errs.Add(new CompilerError("DynamicLinker", 2, 2, "2", string.Format("{0}. {1}", er.Message, er.StackTrace)));
					return errs;
				}
			}
			else
			{
				CompilerErrorCollection errs = new CompilerErrorCollection();
				errs.Add(new CompilerError("DynamicLinker", 1, 1, "1", "Dynamic type not found"));
				return errs;
			}
			return null;
		}
	}
}
