// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class SourceUnboundTypeArgumentSymbol : TypeSymbol
    {
        private readonly ImmutableArray<SyntaxReference> _syntaxRefs;
        private readonly ImmutableArray<Location> _locations;

        public SourceUnboundTypeArgumentSymbol(ImmutableArray<SyntaxReference> syntaxRefs, ImmutableArray<Location> locations)
        {
            _syntaxRefs = syntaxRefs;
            _locations = locations;
        }

        public override bool IsReferenceType => false;

        public override bool IsValueType => false;

        public override TypeKind TypeKind => TypeKind.Unknown;

        public override bool IsRefLikeType => false;

        public override bool IsReadOnly => false;

        public override SymbolKind Kind => SymbolKind.InferredType;

        public override Symbol ContainingSymbol => throw new NotImplementedException();

        public override ImmutableArray<Location> Locations => _locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _syntaxRefs;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => null;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Symbol> GetMembers() => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers(string name) => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;

        protected override ISymbol CreateISymbol()
        {
            return new InferredTypeSymbol(this, CodeAnalysis.NullableAnnotation.None);
        }

        protected override ITypeSymbol CreateITypeSymbol(CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            throw new NotImplementedException();
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument a)
        {
            throw new NotImplementedException();
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            result = this;
            return true;
        }

        internal override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Managed;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return false;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null) =>ImmutableArray<NamedTypeSymbol>.Empty;

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            Debug.Assert(this.Equals(other, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes));
            return this;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            return this;
        }

        internal override IEnumerable<(MethodSymbol Body, MethodSymbol Implemented)> SynthesizedInterfaceMethodImpls()
        {
            return SpecializedCollections.EmptyEnumerable<(MethodSymbol Body, MethodSymbol Implemented)>();
        }
    }
}
