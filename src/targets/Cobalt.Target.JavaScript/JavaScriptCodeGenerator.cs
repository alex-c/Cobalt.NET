using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Exceptions;
using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.BinaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.UnaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.AbstractSyntaxTree.Types;
using Cobalt.Compiler;
using Cobalt.Compiler.TargetFiles;
using Cobalt.Shared;
using Cobalt.Shared.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cobalt.Target.JavaScript
{
    public class JavaScriptCodeGenerator : ITargetCodeGenerator
    {
        public string Platform { get; } = "JavaScript";

        public TargetProgram GenerateTargetCode(CobaltProgram cobaltProgram)
        {
            TargetProgram program = new TargetProgram("JavaScript");
            string js = GenerateProgramCode(cobaltProgram);
            program.AddFile(new TextFile("index.js", js));
            return program;
        }

        private string GenerateProgramCode(CobaltProgram cobaltProgram)
        {
            StringBuilder builder = new StringBuilder();
            if (cobaltProgram.Code.Statements.Any(s => s is StandardInputStatementNode _))
            {
                builder.Append("const $cobalt_stdio=require('readline').createInterface({input:process.stdin,output:process.stdout});function $cobalt_stdin(questionText){return new Promise((resolve,reject)=>{$cobalt_stdio.question(questionText,(input)=>resolve(input));});}");
            }
            builder.Append("(async function(){");
            foreach (StatementNode statement in cobaltProgram.Code.Statements)
            {
                switch (statement)
                {
                    case VariableDeclarationStatementNode variableDeclaration:
                        GenerateVariableDeclarationCode(builder, variableDeclaration);
                        break;
                    case VariableAssignmentStatementNode variableAssignment:
                        GenerateVariableAssignemntCode(builder, variableAssignment);
                        break;
                    case StandardInputStatementNode standardInputStatement:
                        GenerateStandardInputStatementCode(builder, standardInputStatement);
                        break;
                    case StandardOutputStatementNode standardOutputStatement:
                        GenerateStandardOutputStatementCode(builder, standardOutputStatement);

                        break;
                    default:
                        throw new CompilerException($"Code generation for node of type `{statement.GetType()}` not implemented for platform `{Platform}`.");
                }
            }
            builder.Append("process.exit();})();");
            return builder.ToString();
        }

        private void GenerateVariableDeclarationCode(StringBuilder builder, VariableDeclarationStatementNode variableDeclaration)
        {
            builder.Append($"let {variableDeclaration.Identifier.IdentifierName}");
            if (variableDeclaration.Expression != null)
            {
                builder.Append("=");
                GenerateExpressionCode(builder, variableDeclaration.Expression);
            }
            builder.Append(";");
        }

        private void GenerateVariableAssignemntCode(StringBuilder builder, VariableAssignmentStatementNode variableAssignment)
        {
            builder.Append($"{variableAssignment.Identifier.IdentifierName}=");
            GenerateExpressionCode(builder, variableAssignment.Expression);
            builder.Append(";");
        }

        private void GenerateStandardInputStatementCode(StringBuilder builder, StandardInputStatementNode standardInputStatement)
        {
            string identifier = standardInputStatement.Identifier.IdentifierName;

            // Lookup variable symbol
            Symbol variable = null;
            try
            {
                variable = standardInputStatement.LookupSymbol(identifier);
            }
            catch (UndeclaredIdentifierError)
            {
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` failed looking up symbol `{identifier}`.");
            }

            // Get input with hint about type
            if (variable.Type is VariableTypeSignature variableType)
            {
                switch (variableType.CobaltType)
                {
                    case CobaltType.Boolean:
                        builder.Append($"{identifier}=await $cobalt_stdin('Input <{variableType.CobaltType}>: ') == 'true';");
                        break;
                    case CobaltType.Float:
                        builder.Append($"{identifier}=parseFloat(await $cobalt_stdin('Input <{variableType.CobaltType}>: '));");
                        break;
                    case CobaltType.Integer:
                        builder.Append($"{identifier}=parseInt(await $cobalt_stdin('Input <{variableType.CobaltType}>: '));");
                        break;
                    default:
                        throw new CompilerException($"Unknown Cobalt type `{variableType.CobaltType}` encountered in {MethodBase.GetCurrentMethod().Name}.");
                }
            }
            else
            {
                throw new CompilerException($"Symbol looked up in `{MethodBase.GetCurrentMethod().Name}` for identifier `{identifier}` doesn't have a variable type signature.");
            }
        }

        private void GenerateStandardOutputStatementCode(StringBuilder builder, StandardOutputStatementNode standardOutputStatement)
        {

            builder.Append("console.log(");
            GenerateExpressionCode(builder, standardOutputStatement.Expression);
            builder.Append(");");
        }

        private void GenerateExpressionCode(StringBuilder builder, ExpressionNode expression)
        {
            switch (expression)
            {
                case BinaryExpressionNode binaryExpression:
                    GenerateBinaryExpressionCode(builder, binaryExpression);
                    break;
                case UnaryExpressionNode unaryExpression:
                    GenerateUnaryExpressionCode(builder, unaryExpression);
                    break;
                case SingleLeafExpressionNode singleLeafExpression:
                    GenerateExpressionLeafCode(builder, singleLeafExpression.Leaf);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected an expression node. Node is of type `{expression.GetType()}` instead.");
            }
        }

        private void GenerateBinaryExpressionCode(StringBuilder builder, BinaryExpressionNode binaryExpression)
        {
            // TODO: generate parenthesises only where needed

            // Left operand
            if (binaryExpression.LeftOperand is ExpressionNode leftOperandExpression)
            {
                builder.Append("(");
                GenerateExpressionCode(builder, leftOperandExpression);
                builder.Append(")");
            }
            else
            {
                GenerateExpressionLeafCode(builder, binaryExpression.LeftOperand);
            }

            // Operator
            switch (binaryExpression)
            {
                case AdditionNode _:
                    builder.Append("+");
                    break;
                case SubstractionNode _:
                    builder.Append("-");
                    break;
                case MultiplicationNode _:
                    builder.Append("*");
                    break;
                case DivisionNode _:
                    builder.Append("/");
                    break;
                case LogicalAndNode _:
                    builder.Append("&&");
                    break;
                case LogicalOrNode _:
                    builder.Append("||");
                    break;
                case EqualsComparisonNode _:
                    builder.Append("==");
                    break;
                case NotEqualsComparisonNode _:
                    builder.Append("!=");
                    break;
                case GreaterComparisonNode _:
                    builder.Append(">");
                    break;
                case EqualsOrGreaterComparisonNode _:
                    builder.Append(">=");
                    break;
                case LessComparisonNode _:
                    builder.Append("<");
                    break;
                case EqualsOrLessComparisonNode _:
                    builder.Append("<=");
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected a binary expression node.");
            }

            // Right operand
            if (binaryExpression.RightOperand is ExpressionNode rightOperandExpression)
            {
                builder.Append("(");
                GenerateExpressionCode(builder, rightOperandExpression);
                builder.Append(")");
            }
            else
            {
                GenerateExpressionLeafCode(builder, binaryExpression.RightOperand);
            }
        }

        private void GenerateUnaryExpressionCode(StringBuilder builder, UnaryExpressionNode unaryExpression)
        {
            switch (unaryExpression)
            {
                case ArithmeticNegationNode _:
                    builder.Append("-");
                    break;
                case LogicalNegationNode _:
                    builder.Append("!");
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected an unary expression node.");
            }
            builder.Append("(");
            if (unaryExpression.Operand is ExpressionNode expression)
            {
                GenerateExpressionCode(builder, expression);
            }
            else
            {
                GenerateExpressionLeafCode(builder, unaryExpression.Operand);
            }
            builder.Append(")");
        }

        private void GenerateExpressionLeafCode(StringBuilder builder, AstNode node)
        {
            switch (node)
            {
                case LiteralValueNode literalValue:
                    GenerateLiteralValueCode(builder, literalValue);
                    break;
                case IdentifierNode identifier:
                    builder.Append(identifier.IdentifierName);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected an expression leaf node.");
            }
        }

        private void GenerateLiteralValueCode(StringBuilder builder, LiteralValueNode literalValue)
        {
            switch (literalValue)
            {
                case BooleanValueNode booleanValue:
                    builder.Append(booleanValue.Value);
                    break;
                case FloatValueNode floatValue:
                    builder.Append(floatValue.Value);
                    break;
                case IntegerValueNode integerValue:
                    builder.Append(integerValue.Value);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected a literal value node.");
            }
        }
    }
}
