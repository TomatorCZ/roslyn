// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Roslyn.Utilities;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal class TypeVariableMap : AbstractTypeMap
    {
        protected readonly SmallDictionary<ITypeVariableInternal, TypeWithAnnotations> Mapping;

        private TypeVariableMap(SmallDictionary<ITypeVariableInternal, TypeWithAnnotations> mapping)
        {
            Mapping = mapping;
        }

        internal TypeVariableMap() : this(new SmallDictionary<ITypeVariableInternal, TypeWithAnnotations>(ReferenceEqualityComparer.Instance)) { }

        internal void Add(ITypeVariableInternal key, TypeWithAnnotations value) 
        {
            Mapping.Add(key, value);
        }

        protected sealed override TypeWithAnnotations SubstituteTypeParameter(TypeParameterSymbol typeParameter)
        {
            TypeWithAnnotations result;
            if (Mapping.TryGetValue(typeParameter, out result))
            {
                return result;
            }

            return TypeWithAnnotations.Create(typeParameter);
        }

        protected override TypeWithAnnotations SubstituteInferredTypeArgument(SourceUnboundTypeArgumentSymbol typeArgument)
        {
            TypeWithAnnotations result;
            if (Mapping.TryGetValue(typeArgument, out result))
            {
                return result;
            }

            return TypeWithAnnotations.Create(typeArgument);
        }

        private string GetDebuggerDisplay()
        {
            var result = new StringBuilder("[");
            result.Append(this.GetType().Name);
            foreach (var kv in Mapping)
            {
                result.Append(" ").Append(kv.Key).Append(":").Append(kv.Value.Type);
            }

            return result.Append("]").ToString();
        }
    }
}
