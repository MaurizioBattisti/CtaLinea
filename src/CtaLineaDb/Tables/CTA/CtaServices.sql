CREATE TABLE [dbo].[CtaServices]
(
	[ServiceId] INT NOT NULL PRIMARY KEY, 
    [RunTypeId] NCHAR(10) NULL,

    [CategoryId] VARCHAR(10) NULL, 
    [SubCategoryId] VARCHAR(20) NULL, 

    [Note] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Cta_Service_tt] 
		FOREIGN KEY (ServiceId) 
		REFERENCES [dbo].[TtServices](ServiceId)
		ON DELETE CASCADE, 
    CONSTRAINT [FK_CtaServices_Category] 
        FOREIGN KEY (CategoryId) 
        REFERENCES dbo.SheetCategories(CategoryId)
        ON DELETE NO ACTION
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_CtaServices_SubCategory] 
        FOREIGN KEY (SubCategoryId) 
        REFERENCES dbo.SheetSubCategories(SubCategoryId)
        ON DELETE NO ACTION
        ON UPDATE CASCADE

)
