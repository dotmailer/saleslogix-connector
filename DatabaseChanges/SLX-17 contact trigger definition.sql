CREATE TRIGGER [EMCONTACTDELETIONTRIGGER]
ON [CONTACT]
FOR DELETE
AS

SET NOCOUNT ON /*Prevents an NHibernate error*/
DELETE FROM EMADDRESSBOOKMEMBER
	WHERE
		SLXCONTACTID IN (SELECT CONTACTID FROM DELETED) AND
		SLXMEMBERTYPE = 'Contact';		