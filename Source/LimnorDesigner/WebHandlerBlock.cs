/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using LimnorDesigner.Event;
using Limnor.WebBuilder;
using System.Collections.Specialized;
using LimnorDatabase;

namespace LimnorDesigner
{
	public enum EnumWebHandlerBlockType { Client, Dialog, XmlHttp, Submit }
	[Flags]
	public enum EnumWebBlockProcess { None = 0, Download = 1, Upload = 2 }
	public class WebHandlerBlock
	{
		private int _index;
		private List<TaskID>[] _sects;
		private StringCollection _tableNames; //tables to be updated
		public WebHandlerBlock()
		{
			BlockType = EnumWebHandlerBlockType.Client;
			BlockProcess = EnumWebBlockProcess.None;
			_sects = new List<TaskID>[3];
			_sects[0] = new List<TaskID>();
			_sects[1] = new List<TaskID>();
			_sects[2] = new List<TaskID>();
			_tableNames = new StringCollection();
		}
		public StringCollection TableNames { get { return _tableNames; } }
		public EnumWebHandlerBlockType BlockType { get; set; }
		public EnumWebBlockProcess BlockProcess { get; set; }
		public bool UseServer
		{
			get
			{
				if (_sects[1].Count > 0)
				{
					return true;
				}
				if (_sects[2].Count > 0)
				{
					return true;
				}
				return false;
			}
		}
		public bool IsEmpty
		{
			get
			{
				if (_sects[0].Count > 0)
					return false;
				if (_sects[1].Count > 0)
					return false;
				if (_sects[2].Count > 0)
					return false;
				return true;
			}
		}
		public int SectIndex
		{
			get
			{
				return _index;
			}
			set
			{
				if (value >= 0 && value < 4)
					_index = value;
			}
		}
		public List<TaskID> this[int idx]
		{
			get
			{
				return _sects[idx];
			}
		}
		public List<TaskID> Sect0 { get { return _sects[0]; } }
		public List<TaskID> Sect1 { get { return _sects[1]; } }
		public List<TaskID> Sect2 { get { return _sects[2]; } }
		public void AddUpdateTableName(string name)
		{
			if (!_tableNames.Contains(name))
			{
				_tableNames.Add(name);
			}
		}
		public void AddUpdateTableNamesFromExpression(MathNodeRoot r)
		{
			if (r != null)
			{
				Dictionary<UInt32, IMethodPointerNode> mps = new Dictionary<uint, IMethodPointerNode>();
				r.GetMethodPointers(mps);
				if (mps.Count > 0)
				{
					foreach (KeyValuePair<UInt32, IMethodPointerNode> kv in mps)
					{
						if (string.CompareOrdinal("Update", kv.Value.MethodName) == 0)
						{
							EasyDataSet eds = kv.Value.MethodExecuter as EasyDataSet;
							if (eds != null)
							{
								this.AddUpdateTableName(eds.TableName);
							}
						}
					}
				}
			}
		}
		public void AddAction(TaskID t)
		{
			_sects[_index].Add(t);
		}
		public IList<ISourceValuePointer> GetServerVariables(ClassPointer root)
		{
			Dictionary<UInt32, IAction> actions = root.GetActions();
			SourceValuePointerList svs = new SourceValuePointerList();
			foreach (TaskID tid in Sect0) //check client action return values
			{
				HandlerMethodID hmid = tid as HandlerMethodID;
				if (hmid != null)
				{
					WebClientEventHandlerMethod wceh = hmid.HandlerMethod as WebClientEventHandlerMethod;
					if (wceh != null)
					{
						hmid.HandlerMethod.SetActions(actions);
						List<IAction> acts = hmid.HandlerMethod.GetActions();
						foreach (IAction act in acts)
						{
							if (act.ReturnReceiver != null)
							{
								ISourceValuePointer sv = act.ReturnReceiver as ISourceValuePointer;
								if (sv != null && sv.IsWebServerValue() && !sv.IsWebClientValue())
								{
									svs.AddUnique(sv);
								}
							}
						}
					}
				}
				else
				{
					IAction a = tid.GetPublicAction(root);
					if (a != null)
					{
						if (a.ReturnReceiver != null)
						{
							ISourceValuePointer sv = a.ReturnReceiver as ISourceValuePointer;
							if (sv != null && sv.IsWebServerValue() && !sv.IsWebClientValue())
							{
								svs.AddUnique(sv);
							}
						}
					}
				}
			}
			foreach (TaskID tid in Sect1) //check server action parameters
			{
				HandlerMethodID hmid = tid as HandlerMethodID;
				if (hmid != null)
				{
					WebClientEventHandlerMethod wceh = hmid.HandlerMethod as WebClientEventHandlerMethod;
					if (wceh != null)
					{
						hmid.HandlerMethod.SetActions(actions);
						hmid.HandlerMethod.CollectSourceValues(tid.TaskId);
						//
						svs.AddUnique(hmid.HandlerMethod.DownloadProperties);
						//
						List<MethodClass> ml = new List<MethodClass>();
						hmid.HandlerMethod.GetCustomMethods(ml);
						foreach (MethodClass m in ml)
						{
							m.CollectSourceValues(tid.TaskId);
							svs.AddUnique(m.DownloadProperties);
						}
					}
				}
				else
				{
					IAction a = tid.GetPublicAction(root);
					if (a != null && a.ActionMethod != null && a.ActionMethod.Owner != null)
					{
						IFormSubmitter fs = a.ActionMethod.Owner.ObjectInstance as IFormSubmitter;
						if (fs != null && fs.IsSubmissionMethod(a.ActionMethod.MethodName))
						{
							continue;
						}
						svs.AddUnique(a.GetServerProperties(tid.TaskId));
						ISourceValuePointer p = a.ReturnReceiver as ISourceValuePointer;
						if (p != null)
						{
							if (p.IsWebServerValue())
							{
								svs.AddUnique(p);
							}
						}
					}
				}
			}
			//collect download (server) variables from client actions
			foreach (TaskID tid in Sect2)
			{
				HandlerMethodID hmid = tid as HandlerMethodID;
				if (hmid != null)
				{
					WebClientEventHandlerMethod wceh = hmid.HandlerMethod as WebClientEventHandlerMethod;
					if (wceh != null)
					{
						hmid.HandlerMethod.SetActions(actions);
						hmid.HandlerMethod.CollectSourceValues(tid.TaskId);
						//
						svs.AddUnique(hmid.HandlerMethod.DownloadProperties);
						//
						List<MethodClass> ml = new List<MethodClass>();
						hmid.HandlerMethod.GetCustomMethods(ml);
						foreach (MethodClass m in ml)
						{
							m.CollectSourceValues(tid.TaskId);
							svs.AddUnique(m.DownloadProperties);
						}
					}
				}
				else
				{
					IAction a = tid.GetPublicAction(root);
					if (a != null)
					{
						svs.AddUnique(a.GetServerProperties(tid.TaskId));
					}
				}
			}
			return svs;
		}
	}
	public class WebHandlerBlockList : List<WebHandlerBlock>
	{
		private List<ISourceValuePointer> _serverStates;
		public WebHandlerBlockList()
		{
		}
		public void SetServerStates(SourceValuePointerList states)
		{
			_serverStates = states;
		}
		public void AddServerState(ISourceValuePointer v)
		{
			if (_serverStates == null)
			{
				_serverStates = new List<ISourceValuePointer>();
			}
			foreach (ISourceValuePointer sv in _serverStates)
			{
				if (sv.IsSameProperty(v))
				{
					return;
				}
			}
			_serverStates.Add(v);
		}
		public IList<ISourceValuePointer> ServerStates
		{
			get
			{
				return _serverStates;
			}
		}
	}
}
