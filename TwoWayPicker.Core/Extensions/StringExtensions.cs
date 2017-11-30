using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoWayPicker.Core.Extensions
{
    public static class StringExtensions
    {
        public static List<int> ToIntList(this string commaSepString)
        {
            if (String.IsNullOrEmpty(commaSepString))
            {
                //return null;
                return new List<int>();
            }
            else
            {
                try
                {
                    List<int> l = commaSepString.Split(',').Select(int.Parse).ToList();
                    return l;
                }
                catch
                {
                    return new List<int>();
                }

            }
        }
    }
}