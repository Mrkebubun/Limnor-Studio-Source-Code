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
using VPL;

namespace LimnorDesigner.Action
{
	public sealed class WebBuilderUtil
	{
		public static EnumWebValueSources GetActionTypeFromSources(IList<ISourceValuePointer> values)
		{
			return GetActionTypeFromSources(values, EnumWebValueSources.Unknown);
		}
		public static EnumWebActionType GetWebActionType(EnumWebRunAt methodRunAt, EnumWebValueSources parametersSources)
		{
			if (methodRunAt == EnumWebRunAt.Client)
			{
				if (parametersSources == EnumWebValueSources.HasBothValues || parametersSources == EnumWebValueSources.HasServerValues)
				{
					return EnumWebActionType.Download;
				}
				return EnumWebActionType.Client;
			}
			else if (methodRunAt == EnumWebRunAt.Server)
			{
				if (parametersSources == EnumWebValueSources.HasBothValues || parametersSources == EnumWebValueSources.HasClientValues)
				{
					return EnumWebActionType.Upload;
				}
				return EnumWebActionType.Server;
			}
			else
			{
				if (parametersSources == EnumWebValueSources.HasClientValues)
				{
					return EnumWebActionType.Client;
				}
				else if (parametersSources == EnumWebValueSources.HasServerValues)
				{
					return EnumWebActionType.Server;
				}
			}
			return EnumWebActionType.Unknown;
		}
		public static EnumWebValueSources GetActionTypeFromSources(IList<ISourceValuePointer> values, EnumWebValueSources wat)
		{
			if (values != null && values.Count > 0)
			{
				if (wat != EnumWebValueSources.HasBothValues)
				{
					foreach (ISourceValuePointer p in values)
					{
						bool isClientValue = p.IsWebClientValue();
						bool isServerValue = p.IsWebServerValue();
						if (isClientValue && isServerValue)
						{
						}
						else
						{
							if (isClientValue)
							{
								if (wat == EnumWebValueSources.Unknown)
								{
									wat = EnumWebValueSources.HasClientValues;
								}
								else if (wat == EnumWebValueSources.HasServerValues)
								{
									wat = EnumWebValueSources.HasBothValues;
									break;
								}
							}
							else
							{
								if (wat == EnumWebValueSources.Unknown)
								{
									wat = EnumWebValueSources.HasServerValues;
								}
								else if (wat == EnumWebValueSources.HasClientValues)
								{
									wat = EnumWebValueSources.HasBothValues;
									break;
								}
							}
						}
					}
				}
			}
			return wat;
		}
	}
}
