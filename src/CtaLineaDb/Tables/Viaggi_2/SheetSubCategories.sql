CREATE TABLE [dbo].[SheetSubCategories]
(
	[SubCategoryId] VARCHAR(20) NOT NULL PRIMARY KEY, 
    [CategoryId] VARCHAR(10) NOT NULL, 
    [Description] VARCHAR(200) NOT NULL, 

    CONSTRAINT [FK_SubCategories] 
        FOREIGN KEY (CategoryId) 
        REFERENCES dbo.[SheetCategories] (CategoryId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
