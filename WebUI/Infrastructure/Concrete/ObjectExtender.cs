using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using WebUI.Infrastructure.Abstract;

namespace WebUI.Infrastructure.Concrete
{
    public class ObjectExtender : IObjectExtender
    {
        private readonly IDictionary<Tuple<Type, Type>, Assembly>
            _cache = new Dictionary<Tuple<Type, Type>, Assembly>();

        public object Extend(object obj1, object obj2)
        {
            if (obj1 == null) return obj2;
            if (obj2 == null) return obj1;

            var obj1Type = obj1.GetType();
            var obj2Type = obj2.GetType();

            var values = obj1Type.GetProperties()
                .ToDictionary(
                    property => property.Name,
                    property => property.GetValue(obj1, null));

            foreach (var property in obj2Type.GetProperties()
                .Where(pi => !values.ContainsKey(pi.Name)))
                values.Add(property.Name, property.GetValue(obj2, null));

            // check for cached
            var key = Tuple.Create(obj1Type, obj2Type);
            if (!_cache.ContainsKey(key))
            {
                // create assembly containing merged type
                var codeProvider = new CSharpCodeProvider();
                var code = new StringBuilder();

                code.Append("public class mergedType{ \n");
                foreach (var propertyName in values.Keys)
                {
                    // use object for property type, avoids assembly references
                    code.Append(
                        string.Format(
                            "public object @{0}{{ get; set;}}\n",
                            propertyName));
                }
                code.Append("}");

                var compilerResults = codeProvider.CompileAssemblyFromSource(
                    new CompilerParameters
                    {
                        CompilerOptions = "/optimize /t:library",
                        GenerateInMemory = true
                    },
                    code.ToString());

                _cache.Add(key, compilerResults.CompiledAssembly);
            }

            var merged = _cache[key].CreateInstance("mergedType");
            Debug.Assert(merged != null, "merged != null");

            // copy data
            foreach (var propertyInfo in merged.GetType().GetProperties())
            {
                propertyInfo.SetValue(
                    merged,
                    values[propertyInfo.Name],
                    null);
            }

            return merged;
        }
    }
}