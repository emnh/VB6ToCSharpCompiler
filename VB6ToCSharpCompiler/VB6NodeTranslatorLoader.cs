
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public static class VB6NodeTranslatorLoader
    {
        public static IEnumerable<OutToken> Translate(VB6NodeTree nodeTree) {
            if (nodeTree == null)
            {
                throw new ArgumentNullException(nameof(nodeTree));
            }

            var dict = new Dictionary<ContextNodeType, VB6NodeTranslator>();
            
            // dict[ContextNodeType.AmbiguousIdentifierContext] = new VB6NodeTranslatorLogging.AmbiguousIdentifierContext(nodeTree, dict);
            dict[ContextNodeType.AmbiguousIdentifierContext] = new VB6NodeTranslatorLogging.AmbiguousIdentifierContext(nodeTree, dict);
dict[ContextNodeType.AmbiguousKeywordContext] = new VB6NodeTranslatorLogging.AmbiguousKeywordContext(nodeTree, dict);
dict[ContextNodeType.AppActivateStmtContext] = new VB6NodeTranslatorLogging.AppActivateStmtContext(nodeTree, dict);
dict[ContextNodeType.ArgCallContext] = new VB6NodeTranslatorLogging.ArgCallContext(nodeTree, dict);
dict[ContextNodeType.ArgContext] = new VB6NodeTranslatorLogging.ArgContext(nodeTree, dict);
dict[ContextNodeType.ArgDefaultValueContext] = new VB6NodeTranslatorLogging.ArgDefaultValueContext(nodeTree, dict);
dict[ContextNodeType.ArgListContext] = new VB6NodeTranslatorLogging.ArgListContext(nodeTree, dict);
dict[ContextNodeType.ArgsCallContext] = new VB6NodeTranslatorLogging.ArgsCallContext(nodeTree, dict);
dict[ContextNodeType.AsTypeClauseContext] = new VB6NodeTranslatorLogging.AsTypeClauseContext(nodeTree, dict);
dict[ContextNodeType.AttributeStmtContext] = new VB6NodeTranslatorLogging.AttributeStmtContext(nodeTree, dict);
dict[ContextNodeType.BaseTypeContext] = new VB6NodeTranslatorLogging.BaseTypeContext(nodeTree, dict);
dict[ContextNodeType.BeepStmtContext] = new VB6NodeTranslatorLogging.BeepStmtContext(nodeTree, dict);
dict[ContextNodeType.BlockContext] = new VB6NodeTranslatorLogging.BlockContext(nodeTree, dict);
dict[ContextNodeType.BlockIfThenElseContext] = new VB6NodeTranslatorLogging.BlockIfThenElseContext(nodeTree, dict);
dict[ContextNodeType.BlockStmtContext] = new VB6NodeTranslatorLogging.BlockStmtContext(nodeTree, dict);
dict[ContextNodeType.CallContext] = new VB6NodeTranslatorLogging.CallContext(nodeTree, dict);
dict[ContextNodeType.CaseCondElseContext] = new VB6NodeTranslatorLogging.CaseCondElseContext(nodeTree, dict);
dict[ContextNodeType.CaseCondExprContext] = new VB6NodeTranslatorLogging.CaseCondExprContext(nodeTree, dict);
dict[ContextNodeType.CaseCondExprIsContext] = new VB6NodeTranslatorLogging.CaseCondExprIsContext(nodeTree, dict);
dict[ContextNodeType.CaseCondExprToContext] = new VB6NodeTranslatorLogging.CaseCondExprToContext(nodeTree, dict);
dict[ContextNodeType.CaseCondExprValueContext] = new VB6NodeTranslatorLogging.CaseCondExprValueContext(nodeTree, dict);
dict[ContextNodeType.CertainIdentifierContext] = new VB6NodeTranslatorLogging.CertainIdentifierContext(nodeTree, dict);
dict[ContextNodeType.ChDirStmtContext] = new VB6NodeTranslatorLogging.ChDirStmtContext(nodeTree, dict);
dict[ContextNodeType.ChDriveStmtContext] = new VB6NodeTranslatorLogging.ChDriveStmtContext(nodeTree, dict);
dict[ContextNodeType.CloseStmtContext] = new VB6NodeTranslatorLogging.CloseStmtContext(nodeTree, dict);
dict[ContextNodeType.ComparisonOperatorContext] = new VB6NodeTranslatorLogging.ComparisonOperatorContext(nodeTree, dict);
dict[ContextNodeType.ComplexTypeContext] = new VB6NodeTranslatorLogging.ComplexTypeContext(nodeTree, dict);
dict[ContextNodeType.CondExprContext] = new VB6NodeTranslatorLogging.CondExprContext(nodeTree, dict);
dict[ContextNodeType.ConstStmtContext] = new VB6NodeTranslatorLogging.ConstStmtContext(nodeTree, dict);
dict[ContextNodeType.ConstSubStmtContext] = new VB6NodeTranslatorLogging.ConstSubStmtContext(nodeTree, dict);
dict[ContextNodeType.ControlPropertiesContext] = new VB6NodeTranslatorLogging.ControlPropertiesContext(nodeTree, dict);
dict[ContextNodeType.Cp_ControlIdentifierContext] = new VB6NodeTranslatorLogging.Cp_ControlIdentifierContext(nodeTree, dict);
dict[ContextNodeType.Cp_ControlTypeContext] = new VB6NodeTranslatorLogging.Cp_ControlTypeContext(nodeTree, dict);
dict[ContextNodeType.Cp_NestedPropertyContext] = new VB6NodeTranslatorLogging.Cp_NestedPropertyContext(nodeTree, dict);
dict[ContextNodeType.Cp_PropertiesContext] = new VB6NodeTranslatorLogging.Cp_PropertiesContext(nodeTree, dict);
dict[ContextNodeType.Cp_PropertyValueContext] = new VB6NodeTranslatorLogging.Cp_PropertyValueContext(nodeTree, dict);
dict[ContextNodeType.Cp_SinglePropertyContext] = new VB6NodeTranslatorLogging.Cp_SinglePropertyContext(nodeTree, dict);
dict[ContextNodeType.DateStmtContext] = new VB6NodeTranslatorLogging.DateStmtContext(nodeTree, dict);
dict[ContextNodeType.DeclareStmtContext] = new VB6NodeTranslatorLogging.DeclareStmtContext(nodeTree, dict);
dict[ContextNodeType.DeftypeStmtContext] = new VB6NodeTranslatorLogging.DeftypeStmtContext(nodeTree, dict);
dict[ContextNodeType.DeleteSettingStmtContext] = new VB6NodeTranslatorLogging.DeleteSettingStmtContext(nodeTree, dict);
dict[ContextNodeType.DictionaryCallStmtContext] = new VB6NodeTranslatorLogging.DictionaryCallStmtContext(nodeTree, dict);
dict[ContextNodeType.DoLoopStmtContext] = new VB6NodeTranslatorLogging.DoLoopStmtContext(nodeTree, dict);
dict[ContextNodeType.ECS_MemberProcedureCallContext] = new VB6NodeTranslatorLogging.ECS_MemberProcedureCallContext(nodeTree, dict);
dict[ContextNodeType.ECS_ProcedureCallContext] = new VB6NodeTranslatorLogging.ECS_ProcedureCallContext(nodeTree, dict);
dict[ContextNodeType.EndStmtContext] = new VB6NodeTranslatorLogging.EndStmtContext(nodeTree, dict);
dict[ContextNodeType.EnumerationStmt_ConstantContext] = new VB6NodeTranslatorLogging.EnumerationStmt_ConstantContext(nodeTree, dict);
dict[ContextNodeType.EnumerationStmtContext] = new VB6NodeTranslatorLogging.EnumerationStmtContext(nodeTree, dict);
dict[ContextNodeType.EraseStmtContext] = new VB6NodeTranslatorLogging.EraseStmtContext(nodeTree, dict);
dict[ContextNodeType.EventStmtContext] = new VB6NodeTranslatorLogging.EventStmtContext(nodeTree, dict);
dict[ContextNodeType.ExitStmtContext] = new VB6NodeTranslatorLogging.ExitStmtContext(nodeTree, dict);
dict[ContextNodeType.ExplicitCallStmtContext] = new VB6NodeTranslatorLogging.ExplicitCallStmtContext(nodeTree, dict);
dict[ContextNodeType.FieldLengthContext] = new VB6NodeTranslatorLogging.FieldLengthContext(nodeTree, dict);
dict[ContextNodeType.FilecopyStmtContext] = new VB6NodeTranslatorLogging.FilecopyStmtContext(nodeTree, dict);
dict[ContextNodeType.ForEachStmtContext] = new VB6NodeTranslatorLogging.ForEachStmtContext(nodeTree, dict);
dict[ContextNodeType.ForNextStmtContext] = new VB6NodeTranslatorLogging.ForNextStmtContext(nodeTree, dict);
dict[ContextNodeType.FunctionStmtContext] = new VB6NodeTranslatorLogging.FunctionStmtContext(nodeTree, dict);
dict[ContextNodeType.GetStmtContext] = new VB6NodeTranslatorLogging.GetStmtContext(nodeTree, dict);
dict[ContextNodeType.GoToStmtContext] = new VB6NodeTranslatorLogging.GoToStmtContext(nodeTree, dict);
dict[ContextNodeType.ICS_B_MemberProcedureCallContext] = new VB6NodeTranslatorLogging.ICS_B_MemberProcedureCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_B_ProcedureCallContext] = new VB6NodeTranslatorLogging.ICS_B_ProcedureCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_S_DictionaryCallContext] = new VB6NodeTranslatorLogging.ICS_S_DictionaryCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_S_MemberCallContext] = new VB6NodeTranslatorLogging.ICS_S_MemberCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_S_MembersCallContext] = new VB6NodeTranslatorLogging.ICS_S_MembersCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_S_ProcedureOrArrayCallContext] = new VB6NodeTranslatorLogging.ICS_S_ProcedureOrArrayCallContext(nodeTree, dict);
dict[ContextNodeType.ICS_S_VariableOrProcedureCallContext] = new VB6NodeTranslatorLogging.ICS_S_VariableOrProcedureCallContext(nodeTree, dict);
dict[ContextNodeType.IfBlockStmtContext] = new VB6NodeTranslatorLogging.IfBlockStmtContext(nodeTree, dict);
dict[ContextNodeType.IfConditionStmtContext] = new VB6NodeTranslatorLogging.IfConditionStmtContext(nodeTree, dict);
dict[ContextNodeType.IfElseBlockStmtContext] = new VB6NodeTranslatorLogging.IfElseBlockStmtContext(nodeTree, dict);
dict[ContextNodeType.IfElseIfBlockStmtContext] = new VB6NodeTranslatorLogging.IfElseIfBlockStmtContext(nodeTree, dict);
dict[ContextNodeType.ImplicitCallStmt_InBlockContext] = new VB6NodeTranslatorLogging.ImplicitCallStmt_InBlockContext(nodeTree, dict);
dict[ContextNodeType.ImplicitCallStmt_InStmtContext] = new VB6NodeTranslatorLogging.ImplicitCallStmt_InStmtContext(nodeTree, dict);
dict[ContextNodeType.InlineIfThenElseContext] = new VB6NodeTranslatorLogging.InlineIfThenElseContext(nodeTree, dict);
dict[ContextNodeType.KillStmtContext] = new VB6NodeTranslatorLogging.KillStmtContext(nodeTree, dict);
dict[ContextNodeType.LetStmtContext] = new VB6NodeTranslatorLogging.LetStmtContext(nodeTree, dict);
dict[ContextNodeType.LetterrangeContext] = new VB6NodeTranslatorLogging.LetterrangeContext(nodeTree, dict);
dict[ContextNodeType.LineInputStmtContext] = new VB6NodeTranslatorLogging.LineInputStmtContext(nodeTree, dict);
dict[ContextNodeType.LineLabelContext] = new VB6NodeTranslatorLogging.LineLabelContext(nodeTree, dict);
dict[ContextNodeType.LiteralContext] = new VB6NodeTranslatorLogging.LiteralContext(nodeTree, dict);
dict[ContextNodeType.LsetStmtContext] = new VB6NodeTranslatorLogging.LsetStmtContext(nodeTree, dict);
dict[ContextNodeType.MemberCallContext] = new VB6NodeTranslatorLogging.MemberCallContext(nodeTree, dict);
dict[ContextNodeType.MkdirStmtContext] = new VB6NodeTranslatorLogging.MkdirStmtContext(nodeTree, dict);
dict[ContextNodeType.ModuleAttributesContext] = new VB6NodeTranslatorLogging.ModuleAttributesContext(nodeTree, dict);
dict[ContextNodeType.ModuleBlockContext] = new VB6NodeTranslatorLogging.ModuleBlockContext(nodeTree, dict);
dict[ContextNodeType.ModuleBodyContext] = new VB6NodeTranslatorLogging.ModuleBodyContext(nodeTree, dict);
dict[ContextNodeType.ModuleBodyElementContext] = new VB6NodeTranslatorLogging.ModuleBodyElementContext(nodeTree, dict);
dict[ContextNodeType.ModuleConfigElementContext] = new VB6NodeTranslatorLogging.ModuleConfigElementContext(nodeTree, dict);
dict[ContextNodeType.ModuleContext] = new VB6NodeTranslatorLogging.ModuleContext(nodeTree, dict);
dict[ContextNodeType.ModuleHeaderContext] = new VB6NodeTranslatorLogging.ModuleHeaderContext(nodeTree, dict);
dict[ContextNodeType.ModuleOptionsContext] = new VB6NodeTranslatorLogging.ModuleOptionsContext(nodeTree, dict);
dict[ContextNodeType.ModuleReferenceComponentContext] = new VB6NodeTranslatorLogging.ModuleReferenceComponentContext(nodeTree, dict);
dict[ContextNodeType.ModuleReferenceContext] = new VB6NodeTranslatorLogging.ModuleReferenceContext(nodeTree, dict);
dict[ContextNodeType.ModuleReferencesContext] = new VB6NodeTranslatorLogging.ModuleReferencesContext(nodeTree, dict);
dict[ContextNodeType.ModuleReferenceValueContext] = new VB6NodeTranslatorLogging.ModuleReferenceValueContext(nodeTree, dict);
dict[ContextNodeType.NameStmtContext] = new VB6NodeTranslatorLogging.NameStmtContext(nodeTree, dict);
dict[ContextNodeType.OnErrorStmtContext] = new VB6NodeTranslatorLogging.OnErrorStmtContext(nodeTree, dict);
dict[ContextNodeType.OpenStmtContext] = new VB6NodeTranslatorLogging.OpenStmtContext(nodeTree, dict);
dict[ContextNodeType.OptionBaseStmtContext] = new VB6NodeTranslatorLogging.OptionBaseStmtContext(nodeTree, dict);
dict[ContextNodeType.OptionCompareStmtContext] = new VB6NodeTranslatorLogging.OptionCompareStmtContext(nodeTree, dict);
dict[ContextNodeType.OptionExplicitStmtContext] = new VB6NodeTranslatorLogging.OptionExplicitStmtContext(nodeTree, dict);
dict[ContextNodeType.OptionPrivateModuleStmtContext] = new VB6NodeTranslatorLogging.OptionPrivateModuleStmtContext(nodeTree, dict);
dict[ContextNodeType.OutputList_ExpressionContext] = new VB6NodeTranslatorLogging.OutputList_ExpressionContext(nodeTree, dict);
dict[ContextNodeType.OutputListContext] = new VB6NodeTranslatorLogging.OutputListContext(nodeTree, dict);
dict[ContextNodeType.ParserRuleContext] = new VB6NodeTranslatorLogging.ParserRuleContext(nodeTree, dict);
dict[ContextNodeType.PrintStmtContext] = new VB6NodeTranslatorLogging.PrintStmtContext(nodeTree, dict);
dict[ContextNodeType.PropertyGetStmtContext] = new VB6NodeTranslatorLogging.PropertyGetStmtContext(nodeTree, dict);
dict[ContextNodeType.PropertyLetStmtContext] = new VB6NodeTranslatorLogging.PropertyLetStmtContext(nodeTree, dict);
dict[ContextNodeType.PropertySetStmtContext] = new VB6NodeTranslatorLogging.PropertySetStmtContext(nodeTree, dict);
dict[ContextNodeType.PublicPrivateGlobalVisibilityContext] = new VB6NodeTranslatorLogging.PublicPrivateGlobalVisibilityContext(nodeTree, dict);
dict[ContextNodeType.PublicPrivateVisibilityContext] = new VB6NodeTranslatorLogging.PublicPrivateVisibilityContext(nodeTree, dict);
dict[ContextNodeType.PutStmtContext] = new VB6NodeTranslatorLogging.PutStmtContext(nodeTree, dict);
dict[ContextNodeType.RedimSubStmtContext] = new VB6NodeTranslatorLogging.RedimSubStmtContext(nodeTree, dict);
dict[ContextNodeType.ResumeStmtContext] = new VB6NodeTranslatorLogging.ResumeStmtContext(nodeTree, dict);
dict[ContextNodeType.SaveSettingStmtContext] = new VB6NodeTranslatorLogging.SaveSettingStmtContext(nodeTree, dict);
dict[ContextNodeType.SC_CaseContext] = new VB6NodeTranslatorLogging.SC_CaseContext(nodeTree, dict);
dict[ContextNodeType.SC_CondContext] = new VB6NodeTranslatorLogging.SC_CondContext(nodeTree, dict);
dict[ContextNodeType.SC_CondExprContext] = new VB6NodeTranslatorLogging.SC_CondExprContext(nodeTree, dict);
dict[ContextNodeType.SelectCaseStmtContext] = new VB6NodeTranslatorLogging.SelectCaseStmtContext(nodeTree, dict);
dict[ContextNodeType.SetStmtContext] = new VB6NodeTranslatorLogging.SetStmtContext(nodeTree, dict);
dict[ContextNodeType.StartRuleContext] = new VB6NodeTranslatorLogging.StartRuleContext(nodeTree, dict);
dict[ContextNodeType.SubscriptContext] = new VB6NodeTranslatorLogging.SubscriptContext(nodeTree, dict);
dict[ContextNodeType.SubscriptsContext] = new VB6NodeTranslatorLogging.SubscriptsContext(nodeTree, dict);
dict[ContextNodeType.SubStmtContext] = new VB6NodeTranslatorLogging.SubStmtContext(nodeTree, dict);
dict[ContextNodeType.TypeContext] = new VB6NodeTranslatorLogging.TypeContext(nodeTree, dict);
dict[ContextNodeType.TypeHintContext] = new VB6NodeTranslatorLogging.TypeHintContext(nodeTree, dict);
dict[ContextNodeType.TypeOfStmtContext] = new VB6NodeTranslatorLogging.TypeOfStmtContext(nodeTree, dict);
dict[ContextNodeType.TypeStmt_ElementContext] = new VB6NodeTranslatorLogging.TypeStmt_ElementContext(nodeTree, dict);
dict[ContextNodeType.TypeStmtContext] = new VB6NodeTranslatorLogging.TypeStmtContext(nodeTree, dict);
dict[ContextNodeType.UnloadStmtContext] = new VB6NodeTranslatorLogging.UnloadStmtContext(nodeTree, dict);
dict[ContextNodeType.ValueStmtContext] = new VB6NodeTranslatorLogging.ValueStmtContext(nodeTree, dict);
dict[ContextNodeType.VariableListStmtContext] = new VB6NodeTranslatorLogging.VariableListStmtContext(nodeTree, dict);
dict[ContextNodeType.VariableStmtContext] = new VB6NodeTranslatorLogging.VariableStmtContext(nodeTree, dict);
dict[ContextNodeType.VariableSubStmtContext] = new VB6NodeTranslatorLogging.VariableSubStmtContext(nodeTree, dict);
dict[ContextNodeType.VisibilityContext] = new VB6NodeTranslatorLogging.VisibilityContext(nodeTree, dict);
dict[ContextNodeType.VsAddContext] = new VB6NodeTranslatorLogging.VsAddContext(nodeTree, dict);
dict[ContextNodeType.VsAddressOfContext] = new VB6NodeTranslatorLogging.VsAddressOfContext(nodeTree, dict);
dict[ContextNodeType.VsAmpContext] = new VB6NodeTranslatorLogging.VsAmpContext(nodeTree, dict);
dict[ContextNodeType.VsAndContext] = new VB6NodeTranslatorLogging.VsAndContext(nodeTree, dict);
dict[ContextNodeType.VsAssignContext] = new VB6NodeTranslatorLogging.VsAssignContext(nodeTree, dict);
dict[ContextNodeType.VsDivContext] = new VB6NodeTranslatorLogging.VsDivContext(nodeTree, dict);
dict[ContextNodeType.VsEqContext] = new VB6NodeTranslatorLogging.VsEqContext(nodeTree, dict);
dict[ContextNodeType.VsEqvContext] = new VB6NodeTranslatorLogging.VsEqvContext(nodeTree, dict);
dict[ContextNodeType.VsGeqContext] = new VB6NodeTranslatorLogging.VsGeqContext(nodeTree, dict);
dict[ContextNodeType.VsGtContext] = new VB6NodeTranslatorLogging.VsGtContext(nodeTree, dict);
dict[ContextNodeType.VsICSContext] = new VB6NodeTranslatorLogging.VsICSContext(nodeTree, dict);
dict[ContextNodeType.VsImpContext] = new VB6NodeTranslatorLogging.VsImpContext(nodeTree, dict);
dict[ContextNodeType.VsIsContext] = new VB6NodeTranslatorLogging.VsIsContext(nodeTree, dict);
dict[ContextNodeType.VsLeqContext] = new VB6NodeTranslatorLogging.VsLeqContext(nodeTree, dict);
dict[ContextNodeType.VsLikeContext] = new VB6NodeTranslatorLogging.VsLikeContext(nodeTree, dict);
dict[ContextNodeType.VsLiteralContext] = new VB6NodeTranslatorLogging.VsLiteralContext(nodeTree, dict);
dict[ContextNodeType.VsLtContext] = new VB6NodeTranslatorLogging.VsLtContext(nodeTree, dict);
dict[ContextNodeType.VsMidContext] = new VB6NodeTranslatorLogging.VsMidContext(nodeTree, dict);
dict[ContextNodeType.VsMinusContext] = new VB6NodeTranslatorLogging.VsMinusContext(nodeTree, dict);
dict[ContextNodeType.VsModContext] = new VB6NodeTranslatorLogging.VsModContext(nodeTree, dict);
dict[ContextNodeType.VsMultContext] = new VB6NodeTranslatorLogging.VsMultContext(nodeTree, dict);
dict[ContextNodeType.VsNegationContext] = new VB6NodeTranslatorLogging.VsNegationContext(nodeTree, dict);
dict[ContextNodeType.VsNeqContext] = new VB6NodeTranslatorLogging.VsNeqContext(nodeTree, dict);
dict[ContextNodeType.VsNewContext] = new VB6NodeTranslatorLogging.VsNewContext(nodeTree, dict);
dict[ContextNodeType.VsNotContext] = new VB6NodeTranslatorLogging.VsNotContext(nodeTree, dict);
dict[ContextNodeType.VsOrContext] = new VB6NodeTranslatorLogging.VsOrContext(nodeTree, dict);
dict[ContextNodeType.VsPlusContext] = new VB6NodeTranslatorLogging.VsPlusContext(nodeTree, dict);
dict[ContextNodeType.VsPowContext] = new VB6NodeTranslatorLogging.VsPowContext(nodeTree, dict);
dict[ContextNodeType.VsStructContext] = new VB6NodeTranslatorLogging.VsStructContext(nodeTree, dict);
dict[ContextNodeType.VsTypeOfContext] = new VB6NodeTranslatorLogging.VsTypeOfContext(nodeTree, dict);
dict[ContextNodeType.VsXorContext] = new VB6NodeTranslatorLogging.VsXorContext(nodeTree, dict);
dict[ContextNodeType.WhileWendStmtContext] = new VB6NodeTranslatorLogging.WhileWendStmtContext(nodeTree, dict);
dict[ContextNodeType.WithStmtContext] = new VB6NodeTranslatorLogging.WithStmtContext(nodeTree, dict);
dict[ContextNodeType.WriteStmtContext] = new VB6NodeTranslatorLogging.WriteStmtContext(nodeTree, dict);

            
            if (!Enum.TryParse(nodeTree.GetRoot().GetType().Name, out ContextNodeType contextNodeType))
            {
                throw new ArgumentException("contextNodeType");
            }
            return dict[contextNodeType].Translate(nodeTree.GetChildren(nodeTree.GetRoot()));
        }
    }
}
