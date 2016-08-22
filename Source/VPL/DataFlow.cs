/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;

namespace VPL
{

	/// <summary>
	/// property indexer
	/// </summary>
	public class DL_Indexer
	{
		Type[] _parameterTypes;
		public DL_Indexer(Type[] parameterTypes)
		{
			_parameterTypes = parameterTypes;
		}
		public Type[] ParameterTypes
		{
			get
			{
				return _parameterTypes;
			}
		}
		public bool IsSameSignature(object[] ps)
		{
			if (ps == null)
			{
				return (_parameterTypes == null);
			}
			if (_parameterTypes != null)
			{
				if (ps.Length == _parameterTypes.Length)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						if (ps[i] != null)
						{
							if (!_parameterTypes[i].Equals(ps[i].GetType()))
								return false;
						}
					}
					return true;
				}
			}
			return false;
		}
		public bool IsSameSignature(Type[] ps)
		{
			if (ps == null)
			{
				return (_parameterTypes == null);
			}
			if (_parameterTypes != null)
			{
				if (ps.Length == _parameterTypes.Length)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						if (!_parameterTypes[i].Equals(ps[i]))
							return false;
					}
					return true;
				}
			}
			return false;
		}
	}

	#region Pull-link
	public delegate void DelegateSinkPullValue(IDataFlowDestionation sender, string propertyName, object[] parameters);
	public interface IDataFlowDestionation
	{
		event DelegateSinkPullValue OnPullProperty;
	}
	/// <summary>
	/// defines a data source by object and property name
	/// </summary>
	public class DL_DataSource
	{
		object _source;
		string _property;
		public DL_DataSource()
		{
		}
		public object SourceObject
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}
		public string PropertyName
		{
			get
			{
				return _property;
			}
			set
			{
				_property = value;
			}
		}
	}
	/// <summary>
	/// a collection of pull-links.
	/// {IDataFlowDestionation, string, DL_Indexer} indicates the destination property to receive value.
	/// DL_DataSource indicates the data source to provide the value.
	/// A pull-link is an event, OnPullProperty, fired before a "getter" returns the property value.
	/// Compiler's job:
	/// 1. For an object/type participating in a pull-link as a destination, implements IDataFlowDestionation 
	///     and declare a public event OnPullProperty, if it is not already implemented in a parent type.
	/// 2. For those properties participating in a pull-link as a destination, create an override or new property 
	///     to fire OnPullProperty before return the base property, if it is not done in a parent type.
	/// 3. Add PullLinkCollection as a variable of the container object.
	/// 4. At the end of InitializeComponents function, add all the pull-links to PullLinkCollection;
	///     attach PullLinkCollection.SinkPullValue as the event handler to OnPullProperty event of all objects
	///     of the pull-links. If {object} is a Type then use _objectManager to attach the event handler to all
	///     instances.
	/// </summary>
	public class PullLinkCollection : Dictionary<object, Dictionary<string, Dictionary<DL_Indexer, DL_DataSource>>>
	{
		private IObjectManager _objectManager;
		public PullLinkCollection()
		{
		}
		public IObjectManager ObjectManager
		{
			get
			{
				return _objectManager;
			}
			set
			{
				_objectManager = value;
			}
		}
		private Hashtable _dataDestinations;
		/// <summary>
		/// attach pull-link event handler to the destination object
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="data"></param>
		private void onAttachHabdler(object instance, object data)
		{
			IDataFlowDestionation destinationObject = instance as IDataFlowDestionation;
			if (destinationObject == null)
			{
				if (instance is RAIS_R)
				{
					destinationObject = ((RAIS_R)instance).Instance as IDataFlowDestionation;
				}
			}
			if (destinationObject != null)
			{
				destinationObject.OnPullProperty += (DelegateSinkPullValue)data;
			}
		}
		/// <summary>
		/// the event handler to be attached to every IDataFlowDestionation object
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="propertyName"></param>
		/// <param name="parameters"></param>
		private void SinkPullValue(IDataFlowDestionation sender, string propertyName, object[] parameters)
		{
			Dictionary<string, Dictionary<DL_Indexer, DL_DataSource>> v1 = null;
			if (this.ContainsKey(sender))
				v1 = this[sender];
			else
			{
				foreach (object v in this.Keys)
				{
					if (v is RAIS_A)
					{
						if (((RAIS_A)v).IsInstance(sender))
						{
							v1 = this[v];
							break;
						}
					}
				}
			}
			if (v1 != null)
			{
				if (_dataDestinations == null)
					_dataDestinations = new Hashtable();
				if (_dataDestinations.ContainsKey(sender))
					return;
				_dataDestinations.Add(sender, v1);
				try
				{
					Dictionary<DL_Indexer, DL_DataSource> v2;
					if (v1.ContainsKey(propertyName))
					{
						v2 = v1[propertyName];
						DL_DataSource v3 = null;
						Dictionary<DL_Indexer, DL_DataSource>.Enumerator e2 = v2.GetEnumerator();
						while (e2.MoveNext())
						{
							if (e2.Current.Key.IsSameSignature(parameters))
							{
								v3 = e2.Current.Value;
								break;
							}
						}
						if (v3 != null)
						{
							object value = null;
							bool bGotValue = false;
							if (v3.SourceObject is string)
							{
								if (_objectManager != null)
								{
									object objSource = _objectManager.CurrentItem(v3.SourceObject.ToString());
									PropertyInfo piSource = objSource.GetType().GetProperty(v3.PropertyName);
									value = piSource.GetValue(objSource, parameters);
									bGotValue = true;
								}
							}
							else
							{
								PropertyInfo piSource = v3.SourceObject.GetType().GetProperty(v3.PropertyName);
								value = piSource.GetValue(v3.SourceObject, parameters);
								bGotValue = true;
							}
							if (bGotValue)
							{
								PropertyInfo piDest = sender.GetType().GetProperty(propertyName);
								piDest.SetValue(sender, value, parameters);
							}
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					_dataDestinations.Remove(sender);
				}
			}
		}
		public void AddLink(object destinationObject,
			string destinationPropertyName,
			Type[] indexers,
			object sourceObject,
			string sourcePropertyName)
		{
			Dictionary<string, Dictionary<DL_Indexer, DL_DataSource>> v1 = null;
			object tgt = destinationObject;
			if (destinationObject is string && _objectManager != null)
			{
				tgt = _objectManager.FindElement(destinationObject.ToString());
			}
			if (this.ContainsKey(tgt))
				v1 = this[tgt];
			if (v1 == null)
			{
				v1 = new Dictionary<string, Dictionary<DL_Indexer, DL_DataSource>>();
				this.Add(tgt, v1);
				IDataFlowDestionation dest = destinationObject as IDataFlowDestionation;
				if (dest != null)
				{
					dest.OnPullProperty += new DelegateSinkPullValue(SinkPullValue);
				}
				else
				{
					if (_objectManager != null && destinationObject is string)
					{
						string path = destinationObject.ToString();
						if (path.Length > 1)
						{
							delegateOnGetInstance eh = new delegateOnGetInstance(onAttachHabdler);
							_objectManager.WorkOnAllInstances(path, eh, new DelegateSinkPullValue(SinkPullValue));
						}
					}
				}
			}
			Dictionary<DL_Indexer, DL_DataSource> v2;
			if (v1.ContainsKey(destinationPropertyName))
			{
				v2 = v1[destinationPropertyName];
			}
			else
			{
				v2 = new Dictionary<DL_Indexer, DL_DataSource>();
				v1.Add(destinationPropertyName, v2);
			}
			DL_DataSource v3 = null;
			Dictionary<DL_Indexer, DL_DataSource>.Enumerator e2 = v2.GetEnumerator();
			while (e2.MoveNext())
			{
				if (e2.Current.Key.IsSameSignature(indexers))
				{
					v3 = e2.Current.Value;
					break;
				}
			}
			if (v3 == null)
			{
				v3 = new DL_DataSource();
				v2.Add(new DL_Indexer(indexers), v3);
			}
			v3.SourceObject = sourceObject;
			v3.PropertyName = sourcePropertyName;
		}
	}
	#endregion

	#region Push-link
	public delegate void DelegateSinkPushValue(IDataFlowSource sender, string propertyName, object[] parameters, object value);
	public interface IDataFlowSource
	{
		event DelegateSinkPushValue OnPushProperty;
	}
	public class DL_DataDestination
	{
		public DL_DataDestination()
		{
		}
	}
	/// <summary>
	/// a collection of push-links.
	/// {object, string, DL_Indexer} indicates the source property to provide value.
	/// {object, string in StringCollection} indicates the destination property to receive the value.
	/// A push-link is an event, OnPushProperty, fired after a "setter" sets the property value.
	/// Compiler's job:
	/// 1. For an object/type participating in a push-link as a source, implements IDataFlowSource 
	///     and declare a public event IDataFlowSource.OnPushProperty, if it is not already implemented in a parent type.
	/// 2. For those properties participating in a push-link as a source, create an override or new property 
	///     to fire OnPushProperty after setting the base property, if it is not done in a parent type.
	/// 3. Add PushLinkCollection as a variable of the container object.
	/// 4. At the end of InitializeComponents function, add all the push-links to PushLinkCollection;
	///     attach PushLinkCollection.SinkPushValue as the event handler to OnPushProperty event of all objects
	///     of the push-links. If {object} is a Type then use _objectManager to attach the event handler to all
	///     instances.
	/// </summary>
	public class PushLinkCollection : Dictionary<object, Dictionary<string, Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>>>
	{
		private IObjectManager _objectManager;
		public PushLinkCollection()
		{
		}
		public IObjectManager ObjectManager
		{
			get
			{
				return _objectManager;
			}
			set
			{
				_objectManager = value;
			}
		}
		private void onAttachHabdler(object instance, object data)
		{
			IDataFlowSource sourceObject = instance as IDataFlowSource;
			if (sourceObject == null)
			{
				if (instance is RAIS_R)
				{
					sourceObject = ((RAIS_R)instance).Instance as IDataFlowSource;
				}
			}
			if (sourceObject != null)
			{
				sourceObject.OnPushProperty += (DelegateSinkPushValue)data;
			}
		}
		private Hashtable _dataSources;
		private void onSetValue(object instance, object data)
		{
			OM_Assign oa = (OM_Assign)data;
			object v;
			if (instance is RAIS_R)
			{
				v = ((RAIS_R)instance).Instance;
			}
			else
				v = instance;
			setValue(v, oa.PropertyName, oa.Indexer, oa.Value);
		}
		private void setValue(object destination, string propertyName, object[] indexer, object value)
		{
			PropertyInfo pi = destination.GetType().GetProperty(propertyName);
			pi.SetValue(destination, value, indexer);
		}
		/// <summary>
		/// push value from source to destination
		/// </summary>
		/// <param name="sender">data source</param>
		/// <param name="propertyName">source property</param>
		/// <param name="parameters">indexer</param>
		/// <param name="value">value to send</param>
		public void SinkPushValue(IDataFlowSource sender, string propertyName, object[] parameters, object value)
		{
			Dictionary<string, Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>> v1 = null;
			if (this.ContainsKey(sender))
				v1 = this[sender];
			else
			{
				foreach (object v in this.Keys)
				{
					if (v is RAIS_A)
					{
						if (((RAIS_A)v).IsInstance(sender))
						{
							v1 = this[v];
							break;
						}
					}
				}
			}
			if (v1 != null)
			{
				if (_dataSources == null)
					_dataSources = new Hashtable();
				if (_dataSources.ContainsKey(sender))
					return;
				_dataSources.Add(sender, v1);
				try
				{
					Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>> v2;
					if (v1.ContainsKey(propertyName))
					{
						v2 = v1[propertyName];
						Dictionary<object, Dictionary<string, DL_DataDestination>> v3 = null;
						Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>.Enumerator e2 = v2.GetEnumerator();
						while (e2.MoveNext())
						{
							if (e2.Current.Key.IsSameSignature(parameters))
							{
								v3 = e2.Current.Value;
								break;
							}
						}
						if (v3 != null)
						{
							Dictionary<object, Dictionary<string, DL_DataDestination>>.Enumerator e3 = v3.GetEnumerator();
							while (e3.MoveNext())
							{
								Dictionary<string, DL_DataDestination>.Enumerator e4 = e3.Current.Value.GetEnumerator();
								while (e4.MoveNext())
								{
									if (e3.Current.Key is Type)
									{
										EventInfo ei = ((Type)e3.Current.Key).GetEvent("OnVOBStaticSetValue", BindingFlags.Static);
										if (ei != null)
										{
											MethodInfo mi = ei.GetRaiseMethod(true);
											mi.Invoke(null, new object[] { sender, e4.Current.Key, parameters, value });
										}
									}
									else
									{
										if (e3.Current.Key is string)
										{
											OM_Assign oa = new OM_Assign(e4.Current.Key, value, parameters);
											delegateOnGetInstance h = new delegateOnGetInstance(onSetValue);
											_objectManager.WorkOnAllInstances(e3.Current.Key.ToString(), h, oa);
										}
										else
										{
											setValue(e3.Current.Key, e4.Current.Key, parameters, value);
										}
									}
								}
							}
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					_dataSources.Remove(sender);
				}
			}
		}
		public void AddLink(object sourceObject,
			string sourcePropertyName,
			Type[] indexer,
			object destinationObject,
			string destinationPropertyName)
		{
			Dictionary<string, Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>> v1 = null;
			object src = sourceObject;
			if (sourceObject is string && _objectManager != null)
			{
				src = _objectManager.FindElement(sourceObject.ToString());
			}
			if (this.ContainsKey(src))
			{
				v1 = this[src];
			}
			if (v1 == null)
			{
				v1 = new Dictionary<string, Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>>();
				this.Add(src, v1);
				IDataFlowSource source = sourceObject as IDataFlowSource;
				if (source != null)
				{
					source.OnPushProperty += new DelegateSinkPushValue(SinkPushValue);
				}
				else
				{
					if (_objectManager != null && sourceObject is string)
					{
						string path = sourceObject.ToString();
						if (path.Length > 1)
						{
							delegateOnGetInstance eh = new delegateOnGetInstance(onAttachHabdler);
							_objectManager.WorkOnAllInstances(path, eh, new DelegateSinkPushValue(SinkPushValue));
						}
					}
				}
			}
			Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>> v2;
			if (v1.ContainsKey(sourcePropertyName))
			{
				v2 = v1[sourcePropertyName];
			}
			else
			{
				v2 = new Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>();
				v1.Add(sourcePropertyName, v2);
			}
			Dictionary<object, Dictionary<string, DL_DataDestination>> v3 = null;
			Dictionary<DL_Indexer, Dictionary<object, Dictionary<string, DL_DataDestination>>>.Enumerator e2 = v2.GetEnumerator();
			while (e2.MoveNext())
			{
				if (e2.Current.Key.IsSameSignature(indexer))
				{
					v3 = e2.Current.Value;
					break;
				}
			}
			if (v3 == null)
			{
				v3 = new Dictionary<object, Dictionary<string, DL_DataDestination>>();
				v2.Add(new DL_Indexer(indexer), v3);
			}
			Dictionary<string, DL_DataDestination> v4;
			if (v3.ContainsKey(destinationObject))
			{
				v4 = v3[destinationObject];
			}
			else
			{
				v4 = new Dictionary<string, DL_DataDestination>();
				v3.Add(destinationObject, v4);
			}
			if (!v4.ContainsKey(destinationPropertyName))
			{
				v4.Add(destinationPropertyName, new DL_DataDestination());
			}
		}
	}
	#endregion
}
