using System;
using System.Collections.Generic;
using System.Text;

namespace VSPrj
{
    /// <summary>
    /// mapping an object to its project. 
    /// it is a solution wide mapping
    /// </summary>
    public sealed class ObjectProjectMap:Dictionary<object,string>
    {
        private static ObjectProjectMap map = new ObjectProjectMap();
        private ObjectProjectMap()
        {
        }
        public static ObjectProjectMap Map
        {
            get
            {
                return map;
            }
        }
        public static void RemoveObject(object obj)
        {
            if (map.ContainsKey(obj))
            {
                map.Remove(obj);
            }
        }
        public static void RemoveObjectFromProject(string prj)
        {
            List<object> objs = new List<object>();
            foreach (KeyValuePair<object, string> kv in map)
            {
                if (kv.Value == prj)
                {
                    objs.Add(kv.Key);
                }
            }
            foreach (object k in objs)
            {
                map.Remove(k);
            }
        }
        public static void AddObject(object obj, string prj)
        {
            if (!map.ContainsKey(obj))
            {
                map.Add(obj, prj);
            }
        }
        public static string GetProjectFile(object obj)
        {
            string s;
            if (map.TryGetValue(obj, out s))
            {
                return s;
            }
            return null;
        }
        public static LimnorProject GetProject(object obj)
        {
            string s;
            if (map.TryGetValue(obj, out s))
            {
                return LimnorSolution.GetProjectByComponentFile(s);
            }
            return null;
        }
    }
}
