// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class InferredTypeSymbol : TypeSymbol, IInferredTypeSymbol
    {
        private readonly Symbols.SourceUnboundTypeArgumentSymbol _underlying;

        public InferredTypeSymbol(Symbols.SourceUnboundTypeArgumentSymbol underlying, CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            Debug.Assert(underlying is object);
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            Debug.Assert(nullableAnnotation != _underlying.DefaultNullableAnnotation);
            Debug.Assert(nullableAnnotation != this.NullableAnnotation);
            return new InferredTypeSymbol(_underlying, nullableAnnotation);
        }
        internal override Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal override CSharp.Symbol UnderlyingSymbol => _underlying;

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitInferredType(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitInferredType(this);
        }

        protected override TResult Accept<TArgument, TResult>(SymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitInferredType(this, argument);
        }
    }
}
