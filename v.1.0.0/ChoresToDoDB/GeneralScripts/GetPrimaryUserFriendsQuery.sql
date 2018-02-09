USE ChoresToDoDB
DECLARE @PrimaryUserID nvarchar(450)
SET @PrimaryUserID = '<AccountUser.Id>'
SELECT a.ID, a.FirstName, a.MiddleName, a.LastName
FROM AccountUsers a
WHERE a.Id = @PrimaryUserID

-- Find the Primary User friends
SELECT b.Id ,b.FirstName, b.MiddleName, b.LastName
FROM AccountAssociations a INNER JOIN AccountUsers b ON a.AssociatedUserID = b.Id
WHERE a.IsChild = 0 AND a.PrimaryUserID = @PrimaryUserID

-- Find the Primary User's children
SELECT b.Id , b.FirstName, b.MiddleName, b.LastName
FROM AccountAssociations a INNER JOIN AccountUsers b ON a.AssociatedUserID = b.Id
WHERE a.IsChild = 1 AND a.PrimaryUserID = @PrimaryUserID


