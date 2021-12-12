/**
 * Typed value class.
 * 
 * Copyright (c) 2021 Syeerus
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using Interpreter.Errors;

namespace Interpreter
{
    /// <summary>
    /// Represents a typed value.
    /// </summary>
    public class TypedValue
    {
        /// <summary>
        /// The current value of the object.
        /// </summary>
        private object _value;

        /// <summary>
        /// The current data type.
        /// </summary>
        public DataType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The current value of the object.
        /// </summary>
        /// <exception cref="UnsupportedDataTypeError">Thrown when trying to assign an unsupported data type.</exception>
        public virtual object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value == null)
                {
                    Type = DataType.Invalid;
                }
                else
                {
                    Type type = value.GetType();
                    if (type == typeof(int))
                    {
                        Type = DataType.Integer;
                    }
                    else if (type == typeof(float))
                    {
                        Type = DataType.Float;
                    }
                    else if (type == typeof(string))
                    {
                        Type = DataType.String;
                    }
                    else
                    {
                        throw new UnsupportedDataTypeError("Cannot assign an unsupported data type.");
                    }
                }

                _value = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value to assign.</param>
        /// <exception cref="UnsupportedDataTypeError">Thrown when an unsupported data type is given.</exception>
        public TypedValue(object value = null)
        {
            Value = value;
        }
    }
}
