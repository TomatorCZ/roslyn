// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class MethodTypeInferenceTests : OverloadResolutionTestBase
    {
        [Fact]
        public void TestMethodTypeInference()
        {
            // TODO: Fill in the remaining holes in this test, marked with NYT.

            TestOverloadResolutionWithDiff(
@"
using System.Collections.Generic;

interface Con<in CON> {}
interface Cov<out COV> {}
interface Inv<INV> {}
delegate void DCon<in DCON>();
delegate void DCov<out DCOV>();
delegate void DInv<DINV>();

class Animal {}
class Mammal : Animal {}
class Tiger : Mammal {}

delegate int FInt<FINT>(FINT p1);
delegate FUNC0 Func0<FUNC0>();
delegate FUNC1R Func1<FUNC1A, FUNC1R>(FUNC1A p1);

class Class<CLASS> { public enum Enum {} }
class Derived<DER> : Class<DER>, Con<DER>, Inv<DER>, Cov<DER> {}
struct Struct<STRUCT> : Con<STRUCT>, Inv<STRUCT>, Cov<STRUCT> {}
interface Interface<INTERFACE> : Con<INTERFACE>, Inv<INTERFACE>, Cov<INTERFACE> {}

class C 
{ 
    void N1<T1>(FInt<T1> p1){}

    void N32<R32>(Func0<R32> p1){}
    void N32<A32, R32>(A32 p1, Func1<A32, R32> p2){}

    int M32() { return 1; }
    int M32(string s) { return 1; }

    void N41<T41>(Con<T41> p1, ref T41 p2) {}
    void N42<T42>(Con<T42> p1, ref T42[] p2) {}
    void N431<T431>(Con<T431> p1, ref Class<T431> p2) {}
    void N432<T432>(Con<T432> p1, ref Struct<T432> p2) {}
    void N4321<T4321>(Con<T4321> p1, ref Struct<T4321>? p2) {}
    void N4322<T4322>(Con<T4322> p1, ref Class<T4322>.Enum p2) {}
    void N4331<T4331>(Con<T4331> p1, ref DCov<T4331> p2) {}
    void N4332<T4332>(Con<T4332> p1, ref DCon<T4332> p2) {}
    void N4333<T4333>(Con<T4333> p1, ref DInv<T4333> p2) {}
    void N4341<T4341>(Con<T4341> p1, ref Cov<T4341> p2) {}
    void N4342<T4342>(Con<T4342> p1, ref Con<T4342> p2) {}
    void N4343<T4343>(Con<T4343> p1, ref Inv<T4343> p2) {}

    void N51<T51>(T51 p1, T51 p2) {}
    void N5211<T5211>(T5211 p1, T5211[] p2) {}
    void N5212<T5212>(Con<T5212> p1, Struct<T5212>[] p2){}

    void N5231A<T5231A>(T5231A p1, IEnumerable<T5231A> p2) {}
    void N5231B<T5231B>(T5231B p1, ICollection<T5231B> p2) {}
    void N5231C<T5231C>(T5231C p1, IList<T5231C> p2) {}
    void N5231D<T5231D>(T5231D p1, IReadOnlyCollection<T5231D> p2) {}
    void N5231E<T5231E>(T5231E p1, IReadOnlyList<T5231E> p2) {}
    void N5232A<T5232A>(IEnumerable<T5232A> p1) {}
    void N5232B<T5232B>(ICollection<T5232B> p1) {}
    void N5232C<T5232C>(IList<T5232C> p1) {}
    void N5232D<T5232D>(IReadOnlyCollection<T5232D> p1) {}
    void N5232E<T5232E>(IReadOnlyList<T5232E> p1) {}

    void N5321<T5321>(Con<T5321> p1, Class<T5321> p2) {}
    void N5322<T5322>(Con<T5322> p1, Struct<T5322> p2) {}
    void N5323<T5323>(Con<T5323> p1, Struct<T5323>? p2) {}
    void N5331<T5331>(T5331 p1, DCov<T5331> p2) {}
    void N5332<T5332>(ref T5332 p1, DCon<T5332> p2) {}
    void N5333<T5333>(Con<T5333> p1, DInv<T5333> p2) {}
    void N5341<T5341>(T5341 p1, Cov<T5341> p2) {}
    void N5342<T5342>(ref T5342 p1, Con<T5342> p2) {}
    void N5343<T5343>(Con<T5343> p1, Inv<T5343> p2) {}
    void N541<T541>(Con<T541> p1, Class<T541> p2) {}

    void N61<T61>(ref T61 p1, Con<T61> p2) {}
    void N6211<T6211>(ref T6211 p1, Con<T6211[]> p2) {}
    void N6212<T6212>(Con<T6212> p1, Con<Struct<T6212>[]> p2) {}
    void N6231<T6231>(T6231 p1, Con<T6231[]> p2) {}
    void N6232<T6232>(Con<Struct<T6232>[]> p1) {}
    void N631<T631>(Con<T631> p1, Con<Class<T631>> p2) {}
    void N632<T632>(Con<T632> p1, Con<Struct<T632>> p2) {}
    void N6321<T6321>(Con<T6321> p1, Con<Struct<T6321>?> p2) {}
    void N6331<T6331>(ref T6331 p1, Con<DCov<T6331>> p2) {}
    void N6332<T6332>(T6332 p1, Con<DCon<T6332>> p2) {}
    void N6333<T6333>(Con<T6333> p1, Con<DInv<T6333>> p2) {}
    void N6341<T6341>(ref T6341 p1, Con<Cov<T6341>> p2) {}
    void N6342<T6342>(T6342 p1, Con<Con<T6342>> p2) {}
    void N6343<T6343>(Con<T6343> p1, Con<Inv<T6343>> p2) {}
    void N64<T64>(Con<T64> p1, Con<Derived<T64>> p2) {}
    void N6511<T6511>(ref T6511 p1, Con<Derived<T6511>> p2) {}
    void N6512<T6512>(T6512 p1, Con<Derived<T6512>> p2) {}
    void N6513<T6513>(Con<T6513> p1, Con<Derived<T6513>> p2) {}
    void N6531<T6531>(ref T6531 p1, Con<Interface<T6531>> p2) {}
    void N6532<T6532>(T6532 p1, Con<Interface<T6532>> p2) {}
    void N6533<T6533>(Con<T6533> p1, Con<Interface<T6533>> p2) {}

    void N71<A71, R71)(A71 p1, Func1<A71, R71> p2) {}
    Class<T71> M71<T71>(T71 p1) { return null; }

    Tiger tiger;
    Mammal mammal;
    Animal animal;

    Con<Tiger> conTiger;
    Inv<Tiger> invTiger;
    Cov<Tiger> covTiger;

    Con<Mammal> conMammal;
    Cov<Mammal> covMammal;
    Inv<Mammal> invMammal;

    Cov<Animal> covAnimal;
    Inv<Animal> invAnimal;
    Con<Animal> conAnimal;
    
    DCon<Mammal> dconMammal;
    DCov<Mammal> dcovMammal;
    DInv<Mammal> dinvMammal;
    
    Inv<Class<Mammal>> invClsMammal;
    Inv<Class<Mammal[]>> invClsMammalArr;

    Mammal[] mammalArr;
    Struct<Mammal>[] structMammalArr;
    Con<Mammal[]> conMammalArr;
    Con<Struct<Mammal>[]> conStructMammalArr;
    
    Class<Mammal> classMammal;
    Derived<Mammal> derivedMammal;
    Interface<Mammal> interfaceMammal;
    Struct<Mammal> structMammal;
    
    Con<Class<Mammal>> conClassMammal;
    Con<Struct<Mammal>> conStructMammal;
    Con<Struct<Mammal>?> conNullableStructMammal;
    Con<DCov<Mammal>> conDCovMammal;
    Con<DCon<Mammal>> conDConMammal;
    Con<DInv<Mammal>> conDInvMammal;
    Con<Cov<Mammal>> conCovMammal;
    Con<Con<Mammal>> conConMammal;
    Con<Inv<Mammal>> conInvMammal;

    Struct<Mammal>? nullableStructMammal;
    Class<Mammal>.Enum classMammalEnum;

    string str;
    

    void M() 
    { 
        // Method type inference test plan
        //
        // Method type inference is complicated and there are a lot of possibilities to test
        //
        // features not yet implemented are marked NYI
        // features not yet tested are marked NYT
        //
        // Strategies:
        //
        // When attempting to prove that an exact bound has been made, make an upper bound of Animal 
        // and an exact bound of Mammal. The parameter should be resolved as Mammal, even though Mammal is more specific.
        //
        // When attempting to prove that a lower bound has been made, make a lower bound of of Animal and 
        // a lower bound of Mammal. The parameter should be resolved as Animal; the Mammal bound is allowed to
        // relax and become Animal.
        //
        // When attempting to prove that an upper bound has been made, make an exact bound of Tiger and
        // an upper bound of Mammal. The parameter should be resolved as Tiger; the Mammal bound is allowed
        // to become Tiger.

        // 1 explicit lambda parameter to delegate parameter type (exact)

        N1((double x)=>123);                   //-C.N1<double>(FInt<double>)

        // 2 inferred lambda return type inference (NYI)
        //   2.1 to void delegate (NYI)
        //   2.2 to non-void delegate (NYI)

        // 3 method group return type inference
        //   3.1 to void delegate (NYT)
        //   3.2 to non void delegate 

        N32(M32);           //-C.N32<int>(Func0<int>)
        N32(str, M32);      //-C.N32<string, int>(string, Func1<string, int>)

        // 4 exact inference
        //   4.1 from any type S to method type parameter T (exact)
        //   4.2 from array S[] to array T[]
        //     4.2.1 where ranks match (exact)
        //     4.2.2 where ranks do not match (no inference made) (NYT)
        //   4.3 from constructed type 
        //     4.3.1 from class C<S> to class C<T> (exact)
        //     4.3.2 from struct C<S> to struct C<T> (exact)
        //       4.3.2.1 from nullable N<S> to N<T> (exact)
        //       4.3.2.2 from enum C<S>.E to enum C<T>.E (exact)
        //     4.3.3 from delegate C<S> to C<T>
        //       4.3.3.1 covariant (exact)
        //       4.3.3.2 contravariant (exact)
        //       4.3.3.3 invariant (exact)
        //     4.3.4 from interface C<S> to interface C<T>
        //       4.3.4.1 covariant (exact)
        //       4.3.4.2 contravariant (exact)
        //       4.3.4.3 invariant (exact)

        N41(conAnimal, ref mammal);                     //-C.N41<Mammal>(Con<Mammal>, ref Mammal)
        N42(conAnimal, ref mammalArr);                  //-C.N42<Mammal>(Con<Mammal>, ref Mammal[])
        N431(conAnimal, ref classMammal);               //-C.N431<Mammal>(Con<Mammal>, ref Class<Mammal>)
        N432(conAnimal, ref structMammal);              //-C.N432<Mammal>(Con<Mammal>, ref Struct<Mammal>)
        N4321(conAnimal, ref nullableStructMammal);     //-C.N4321<Mammal>(Con<Mammal>, ref Struct<Mammal>?)
        N4322(conAnimal, ref classMammalEnum);          //-C.N4322<Mammal>(Con<Mammal>, ref Class<Mammal>.Enum)
        N4331(conAnimal, ref dcovMammal);               //-C.N4331<Mammal>(Con<Mammal>, ref DCov<Mammal>)
        N4332(conAnimal, ref dconMammal);               //-C.N4332<Mammal>(Con<Mammal>, ref DCon<Mammal>)
        N4333(conAnimal, ref dinvMammal);               //-C.N4333<Mammal>(Con<Mammal>, ref DInv<Mammal>)
        N4341(conAnimal, ref covMammal);                //-C.N4341<Mammal>(Con<Mammal>, ref Cov<Mammal>)
        N4342(conAnimal, ref conMammal);                //-C.N4342<Mammal>(Con<Mammal>, ref Con<Mammal>)
        N4343(conAnimal, ref invMammal);                //-C.N4343<Mammal>(Con<Mammal>, ref Inv<Mammal>)

        // 5 lower bound inference
        //   5.1 from any type to method type parameter T (lower)
        //   5.2 from array S[]
        //     5.2.1 to array T[] of same rank
        //       5.2.1.1 of reference type (lower)
        //       5.2.1.2 of value type (exact)
        //     5.2.2 to array T[] of different rank (no inference made) (NYT)
        //     5.2.3 from 1-d array S[] to IEnumerable<T>, ICollection<T>, IList<T>, IReadOnlyList<T> and IReadOnlyCollection<T>
        //       5.2.3.1 of reference type (lower)
        //       5.2.3.2 of value type (exact)
        //   5.3 from constructed type to matching constructed type
        //     5.3.1 from class C<S> to C<T> (exact)
        //     5.3.2 from struct C<S> to C<T> (exact)
        //       5.3.2.1 nullable N<S> to N<T> (exact)
        //     5.3.3 from delegate C<S> to C<T>
        //       5.3.3.1 covariant (lower)
        //       5.3.3.2 contravariant (upper)
        //       5.3.3.3 invariant (exact)
        //     5.3.4 from interface C<S> to C<T>
        //       5.3.4.1 covariant (lower)
        //       5.3.4.2 contravariant (upper)
        //       5.3.4.3 invariant (exact)
        //   5.4 base class
        //     5.4.1 from class D with base class C<S> to C<T> (exact)
        //     5.4.2 from type parameter D with effective base class C<S> to C<T> (exact) (NYI)
        //     5.4.3 from type parameter D with effective base class E that has base class C<S> to C<T> (exact) (NYI)

        N51(animal, mammal);                    //-C.N51<Animal>(Animal, Animal)
        N5211(animal, mammalArr);               //-C.N5211<Animal>(Animal, Animal[])
        N5212(conAnimal, structMammalArr);      //-C.N5212<Mammal>(Con<Mammal>, Struct<Mammal>[])
        N5231A(animal, mammalArr);              //-C.N5231A<Animal>(Animal, System.Collections.Generic.IEnumerable<Animal>)
        N5231B(animal, mammalArr);              //-C.N5231B<Animal>(Animal, System.Collections.Generic.ICollection<Animal>)
        N5231C(animal, mammalArr);              //-C.N5231C<Animal>(Animal, System.Collections.Generic.IList<Animal>)
        N5231D(animal, mammalArr);              //-C.N5231D<Animal>(Animal, System.Collections.Generic.IReadOnlyCollection<Animal>)
        N5231E(animal, mammalArr);              //-C.N5231E<Animal>(Animal, System.Collections.Generic.IReadOnlyList<Animal>)
        N5232A(structMammalArr);                //-C.N5232A<Struct<Mammal>>(System.Collections.Generic.IEnumerable<Struct<Mammal>>)
        N5232B(structMammalArr);                //-C.N5232B<Struct<Mammal>>(System.Collections.Generic.ICollection<Struct<Mammal>>)
        N5232C(structMammalArr);                //-C.N5232C<Struct<Mammal>>(System.Collections.Generic.IList<Struct<Mammal>>)
        N5232D(structMammalArr);                //-C.N5232D<Struct<Mammal>>(System.Collections.Generic.IReadOnlyCollection<Struct<Mammal>>)
        N5232E(structMammalArr);                //-C.N5232E<Struct<Mammal>>(System.Collections.Generic.IReadOnlyList<Struct<Mammal>>)
        N5321(conAnimal, classMammal);          //-C.N5321<Mammal>(Con<Mammal>, Class<Mammal>)
        N5322(conAnimal, structMammal);         //-C.N5322<Mammal>(Con<Mammal>, Struct<Mammal>)
        N5323(conAnimal, nullableStructMammal); //-C.N5323<Mammal>(Con<Mammal>, Struct<Mammal>?)
        N5331(animal, dcovMammal);              //-C.N5331<Animal>(Animal, DCov<Animal>)
        N5332(ref tiger, dconMammal);           //-C.N5332<Tiger>(ref Tiger, DCon<Tiger>)
        N5333(conAnimal, dinvMammal);           //-C.N5333<Mammal>(Con<Mammal>, DInv<Mammal>)
        N5341(animal, covMammal);               //-C.N5341<Animal>(Animal, Cov<Animal>)
        N5342(ref tiger, conMammal);            //-C.N5342<Tiger>(ref Tiger, Con<Tiger>)
        N5343(conAnimal, invMammal);            //-C.N5343<Mammal>(Con<Mammal>, Inv<Mammal>)
        N541(conAnimal, derivedMammal);         //-C.N541<Mammal>(Con<Mammal>, Class<Mammal>)

        //   5.5 base interface 
        //     in all cases, base interface must be 'unique'. (NYT)
        //     5.5.1 from class D with base interface C<S> to C<T> 
        //       5.5.1.1 covariant (lower)
        //       5.5.1.2 contravariant (upper)
        //       5.5.1.3 invariant (exact)
        //     5.5.2 from struct D with base interface C<S> to C<T>
        //       5.5.2.1 covariant (lower)
        //       5.5.2.2 contravariant (upper)
        //       5.5.2.3 invariant (exact)
        //     5.5.3 from interface D with base interface C<S> to C<T>
        //       5.5.3.1 covariant (lower)
        //       5.5.3.2 contravariant (upper)
        //       5.5.3.3 invariant (exact)

        // Just re-use the methods from section 5.3.4

        N5341(animal, derivedMammal);               //-C.N5341<Animal>(Animal, Cov<Animal>)
        N5342(ref tiger, derivedMammal);            //-C.N5342<Tiger>(ref Tiger, Con<Tiger>)
        N5343(conAnimal, derivedMammal);            //-C.N5343<Mammal>(Con<Mammal>, Inv<Mammal>)
        N5341(animal, structMammal);                //-C.N5341<Animal>(Animal, Cov<Animal>)
        N5342(ref tiger, structMammal);             //-C.N5342<Tiger>(ref Tiger, Con<Tiger>)
        N5343(conAnimal, structMammal);             //-C.N5343<Mammal>(Con<Mammal>, Inv<Mammal>)
        N5341(animal, interfaceMammal);             //-C.N5341<Animal>(Animal, Cov<Animal>)
        N5342(ref tiger, interfaceMammal);          //-C.N5342<Tiger>(ref Tiger, Con<Tiger>)
        N5343(conAnimal, interfaceMammal);          //-C.N5343<Mammal>(Con<Mammal>, Inv<Mammal>)


        //     5.5.4 from type parameter D with effective base class E that implements interface C<S> to C<T> (NYI)
        //       5.5.4.1 covariant (lower)(NYI)
        //       5.5.4.2 contravariant (upper)(NYI)
        //       5.5.4.3 invariant (exact)(NYI)
        //     5.5.5 from type parameter D with effective interface set member C<S> to C<T> (NYI)
        //       5.5.5.1 covariant (lower)(NYI)
        //       5.5.5.2 contravariant (upper)(NYI)
        //       5.5.5.3 invariant (exact)(NYI)
        //     5.5.6 from type parameter D with effective interface set member E that inherits from C<S> to C<T> (NYI)
        //       5.5.6.1 covariant (lower)(NYI)
        //       5.5.6.2 contravariant (upper)(NYI)
        //       5.5.6.3 invariant (exact)(NYI)


        // 6 upper bound inference
        //   6.1 from any type S to method type parameter T (upper)
        //   6.2 to array T[]
        //     6.2.1 from array S[] of same rank
        //       6.2.1.1 of reference type (upper)
        //       6.2.1.2 of value type (exact)
        //     6.2.2 from array S[] of different rank (no inference made) (NYT)
        //     6.2.3 from IEnumerable<S>, ICollection<S> or IList<S> to one-d array T[] 
        //       6.2.3.1 of reference type (upper) 
        //       6.2.3.2 of value type (exact)
        //   6.3 from constructed type to matching constructed type
        //     6.3.1 from class C<S> to C<T> (exact)
        //     6.3.2 from struct C<S> to C<T> (exact)
        //       6.3.2.1 nullable N<S> to N<T> (exact)
        //     6.3.3 from delegate C<S> to C<T>
        //       6.3.3.1 covariant (upper)
        //       6.3.3.2 contravariant (lower)
        //       6.3.3.3 invariant (exact)
        //     6.3.4 from interface C<S> to C<T>
        //       6.3.4.1 covariant (upper)
        //       6.3.4.2 contravariant (lower)
        //       6.3.4.3 invariant (exact)
        //   6.4 from class C<S> to class D with base class C<T> (exact)
        //   6.5 base interface - in all cases, base interface must be 'unique'.
        //     6.5.1 from interface C<S> to class D with base interface C<T> 
        //       6.5.1.1 covariant (upper)
        //       6.5.1.2 contravariant (lower)
        //       6.5.1.3 invariant (exact)
        //     6.5.2 from interface C<S> to struct D with base interface C<T> (NYT)
        //       These are unusual cases because here method type inference will succeed but will
        //       always produce a result that is not applicable. If you have an upper bound inference
        //       because you are inferring from Con<Struct<S>> to Con<I<T>> where Struct<S> implements I<S>
        //       then we make the upper bound inference from I<S> to I<T>. If that succeeds then applicability
        //       checking will fail, because Con<any value type> is not convertible to Con<any reference type>.
        //       In order to verify that type inference is succeeding here we'll have to make some error cases
        //       and see if we're getting 'not applicable' errors vs 'type inference failed' errors.
        //       6.5.2.1 covariant (upper)(NYT)
        //       6.5.2.2 contravariant (lower)(NYT)
        //       6.5.2.3 invariant (exact)(NYT)
        //     6.5.3 from interface C<S> to interface D with base interface C<T>
        //       6.5.3.1 covariant (upper)
        //       6.5.3.2 contravariant (lower)
        //       6.5.3.3 invariant (exact)

        N61(ref tiger, conMammal);                  //-C.N61<Tiger>(ref Tiger, Con<Tiger>)
        N6211(ref tiger, conMammalArr);             //-C.N6211<Tiger>(ref Tiger, Con<Tiger[]>)
        N6212(conAnimal, conStructMammalArr);       //-C.N6212<Mammal>(Con<Mammal>, Con<Struct<Mammal>[]>)
        N6231(tiger, (Con<IEnumerable<Mammal>>)null);               //-C.N6231<Mammal>(Mammal, Con<Mammal[]>)
        N6231(tiger, (Con<IList<Mammal>>)null);                     //-C.N6231<Mammal>(Mammal, Con<Mammal[]>)
        N6231(tiger, (Con<IReadOnlyList<Mammal>>)null);             //-C.N6231<Mammal>(Mammal, Con<Mammal[]>)
        N6231(tiger, (Con<IReadOnlyCollection<Mammal>>)null);       //-C.N6231<Mammal>(Mammal, Con<Mammal[]>)
        N6232((Con<IEnumerable<Struct<Mammal>>>)null);              //-C.N6232<Mammal>(Con<Struct<Mammal>[]>)
        N6232((Con<IList<Struct<Mammal>>>)null);                    //-C.N6232<Mammal>(Con<Struct<Mammal>[]>)
        N6232((Con<IReadOnlyList<Struct<Mammal>>>)null);            //-C.N6232<Mammal>(Con<Struct<Mammal>[]>)
        N6232((Con<IReadOnlyCollection<Struct<Mammal>>>)null);      //-C.N6232<Mammal>(Con<Struct<Mammal>[]>)
        N631(conAnimal, conClassMammal);            //-C.N631<Mammal>(Con<Mammal>, Con<Class<Mammal>>)
        N632(conAnimal, conStructMammal);           //-C.N632<Mammal>(Con<Mammal>, Con<Struct<Mammal>>)
        N6321(conAnimal, conNullableStructMammal);  //-C.N6321<Mammal>(Con<Mammal>, Con<Struct<Mammal>?>)
        N6331(ref tiger, conDCovMammal);            //-C.N6331<Tiger>(ref Tiger, Con<DCov<Tiger>>)
        N6332(animal, conDConMammal);               //-C.N6332<Animal>(Animal, Con<DCon<Animal>>)
        N6333(conAnimal, conDInvMammal);            //-C.N6333<Mammal>(Con<Mammal>, Con<DInv<Mammal>>)
        N6341(ref tiger, conCovMammal);             //-C.N6341<Tiger>(ref Tiger, Con<Cov<Tiger>>)
        N6342(animal, conConMammal);                //-C.N6342<Animal>(Animal, Con<Con<Animal>>)
        N6343(conAnimal, conInvMammal);             //-C.N6343<Mammal>(Con<Mammal>, Con<Inv<Mammal>>)
        N64(conAnimal, conClassMammal);             //-C.N64<Mammal>(Con<Mammal>, Con<Derived<Mammal>>)
        N6511(ref tiger, conCovMammal);             //-C.N6511<Tiger>(ref Tiger, Con<Derived<Tiger>>)
        N6512(animal, conConMammal);                //-C.N6512<Animal>(Animal, Con<Derived<Animal>>)
        N6513(conAnimal, conInvMammal);             //-C.N6513<Mammal>(Con<Mammal>, Con<Derived<Mammal>>)
        N6531(ref tiger, conCovMammal);             //-C.N6531<Tiger>(ref Tiger, Con<Interface<Tiger>>)
        N6532(animal, conConMammal);                //-C.N6532<Animal>(Animal, Con<Interface<Animal>>)
        N6533(conAnimal, conInvMammal);             //-C.N6533<Mammal>(Con<Mammal>, Con<Interface<Mammal>>)

        // 7 Additional interesting scenarios:
        // 7.1 Method type inference where the argument is a method group containing a generic method that also
        //     needs method type inference:

        N71(1, M71); //-C.N71<int, Class<int>>(int, Func1<int, Class<int>>)
        
    }
}
");
        }

        [Fact]
        public void TestLambdaTypeInference()
        {
            TestOverloadResolutionWithDiff(
@"

interface IE<out COV> {}
delegate R F<out R> ();
delegate R F<in A, out R> (A a);
delegate R F<in A1, in A2, out R> (A1 a1, A2 a2);

class Customer { public string Name; public int ID; }
class Order { public decimal Total; public int? CustomerID; }

class L<T> { public void D<R>(F<T, R> f){} }

class C 
{ 
    IE<T> Where<T>(IE<T> s, F<T, bool> f) { return null; }
    IE<R> Select<A, R>(IE<A> s, F<A, R> f) { return null; }
    IE<R> Join<I, O, K, R>(IE<I> inner, IE<O> outer, F<I, K> innerKey, F<O, K> outerKey, F<I, O, R> selector) { return null; }
    T Apply<T>(F<T> f) { return f(); }
    T Apply<T>(F<T> f1, F<T> f2) { return f1(); }
    T Apply<T>(params F<T>[] fs) { return fs[0](); }
    static int GetID(Customer c) { return c.ID; }
 
    IE<Customer> customers;
    IE<Order> orders;
    
    void M() 
    { 
        object x1 = Where(customers, c=>c.Name == null);
        //-C.Where<Customer>(IE<Customer>, F<Customer, bool>)
        object x2 = Select(customers, c=>c.Name);
        //-C.Select<Customer, string>(IE<Customer>, F<Customer, string>)
        object x3 = Join(customers, orders, c=>c.ID, o=>o.CustomerID, (c,o)=>o.Total);
        //-C.Join<Customer, Order, int?, decimal>(IE<Customer>, IE<Order>, F<Customer, int?>, F<Order, int?>, F<Customer, Order, decimal>)
        object x4 = Select(customers, c=>{ if (c.Name == string.Empty) return null; else return c.Name; });
        //-C.Select<Customer, string>(IE<Customer>, F<Customer, string>)
        object x5 = Apply(()=>new Customer());
        //-C.Apply<Customer>(F<Customer>)
        object x6 = Apply(()=>new Customer(), ()=>{while(true){}});
        //-C.Apply<Customer>(F<Customer>, F<Customer>)
        object x7 = Apply(()=>1, ()=>2, ()=>{while(true){}});
        //-C.Apply<int>(params F<int>[])

        // Make sure that overload resolution and type inference involving method group conversions works:
        object x8 = Select(customers, GetID); 
        //-C.Select<Customer, int>(IE<Customer>, F<Customer, int>)

        (new L<int>()).D(x=>123.4);
        //-L<int>.D<double>(F<int, double>)

    }
}
");
        }

        [Fact]
        public void TestMethodTypeInferenceErrors()
        {
            var source = @"
class C 
{ 
    delegate R F<out R>();
    static T Apply<T>(F<T> f) { return f(); }
    static void M() 
    {
      Apply(delegate { while (true) { } });
    }
}
";
            CreateCompilation(source).VerifyDiagnostics(
                // (8,7): error CS0411: The type arguments for method 'C.Apply<T>(C.F<T>)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //       Apply(delegate { while (true) { } });
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "Apply").WithArguments("C.Apply<T>(C.F<T>)").WithLocation(8, 7));
        }

        [Fact, WorkItem(578362, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/578362")]
        public void TypeInferenceDynamicByRef()
        {
            string source = @"
class C
{
    static void Main()
    {
        dynamic d = null;
        Goo(ref d);
    }
 
    static void Goo<T>(ref T[] x)
    {
    }
}
";
            CreateCompilationWithMscorlib40AndSystemCore(source, new[] { CSharpRef }).VerifyEmitDiagnostics(
                // (7,9): error CS0411: The type arguments for method 'C.Goo<T>(ref T[])' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //         Goo(ref d);
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "Goo").WithArguments("C.Goo<T>(ref T[])"));
        }

        [WorkItem(541810, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541810")]
        [Fact]
        public void TestMethodTypeInferenceWhenFixedParameterIsOpenGenericType()
        {
            var source = @"
using System.Collections.Generic;
class Test
{
    static void Goo<V>(V x)
    {
        I<V> i = null;
        var y = new List<string>();
        i.method(x, y);
    }
    static void Main()
    {
        Goo(1);
    }
    interface I<T>
    {
        void method<U>(T x, List<U> y);
    }
}";
            CompileAndVerify(source).VerifyDiagnostics();
        }

        [WorkItem(541811, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541811")]
        [Fact]
        public void TestMethodTypeInferenceWhenFixedParameterIsOpenGenericType2()
        {
            var source = @"
using System.Collections.Generic;
class Test
{
    static void Main()
    {
        Test.Run(1, 2);
    }
    public static void Run<T, U>(T t, U u)
    {
        I<U> i = new Outer<U, T>();
        i.Goo(u, new List<string>());
    }
    interface I<A>
    {
        void Goo<B>(A a, List<B> y);
    }
    class Outer<P, Q> : I<P>
    {
        void I<P>.Goo<S>(P p, List<S> y)
        {
        }
    }
}";

            CreateCompilation(source).VerifyDiagnostics();
        }

        [Fact]
        public void TestMethodTypeInferenceWhenFixedParameterIsOpenGenericType03()
        {
            var source = @"
class Test
{
    class C<T>
    {
        public static void M<U>(T t, U u)
        {
            // The inference is a bit tricky here because there
            // are two distinct things both called 'U'. There is the
            // 'U' of the caller, and the 'U' of the callee; they are 
            // different types. We must not reason 'the method here
            // is C<U>.M<U>(U, U) so let's infer U' from both arguments.
            // Rather, we must say that this is C<U1>.M<U2>(U1, U2) and
            // infer U2 alone. 

            C<U>.M(u, 123);

            // For example, the call in Main below will call 
            // C<string>.M<double>. That method will call
            // C<double>.M<int>, which will then call
            // C<int>.M<int>. 
        }
    }
    static void Main()
    {
        C<string>.M<double>(null, 1.0);
    }
}";
            CompileAndVerify(source).VerifyDiagnostics();
        }

        [WorkItem(541887, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541887")]
        [Fact()]
        public void Bug8785_1()
        {
            // Right now Roslyn allows this syntax; the missing type arguments are ignored and we
            // do method type inference as though they were not there at all.
            // This should be illegal; we could produce an error at parse time or semantic
            // analysis time.

            var source = @"
class Program
{
    static void Main(string[] args)
    {
        var s = Goo<>(123, 345);
    }
    public static int Goo<T, U>(T t, U u)
    {
        return 1;
    }
}
";
            CreateCompilation(source).VerifyDiagnostics(
                // (6,17): error CS0305: Using the generic method 'Program.Goo<T, U>(T, U)' requires 2 type arguments
                //         var s = Goo<>(123, 345);
                Diagnostic(ErrorCode.ERR_BadArity, "Goo<>").WithArguments("Program.Goo<T, U>(T, U)", "method", "2"));
        }

        [Fact]
        public void Bug50782_1()
        {
            string source = """
using System.Diagnostics.CodeAnalysis;
#nullable enable

interface IOperation<T> 
{ }

public class StringOperation : IOperation<string?> 
{ }

public class C 
{   
    static void Main() 
    {
        TestA(new StringOperation(), out string? discardA);
        TestA(new StringOperation(), out string? _);
        TestA(new StringOperation(), out var _);
        TestA(new StringOperation(), out _);
        
        TestB(new StringOperation(), out string? discardB);
        TestB(new StringOperation(), out string? _);
        TestB(new StringOperation(), out var _);
        TestB(new StringOperation(), out _);
        
        TestC<string?>(out string? discardC);
        TestC<string?>(out string? _);
        TestC<string?>(out var _);
        TestC<string?>(out _);
        
        TestD<string?>(out string? discardD);
        TestD<string?>(out string? _);
        TestD<string?>(out var _);
        TestD<string?>(out _);
        
        TestE(out string? discardE);
        TestE(out var discardEVar);
        TestE(out string? _);
        TestE(out var _);
        TestE(out _);
        
        TestF(out string? discardF);
        TestF(out string? _);
    }
   
    static void TestA<T>(IOperation<T> operation, [MaybeNull] out T result) => result = default;
    static void TestB<T>(IOperation<T> operation, out T result) => result = default!;
    
    static void TestC<T>([MaybeNull] out T result) => result = default;
    static void TestD<T>(out T result) => result = default!;

    static void TestE([MaybeNull] out string result) => result = default;
    static void TestF(out string result) => result = "";
}
""";
            var comp = CreateCompilation(source, targetFramework: TargetFramework.Net70);
            comp.VerifyDiagnostics();

            var tree = comp.SyntaxTrees.Single();
            var model = comp.GetSemanticModel(tree);

            // out _
            foreach (var discardOut in GetDiscardIdentifiers(tree))
            {
                CheckDiscard(model, discardOut, "System.String?");
            }

            // out T _, out var _, out T? _
            foreach (var discardDecl in GetDiscardDesignations(tree))
            {
                CheckDiscard(model, discardDecl, "System.String?");
            }
        }

        [Fact]
        public void Bug50782_2()
        {
            string source = """
#nullable enable

interface IOperation<T> 
{ }

public class StringOperation : IOperation<string> 
{ }

public class C 
{   
    static void Main() 
    {
        TestA(new StringOperation(), out string discardA);
        TestA(new StringOperation(), out string _);
        TestA(new StringOperation(), out var _);
        TestA(new StringOperation(), out _);

        TestB(out string discardB);
        TestB(out string _);
        TestB<string>(out var discardVarB);
        TestB<string>(out var _);
        TestB<string>(out _);

        TestC(out var _);
        TestC(out _);
    }
   
    static void TestA<T>(IOperation<T> operation, out T result) => result = default!;
    static void TestB<T>(out T result) => result = default!;
    static void TestC(out string result) => result = "";
}
""";
            var comp = CreateCompilation(source, targetFramework: TargetFramework.Net70);
            comp.VerifyDiagnostics();

            var tree = comp.SyntaxTrees.Single();
            var model = comp.GetSemanticModel(tree);

            // out _
            foreach (var discardOut in GetDiscardIdentifiers(tree))
            {
                CheckDiscard(model, discardOut, "System.String!");
            }

            // out var _, out T _
            foreach (var discardDecl in GetDiscardDesignations(tree))
            {
                CheckDiscard(model, discardDecl, "System.String!");
            }
        }

        [Fact]
        public void Bug50782_3()
        {
            string source = """
#nullable enable

interface IOperation<T> 
{ }

public class StringOperation : IOperation<string> 
{ }

public class C 
{   
    static void Main() 
    {
        TestA(new StringOperation(), out string? discardA);
        TestA(new StringOperation(), out string? _);

        TestC(out string? discardC);
        TestC(out string? _);
    }
   
    static void TestA<T>(IOperation<T> operation, out T result) => result = default!;
    static void TestC(out string result) => result = "";
}
""";
            var comp = CreateCompilation(source, targetFramework: TargetFramework.Net70);
            comp.VerifyDiagnostics();

            var tree = comp.SyntaxTrees.Single();
            var model = comp.GetSemanticModel(tree);

            // out _
            foreach (var discardOut in GetDiscardIdentifiers(tree))
            {
                CheckDiscard(model, discardOut, "System.String?");
            }

            // out var _, out T _
            foreach (var discardDecl in GetDiscardDesignations(tree))
            {
                CheckDiscard(model, discardDecl, "System.String?");
            }
        }

        [Fact]
        public void Bug50782_4()
        {
            string source = """
#nullable enable
void M((string, string?) tuple)
{
    (_, _) = tuple;
    (string _, string _) = tuple;
}
""";

            var comp = CreateCompilation(source, targetFramework: TargetFramework.Net70);
            comp.VerifyDiagnostics(new[] { 
                 // (2,6): warning CS8321: The local function 'M' is declared but never used
                // void M((string, string?) tuple)
                Diagnostic(ErrorCode.WRN_UnreferencedLocalFunction, "M").WithArguments("M").WithLocation(2, 6),
                // (5,28): warning CS8600: Converting null literal or possible null value to non-nullable type.
                //     (string _, string _) = tuple;
                Diagnostic(ErrorCode.WRN_ConvertingNullableToNonNullable, "tuple").WithLocation(5, 28)
            });
        }

        private static void CheckDiscard(SemanticModel model, DiscardDesignationSyntax discard, string type)
        {
            Assert.Null(model.GetDeclaredSymbol(discard));
            Assert.Null(model.GetTypeInfo(discard).Type);
            Assert.Null(model.GetSymbolInfo(discard).Symbol);
            var declaration = (DeclarationExpressionSyntax)discard.Parent;
            Assert.Equal(type, model.GetTypeInfo(declaration).Type.ToTestDisplayString(includeNonNullable: true));
            Assert.Null(model.GetSymbolInfo(declaration).Symbol);
        }

        private static void CheckDiscard(SemanticModel model, IdentifierNameSyntax discard, string type)
        {
            Assert.Null(model.GetDeclaredSymbol(discard));
            var discardSymbol = (IDiscardSymbol)model.GetSymbolInfo(discard).Symbol;
            Assert.Equal(type, discardSymbol.Type.ToTestDisplayString(includeNonNullable: true));
            Assert.Equal(type, model.GetTypeInfo(discard).Type.ToTestDisplayString(includeNonNullable: true));
        }

        private static IEnumerable<DiscardDesignationSyntax> GetDiscardDesignations(SyntaxTree tree)
        {
            return tree.GetRoot().DescendantNodes().OfType<DiscardDesignationSyntax>();
        }

        private static IEnumerable<IdentifierNameSyntax> GetDiscardIdentifiers(SyntaxTree tree)
        {
            return tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().Where(i => i.Identifier.ContextualKind() == SyntaxKind.UnderscoreToken);
        }

        [WorkItem(541887, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541887")]
        [Fact]
        public void Bug8785_2()
        {
            // We should produce a parse error here, but *not* a semantic analysis error
            // stating that type "" could not be found.

            var source = @"
class Program
{
    static void Main(string[] args)
    {
        var s = Goo<int, >(123, 345);
    }
    public static int Goo<T, U>(T t, U u)
    {
        return 1;
    }
}
";
            CreateCompilation(source).VerifyDiagnostics(
                // (6,26): error CS1031: Type expected
                //         var s = Goo<int, >(123, 345);
                Diagnostic(ErrorCode.ERR_TypeExpected, ">"),

                // CONSIDER: we would prefer not to report this cascading diagnostic.

                // (6,33): error CS1503: Argument 2: cannot convert from 'int' to '?'
                //         var s = Goo<int, >(123, 345);
                Diagnostic(ErrorCode.ERR_BadArgType, "345").WithArguments("2", "int", "?"));
        }

        [WorkItem(542591, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542591")]
        [Fact]
        public void Bug9877()
        {
            // No NRE
            var source = @"
class Program
{
    public static void M<T>(System.Func<T> f) 
    {
        M(E.A);
    }
}
";
            CreateCompilation(source).VerifyDiagnostics(
                // (6,11): error CS0103: The name 'E' does not exist in the current context
                Diagnostic(ErrorCode.ERR_NameNotInContext, "E").WithArguments("E"));
        }

        [WorkItem(9145, "http://vstfdevdiv:8080/DevDiv_Projects/Roslyn/_workitems/edit/9145")]
        [Fact]
        public void Bug9145()
        {
            string source = @"
using System;
using System.Collections.Generic;
public interface IParser<TSource, TSource1> { }
public class ParserQueryContext<TSource, TSource1, T> : IParser<TSource, TSource1> { }

public static partial class ParserExtensions
{
    public static IParser<TSource, TResult> Select<TSource, TIntermediate, TResult>(this IParser<TSource, TIntermediate> parser, Func<TIntermediate, TResult> selector) { return null; }
    public static ParserQueryContext<TParseSource, TParseResult, TResult> Select<TParseSource, TParseResult, TSource, TResult>(this ParserQueryContext<TParseSource, TParseResult, TSource> source, Func<TSource, TResult> selector) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> Exactly<TSource, TResult>(this IParser<TSource, TResult> parser, int count) { return null; }
    public static IParser<TSource, TResult> Where<TSource, TResult>(this IParser<TSource, TResult> parser, Func<TResult, bool> predicate) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> OneOrMore<TSource, TResult>(this IParser<TSource, TResult> parser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> OneOrMore<TSource, TResult>(this IParser<TSource, TResult> parser, IParser<TSource, TResult> separator) { return null; }
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TCollection, TResult>(this IParser<TSource, TFirstResult> parser, Func<TFirstResult, IEnumerable<TCollection>> collectionSelector, Func<TFirstResult, TCollection, TResult> resultSelector) { return null; }
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TSecondResult, TResult>(this IParser<TSource, TFirstResult> parser, Func<TFirstResult, IParser<TSource, TSecondResult>> parserSelector, Func<TFirstResult, TSecondResult, TResult> resultSelector) { return null; }
    public static IParser<TSource, TResult> Not<TSource, TNotResult, TResult>(this IParser<TSource, TResult> parser, IParser<TSource, TNotResult> notParser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> NoneOrMore<TSource, TResult>(this IParser<TSource, TResult> parser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> And<TSource, TResult>(this IParser<TSource, IEnumerable<TResult>> parser, IParser<TSource, IEnumerable<TResult>> nextParser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> And<TSource, TResult>(this IParser<TSource, IEnumerable<TResult>> parser, IParser<TSource, TResult> nextParser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> And<TSource, TResult>(this IParser<TSource, TResult> parser, IParser<TSource, IEnumerable<TResult>> nextParser) { return null; }
    public static IParser<TSource, IEnumerable<TResult>> And<TSource, TResult>(this IParser<TSource, TResult> parser, IParser<TSource, TResult> nextParser) { return null; }
    public static IParser<TSource, IList<TResult>> ToList<TSource, TResult>(this IParser<TSource, IEnumerable<TResult>> parser) { return null; }

    public static IEnumerable<TResult> Parse<TSource, TResult>(this IEnumerable<TSource> source, IParser<TSource, TResult> parser) { return null; }

    public static IEnumerable<TResult> Parse<TSource, TResult>(this IEnumerable<TSource> source, Func<ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>, ParserQueryContext<TSource, TSource, IParser<TSource, TResult>>> grammarSelector) { return null; }
    public static IEnumerable<TResult> Parse<TSource, TResult>(this IEnumerable<TSource> source, Func<ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>, ParserQueryContext<TSource, TSource, IParser<TSource, IEnumerable<TResult>>>> grammarSelector) { return null; }
}

class Program
{
    static void Main(string[] args)
    {
        var arr = new byte[] { 0xff, 0xff, 0x00, 0xc9, 0xff, 0xfe, 0xfe, 0xff, 0xff };
        var result = arr./*<bind>*/Parse/*</bind>*/(parser =>
                                   from next in parser
                                   let val1 = next.Where(v => v == 0xff)
                                   let val2 = next.Where(v => v == 0xfe)
                                   let end = val1.Exactly(2)
                                   let packet = from _1 in val1.OneOrMore()
                                                from r in next.Not(val2).And(next.Not(end).NoneOrMore())
                                                from _2 in end
                                                select r
                                   select packet.ToList()
                         );
    }
}

";
            CreateCompilationWithMscorlib40(source, references: new[] { TestMetadata.Net40.SystemCore }).VerifyDiagnostics();
        }

        [WorkItem(543691, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543691")]
        [Fact]
        public void Bug()
        {
            string source = @"
class Program
{
    static void M<T>(T? t1, T t2) where T : struct 
    {
        System.Console.WriteLine(typeof(T));
    }
    static void Main()
    {
        M((char?)null, (int)0);
    }
}

";
            CreateCompilation(source).VerifyDiagnostics();
        }

        [WorkItem(649800, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/649800")]
        [Fact]
        public void InferringVoid()
        {
            // inference off a void method should fail
            var source = @"
public class Test
{
    static void M<T>(T t) { }
    static void M1<T>(ref T t) { }
    public static void Main()
    {
        M(Main());
        M1(ref Main());
    }
 
}
";
            CreateCompilation(source).VerifyDiagnostics(
                // (8,9): error CS0411: The type arguments for method 'Test.M<T>(T)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //         M(Main());
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "M").WithArguments("Test.M<T>(T)"),
                // (9,16): error CS1510: A ref or out argument must be an assignable variable
                //         M1(ref Main());
                Diagnostic(ErrorCode.ERR_RefLvalueExpected, "Main()")
            );
        }

        [WorkItem(717264, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/717264")]
        [Fact]
        public void SubstitutedMethod()
        {
            var source = @"
using System;

public class C<T>
{
    public void M<U>(Func<T, U> f1, Func<long, U> f2) { }

    void Test(C<char> c)
    {
        c.M(x => x, y => 'a');
    }
}
";
            var comp = CreateCompilation(source);
            comp.VerifyDiagnostics();

            var tree = comp.SyntaxTrees.Single();
            var model = comp.GetSemanticModel(tree);

            var syntax = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();

            var method = (IMethodSymbol)model.GetSymbolInfo(syntax).Symbol;
            Assert.Equal(SpecialType.System_Char, method.TypeArguments.Single().SpecialType);
            Assert.Equal("void C<System.Char>.M<System.Char>(System.Func<System.Char, System.Char> f1, System.Func<System.Int64, System.Char> f2)", method.ToTestDisplayString());
        }

        [WorkItem(717264, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/717264")]
        [Fact]
        public void SubstitutedMethod_Params()
        {
            var source = @"
using System;

public class C<T>
{
    public void M<U>(Func<T, U> f1, Func<long, U> f2, params int[] a) { }

    void Test(C<char> c)
    {
        c.M(x => x, y => 'a', 1, 2, 3);
    }
}
";
            var comp = CreateCompilation(source);
            comp.VerifyDiagnostics();

            var tree = comp.SyntaxTrees.Single();
            var model = comp.GetSemanticModel(tree);

            var syntax = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();

            var method = (IMethodSymbol)model.GetSymbolInfo(syntax).Symbol;
            Assert.Equal(SpecialType.System_Char, method.TypeArguments.Single().SpecialType);
            Assert.Equal("void C<System.Char>.M<System.Char>(System.Func<System.Char, System.Char> f1, System.Func<System.Int64, System.Char> f2, params System.Int32[] a)", method.ToTestDisplayString());
        }

        [WorkItem(8712, "https://github.com/dotnet/roslyn/issues/8712")]
        [Fact]
        public void EnumerableJoinIntellisenseForParameterTypesShouldPopOutAutoComplete_1()
        {
            var source = @"
using System.Collections.Generic;
using System.Linq;

public class Book
{
    public int AuthorId { get; set; }
    public string Title { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Test
{
    public static void NoIntellisenseInEnumerableJoin()
    {
        IEnumerable<Book> books = null;
        IEnumerable<Author> authors = null;

        var test = books.Join(authors, b => b.    // !!Fails here!!
    }
}";

            var compilation = CreateCSharpCompilation(source);
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var book = (IdentifierNameSyntax)tree.GetRoot().DescendantTokens().Last(t => t.Text == "b").Parent;
            var bookType = model.GetTypeInfo(book).Type;

            Assert.Equal("Book", bookType.Name);
        }

        [WorkItem(8712, "https://github.com/dotnet/roslyn/issues/8712")]
        [Fact]
        public void EnumerableJoinIntellisenseForParameterTypesShouldPopOutAutoComplete_2()
        {
            var source = @"
using System.Collections.Generic;
using System.Linq;

public class Book
{
    public int AuthorId { get; set; }
    public string Title { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Test
{
    public static void NoIntellisenseInEnumerableJoin()
    {
        IEnumerable<Book> books = null;
        IEnumerable<Author> authors = null;

        var test = books.Join(authors, b => b.AuthorId, a => a.    // !!Fails here!!
    }
}";

            var compilation = CreateCSharpCompilation(source);
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var author = (IdentifierNameSyntax)tree.GetRoot().DescendantTokens().Last(t => t.Text == "a").Parent;
            var authorType = model.GetTypeInfo(author).Type;

            Assert.Equal("Author", authorType.Name);
        }

        [WorkItem(8712, "https://github.com/dotnet/roslyn/issues/8712")]
        [Fact]
        public void EnumerableJoinIntellisenseForParameterTypesShouldPopOutAutoComplete_3()
        {
            var source = @"
using System.Collections.Generic;
using System.Linq;

public class Book
{
    public int AuthorId { get; set; }
    public string Title { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Test
{
    public static void NoIntellisenseInEnumerableJoin()
    {
        IEnumerable<Book> books = null;
        IEnumerable<Author> authors = null;

        var test = books.Join(authors, b => b.AuthorId, a => a.Id, (bookResult, authorResult) => new { bookResult, authorResult });
    }
}";

            var compilation = CreateCSharpCompilation(source).VerifyDiagnostics();
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var bookResult = (IdentifierNameSyntax)tree.GetRoot().DescendantTokens().Last(t => t.Text == "bookResult").Parent;
            var bookResultType = model.GetTypeInfo(bookResult).Type;
            Assert.Equal("Book", bookResultType.Name);

            var authorResult = (IdentifierNameSyntax)tree.GetRoot().DescendantTokens().Last(t => t.Text == "authorResult").Parent;
            var authorResultType = model.GetTypeInfo(authorResult).Type;
            Assert.Equal("Author", authorResultType.Name);
        }
        [Fact]
        [Fact]
        public void MethodTypeHints_Simple()
        {
            var source = """
    using System;

    public class Program
    {
        public static void M1()
        {
            GH.F1<int, _>("hi"); // GH.F1<int, string>(string)
            GH.F2<_, _>(1, 1); //  GH.F2<int, int>(int, int)
            GH.F2<_>(1,1); // GH.F2<int>(int,int)
            GH.F2(1,1); // GH.F2<int>(int,int)
            GH.F3<int, _, string, _>(new GC2<byte, short>()); // GH.F3<int, byte, string, short>(GC2<byte, short>)
            GH.F4<_, _, string>(intVal => intVal + 1, x => x.ToString(),str => str.Length); // // GH.F4<int, int, string>(Func<int, int>, Func<int, string>, Func<string, int>)
            GH.F5((byte)1); // GH.F5<byte>(byte)
            GH.F5(1); // GH.F5(int)
            GH.F6<_>(1, new GC1<byte>()); // GH.F6<byte>(int p1, params GC1[] args)
            GH.F6<_>(1, new GC1<byte>(), new GC1<byte>()); // GH.F6<byte>(int p1, params GC1[] args)
        }

        public static void M2() 
        {
            B1<int, _>("hi"); // B1<int, string>(string)
            B3<int, _, string, _>(new GC2<byte, short>()); // B3<int, byte, string, short>(GC2<byte, short>)
            B4<_, _, string>(intVal => intVal + 1, x => x.ToString(),str => str.Length); // B4<int, int, string>(Func<int, int>, Func<int, string>, Func<string, int>)

            void B1<T1, T2>(T2 p2) {}

            //Local functions don't have overloading

            void B3<T1, T2, T3, T4>(GC2<T2, T4> p24) {}

            void B4<T1, T2, T3>(Func<T1, T2> p12, Func<T2, T3> p23, Func<T3, T1> p31) {}

            //Local functions don't have overloading
        }
    }

        public class GC2<T1, T2> {}
        public class GC1<T1> {}

        public class GH
        {
            public static void F1<T1, T2>(T2 p2) { }

            public static void F2<T1>(T1 p1, int p2) { }
            public static void F2<T1, T2>(T1 p1, T2 p2) { }

            public static void F3<T1, T2, T3, T4>(GC2<T2, T4> p24) { }

            public static void F4<T1, T2, T3>(Func<T1, T2> p12, Func<T2, T3> p23, Func<T3, T1> p31) { }

            public static void F5<T1>(T1 p1) {}
            public static void F5(int p1) {}

            public static void F6<T>(int p1, params GC1<T>[] args) {}
        }
    """;

            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                    .RegularPreview
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                    );
            compilation.VerifyDiagnostics();
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var methodCalls = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().ToArray();
            string[] callsites = new[] {
                    "void GH.F1<System.Int32, System.String>(System.String p2)",
                    "void GH.F2<System.Int32, System.Int32>(System.Int32 p1, System.Int32 p2)",
                    "void GH.F2<System.Int32>(System.Int32 p1, System.Int32 p2)",
                    "void GH.F2<System.Int32>(System.Int32 p1, System.Int32 p2)",
                    "void GH.F3<System.Int32, System.Byte, System.String, System.Int16>(GC2<System.Byte, System.Int16> p24)",
                    "void GH.F4<System.Int32, System.Int32, System.String>(System.Func<System.Int32, System.Int32> p12, System.Func<System.Int32, System.String> p23, System.Func<System.String, System.Int32> p31)",
                    "System.String System.Int32.ToString()",
                    "void GH.F5<System.Byte>(System.Byte p1)",
                    "void GH.F5(System.Int32 p1)",
                    "void GH.F6<System.Byte>(System.Int32 p1, params GC1<System.Byte>[] args)",
                    "void GH.F6<System.Byte>(System.Int32 p1, params GC1<System.Byte>[] args)",
                    "void B1<System.Int32, System.String>(System.String p2)",
                    "void B3<System.Int32, System.Byte, System.String, System.Int16>(GC2<System.Byte, System.Int16> p24)",
                    "void B4<System.Int32, System.Int32, System.String>(System.Func<System.Int32, System.Int32> p12, System.Func<System.Int32, System.String> p23, System.Func<System.String, System.Int32> p31)",
                    "System.String System.Int32.ToString()"
                };

            CheckCallSites(model, methodCalls, callsites);
        }

        private static void CheckCallSites(SemanticModel model, InvocationExpressionSyntax[] invocations, string[] callsites)
        {
            Assert.True(invocations.Length == callsites.Length);
            for (int i = 0; i < invocations.Length; i++)
                CheckCallSite(model, invocations[i], callsites[i]);
        }

        private static void CheckCallSite(SemanticModel model, InvocationExpressionSyntax invocation, string callsite)
        {
            Assert.Null(model.GetDeclaredSymbol(invocation));
            Assert.NotNull(model.GetTypeInfo(invocation).Type);
            var symbol = model.GetSymbolInfo(invocation).Symbol as Symbols.PublicModel.MethodSymbol;
            var text = symbol.ToTestDisplayString(includeNonNullable: true);
            Assert.NotNull(symbol);
            Assert.Equal(callsite, symbol.ToTestDisplayString(includeNonNullable: true));
        }

        private static void CheckCallSites(SemanticModel model, ObjectCreationExpressionSyntax[] invocations, string[] callsites)
        {
            Assert.True(invocations.Length == callsites.Length);
            for (int i = 0; i < invocations.Length; i++)
                CheckCallSite(model, invocations[i], callsites[i]);
        }

        private static void CheckCallSite(SemanticModel model, ObjectCreationExpressionSyntax invocation, string callsite)
        {
            Assert.Null(model.GetDeclaredSymbol(invocation));
            Assert.NotNull(model.GetTypeInfo(invocation).Type);
            var symbol = model.GetSymbolInfo(invocation).Symbol as Symbols.PublicModel.MethodSymbol;
            Assert.NotNull(symbol);
            Assert.Equal(callsite, symbol.ToTestDisplayString(includeNonNullable: true));
        }

        [Fact]
        public void MethodTypeHints_Nested()
        {
            var source = """
    public class Program
    {
        public static void M1()
        {
            B1<int> temp = new B1<int>();
            B2<int, string> temp2 = new B2<int, string>();
            C2<int, B> temp3 = new C2<int, B>();

            F1<A1<_>>(temp); // F1<A1<int>>(A1<int>)
            F1<A2<_, string>>(temp2); // F1<A2<int, string>>
            F1<I2<_, A>>(temp3); // F1<I2<int, A>>(I2<int, A>)
        } 

        public static void F1<T1>(T1 p1) {}
    }

    public class A {}
    public class B : A{}
    public class A1<T> {}
    public class A2<T1, T2> {}
    public class B1<T> : A1<T> {}
    public class B2<T1, T2> : A2<T1, T2> {}
    public interface I2<in T1, out T2> {}
    public class C2<T1, T2> : I2<T1, T2> {}
    """;

            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                    .RegularPreview
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                    );
            compilation.VerifyDiagnostics();
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var methodCalls = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().ToArray();
            string[] callsites = new[] {
                    "void Program.F1<A1<System.Int32>>(A1<System.Int32> p1)",
                    "void Program.F1<A2<System.Int32, System.String>>(A2<System.Int32, System.String> p1)",
                    "void Program.F1<I2<System.Int32, A>>(I2<System.Int32, A> p1)"
                };

            CheckCallSites(model, methodCalls, callsites);
        }

        [Fact]
        public void MethodTypeHints_Nullability()
        {
            var source = """
    #nullable enable

    public class Program
    {
        public static void M1()
        {
            string? temp1 = null;
            string temp2 = "";
            C2<int, string> temp3 = new C2<int, string>();
            C2<int, string?> temp4 = new C2<int, string?>();

            F1<int, _>(temp1); // F1<int, string>(string?)
            F1<int, _>(temp2); // F1<int, string>(string?)
            F1<int?, _>(temp1); // F1<int, string>(string?)
            F1<int?, _>(temp2); // F1<int, string>(string?)
            F2<int, _>(temp1); // F2<int, string?>(string?)
            F2<int, _>(temp2); // F2<int, string>(string)
            F2<int?, _>(temp1); // F2<int, string?>(string?)
            F2<int?, _>(temp2); // F2<int?, string>(string)


            F<I2<_, string?>>(temp3); // F<I2<int, string?>>(C2<int, string?>)
            F<C2<_, string?>>(temp3); // Error: Can't infer
            F<I2<_, _>>(temp3); // F<I2<int, string!>>(C2<int, string!>)
            F<C2<_, _>>(temp3); // F<C2<int, string!>>(C2<int, string!>)
            F<I2<_, _>>(temp4); // F<I2<int, string?>>(C2<int, string?>)
            F<C2<_, _>>(temp4); // F<C2<int, string?>>(C2<int, string?>)
            F<I2<_, string>>(temp4); // Error: Can't infer
            F<C2<_, string>>(temp4); // Error: Can't infer
        } 

        public static void F1<T1, T2>(T2? p2) { }
        public static void F2<T1, T2>(T2 p2) { }
        public static void F<T1>(T1 p1) {}
        public static void F<T1>(T1 p1, T1 p2) {}
    }

    public class A {}
    public interface I2<in T1, out T2> {}
    public class C2<T1, T2> : I2<T1, T2> {}
    """;
            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                    .RegularPreview
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                    );
            compilation.VerifyDiagnostics(new[] {
                // (23,27): warning CS8620: Argument of type 'C2<int, string>' cannot be used for parameter 'p1' of type 'C2<int, string?>' in 'void Program.F<C2<int, string?>>(C2<int, string?> p1)' due to differences in the nullability of reference types.
                //         F<C2<_, string?>>(temp3); // Error: Can't infer
                Diagnostic(ErrorCode.WRN_NullabilityMismatchInArgument, "temp3").WithArguments("C2<int, string>", "C2<int, string?>", "p1", "void Program.F<C2<int, string?>>(C2<int, string?> p1)").WithLocation(23, 27),
                // (28,26): warning CS8620: Argument of type 'C2<int, string?>' cannot be used for parameter 'p1' of type 'I2<int, string>' in 'void Program.F<I2<int, string>>(I2<int, string> p1)' due to differences in the nullability of reference types.
                //         F<I2<_, string>>(temp4); // Error: Can't infer
                Diagnostic(ErrorCode.WRN_NullabilityMismatchInArgument, "temp4").WithArguments("C2<int, string?>", "I2<int, string>", "p1", "void Program.F<I2<int, string>>(I2<int, string> p1)").WithLocation(28, 26),
                // (29,26): warning CS8620: Argument of type 'C2<int, string?>' cannot be used for parameter 'p1' of type 'C2<int, string>' in 'void Program.F<C2<int, string>>(C2<int, string> p1)' due to differences in the nullability of reference types.
                //         F<C2<_, string>>(temp4); // Error: Can't infer
                Diagnostic(ErrorCode.WRN_NullabilityMismatchInArgument, "temp4").WithArguments("C2<int, string?>", "C2<int, string>", "p1", "void Program.F<C2<int, string>>(C2<int, string> p1)").WithLocation(29, 26)
            });

            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var methodCalls = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().ToArray();
            string[] callsites = new[] {
                    "void Program.F1<System.Int32, System.String!>(System.String? p2)",
                    "void Program.F1<System.Int32, System.String!>(System.String? p2)",
                    "void Program.F1<System.Int32?, System.String!>(System.String? p2)",
                    "void Program.F1<System.Int32?, System.String!>(System.String? p2)",
                    "void Program.F2<System.Int32, System.String?>(System.String? p2)",
                    "void Program.F2<System.Int32, System.String!>(System.String! p2)",
                    "void Program.F2<System.Int32?, System.String?>(System.String? p2)",
                    "void Program.F2<System.Int32?, System.String!>(System.String! p2)",
                    "void Program.F<I2<System.Int32, System.String?>!>(I2<System.Int32, System.String?>! p1)",
                    "void Program.F<C2<System.Int32, System.String?>!>(C2<System.Int32, System.String?>! p1)",
                    "void Program.F<I2<System.Int32, System.String!>!>(I2<System.Int32, System.String!>! p1)",
                    "void Program.F<C2<System.Int32, System.String!>!>(C2<System.Int32, System.String!>! p1)",
                    "void Program.F<I2<System.Int32, System.String?>!>(I2<System.Int32, System.String?>! p1)",
                    "void Program.F<C2<System.Int32, System.String?>!>(C2<System.Int32, System.String?>! p1)",
                    "void Program.F<I2<System.Int32, System.String!>!>(I2<System.Int32, System.String!>! p1)",
                    "void Program.F<C2<System.Int32, System.String!>!>(C2<System.Int32, System.String!>! p1)"
                };

            CheckCallSites(model, methodCalls, callsites);
        }

        [Fact]
        public void MethodTypeHints_Dynamic()
        {
            var source = """
    public class Program
    {
        public static void M1()
        {
            dynamic temp = "";
            Foo<string,_>("", temp, 1); // Not checked because of dynamic
            Foo<_, string>(1, temp, 1); // Not checked because of dynamic
            temp.Foo<string, _>(temp);  // _ is threated as type because we can't do partial type inference
        }

        public static void Foo<T1, T2>(T1 p1, T2 p2, T1 p3) {}
    }
    """;

            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                    .RegularPreview
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                    );
            compilation.VerifyDiagnostics( new[] {
                // (8,26): error CS0246: The type or namespace name '_' could not be found (are you missing a using directive or an assembly reference?)
                //         temp.Foo<string, _>(temp); 
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "_").WithArguments("_").WithLocation(8, 26)
            });
        }

        [Fact]
        public void MethodTypeHints_NotInferredRecovery()
        {
            var source = """
    #nullable enable

    public class Program 
    {
        public void M() 
        {
            GH.F1<_,_>(""); // Error: can't infer T1
            GH.F1<_,int>(""); // Error: T2 == int
            GH.F1<_,byte>(257); // Error: T2 = byte
        }
    }

    public class GH
    {
        public static void F1<T1, T2>(T2 p2) { }
    }
    """;

            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                    .RegularPreview
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                    .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                    );
            compilation.VerifyDiagnostics(new[] {
                // (7,12): error CS0411: The type arguments for method 'GH.F1<T1, T2>(T2)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //         GH.F1<_,_>(""); // Error: can't infer T1
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "F1<_,_>").WithArguments("GH.F1<T1, T2>(T2)").WithLocation(7, 12),
                // (8,12): error CS0411: The type arguments for method 'GH.F1<T1, T2>(T2)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //         GH.F1<_,int>(""); // Error: T2 == int
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "F1<_,int>").WithArguments("GH.F1<T1, T2>(T2)").WithLocation(8, 12),
                // (9,12): error CS0411: The type arguments for method 'GH.F1<T1, T2>(T2)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
                //         GH.F1<_,byte>(257); // Error: T2 = byte
                Diagnostic(ErrorCode.ERR_CantInferMethTypeArgs, "F1<_,byte>").WithArguments("GH.F1<T1, T2>(T2)").WithLocation(9, 12)
            });
        }

        [Fact]
        public void MethodTypeHints_Target()
        {
            var source = """
    using System;

    public class Program 
    {
        public void M() 
        {
            int temp1 = Foo1(); // Inferred: Foo1<int>()
            Bar1(Foo1()); // Inferred: Bar1(Foo1<int>())
            B<int> temp2 = Foo2(); // Inferred: Foo2<A<int>>()
            //G<int, _> temp3 = Foo3<_, int>(); // Can't be inferred Foo3 can't be inferred without target and target doesn't have natural type
            //Bar3(new(), "");
            Bar2(Foo1(), (A<int>)null); // Inferred: Bar2<int>(Foo1<int>(), (List<int>)null) 
        }
    
       

        T Foo1<T>() {throw new NotImplementedException();}
        A<T> Foo2<T>() {throw new NotImplementedException();}
        G<T1,T2> Foo3<T1, T2>() {throw new NotImplementedException();}

        void Bar1(int p1) {}
        void Bar2<T>(T p1, B<T> p2) {}
        void Bar3<T>(T p1, T p2) {}

        class G<T1, T2> {}
        class A<T> : B<T> {}
        class B<T> {}
    }
    """;

            var compilation = CreateCSharpCompilation(source, parseOptions: TestOptions
                            .RegularPreview
                            .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceInVariableDecl))
                            .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceMethodInvocation))
                            .WithFeature(nameof(MessageID.IDS_FeaturePartialTypeInferenceObjectCreation))
                            );
            compilation.VerifyDiagnostics();
        }
    }
}
