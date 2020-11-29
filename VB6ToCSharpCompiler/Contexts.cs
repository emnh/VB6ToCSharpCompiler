﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class Contexts
    {
        public string[] GetContexts()
        {
            return @"
AmbiguousIdentifierContext
AppActivateStmtContext
ArgCallContext
ArgContext
ArgDefaultValueContext
ArgsCallContext
AsTypeClauseContext
AttributeStmtContext
BeepStmtContext
BlockIfThenElseContext
CallContext
CaseCondElseContext
CaseCondExprContext
CaseCondExprIsContext
CaseCondExprToContext
CaseCondExprValueContext
CertainIdentifierContext
ChDirStmtContext
ChDriveStmtContext
CloseStmtContext
ConstStmtContext
ConstSubStmtContext
DateStmtContext
DeclareStmtContext
DeftypeStmtContext
DeleteSettingStmtContext
DictionaryCallStmtContext
DoLoopStmtContext
ECS_MemberProcedureCallContext
ECS_ProcedureCallContext
EnumerationStmtContext
EnumerationStmt_ConstantContext
EventStmtContext
ExitStmtContext
ExplicitCallStmtContext
ForEachStmtContext
ForNextStmtContext
FunctionStmtContext
ICS_B_MemberProcedureCallContext
ICS_B_ProcedureCallContext
ICS_S_DictionaryCallContext
ICS_S_MemberCallContext
ICS_S_MembersCallContext
ICS_S_ProcedureOrArrayCallContext
ICS_S_VariableOrProcedureCallContext
IfBlockStmtContext
IfConditionStmtContext
IfElseBlockStmtContext
IfElseIfBlockStmtContext
ImplicitCallStmt_InBlockContext
ImplicitCallStmt_InStmtContext
InlineIfThenElseContext
LetStmtContext
LetterrangeContext
LineLabelContext
LiteralContext
ModuleConfigElementContext
ModuleContext
ModuleHeaderContext
OnErrorStmtContext
OpenStmtContext
OptionBaseStmtContext
OptionCompareStmtContext
OptionExplicitStmtContext
OptionPrivateModuleStmtContext
ParserRuleContext
PrintStmtContext
PropertyGetStmtContext
PropertyLetStmtContext
PropertySetStmtContext
PublicPrivateGlobalVisibilityContext
PublicPrivateVisibilityContext
RedimSubStmtContext
ResumeStmtContext
SC_CaseContext
SC_CondContext
SC_CondExprContext
SaveSettingStmtContext
SelectCaseStmtContext
SetStmtContext
StartRuleContext
SubStmtContext
TypeHintContext
TypeStmtContext
TypeStmt_ElementContext
ValueStmtContext
VariableListStmtContext
VariableStmtContext
VariableSubStmtContext
VisibilityContext
VsAddContext
VsAddressOfContext
VsAmpContext
VsAndContext
VsAssignContext
VsDivContext
VsEqContext
VsEqvContext
VsGeqContext
VsGtContext
VsICSContext
VsImpContext
VsIsContext
VsLeqContext
VsLikeContext
VsLiteralContext
VsLtContext
VsMidContext
VsMinusContext
VsModContext
VsMultContext
VsNegationContext
VsNeqContext
VsNewContext
VsNotContext
VsOrContext
VsPlusContext
VsPowContext
VsStructContext
VsTypeOfContext
VsXorContext
WhileWendStmtContext
WithStmtContext
WriteStmtContext
callContext
condExprContext
constSubStmtContext
ifElseIfBlockStmtContext
memberCallContext
sc_CondExprContext
variableSubStmtContext".Split('\n');
        }
    }
}
