using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWayPicker.Core.Helpers.Implementations
{
    public class MemberPreviousValueRepo
    {
        private static MemberPreviousValueRepo _instance;
        private static object _lock = new object();
        private Dictionary<string, string> _contentIdKeyMap;
        private MemberPreviousValueRepo()
        {
            _contentIdKeyMap = new Dictionary<string, string>();
        }

        public static MemberPreviousValueRepo Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lock)
                    {
                        if(_instance == null)
                        {
                            _instance = new MemberPreviousValueRepo();
                        }
                    }
                }
                return _instance;
            }
        }

        public void SaveValue(string keyContentId, string propertyAlias, string valueContentId)
        {
            string key = GenerateKey(keyContentId, propertyAlias);

            if(_contentIdKeyMap.ContainsKey(key))
            {
                _contentIdKeyMap[key] = valueContentId;
            }
            else
            {
                _contentIdKeyMap.Add(key, valueContentId);
            }
            
        }

        public string GetAndRemoveValue(string keyContentId, string propertyAlias)
        {
            string key = GenerateKey(keyContentId, propertyAlias);

            if(_contentIdKeyMap.ContainsKey(key))
            {
                string value = _contentIdKeyMap[key];
                _contentIdKeyMap.Remove(key);
                return value;
            }
            else
            {
                return "";
            }
        }


        private string GenerateKey(string keyContentId, string alias)
        {
            return $"{keyContentId}:{alias}";
        }
        
    }
}
