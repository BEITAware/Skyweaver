namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public enum SessionFlowNodeKind
    {
        UserInput = 0,
        Return = 1,
        Agent = 2,
        LogicAnd = 3,
        LogicOr = 4,
        LogicXor = 5,
        LogicNot = 6,
        LogicExecution = 7,
        NextLogicExecution = 8
    }
}
