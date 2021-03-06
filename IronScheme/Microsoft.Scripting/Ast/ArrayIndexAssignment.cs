/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;

namespace Microsoft.Scripting.Ast {
    public class ArrayIndexAssignment : Expression {
        private readonly Expression /*!*/ _array;
        private readonly Expression /*!*/ _index;
        private readonly Expression /*!*/ _value;
        private readonly Type /*!*/ _elementType;

        internal ArrayIndexAssignment(Expression /*!*/ array, Expression /*!*/ index, Expression /*!*/ value)
            : base(AstNodeType.ArrayIndexAssignment) {
            _array = array;
            _index = index;
            _value = value;
            _elementType = array.Type.GetElementType();
        }

        public Expression Array {
            get { return _array; }
        }

        public Expression Index {
            get { return _index; }
        }

        public Expression Value {
            get { return _value; }
        }

        public override Type Type {
            get {
                return typeof(void);
            }
        }

        public override void Emit(CodeGen cg) {
            //_value.Emit(cg);

            // Save the expression value - order of evaluation is different than that of the Stelem* instruction
            //Slot temp = cg.GetLocalTmp(_elementType);
            //temp.EmitSet(cg);

            // Emit the array reference
            _array.Emit(cg);
            // Emit the index (as integer)
            _index.Emit(cg);
            // Emit the value
            _value.EmitAs(cg, _elementType);
            //temp.EmitGet(cg);
            // Store it in the array
            EmitLocation(cg);
            cg.EmitStoreElement(_elementType);
            
            //temp.EmitGet(cg); //DO NOT WANT!!!
            //cg.FreeLocalTmp(temp);
        }
    }

    public static partial class Ast {
        public static ArrayIndexAssignment AssignArrayIndex(Expression array, Expression index, Expression value) {
            Contract.RequiresNotNull(array, "array");
            Contract.RequiresNotNull(index, "index");
            Contract.RequiresNotNull(value, "value");
            Contract.Requires(index.Type == typeof(int), "index", "Array index must be an int.");

            Type arrayType = array.Type;
            Contract.Requires(arrayType.IsArray, "array", "Array argument must be array.");
            Contract.Requires(arrayType.GetArrayRank() == 1, "index", "Incorrect number of indices.");
            //Contract.Requires(value.Type == arrayType.GetElementType(), "value", "Value type must match the array element type.");

            return new ArrayIndexAssignment(array, index, value);
        }
    }
}
