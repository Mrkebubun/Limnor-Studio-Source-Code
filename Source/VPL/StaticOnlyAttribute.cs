/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class ReflectionMemberInfoSelectScopeAttribute : Attribute
	{
		private EnumReflectionMemberInfoSelectScope _scope = EnumReflectionMemberInfoSelectScope.InstanceOnly;
		public ReflectionMemberInfoSelectScopeAttribute()
		{
		}
		public ReflectionMemberInfoSelectScopeAttribute(EnumReflectionMemberInfoSelectScope scope)
		{
			_scope = scope;
		}
		public EnumReflectionMemberInfoSelectScope Scope
		{
			get
			{
				return _scope;
			}
		}
	}
	public class ReflectionMemberInfoSelectIncludeSpecialAttribute : Attribute
	{
		private static ReflectionMemberInfoSelectIncludeSpecialAttribute _yes;
		public ReflectionMemberInfoSelectIncludeSpecialAttribute()
		{
		}
		public static ReflectionMemberInfoSelectIncludeSpecialAttribute Yes
		{
			get
			{
				if (_yes == null)
				{
					_yes = new ReflectionMemberInfoSelectIncludeSpecialAttribute();
				}
				return _yes;
			}
		}
	}
}
