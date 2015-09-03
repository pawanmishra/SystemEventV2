BEGIN TRY
BEGIN TRANSACTION

SET NOCOUNT ON

declare @temp_TimeTracker table (
	UserName nvarchar(100),
	[Date] date,
	MeetingMinutes int,
	ActiveMinutes int,
	IsworkingDay bit
	)

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF

--__USER_PERMISISON_STATEMENTS__

SET QUOTED_IDENTIFIER ON

SET NOCOUNT OFF

DECLARE @stmt nvarchar(MAX)
DECLARE @stmtHistory nvarchar(MAX)

-- merge primary patterns
PRINT '
Merging time tracker details...'

SET NOCOUNT ON

IF OBJECT_ID('tempdb..#temp_TimeTracker') IS NOT null
	DROP TABLE #temp_TimeTracker
SELECT * INTO #temp_TimeTracker FROM @temp_TimeTracker

SET NOCOUNT OFF

SET @stmt = N'
MERGE dbo.TimeTracker AS t
USING (
	SELECT *
	FROM #temp_TimeTracker 
	) AS s ON t.Date = s.Date and t.UserName = s.UserName
WHEN MATCHED THEN
	UPDATE SET t.IsWorkingDay = s.IsWorkingDay, t.MeetingMinutes = s.MeetingMinutes, 
				t.ActiveMinutes = t.ActiveMinutes + s.ActiveMinutes, t.LastUpdate = getdate()
WHEN NOT MATCHED BY TARGET
	THEN INSERT (UserName, [Date], IsWorkingDay, MeetingMinutes, ActiveMinutes, StartTime, LastUpdate) 
		VALUES (s.UserName, s.Date, s.IsWorkingDay, s.MeetingMinutes, s.ActiveMinutes, getdate(), getdate());'

EXEC sp_executesql @stmt

SET @stmtHistory = N'
DECLARE @identity int
SELECT @identity = t.id FROM dbo.TimeTracker t inner join #temp_TimeTracker tt ON
	t.UserName = tt.UserName and t.Date = tt.Date

INSERT INTO dbo.TimeTrackerHistory(ParentId, Date, MeetingMinutes, ActiveMinutes, UserName, LastUpdate)
SELECT @identity, tt.Date, tt.MeetingMinutes, tt.ActiveMinutes, tt.UserName, getdate() FROM #temp_TimeTracker tt;'

EXEC sp_executesql @stmtHistory


COMMIT 

END TRY
BEGIN CATCH
	DECLARE @ERROR_MESSAGE nvarchar(4000)
	SET @ERROR_MESSAGE = ERROR_MESSAGE()
	RAISERROR(@ERROR_MESSAGE, 16, 1)

	IF @@TRANCOUNT > 0 ROLLBACK
END CATCH

IF OBJECT_ID('tempdb..#temp_TimeTracker') IS NOT null
	DROP TABLE #temp_TimeTracker

