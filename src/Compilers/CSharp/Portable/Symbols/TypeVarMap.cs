// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Symbols.Source;
using Roslyn.Utilities;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal class TypeVarMap : AbstractTypeMap
    {
        protected readonly SmallDictionary<TypeSymbol, TypeWithAnnotations> Mapping;

        private TypeVarMap(SmallDictionary<TypeSymbol, TypeWithAnnotations> mapping)
        {
            Mapping = mapping;
        }

        internal TypeVarMap() : this(new SmallDictionary<TypeSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance)) { }

        internal void Add(TypeSymbol key, TypeWithAnnotations value)
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

        protected override TypeWithAnnotations SubstituteInferredTypeArgument(SourceInferredTypeArgumentSymbol typeArgument)
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
