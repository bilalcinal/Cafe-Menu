CREATE PROCEDURE sp_CreateUser
    @Name NVARCHAR(100),
    @Surname NVARCHAR(100),
    @UserName NVARCHAR(100),
    @PlainPassword NVARCHAR(200),
    @TenantId INT,
    @RoleId INT,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Name IS NULL OR @Surname IS NULL OR @UserName IS NULL OR @PlainPassword IS NULL
    BEGIN
        RAISERROR('Name, Surname, UserName ve PlainPassword parametreleri gereklidir.', 16, 1);
        RETURN -1;
    END

    IF @TenantId IS NULL OR @TenantId <= 0
    BEGIN
        RAISERROR('TenantId geçerli bir değer olmalıdır.', 16, 1);
        RETURN -1;
    END

    IF @RoleId IS NULL OR @RoleId <= 0
    BEGIN
        RAISERROR('RoleId geçerli bir değer olmalıdır.', 16, 1);
        RETURN -1;
    END

    DECLARE @Salt VARBINARY(32) = CRYPT_GEN_RANDOM(32);
    DECLARE @Hash VARBINARY(64) = HASHBYTES('SHA2_256', @Salt + CAST(@PlainPassword AS VARBINARY(200)));

    INSERT INTO [USER] (NAME, SURNAME, USERNAME, HASHPASSWORD, SALTPASSWORD, TENANTID, ROLEID, IsDeleted, CreatedDate, CreatorUserId)
    VALUES (@Name, @Surname, @UserName, @Hash, @Salt, @TenantId, @RoleId, 0, GETUTCDATE(), NULL);

    SET @UserId = SCOPE_IDENTITY();
END

