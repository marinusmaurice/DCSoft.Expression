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
    /// Expression execution context object.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public interface IDCExpressionContext
    {
        /// <summary>
        /// Executes a function.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="parameters">Parameter list.</param>
        /// <returns>The function return value.</returns>
        object ExecuteFunction(string name, object[] parameters);
        /// <summary>
        /// Gets a variable value.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>The variable value.</returns>
        object GetVariableValue(string name);
    }
}
