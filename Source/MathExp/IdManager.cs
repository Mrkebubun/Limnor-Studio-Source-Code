/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;

namespace MathExp
{
    class IdManager
    {
        List<int> generalIdList;
        public IdManager()
        {
            generalIdList = new List<int>();
        }
        public void LoadRegisteredIDs(List<int> ids)
        {
            generalIdList.AddRange(ids);
        }
        /// <summary>
        /// create comma delimited integer list
        /// </summary>
        /// <returns></returns>
        public string GetAllIDs()
        {
            System.Text.StringBuilder s = new StringBuilder();
            if (generalIdList.Count > 0)
            {
                s.Append(generalIdList[0].ToString());
                for (int i = 1; i < generalIdList.Count; i++)
                {
                    s.Append(",");
                    s.Append(generalIdList[i].ToString());
                }
            }
            return s.ToString();
        }
        public void RegisterGlobalId(int id)
        {
            if (!generalIdList.Contains(id))
            {
                generalIdList.Add(id);
            }
        }
        public int CreateNewGlobalId()
        {
            lock (generalIdList)
            {
                int id = 1;
                while (generalIdList.Contains(id))
                {
                    id++;
                }
                generalIdList.Add(id);
                return id;
            }
        }
    }
}
