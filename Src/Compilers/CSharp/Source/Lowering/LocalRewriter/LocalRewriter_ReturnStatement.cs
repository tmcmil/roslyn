﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitReturnStatement(BoundReturnStatement node)
        {
            BoundStatement rewritten = (BoundStatement)base.VisitReturnStatement(node);

            // NOTE: we will apply sequence points to synthesized return 
            // statements if they are contained in lambdas and have expressions
            // or if they are expression-bodied properties.
            // We do this to ensure that expression lambdas and expression-bodied
            // properties have sequence points.
            if (this.generateDebugInfo &&
                (!rewritten.WasCompilerGenerated || 
                 (node.ExpressionOpt != null && IsLambdaOrExpressionBodiedMember)))
            {
                // We're not calling AddSequencePoint since it ignores compiler-generated nodes.
                return new BoundSequencePoint(rewritten.Syntax, rewritten);
            }

            return rewritten;
        }

        private bool IsLambdaOrExpressionBodiedMember
        {
            get
            {
                if (this.factory.CurrentMethod is LambdaSymbol)
                {
                    return true;
                }
                var property = this.factory.CurrentMethod as SourcePropertyAccessorSymbol;
                return property != null
                    && property.HasExpressionBody;
            }
        }
    }
}
