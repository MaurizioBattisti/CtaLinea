CREATE VIEW [dbo].[vw_RunFirstTags]
AS 
WITH CTE_Tags AS
(
	SELECT rt.RunId,
			t.TagId,
			t.TagName,
			t.BgColor,
			t.Color,
			ROW_NUMBER() OVER (PARTITION BY rt.RunId ORDER BY t.Ordinal, t.TagName,  t.TAgId) AS Number
		FROM dbo.RunTags rt
		INNER JOIN dbo.Tags t
			ON rt.TagId = t.TagId
)
SELECT t.RunId,
		t.TagId,
		t.TagName,
		t.BgColor,
		t.Color
	FROM CTE_Tags t
	WHERE t.Number = 1

