using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CRE132.Compiler;

// Injects a budget check into every loop body and before every goto, so an infinite loop in
// student code throws BudgetExceededException instead of freezing the tab - single-threaded
// WebAssembly gives a runaway loop no other way out.
//
// The check is spelled with global:: so a student namespace named CRE132 cannot capture it.
//
// SourceCompiler reports compile errors from the ORIGINAL tree and only executes this
// instrumented one, so nothing injected here can ever shift an error's line number.
public sealed class LoopBudgetRewriter : CSharpSyntaxRewriter
{
    static readonly StatementSyntax Check =
        SyntaxFactory.ParseStatement("global::CRE132.Engine.RunBudget.Step();");

    public static SyntaxTree Instrument(SyntaxTree tree) =>
        tree.WithRootAndOptions(new LoopBudgetRewriter().Visit(tree.GetRoot()), tree.Options);

    public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
    {
        var visited = (WhileStatementSyntax)base.VisitWhileStatement(node)!;
        return visited.WithStatement(Guard(visited.Statement));
    }

    public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
    {
        var visited = (DoStatementSyntax)base.VisitDoStatement(node)!;
        return visited.WithStatement(Guard(visited.Statement));
    }

    public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
    {
        var visited = (ForStatementSyntax)base.VisitForStatement(node)!;
        return visited.WithStatement(Guard(visited.Statement));
    }

    public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
    {
        var visited = (ForEachStatementSyntax)base.VisitForEachStatement(node)!;
        return visited.WithStatement(Guard(visited.Statement));
    }

    public override SyntaxNode? VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
    {
        var visited = (ForEachVariableStatementSyntax)base.VisitForEachVariableStatement(node)!;
        return visited.WithStatement(Guard(visited.Statement));
    }

    // A goto can loop through a label with no loop statement in sight, so the jump itself pays.
    // Wrapping the goto in a block is legal everywhere a statement is.
    public override SyntaxNode? VisitGotoStatement(GotoStatementSyntax node) =>
        SyntaxFactory.Block(Check, node);

    static BlockSyntax Guard(StatementSyntax body) =>
        body is BlockSyntax block
            ? block.WithStatements(block.Statements.Insert(0, Check))
            : SyntaxFactory.Block(Check, body);
}
