/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project -- ASP.NET Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Globalization;
using System.Web.UI;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace WebServerProcessor
{
	public abstract class JsonWebServerProcessor : Page
	{
		protected WebClientRequest clientRequest;
		protected WebServerResponse serverResponse;
		protected bool shouldSendResponse;
		private List<IFileUploador> _fileUploaders;
		//
		protected virtual void OnRequestStart() { }
		protected abstract void OnRequestGetData(string dataName);
		protected abstract void OnRequestPutData(string dataName);

		protected abstract void OnRequestFinish();
		//
		protected const string DEBUG_SYMBOL = "F3E767376E6546a8A15D97951C849CE5";
		//
		protected bool DEBUG;
		public bool Stop;

		static public string FormExceptionText(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			if (e.StackTrace != null)
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				if (e.StackTrace != null)
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
		//
		public string Name { get; set; }
		public void SetServerComponentName(string name)
		{
			serverResponse.serverComponentName = name;
		}
		public void OutputException(Exception e)
		{
			if (e != null)
			{
				Response.Write("Exception type:");
				Response.Write(e.GetType().AssemblyQualifiedName);
				Response.Write("<br>");
				Response.Write("Exception message:");
				Response.Write(e.Message);
				Response.Write("<br>");
				Response.Write("Stack trace:");
				if (!string.IsNullOrEmpty(e.StackTrace))
				{
					Response.Write(e.StackTrace);
				}
				Response.Write("<br>");
				Response.Write("Source:");
				if (!string.IsNullOrEmpty(e.Source))
				{
					Response.Write(e.Source);
				}
				Response.Write("<br>");
				if (e.InnerException != null)
				{
					Response.Write("Inner exception:<br>");
					OutputException(e.InnerException);
				}
			}
		}
		public void LogDebugInfo(string info, params object[] values)
		{
			if (DEBUG)
			{
				if (values != null && values.Length > 0)
				{
					Response.Write(string.Format(CultureInfo.InvariantCulture, info, values));
				}
				else
				{
					Response.Write(info);
				}
				Response.Write("<br>");
			}
		}
		public string GetCookie(string name)
		{
			HttpCookie cookie = Request.Cookies.Get(name);
			if (cookie == null)
			{
				return string.Empty;
			}
			return cookie.Value;
		}
		public void SetCookie(string name, string value)
		{
			HttpCookie cookie = new HttpCookie(name, value);
			cookie.Expires = DateTime.Now.AddMinutes(20);
			cookie.HttpOnly = false;
			Response.Cookies.Add(cookie);
		}
		private CookieCollection _cookies;
		public CookieCollection Cookies
		{
			get
			{
				if (_cookies == null)
				{
					_cookies = new CookieCollection(this);
				}
				return _cookies;
			}
		}
		//
		public void AddClientScript(string script, params object[] values)
		{
			int n = 0;
			if (serverResponse.Calls != null)
			{
				n = serverResponse.Calls.Length;
			}
			string[] ss = new string[n + 1];
			if (serverResponse.Calls != null)
			{
				serverResponse.Calls.CopyTo(ss, 0);
			}
			if (values != null && values.Length > 0)
			{
				ss[n] = string.Format(CultureInfo.InvariantCulture, script, values);
			}
			else
			{
				ss[n] = script;
			}
			serverResponse.Calls = ss;
		}
		protected virtual void OnRequestExecution(string method, string value)
		{
			if (DEBUG)
			{
				Response.Write("execute:");
				if (!string.IsNullOrEmpty(method))
				{
					Response.Write(method);
				}
				Response.Write(", value:");
				if (!string.IsNullOrEmpty(value))
				{
					Response.Write(value);
				}
				Response.Write("<br>");
			}
			try
			{
				System.Reflection.MethodInfo mif = this.GetType().GetMethod(method);
				if ((mif != null))
				{
					ParameterInfo[] pifs = mif.GetParameters();
					if (pifs == null || pifs.Length == 0)
					{
						mif.Invoke(this, new object[] { });
					}
					else
					{
						if (pifs.Length == 1)
						{
							if (pifs[0].ParameterType.Equals(typeof(string)))
							{
								mif.Invoke(this, new object[] { value });
							}
							else
							{
								TypeConverter converter = TypeDescriptor.GetConverter(pifs[0].ParameterType);
								if (converter != null)
								{
									object obj = converter.ConvertFromInvariantString(value);
									mif.Invoke(this, new object[] { obj });
								}
								else
								{
									//let it fail
									mif.Invoke(this, new object[] { value });
								}
							}
						}
						else
						{
							//let it fail
							mif.Invoke(this, new object[] { value });
						}
					}
				}
			}
			catch (Exception err)
			{
				if (!DEBUG) //if DEBUG is true then the info is already sent
				{
					Response.Write("execute:");
					if (!string.IsNullOrEmpty(method))
					{
						Response.Write(method);
					}
					Response.Write(", value:");
					if (!string.IsNullOrEmpty(value))
					{
						Response.Write(value);
					}
					Response.Write("<br>");
				}
				Response.Write("Error executing the method. Error message:");
				OutputException(err);
				Response.Write("<br>");
			}
		}
		protected void AddDownloadValue(string name, object value)
		{
			if (serverResponse.values == null)
			{
				serverResponse.values = new JsonDataStringDictionary(null);
			}
			if (serverResponse.values.ContainsKey(name))
			{
				serverResponse.values[name] = value;
			}
			else
			{
				serverResponse.values.Add(name, value);
			}
		}
		protected void AddUpdatedTableName(string name, string errormessage)
		{
			string vn = string.IsNullOrEmpty(errormessage) ? "updatedtables" : "updatefailedtables";
			if (serverResponse.values == null)
			{
				serverResponse.values = new JsonDataStringDictionary(null);
			}
			object v;
			string[] ns = new string[] { };
			NameValue[] vs = new NameValue[] { };
			if (!serverResponse.values.TryGetValue(vn, out v))
			{
				if (string.IsNullOrEmpty(errormessage))
				{
					ns = new string[] { };
					serverResponse.values.Add(vn, ns);
				}
				else
				{
					vs = new NameValue[] { };
					serverResponse.values.Add(vn, vs);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(errormessage))
				{
					ns = v as string[];
					if (ns == null)
					{
						ns = new string[] { };
						serverResponse.values[vn] = ns;
					}
				}
				else
				{
					vs = v as NameValue[];
					if (vs == null)
					{
						vs = new NameValue[] { };
						serverResponse.values[vn] = vs;
					}
				}
			}
			if (string.IsNullOrEmpty(errormessage))
			{
				string[] ns2 = new string[ns.Length + 1];
				ns.CopyTo(ns2, 0);
				ns2[ns.Length] = name;
				serverResponse.values[vn] = ns2;
			}
			else
			{
				NameValue[] vs2 = new NameValue[vs.Length + 1];
				vs.CopyTo(vs2, 0);
				vs2[ns.Length] = new NameValue(name, errormessage);
				serverResponse.values[vn] = vs2;
			}
		}
		protected virtual void Dispose(bool disposing) { }
		protected virtual IFileUploador CreateFileUploader(HttpPostedFile f) { return null; }
		protected virtual IList<IFileUploador> GetFileUploador()
		{
			return null;
		}
		public string AspxPhysicalFolder
		{
			get
			{
				return Server.MapPath(".");
			}
		}
		public string IPAddress
		{
			get
			{
				return Request.ServerVariables["remote_addr"];
			}
		}
		public void AddUploadedFilePathes(string[] names)
		{
			AddDownloadValue("SavedFiles", names);
		}
		//
		protected override void OnLoad(EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "text/plain";
			shouldSendResponse = true;
			serverResponse = new WebServerResponse();
			_fileUploaders = new List<IFileUploador>();
			string rawData = string.Empty;
			if (Request.Files != null && Request.Files.Count > 0)
			{
				rawData = Request.Form["clientRequest"];
				if (rawData == null)
				{
					Response.Write("clientRequest is null.");
					rawData = string.Empty;
				}
				IList<IFileUploador> lst0 = GetFileUploador();
				if (lst0 != null && lst0.Count > 0)
				{
					_fileUploaders.AddRange(lst0);
				}
				for (int i = 0; i < Request.Files.Count; i++)
				{
					if (i < _fileUploaders.Count)
					{
						_fileUploaders[i].SetPostedFile(Request.Files[i]);
					}
					else
					{
						IFileUploador fu = CreateFileUploader(Request.Files[i]);
						fu.SetAspxPage(this);
						_fileUploaders.Add(fu);
					}
				}
			}
			else
			{
				using (StreamReader sr = new StreamReader(Request.InputStream))
				{
					rawData = sr.ReadToEnd();
				}
			}
			if (rawData.StartsWith(DEBUG_SYMBOL, StringComparison.Ordinal))
			{
				DEBUG = true;
				rawData = rawData.Substring(DEBUG_SYMBOL.Length);
			}
			if (DEBUG)
			{
				Response.Write("client request:");
				Response.Write(rawData);
				Response.Write("<br />");
				if (Request.Files != null && Request.Files.Count > 0)
				{
					Response.Write("File uploaded:");
					Response.Write(Request.Files.Count);
					Response.Write("\r\n");
					if (_fileUploaders == null)
					{
						Response.Write("File uploador not found.\r\n");
					}
					else
					{
						Response.Write("File uploador found:");
						Response.Write(_fileUploaders.Count);
						Response.Write("\r\n");
					}
					for (int i = 0; i < Request.Files.Count; i++)
					{
						Response.Write(" File ");
						Response.Write(i);
						Response.Write(" File name:");
						Response.Write(Request.Files[i].FileName);
						Response.Write(" File size:");
						Response.Write(Request.Files[i].ContentLength);
						Response.Write("\r\n");
					}
				}
			}
			try
			{
				clientRequest = new WebClientRequest();
				clientRequest.FromJsonText(rawData);
				if (DEBUG)
				{
					Response.Write("Number of client commands:");
					Response.Write(clientRequest.GetNumberOfCalls());
					Response.Write("<br>");
				}
				OnRequestStart();
				if (!Stop)
				{
					if (clientRequest.Calls != null && clientRequest.Calls.Length > 0)
					{
						for (int i = 0; i < clientRequest.Calls.Length; i++)
						{
							try
							{
								if (string.CompareOrdinal(clientRequest.Calls[i].method, "jsonDb_getData") == 0)
								{
									if (DEBUG)
									{
										Response.Write("jsonDb_getData start:");
										if (!string.IsNullOrEmpty(clientRequest.Calls[i].value))
										{
											Response.Write(clientRequest.Calls[i].value);
										}
										Response.Write("<br>");
									}
									OnRequestGetData(clientRequest.Calls[i].value);
									if (DEBUG)
									{
										Response.Write("jsonDb_getData end:");
										if (!string.IsNullOrEmpty(clientRequest.Calls[i].value))
										{
											Response.Write(clientRequest.Calls[i].value);
										}
										Response.Write("<br>");
									}
								}
								else if (string.CompareOrdinal(clientRequest.Calls[i].method, "jsonDb_putData") == 0)
								{
									if (DEBUG)
									{
										Response.Write("jsonDb_putData start:");
										if (!string.IsNullOrEmpty(clientRequest.Calls[i].value))
										{
											Response.Write(clientRequest.Calls[i].value);
										}
										Response.Write("<br>");
									}
									OnRequestPutData(clientRequest.Calls[i].value);
									if (DEBUG)
									{
										Response.Write("jsonDb_putData end:");
										if (!string.IsNullOrEmpty(clientRequest.Calls[i].value))
										{
											Response.Write(clientRequest.Calls[i].value);
										}
										Response.Write("<br>");
									}
								}
								else
								{
									if (DEBUG)
									{
										Response.Write(" execute : ");

									}
									if (!string.IsNullOrEmpty(clientRequest.Calls[i].method))
									{
										if (DEBUG)
										{
											Response.Write(clientRequest.Calls[i].method);
											Response.Write("(");
											if (!string.IsNullOrEmpty(clientRequest.Calls[i].value))
											{
												Response.Write(clientRequest.Calls[i].value);
											}
											Response.Write(")<br> ");
										}
										OnRequestExecution(clientRequest.Calls[i].method, clientRequest.Calls[i].value);
									}
									if (DEBUG)
									{
										Response.Write("Finish executing the method. <br>");
									}
								}
							}
							catch (Exception err)
							{
								Response.Write(err + " - " + clientRequest.Calls[i].ToString() + "<br>");
							}
						}
					}
					OnRequestFinish();
				}
				if (shouldSendResponse)
				{
					if (DEBUG)
					{
						Response.Write(DEBUG_SYMBOL);
					}
					Response.Write(serverResponse.ToJsonText());
				}
			}
			catch (Exception err0)
			{
				OutputException(err0);
				Response.Write(DEBUG_SYMBOL);
				if (serverResponse != null)
				{
					Response.Write(serverResponse.ToJsonText());
				}
			}
		}

		protected void UpdateData(IDataTableUpdator db, string tableName)
		{
			if (string.Compare(db.TableName, tableName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return;
			}
			if (clientRequest.Data != null)
			{
				if (clientRequest.Data.Tables != null && clientRequest.Data.Tables.Length > 0)
				{
					for (int i = 0; i < clientRequest.Data.Tables.Length; i++)
					{
						if (!string.IsNullOrEmpty(db.TableName) && string.Compare(db.TableName, clientRequest.Data.Tables[i].TableName, StringComparison.OrdinalIgnoreCase) != 0)
						{
							continue;
						}
						if (clientRequest.Data.Tables[i].Rows == null || clientRequest.Data.Tables[i].Rows.Length == 0)
						{
							continue;
						}
						if (clientRequest.Data.Tables[i].Columns == null || clientRequest.Data.Tables[i].Columns.Length == 0)
						{
							continue;
						}
						bool[] colReadOnly = new bool[clientRequest.Data.Tables[i].Columns.Length];
						colReadOnly[0] = true;
						//
						bool hasUpdate = false;
						bool hasAdded = false;
						bool hasDeleted = false;
						int[] keyColumnIndex = null;
						for (int r = 0; r < clientRequest.Data.Tables[i].Rows.Length; r++)
						{
							JsonDataRowUpdate ru = clientRequest.Data.Tables[i].Rows[r] as JsonDataRowUpdate; ;
							if (!hasAdded)
							{
								if (ru.Added)
								{
									hasAdded = true;
								}
							}
							if (!hasDeleted)
							{
								if (ru.Deleted)
								{
									hasDeleted = true;
								}
							}
							if (!hasUpdate)
							{
								if (!ru.Added && !ru.Deleted)
								{
									hasUpdate = true;
								}
							}
							if (hasAdded && hasUpdate && hasDeleted)
							{
								break;
							}
						}
						if (hasUpdate || hasDeleted)
						{
							keyColumnIndex = new int[clientRequest.Data.Tables[i].PrimaryKey.Length];
							for (int k = 0; k < clientRequest.Data.Tables[i].PrimaryKey.Length; k++)
							{
								for (int c = 0; c < clientRequest.Data.Tables[i].Columns.Length; c++)
								{
									if (string.Compare(clientRequest.Data.Tables[i].PrimaryKey[k], clientRequest.Data.Tables[i].Columns[c].Name, StringComparison.OrdinalIgnoreCase) == 0)
									{
										keyColumnIndex[k] = c;
										break;
									}
								}
							}
						}
						if (hasUpdate || hasDeleted || hasAdded)
						{
							if (DEBUG)
							{
								Response.Write("Rows to process:");
								Response.Write(clientRequest.Data.Tables[i].Rows.Length);
								Response.Write("<br>");
							}
							int nUpdated = 0;
							int nAdded = 0;
							int nDeleted = 0;
							db.Open();
							try
							{
								//
								if (hasUpdate)
								{
									bool firstC = true;
									DbCommand cmdUpdate = db.CreateCommand();
									Dictionary<int, DbParameter> updateParameters = new Dictionary<int, DbParameter>();
									Dictionary<int, DbParameter> filterParameters = new Dictionary<int, DbParameter>();
									StringBuilder sbQry = new StringBuilder("UPDATE ");
									sbQry.Append(db.NameDelimitBegin);
									sbQry.Append(db.SourceTableName);
									sbQry.Append(db.NameDelimitEnd);
									sbQry.Append(" SET ");
									for (int c = 0; c < clientRequest.Data.Tables[i].Columns.Length; c++)
									{
										if (colReadOnly[c])
										{
										}
										else
										{
											if (firstC)
											{
												firstC = false;
											}
											else
											{
												sbQry.Append(", ");
											}
											sbQry.Append(db.NameDelimitBegin);
											sbQry.Append(clientRequest.Data.Tables[i].Columns[c].Name);
											sbQry.Append(db.NameDelimitEnd);
											sbQry.Append("=");
											DbParameter p = db.AddParameter(cmdUpdate, clientRequest.Data.Tables[i].Columns[c], c);
											sbQry.Append(p.ParameterName);
											updateParameters.Add(c, p);
										}
									}
									sbQry.Append(" WHERE ");
									for (int c = 0; c < clientRequest.Data.Tables[i].PrimaryKey.Length; c++)
									{
										if (c > 0)
										{
											sbQry.Append(" AND ");
										}
										sbQry.Append(db.NameDelimitBegin);
										sbQry.Append(clientRequest.Data.Tables[i].PrimaryKey[c]);
										sbQry.Append(db.NameDelimitEnd);
										sbQry.Append("=");
										DbParameter p = db.AddFilterParameter(cmdUpdate, clientRequest.Data.Tables[i].GetColumnByName(clientRequest.Data.Tables[i].PrimaryKey[c]), c);
										sbQry.Append(p.ParameterName);
										filterParameters.Add(c, p);
									}
									cmdUpdate.CommandText = sbQry.ToString();
									if (DEBUG)
									{
										Response.Write("Update query:");
										Response.Write(cmdUpdate.CommandText);
										Response.Write("<br>");
										foreach (KeyValuePair<int, DbParameter> kv in updateParameters)
										{
											Response.Write("Parameter name:");
											Response.Write(kv.Value.ParameterName);
											Response.Write(" Parameter type:");
											Response.Write(kv.Value.DbType.ToString());
											Response.Write("<br>");
										}
										foreach (KeyValuePair<int, DbParameter> kv in filterParameters)
										{
											Response.Write("Parameter name:");
											Response.Write(kv.Value.ParameterName);
											Response.Write(" Parameter type:");
											Response.Write(kv.Value.DbType.ToString());
											Response.Write("<br>");
										}
									}
									//
									for (int r = 0; r < clientRequest.Data.Tables[i].Rows.Length; r++)
									{
										JsonDataRowUpdate ru = clientRequest.Data.Tables[i].Rows[r] as JsonDataRowUpdate;
										if (!ru.Added && !ru.Deleted)
										{
											for (int c = 0; c < clientRequest.Data.Tables[i].Columns.Length; c++)
											{
												if (colReadOnly[c])
												{
												}
												else
												{
													updateParameters[c].Value = ru.ItemArray[c];
												}
											}
											for (int c = 0; c < clientRequest.Data.Tables[i].PrimaryKey.Length; c++)
											{
												filterParameters[c].Value = ru.ItemArray[keyColumnIndex[c]];
											}
											cmdUpdate.Parameters.Clear();
											foreach (KeyValuePair<int, DbParameter> kv in updateParameters)
											{
												cmdUpdate.Parameters.Add(kv.Value);
											}
											foreach (KeyValuePair<int, DbParameter> kv in filterParameters)
											{
												cmdUpdate.Parameters.Add(kv.Value);
											}
											cmdUpdate.ExecuteNonQuery();
											nUpdated++;
										}
									}
								}
								if (hasAdded)
								{
									bool firstC = true;
									Dictionary<int, DbParameter> updateParameters = new Dictionary<int, DbParameter>();
									DbCommand cmdInsert = db.CreateCommand();
									StringBuilder sbQry = new StringBuilder("INSERT INTO ");
									StringBuilder sbValues = new StringBuilder();
									sbQry.Append(db.NameDelimitBegin);
									sbQry.Append(db.SourceTableName);
									sbQry.Append(db.NameDelimitEnd);
									sbQry.Append(" (");
									for (int c = 0; c < clientRequest.Data.Tables[i].Columns.Length; c++)
									{
										if (colReadOnly[c])
										{
										}
										else
										{
											if (firstC)
											{
												firstC = false;
											}
											else
											{
												sbQry.Append(", ");
												sbValues.Append(",");
											}
											sbQry.Append(db.NameDelimitBegin);
											sbQry.Append(clientRequest.Data.Tables[i].Columns[c].Name);
											sbQry.Append(db.NameDelimitEnd);
											DbParameter p = db.AddParameter(cmdInsert, clientRequest.Data.Tables[i].Columns[c], c);
											sbValues.Append(p.ParameterName);
											updateParameters.Add(c, p);
										}
									}
									sbQry.Append(") VALUES (");
									sbQry.Append(sbValues.ToString());
									sbQry.Append(")");
									//
									cmdInsert.CommandText = sbQry.ToString();
									if (DEBUG)
									{
										Response.Write("Insert query:");
										Response.Write(cmdInsert.CommandText);
										Response.Write("<br>");
										foreach (KeyValuePair<int, DbParameter> kv in updateParameters)
										{
											Response.Write("Parameter name:");
											Response.Write(kv.Value.ParameterName);
											Response.Write(" Parameter type:");
											Response.Write(kv.Value.DbType.ToString());
											Response.Write("<br>");
										}
									}
									//
									for (int r = 0; r < clientRequest.Data.Tables[i].Rows.Length; r++)
									{
										JsonDataRowUpdate ru = clientRequest.Data.Tables[i].Rows[r] as JsonDataRowUpdate;
										if (ru.Added)
										{
											for (int c = 0; c < clientRequest.Data.Tables[i].Columns.Length; c++)
											{
												if (colReadOnly[c])
												{
												}
												else
												{
													updateParameters[c].Value = ru.ItemArray[c];
												}
											}
											cmdInsert.Parameters.Clear();
											foreach (KeyValuePair<int, DbParameter> kv in updateParameters)
											{
												cmdInsert.Parameters.Add(kv.Value);
											}
											cmdInsert.ExecuteNonQuery();
											nAdded++;
										}
									}
								}
								if (hasDeleted)
								{
									Dictionary<int, DbParameter> filterParameters = new Dictionary<int, DbParameter>();
									DbCommand cmdDelete = db.CreateCommand();
									StringBuilder sbQry = new StringBuilder("DELETE FROM ");
									sbQry.Append(db.NameDelimitBegin);
									sbQry.Append(db.SourceTableName);
									sbQry.Append(db.NameDelimitEnd);
									sbQry.Append(" WHERE ");
									for (int c = 0; c < clientRequest.Data.Tables[i].PrimaryKey.Length; c++)
									{
										if (c > 0)
										{
											sbQry.Append(" AND ");
										}
										sbQry.Append(db.NameDelimitBegin);
										sbQry.Append(clientRequest.Data.Tables[i].PrimaryKey[c]);
										sbQry.Append(db.NameDelimitEnd);
										sbQry.Append("=");
										DbParameter p = db.AddFilterParameter(cmdDelete, clientRequest.Data.Tables[i].GetColumnByName(clientRequest.Data.Tables[i].PrimaryKey[c]), c);
										sbQry.Append(p);
										filterParameters.Add(c, p);
									}
									cmdDelete.CommandText = sbQry.ToString();
									if (DEBUG)
									{
										Response.Write("Insert query:");
										Response.Write(cmdDelete.CommandText);
										Response.Write("<br>");
										foreach (KeyValuePair<int, DbParameter> kv in filterParameters)
										{
											Response.Write("Parameter name:");
											Response.Write(kv.Value.ParameterName);
											Response.Write(" Parameter type:");
											Response.Write(kv.Value.DbType.ToString());
											Response.Write("<br>");
										}
									}
									//
									for (int r = 0; r < clientRequest.Data.Tables[i].Rows.Length; r++)
									{
										JsonDataRowUpdate ru = clientRequest.Data.Tables[i].Rows[r] as JsonDataRowUpdate;
										if (ru.Deleted)
										{
											for (int c = 0; c < clientRequest.Data.Tables[i].PrimaryKey.Length; c++)
											{
												filterParameters[c].Value = ru.ItemArray[keyColumnIndex[c]];
											}
											cmdDelete.Parameters.Clear();
											foreach (KeyValuePair<int, DbParameter> kv in filterParameters)
											{
												cmdDelete.Parameters.Add(kv.Value);
											}
											cmdDelete.ExecuteNonQuery();
											nDeleted++;
										}
									}
								}
								db.SetSuccessMessage();
								AddUpdatedTableName(tableName, null);
							}
							catch(Exception er)
							{
								string errMsg = FormExceptionText(er);
								db.SetErrorMessage(errMsg);
								AddUpdatedTableName(tableName, errMsg);
								if (DEBUG)
								{
									Response.Write("UpdateData failed. Error message: ");
									Response.Write(errMsg);
									Response.Write("<br>");
								}
							}
							finally
							{
								db.Close();
								if (DEBUG)
								{
									Response.Write("Updated rows:");
									Response.Write(nUpdated);
									Response.Write("<br>");
									Response.Write("Added rows:");
									Response.Write(nAdded);
									Response.Write("<br>");
									Response.Write("Deleted rows:");
									Response.Write(nDeleted);
									Response.Write("<br>");
								}
							}
						}
					}
				}
			}
		}
		protected void SetData(DataSet ds)
		{
			if (ds.Tables.Count > 0)
			{
				try
				{
					JsonDataSet.SetDebugMode(DEBUG, Response);
					if (serverResponse.Data == null)
					{
						if (DEBUG)
						{
							Response.Write(" SetData <br>");
						}
						serverResponse.Data = new JsonDataSet(ds);
					}
					else
					{
						if (DEBUG)
						{
							Response.Write(" MergeDataSet <br>");
						}
						serverResponse.Data.MergeDataSet(new JsonDataSet(ds));
					}
				}
				catch (Exception err)
				{
					Response.Write("Error setting data<br>");
					OutputException(err);
				}
			}
		}
		public IList<IFileUploador> FileUploaderList
		{
			get
			{
				return _fileUploaders;
			}
		}
	}
	public class NameValue
	{
		public NameValue(string name, string val)
		{
			key = name;
			value = val;
		}
		public string key { get; set; }
		public string value { get; set; }
	}
}
