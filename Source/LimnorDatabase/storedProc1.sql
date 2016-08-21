drop PROCEDURE StoredProc1;
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE StoredProc1
	@Param1 int, 
	@Param2 nvarchar(50) output
AS
BEGIN

	SELECT * FROM Table_1 WHERE [ID] > @Param1;
	SELECT * FROM Table_2;

    SET @Param2 = N'Test return';
END
GO
