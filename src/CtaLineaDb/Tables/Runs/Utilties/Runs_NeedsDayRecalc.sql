CREATE TABLE [dbo].[Runs_NeedsDayRecalc]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
)

GO

CREATE TRIGGER [dbo].[trg_ai_Runs_NeedsDayRecalc]
    ON [dbo].[Runs_NeedsDayRecalc]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        DELETE FROM dbo.Budgets WHERE BudgetType = 'LAST CALC'
    END