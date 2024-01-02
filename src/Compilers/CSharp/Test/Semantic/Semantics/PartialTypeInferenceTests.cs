// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class PartialTypeInferenceTests : CompilingTestBase
    {
        #region Helpers
        [Flags]
        internal enum Symbols
        {
            Methods = 1,
            ObjectCreation = 2
        }

        internal static SymbolDisplayFormat TestCallSiteDisplayStringFormat = SymbolDisplayFormat.TestFormat
            .WithParameterOptions(SymbolDisplayFormat.TestFormat.ParameterOptions & ~SymbolDisplayParameterOptions.IncludeName)
            .WithMemberOptions(SymbolDisplayFormat.TestFormat.MemberOptions & ~SymbolDisplayMemberOptions.IncludeType)
            .WithMiscellaneousOptions(SymbolDisplayFormat.TestFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        internal static void TestCallSites(string source, Symbols symbolsToCheck, ImmutableArray<DiagnosticDescription> expectedDiagnostics)
        {
            var compilationOptions = TestOptions.RegularPreview
                .WithFeature(nameof(MessageID.IDS_FeaturePartialMethodTypeInference))
                .WithFeature(nameof(MessageID.IDS_FeaturePartialConstructorTypeInference));

            var compilation = CreateCompilation(source, parseOptions: compilationOptions);

            //Verify errors
            compilation.VerifyDiagnostics(expectedDiagnostics.ToArray());

            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            //Verify symbols
            var results = string.Join("\n", model.SyntaxTree.GetRoot().DescendantNodesAndSelf().Where(node =>
                    (node is InvocationExpressionSyntax invocation && symbolsToCheck.HasFlag(Symbols.Methods))
                    || (node is ObjectCreationExpressionSyntax && symbolsToCheck.HasFlag(Symbols.ObjectCreation))
                )
                .Select(node => model.GetSymbolInfo(node).Symbol)
                .Where(symbol => symbol != null)
                .Select(symbol => symbol.ToDisplayString(TestCallSiteDisplayStringFormat))
                .ToArray()
            );

            var expected = string.Join("\n", source
                .Split(new[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Contains("//-"))
                .Select(x => x.Substring(x.IndexOf("//-", StringComparison.Ordinal) + 3))
                .ToArray());

            AssertEx.EqualOrDiff(expected, results);
        }
        internal static void TestCallSites(string source, Symbols symbolsToCheck) => TestCallSites(source, symbolsToCheck, ImmutableArray<DiagnosticDescription>.Empty);
        #endregion

        #region PartialMethodTypeInference
        [Fact]
        public void PartialMethodTypeInference_UnderscoreClass()
        {
            TestCallSites("""
class P
{
    static void M() 
    {
        F1<_>(1); //-P.F1<int>(int)
    }

    static void F1<T>(T p) {}
}

class _ {}
""",
                Symbols.Methods,
                ImmutableArray.Create(
                    // (11,7): warning CS9214: Types and aliases should not be named '_'.
                    // class _ {}
                    Diagnostic(ErrorCode.WRN_UnderscoreNamedDisallowed, "_").WithLocation(11, 7)
                )
            );
        }

        [Fact]
        public void PartialMethodTypeInference_Syntax()
        {
            TestCallSites("""
using System;

namespace X;
#nullable enable

class P
{
    static void M() 
    {
        A temp1 = new A();
        F<_>(temp1); //-X.P.F<X.P.A>(X.P.A)
        P.F<_>(temp1); //-X.P.F<X.P.A>(X.P.A)
        global::X.P.F<_>(temp1); //-X.P.F<X.P.A>(X.P.A)

        A? temp2 = null;
        F<_?>(temp2); //-X.P.F<X.P.A?>(X.P.A?)

        A<A?>? temp3 = null;
        F<A<_?>?>(temp3); //-X.P.F<X.P.A<X.P.A?>?>(X.P.A<X.P.A?>?)

        A.B<A> temp4 = new A.B<A>();
        F<global::X.P.A.B<_>>(temp4); //-X.P.F<X.P.A.B<X.P.A>>(X.P.A.B<X.P.A>)
        F<A.B<_>>(temp4); //-X.P.F<X.P.A.B<X.P.A>>(X.P.A.B<X.P.A>)

        A[] temp5 = new A[1];
        F<_[]>(temp5); //-X.P.F<X.P.A[]>(X.P.A[])

        A<A>[] temp6 = new A<A>[1];
        F<A<_>[]>(temp6); //-X.P.F<X.P.A<X.P.A>[]>(X.P.A<X.P.A>[])

        var temp7 = (1, 1);
        F<(_, _)>(temp7); //-X.P.F<(int, int)>((int, int))

        (new B())
        .F<_>(1) //-X.P.B.F<int>(int)
        .F<_>(1) //-X.P.B.F<int>(int)
        .F<_>(1); //-X.P.B.F<int>(int)

        A<_>.F<_>(temp1); //-X.P.A<_>.F<X.P.A>(X.P.A)
    }

    static void F<T>(T p) {}

    class A 
    {
        public class B<T> {}
    }
    class A<T1> 
    {
        public static void F<T2>(T2 p1) {}
    }
    class B 
    {
        public B F<T>(T p) { throw new NotImplementedException(); }  
    }
}
""",
                Symbols.Methods,
                ImmutableArray.Create(
                    // (39,11): error CS0246: The type or namespace name '_' could not be found (are you missing a using directive or an assembly reference?)
                    //         A<_>.F<_>(temp1);
                    Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "_").WithArguments("_").WithLocation(39, 11)
                )
            );
        }

        [Fact]
        public void PartialMethodTypeInference_Simple()
        {
            TestCallSites("""
using System;

public class P
{
    public void M() 
    {
        F1<_, string>(1); //-P.F1<int, string>(int)
        F2<_,_>(1,""); //-P.F2<int, string>(int, string)
        F3<int, _, string, _>(new G2<string, string>()); //-P.F3<int, string, string, string>(P.G2<string, string>)
        F4<_, _, string>(x => x + 1, y => y.ToString(),z => z.Length); //-P.F4<int, int, string>(System.Func<int, int>, System.Func<int, string>, System.Func<string, int>)
                                                                       //-int.ToString()
        F5<string>(1); //-P.F5<string>(int, params string[])
        F5<_>(1, ""); //-P.F5<string>(int, params string[])
        F5<_>(1, "", ""); //-P.F5<string>(int, params string[])
    }
    void F1<T1, T2>(T1 p1) {}
    void F2<T1, T2>(T1 p1, T2 p2) {}
    void F2<T1>(T1 p1, string p2) {}
    void F3<T1, T2, T3, T4>(G2<T2, T4> p24) {}
    class G2<T1, T2> {}
    void F4<T1, T2, T3>(Func<T1, T2> p12, Func<T2, T3> p23, Func<T3, T1> p31) { }
    void F5<T>(int p1, params T[] args) {}
}
""",
        Symbols.Methods
    );
        }

        [Fact]
        public void PartialMethodTypeInference_Nested()
        {
            TestCallSites("""
class P
{
    void M() 
    {
        B1<int> temp1 = null;
        F6<A1<_>>(temp1); //-P.F6<P.A1<int>>(P.A1<int>)

        B2<int, string> temp2 = null;
        F6<A2<_, string>>(temp2); //-P.F6<P.A2<int, string>>(P.A2<int, string>)

        C2<int, B> temp3 = null;
        F6<I2<_, A>>(temp3); //-P.F6<P.I2<int, P.A>>(P.I2<int, P.A>)
    }   

    void F6<T1>(T1 p1) {}

    class A {}
    class B : A{}
    class A1<T> {}
    class B1<T> : A1<T> {}
    class A2<T1, T2> {}
    class B2<T1, T2> : A2<T1, T2> {}
    interface I2<in T1, out T2> {}
    class C2<T1, T2> : I2<T1, T2> {}
} 
""",
        Symbols.Methods
    );
        }

        [Fact]
        public void PartialMethodTypeInference_Dynamic()
        {
            TestCallSites("""
class P {
    void M1() 
    {
        dynamic temp4 = "";

        // Warning: Inferred type argument is not supported by runtime (type hints will not be used at all)
        temp4.M<_>();

        // Warning: Inferred type argument is not supported by runtime (type hints will not be used at all)
        F7<string, _>("", temp4, 1); //-P.F7<T1, T2>(T1, T2, T1)
        
        // Warning: Inferred type argument is not supported by runtime (type hints will not be used at all)
        F7<_, string>(1, temp4, 1); //-P.F7<T1, T2>(T1, T2, T1)
        
        // Warning: Inferred type argument is not supported by runtime (type hints will not be used at all)
        temp4.F7<string, _>(temp4);  
    }

    void F7<T1, T2>(T1 p1, T2 p2, T1 p3) {}
}
""",
        Symbols.Methods,
        ImmutableArray.Create(
                // (7,9): warning CS9212: Type hints will not be considered in type inference of dynamic call.
                //         temp4.M<_>();
                Diagnostic(ErrorCode.WRN_TypeHintsInDynamicCall, "temp4.M<_>()").WithLocation(7, 9),
                // (10,9): warning CS9212: Type hints will not be considered in type inference of dynamic call.
                //         F7<string, _>("", temp4, 1);
                Diagnostic(ErrorCode.WRN_TypeHintsInDynamicCall, @"F7<string, _>("""", temp4, 1)").WithLocation(10, 9),
                // (13,9): warning CS9212: Type hints will not be considered in type inference of dynamic call.
                //         F7<_, string>(1, temp4, 1); 
                Diagnostic(ErrorCode.WRN_TypeHintsInDynamicCall, "F7<_, string>(1, temp4, 1)").WithLocation(13, 9),
                // (16,9): warning CS9212: Type hints will not be considered in type inference of dynamic call.
                //         temp4.F7<string, _>(temp4);  
                Diagnostic(ErrorCode.WRN_TypeHintsInDynamicCall, "temp4.F7<string, _>(temp4)").WithLocation(16, 9)
        )
    );
        }

        [Fact]
        public void PartialMethodTypeInference_ErrorRecovery()
        {
            TestCallSites("""
class P {
    void M1() 
    {
        F1<_,_>(""); // Error: Can't infer T2
        F1<int, string>(""); // Error: int != string
        F1<byte,_>(257); // Error: Can't infer T2
    }

    void F1<T1, T2>(T1 p1) {}
}
""",
        Symbols.Methods,
        ImmutableArray.Create(
            // (4,9): error CS0411: The type arguments for method 'P.F1<T1, T2>(T1)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
            //         F1<_,_>(""); // Error: Can't infer T2
            Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "F1<_,_>").WithArguments("P.F1<T1, T2>(T1)").WithLocation(4, 9),
            // (5,25): error CS1503: Argument 1: cannot convert from 'string' to 'int'
            //         F1<int, string>(""); // Error: int != string
            Diagnostic(ErrorCode.ERR_BadArgType, @"""""").WithArguments("1", "string", "int").WithLocation(5, 25),
            // (6,9): error CS0411: The type arguments for method 'P.F1<T1, T2>(T1)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
            //         F1<byte,_>(257); // Error: Can't infer T2
            Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "F1<byte,_>").WithArguments("P.F1<T1, T2>(T1)").WithLocation(6, 9)
        )
    );
        }

        [Fact]
        public void PartialMethodTypeInference_Nullable()
        {
            TestCallSites("""
#nullable enable
class P {
    void M1() 
    {
        string? temp5 = null;
        string? temp5a = null;
        string? temp5b = null;
        string temp6 = "";
        C2<int, string> temp7 = new C2<int, string>();
        C2<int, string?> temp8 = new C2<int, string?>();
        C2<string?, int> temp9 = new C2<string?, int>();

        // Inferred: [T1 = int, T2 = string!]
        F8<int, _>(temp5);  
        // Inferred: [T1 = int, T2 = string!] 
        F8<int, _>(temp6);
        // Inferred: [T1 = int?, T2 = string!] 
        F8<int?, _>(temp5); 
        // Inferred: [T1 = int?, T2 = string!]
        F8<int?, _>(temp6);  
        // Error: _ is non nullable
        F9<int, _>(temp5a);
        // Inferred: [T1 = int, T2 = string!]
        F9<int, _>(temp6);
        // Error: _ is non nullable
        F9<int?, _>(temp5b);
        // Inferred: [T1 = int?, T2 = string!]
        F9<int?, _>(temp6);  
        
        //Inferred: [T1 = I2<int, string?>!] Can convert string to string? because of covariance
        F10<I2<_, string?>>(temp7);
        //Error: Can't convert string? to string because of invariance
        F10<C2<_, string?>>(temp7); 
        //Inferred: [T1 = I2<System.Int32, System.String!>!]
        F10<I2<_, _>>(temp7);
        //Inferred: [T1 = C2<System.Int32, System.String!>!]
        F10<C2<_, _>>(temp7);
        //Inferred: [T1 = I2<System.Int32, System.String?>!]
        F10<I2<_, _?>>(temp8);
        //Inferred: [T1 = C2<System.Int32, System.String?>!]
        F10<C2<_, _?>>(temp8);
        //Error: Can't convert string? to string because of covariance
        F10<I2<_, string>>(temp8);
        //Error: Can't convert string? to string because of invariance
        F10<C2<_, string>>(temp8);
        //Inferred: [T1 = I2<System.String!, System.Int32>!] Can convert string to string? because of contravariance
        F10<I2<_, int>>(temp9); 
        
        //Inferred: [T1 = string?]
        F10<_?>("maybe null");
        //Inferred: [T1 = I2<string?, int>!]
        F10<I2<_, _?>>(temp7);
        //Inferred: [T1 = System.Int32] in order to be coherent with the current inference.
        F10<_?>(1);
        //Error: Can't be inferred because void F12<T>(Nullable<T> p ) {} and F(1) is not inferred either.
        F10<Nullable<_>>(1);
    }

    interface I2<in T1, out T2> {}
    class C2<T1, T2> : I2<T1, T2> {}

    void F8<T1, T2>(T2? p2) { }
    void F9<T1, T2>(T2 p2) { }
    void F10<T1>(T1 p1) {}
}
""",
        Symbols.Methods
    );

            //TODO: Warnings and errors
            //TODO: Signatures
        }
        #endregion

        #region PartialConstructorTypeInference
        [Fact]
        public void PartialConstructorTypeInference_UnderscoreClass()
        {
            TestCallSites("""
class P
{
    static void M() 
    {
        new F1<_>(null); //-P.F1<_>..ctor(_)
    }

    class F1<T> { public F1(T p) {} }
}

class _ {}
""",
                Symbols.ObjectCreation
            );

            //TODO: Warning _ usage
        }

        [Fact]
        public void PartialConstructorTypeInference_Syntax()
        {
            TestCallSites("""
namespace X;
#nullable enable

class P
{
    static void M() 
    {
        A temp1 = new A();
        new F<_>(temp1);
        new P.F<_>(temp1);
        new global::X.P.F<_>(temp1);

        A? temp2 = null;
        new F<_?>(temp2);

        A<A?>? temp3 = null;
        new F<A<_?>?>(temp3);

        A.B<A> temp4 = new A.B<A>();
        new F<global::X.P.A.B<_>>(temp4);
        new F<A.B<_>>(temp4);

        A[] temp5 = new A[1];
        new F<_[]>(temp5);

        A<A>[] temp6 = new A<A>[1];
        new F<A<_>[]>(temp6);

        var temp7 = (1, 1);
        new F<(_, _)>(temp7);

        new A<_>.F<_>(temp1); // Error
        new _(); // Error
        var temp8 = new Del<_>(Foo); //Error
    }

    class F<T> { public F(T p) {} }
    class A 
    {
        public class B<T> {}
    }
    class A<T1> 
    {
        public class F<T2>{ public F(T2 p1) {} }
    }

    delegate T Del<T>(T p1);
    int Foo(int p) {return p;}
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Errors
            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Simple()
        {
            TestCallSites("""
namespace X;

public class P
{
    public void M2() 
    {
        // Inferred: [T1 = int, T2 = string] Simple test
        new F1<_, string>(1); 
        // Inferred: [T1 = int, T2 = string] Choose overload based on arity
        new F2<_,_>(1,""); 
        // Inferred: [T1 = int, T2 = string, T3 = string, T4 = string] Constructed type
        new F3<int, _, string, _>(new G2<string, string>()); 
        // Inferred: [T1 = int, T2 = int, T3 = string] Circle of dependency
        new F4<_, _, string>(x => x + 1, y => y.ToString(),z => z.Length); 
        // Inferred: [T1 = string] Expanded form #1
        new F5<string>(1); 
        // Inferred: [T1 = string] Expanded form #2
        new F5<_>(1, ""); 
        // Inferred: [T1 = string] Expanded form #3
        new F5<_>(1, "", "");
    }

    class F1<T1, T2>{ public F1(T1 p1){} }
    class F2<T1, T2>{ public F2(T1 p1, T2 p2) {} }
    class F2<T1>{ public F2(T1 p1, string p2) {} }
    class F3<T1, T2, T3, T4>{ public F3(G2<T2, T4> p24) {} }
    class G2<T1, T2> {}
    class F4<T1, T2, T3>{ public F4(Func<T1, T2> p12, Func<T2, T3> p23, Func<T3, T1> p31) { } }
    class F5<T>{ public F5(int p1, params T[] args) {} }
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Nested()
        {
            TestCallSites("""
class P
{
    void M1() 
    {
        B1<int> temp1 = null;
        // Inferred: [ T1 = A1<int> ] Wrapper conversion
        new F6<A1<_>>(temp1); 

        B2<int, string> temp2 = null;
        // Inferred: [ T1 = A2<int, string> ] Wrapper conversion with type argument
        new F6<A2<_, string>>(temp2); 

        C2<int, B> temp3 = null;
        // Inferred: [ I2<int, A> ] Wrapper conversion with type argument conversion
        new F6<I2<_, A>>(temp3); 
    }   

    class F6<T1>
    { 
        public F6(T1 p1) {}
    }

    class A {}
    class B : A{}
    class A1<T> {}
    class B1<T> : A1<T> {}
    class A2<T1, T2> {}
    class B2<T1, T2> : A2<T1, T2> {}
    interface I2<in T1, out T2> {}
    class C2<T1, T2> : I2<T1, T2> {}
}      
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Target()
        {
            TestCallSites("""
using System.Collections.Generic;

class P 
{
    void Test_VariableDeclaration() 
    {
        // Inferred: [T = int]
        C1<int> temp1 = new C2<_>();
    }
    
    void Test_ClassObjectCreation()
    {
        // Inferred: [T = int]
        new C4(new C2<_>());

        // Inferred: [T = int]
        new C5<_>(new C5<_>(1));

        // Inferred: [T = int]  
        new C3<_>(new C2<_>(), 1);
    }

    class C3<T>
    {
        public C3(C1<T> p1, T p2) {}
    }
    class C4 
    {
        public C4(C1<int> p1) {}
    }

    void Test_InvocationExpression()
    {
        // Inferred: [T = int]
        F2(new C2<_>());

        // Inferred: [T = int]
        F3(new C5<_>(1));

        // Inferred: [T = int]
        F1(new C2<_>(), 1);
    }

    class C5<T> : C1<T>
    {
        public C5(T p1) {}
    }

    void F1<T>(C1<T> p1, T p2) {}
    void F2(C1<int> p1) {}
    void F3<T>(T p1) {}

    void Test_ArrayInitializer()
    {
        // Inferred: [T = int]
        var temp1 = new C1<int>[] { new C2<_>() };

        // Inferred: [T = int]
        var temp2 = new [] { new C1<int>(), new C2<_>() };
    }

    void Test_ObjectInitializer()
    {
        // Inferred: [T = int]
        new M4 { P1 = new C2<_>()};
    }

    class M4 
    {
        public C1<int> P1 = null;
    }

    void Test_CollectionInitializer()
    {
        // Inferred: [T = int]
        new List<C1<int>> { new C2<_>() };
    }

    void Test_Lambda()
    {
        // Inferred: [T = int]
        Func<C1<int>> temp2 = () => new C2<_>();
    }

    void Test_Assignment() 
    {
        // Inferred: [T = int]
        C1<int> temp3 = null;
        temp3 = new C2<_>();
    }
    
    
    C1<int> Test_Return1() {
        // Inferred: [T = int]
        return new C2<_>();
    }

    //Inferred: [T = int]
    C1<int> Test_Return2() => new C2<_>();

    // Inferred: [T = int]
    C1<int> Test_FieldInitializer = new C2<_>();

    class C1<T> {}
    class C2<T> : C1<T> {}
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Constraints()
        {
            TestCallSites("""
class Program
{
    void M() 
    {
        // Inferred: [T1 = int, T2 = int, T3 = int, T4 = int, T5 = C1<int>]
        // Combinining type constraints from target, constructor, type argument list, object initializer and where clause to determine type of parameters
        F1(new C9<_,_,_,int,_>(1) {Prop1 = 1},1);
    }

    void F1<T>(C1<T> p1, T p2) {}

    class C1<T> {}

    class C9<T1, T2, T3, T4, T5> : C1<T3> where T5 : C1<T4>
    {
        public C9(T1 p1) {}
        public T2 Prop1 {get;set;}
    }
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Complex()
        {
            TestCallSites("""
class Program 
{
    public void M1() 
    {
        // Inferred: [T1 = C1<int>, T2 = int]
        // Using constraints to determine type parameters
        new C4<_,_>(1);

        // Inferred: [T1 = C7, T2 = C7]
        new C5<_,_>(new C7());
    }
}

class C1<T> {}
class C4<T1, T2> where T1 : C1<T2> 
{
    public C4(T2 p1) {}
}

public class C5<T1, T2> 
    where T1 : C6<T2>
    where T2 : C6<T1>
{
    public C5(T1 p1) {}
}

public class C6<T> {}
public class C7 : C6<C7> {}
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_ErrorRecovery()
        {
            TestCallSites("""
class P {
    void M1() 
    {
        new F1<_,_>(""); // Error: Can't infer T2
        new F1<int, string>(""); // Error: int != string
        new F1<byte,_>(257); // Error: Can't infer T2    
        F2(new F1<_,_>("")); // Error
        F3(new F1<_,_>(1)); // Error
        new F1<_,_>(new F1<_,_>(1)); // Error
    }
    class F1<T1, T2>{ public F1(T1 p1) {} }
    void F2<T1>(T1 p1) {}
    void F3<T1, T2>(T1 p1) {}
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Errors
            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Dynamic()
        {
            TestCallSites("""
class P {
    void M1() 
    {
        dynamic temp4 = "";
            
        // Inferred: [T1 = int] Error: T1 = string & int
        new F7<string, _>("", temp4, 1);
                    
        // Inferred: [T1 = int] Warning: Inferred type argument is not supported by runtime (type hints will not be used at all)
        new F7<_, string>(1, temp4, 1);
    }
            
    class F7<T1, T2>{ public F7(T1 p1, T2 p2, T1 p3) {} }
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Warning
            //TODO: Signatures
        }

        [Fact]
        public void PartialConstructorTypeInference_Nullable()
        {
            TestCallSites("""
#nullable enable
class P
{
    void M() 
    {
        // Inferred: [T = string?]
        string? temp1 = null;
        var temp2 = new C1<_?>(temp1);
        C1<string?> temp3 = temp2;

        // Inferred: [T = string?]
        temp3 = new C1<_?>(temp1);

        //Inferred: [T = string?]
        F1(new C2<_?>(), temp1);

        //Inferred: [T = string?]
        new C3<_?> {Prop1 = temp1};
        
    }

    void F1<T>(C2<T> p1, T p2) {}

    class C2<T> 
    {
        public C2() {}
    }

    class C1<T>
    {
        public C1(T p1) {}
    }

    class C3<T> 
    {
       public T Prop1 {get;set;}
    }
}
""",
        Symbols.ObjectCreation
    );

            //TODO: Signatures
        }
        #endregion
    }
}
