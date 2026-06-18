/*

    DCSoft.Expression Numerical Expression Engine

 Nanjing Duchang Information Technology Co., Ltd. 2018 All Rights Reserved
 Company website: http://www.dcwriter.cn

 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DCSoft.Expression
{
    /// <summary>
    /// Variable expression item.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public class DCVariableExpressionItem : DCExpressionItem
    {
        /// <summary>
        /// Initializes the object.
        /// </summary>
        public DCVariableExpressionItem()
        {
        }

        private string _Name = null;
        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// Evaluates the expression and retrieves the variable value from the context.
        /// </summary>
        /// <param name="context">The context object.</param>
        /// <returns>The variable value.</returns>
        public override object Eval(IDCExpressionContext context)
        {
            return context.GetVariableValue(this.Name);
        }
        /// <summary>
        /// Outputs a debug string.
        /// </summary>
        /// <param name="indentLevel">Indent level.</param>
        /// <param name="str">String builder.</param>
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            str.AppendLine(new string(' ', indentLevel * 3) + "VAR:" + this.Name);
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return "VAR:" + this.Name;
        }
    }
}
