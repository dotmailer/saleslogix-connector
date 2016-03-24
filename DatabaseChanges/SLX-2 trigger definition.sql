CREATE TRIGGER [EMLEADCONVERSIONTRIGGER]
ON [HIST_LEAD]
FOR INSERT, UPDATE
AS

SET NOCOUNT ON /*Prevents an NHibernate error*/
BEGIN TRY
	DECLARE EMLEADCONVERSIONTRIGGERCURSOR CURSOR FOR
	select LEADID, CONTACTID from Inserted;
	declare @leadid varchar(max);
	declare @contactid varchar(max);

	open EMLEADCONVERSIONTRIGGERCURSOR;
	fetch next from EMLEADCONVERSIONTRIGGERCURSOR into @leadid, @contactid
	while @@FETCH_STATUS = 0
	BEGIN
		IF @contactid is not null
		BEGIN
			update emaddressbookmember 
			set
				slxcontactid = @contactid,
				slxmembertype = 'Contact',
				slxleadid = null
			where
				(emaddressbookmember.slxmembertype = 'Lead') and
				(emaddressbookmember.slxleadid = @leadid);
		END
	
		FETCH NEXT FROM EMLEADCONVERSIONTRIGGERCURSOR into @leadid, @contactid;
	END;	
END TRY
BEGIN CATCH
	/*To ensure the cursor is closed if an error occurs*/
END CATCH

CLOSE EMLEADCONVERSIONTRIGGERCURSOR;
DEALLOCATE EMLEADCONVERSIONTRIGGERCURSOR;