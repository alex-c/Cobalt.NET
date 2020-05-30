using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.Compiler;
using Cobalt.Compiler.TargetFiles;
using Cobalt.Shared.Exceptions;
using System.Linq;
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
            builder.Append("})();");
            return builder.ToString();
        }

        private void GenerateVariableDeclarationCode(StringBuilder builder, VariableDeclarationStatementNode variableDeclaration)
        {
            builder.Append($"let {variableDeclaration.Identifier.IdentifierName}=");
            GenerateExpressionCode(builder, variableDeclaration.Expression);
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
            // TODO: find symbol type...
            builder.Append($"let {standardInputStatement.Identifier.IdentifierName}=await $cobalt_stdin('Input <TODO:Type>: ');");
        }

        private void GenerateStandardOutputStatementCode(StringBuilder builder, StandardOutputStatementNode standardOutputStatement)
        {

            builder.Append("console.log(");
            GenerateExpressionCode(builder, standardOutputStatement.Expression);
            builder.Append(");");
        }

        private void GenerateExpressionCode(StringBuilder builder, ExpressionNode expression)
        {
            // TODO: implement
        }
    }
}
