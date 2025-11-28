DECLARE @RC int
DECLARE @Name nvarchar(100) = 'Admin'
DECLARE @Surname nvarchar(100) = 'User'
DECLARE @UserName nvarchar(100) = 'admin'
DECLARE @PlainPassword nvarchar(200) = 'Admin123!'
DECLARE @TenantId int = 1
DECLARE @RoleId int = 1
DECLARE @UserId int

EXECUTE @RC = [dbo].[sp_CreateUser] 
   @Name,
   @Surname,
   @UserName,
   @PlainPassword,
   @TenantId,
   @RoleId,
   @UserId OUTPUT

SELECT @UserId AS CreatedUserId, @RC AS ReturnCode

