CREATE PROCEDURE sp_ValidateUser
    @UserName NVARCHAR(100),
    @PlainPassword NVARCHAR(200),
    @TenantId INT,
    @IsValid BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StoredHash VARBINARY(64);
    DECLARE @StoredSalt VARBINARY(32);

    SELECT @StoredHash = HASHPASSWORD, @StoredSalt = SALTPASSWORD
    FROM [USER]
    WHERE USERNAME = @UserName AND TENANTID = @TenantId AND IsDeleted = 0;

    IF @StoredHash IS NULL OR @StoredSalt IS NULL
    BEGIN
        SET @IsValid = 0;
        RETURN;
    END

    DECLARE @ComputedHash VARBINARY(64) = HASHBYTES('SHA2_256', @StoredSalt + CAST(@PlainPassword AS VARBINARY(200)));

    IF @ComputedHash = @StoredHash
        SET @IsValid = 1;
    ELSE
        SET @IsValid = 0;
END

