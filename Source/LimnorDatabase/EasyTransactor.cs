/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using System.Data.Common;
using System.Data;
using VPL;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace LimnorDatabase
{
	[ToolboxBitmapAttribute(typeof(EasyTransactor), "Resources.dbTransactor.bmp")]
	[Description("This component executes database updating commands in a transaction. It can be used for adding new records, modifying existing records, or deleting records.")]
	public class EasyTransactor : IComponent, IComponentReferenceHolder, INotifyComponentChanged
	{
		#region fields and constructors
		private string _errMsg;
		private DbTransaction _transaction;
		private EasyUpdator[] _commands;
		private string[] _commandNames;
		public EasyTransactor()
		{
			ShowErrorMessage = true;
		}

		#endregion

		#region private methods
		private void SetError(string sErr)
		{
			_errMsg = sErr;
			if (!string.IsNullOrEmpty(_errMsg))
			{
				if (ExecutionError != null)
				{
					ExecutionError(this, EventArgs.Empty);
				}
				if (ShowErrorMessage)
				{
					FormLog.ShowMessage(_errMsg);
				}
			}
		}
		private void adjustCommands()
		{
			if (_commands == null)
			{
				_commands = new EasyUpdator[1];
			}
			else
			{
				bool bHasNull = false;
				for (int i = 0; i < _commands.Length; i++)
				{
					if (_commands[i] == null)
					{
						bHasNull = true;
					}
					else
					{
						if (i > 0)
						{
							for (int j = 0; j < i; j++)
							{
								if (_commands[j] != null)
								{
									if (_commands[j].Site != null)
									{
										if (string.CompareOrdinal(_commands[j].Site.Name, _commands[i].Site.Name) == 0)
										{
											_commands[i] = null;
											bHasNull = true;
										}
									}
								}
							}
						}
					}
				}
				if (!bHasNull)
				{
					EasyUpdator[] a = new EasyUpdator[_commands.Length + 1];
					_commands.CopyTo(a, 0);
					_commands = a;
				}
			}
		}
		#endregion

		#region Events
		[Description("Occurs when the Execute method fails. The ErrorMessage property gives information about why the execution failed.")]
		public event EventHandler ExecutionError = null;
		[Description("Occurs when an ExecuteActionsInTransaction action is executed. All Execute actions belonging to DatabaseCommands and assigned to this event will be executed in one signle database transaction. Do not assign an ExecuteActionsInTransaction action to this event because it will cause a deadlock.")]
		public event ExecuteInOneTransactionHandler ExecuteInOneTransaction = null;
		#endregion

		#region Methods
		private object _transactionObject = new object();
		[Description("Executing an action created from this method causes event ExecuteInOneTransaction to occur and thus all actions assigned to event ExecuteInOneTransaction are executed. All Execute actions belonging to components listed in DatabaseCommands are executed in one single database transaction. The transaction isolation level is specified by parameter isolationLevel.")]
		public bool ExecuteActionsInTransaction(IsolationLevel isolationLevel)
		{
			if (ExecuteInOneTransaction != null && _commands != null && _commands.Length > 0)
			{
				string sMsg = string.Empty;
				SetError(sMsg);
				Connection connect = _commands[0].DatabaseConnection.ConnectionObject;
				if (connect != null)
				{
					if (connect.IsConnectionReady)
					{
						lock (_transactionObject)
						{
							try
							{
								connect.Open();
								_transaction = connect.BeginTransaction(isolationLevel);
								for (int i = 0; i < _commands.Length; i++)
								{
									_commands[i].SetTransaction(_transaction, _commands[0].DatabaseConnection);
								}
								ExecuteInOneTransaction(this, new EventArgsDbTransaction(isolationLevel));
								if (_transaction != null)
								{
									_transaction.Commit();
								}
							}
							catch (Exception exp)
							{
								sMsg = ExceptionLimnorDatabase.FormExceptionText(exp);
								if (_transaction != null)
								{
									if (_transaction.Connection != null && _transaction.Connection.State == ConnectionState.Open)
									{
										try
										{
											_transaction.Rollback();
										}
										catch
										{
										}
									}
								}
							}
							finally
							{
								connect.Close();
								if (_transaction != null)
								{
									_transaction.Dispose();
									_transaction = null;
								}
							}
						}
					}
				}
				else
				{
					sMsg = "Database connection not ready";
				}
				if (!string.IsNullOrEmpty(sMsg))
				{
					SetError(sMsg);
					return false;
				}
				return true;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				if (ExecuteInOneTransaction == null)
				{
					sb.Append("There are not actions assigned to ExecuteInOneTransaction");
				}
				if (_commands == null || _commands.Length == 0)
				{
					sb.Append("There are not execution commands");
				}
				SetError(sb.ToString());
				return false;
			}
		}
		/// <summary>
		/// component names are used by compilers to get components for calling SetComponentReferences
		/// </summary>
		/// <returns>component names</returns>
		public string[] GetComponentReferenceNames()
		{
			if (_commandNames == null)
			{
				return new string[] { };
			}
			return _commandNames;
		}
		#endregion

		#region Properties
		[Category("Database")]
		[DefaultValue(true)]
		[Description("Controls whether error message is displayed when event ExecutionError occurs.")]
		public bool ShowErrorMessage
		{
			get;
			set;
		}
		[Category("Database")]
		[Description("Gets an array containing EasyUpdator components to be included in the database transaction when ExecuteActionsInTransaction is executed. The database connection for the first EasyUpdator will be used for all other EasyUpdator components.")]
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		public EasyUpdator[] DatabaseCommands
		{
			get
			{
				adjustCommands();

				return _commands;
			}
		}
		[Category("Database")]
		[Description("Gets the error message for the last Execute action if it failed")]
		public string ErrorMessage
		{
			get
			{
				return _errMsg;
			}
		}

		[Browsable(false)]
		public string[] ComponentReferenceNames
		{
			get
			{
				StringCollection sc = new StringCollection();
				if (_commands != null)
				{
					for (int i = 0; i < _commands.Length; i++)
					{
						if (_commands[i] != null && _commands[i].Site != null && !sc.Contains(_commands[i].Site.Name))
						{
							sc.Add(_commands[i].Site.Name);
						}
					}
				}
				string[] ss = new string[sc.Count];
				sc.CopyTo(ss, 0);
				return ss;
			}
			set
			{
				_commandNames = value;
			}
		}


		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IComponentReferenceHolder Members
		[Browsable(false)]
		public void ResolveComponentReferences(object[] components)
		{
			if (_commandNames != null && _commandNames.Length > 0 && components != null)
			{
				List<EasyUpdator> lst = new List<EasyUpdator>();
				for (int i = 0; i < _commandNames.Length; i++)
				{
					if (!string.IsNullOrEmpty(_commandNames[i]))
					{
						for (int j = 0; j < components.Length; j++)
						{
							EasyUpdator dc = components[j] as EasyUpdator;
							if (dc != null && dc.Site != null)
							{
								if (string.Compare(dc.Site.Name, _commandNames[i], StringComparison.Ordinal) == 0)
								{
									lst.Add(dc);
									break;
								}
							}
						}
					}
				}
				_commands = new EasyUpdator[lst.Count];
				lst.CopyTo(_commands, 0);
			}
		}
		[Browsable(false)]
		public void SetComponentReferences(object[] components)
		{
			List<EasyUpdator> lst = new List<EasyUpdator>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					EasyUpdator eu = components[i] as EasyUpdator;
					if (eu != null)
					{
						lst.Add(eu);
					}
				}
			}
			_commands = new EasyUpdator[lst.Count];
			lst.CopyTo(_commands, 0);
		}
		#endregion

		#region INotifyComponentChanged Members
		[Browsable(false)]
		public bool OnComponentChanged(PropertyValueChangedEventArgs e)
		{
			if (e != null && e.ChangedItem != null && e.ChangedItem.Parent != null && e.ChangedItem.Parent.PropertyDescriptor != null)
			{
				if (string.CompareOrdinal(e.ChangedItem.Parent.PropertyDescriptor.Name, "DatabaseCommands") == 0)
				{
					adjustCommands();
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}
