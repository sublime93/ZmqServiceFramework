using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HashLib;

namespace Common
{
    public static class SignatureGenerator
    {
        public static Dictionary<string, MethodInfo> GetSignatureList(Type type)
        {
            var dict = new Dictionary<string, MethodInfo>();

            foreach (var m in type.GetMethods())
            {
                var hashString = m.GetParameters().Aggregate(m.Name, (current, p) => current + (p.Name + p.ParameterType.Name));

                dict.Add(HashFactory.Hash64.CreateMurmur2().ComputeString(hashString, Encoding.ASCII).ToString(), m);
            }

            return dict;
        } 
    }
}
