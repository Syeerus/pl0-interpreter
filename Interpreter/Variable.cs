/**
 * Variable representation within environments.
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
    /// A variable.
    /// Doesn't 
    /// </summary>
    public class Variable
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
        /// <exception cref="ReassignConstantError">Thrown when a constant is reassigned.</exception>
        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (IsConstant)
                {
                    throw new ReassignConstantError("Cannot reassign a constant.");
                }

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
                    // Silently fail.
                    Type = DataType.Invalid;
                    return;
                }

                _value = value;
            }
        }

        /// <summary>
        /// If the variable should be marked as constant.
        /// </summary>
        public readonly bool IsConstant;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value of the variable.</param>
        /// <param name="isConst">If the variable should be marked as constant.</param>
        public Variable(object value = null, bool isConst = false)
        {
            Value = value;
            IsConstant = isConst;
        }
    }
}
