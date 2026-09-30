CREATE OR ALTER PROCEDURE dbo.PAQ_ParametrosGral_Update
    @Programa NVARCHAR(50),
    @Clave NVARCHAR(50),
    @Valor NVARCHAR(MAX),
    @IdempotencyKey NVARCHAR(64),
    @CompanyId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Tipo CHAR(1);

    SELECT @Tipo = tipo_valor
    FROM dbo.PQ_PARAMETROS_GRAL
    WHERE Programa = @Programa
      AND Clave = @Clave;

    IF @Tipo IS NULL
    BEGIN
        SELECT
            CAST('NOT_FOUND' AS NVARCHAR(64)) AS resultCode,
            CAST(N'Parámetro no encontrado.' AS NVARCHAR(4000)) AS errorMessage;
        RETURN;
    END;

    IF @Tipo = 'B'
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_Bool = CASE LOWER(@Valor)
            WHEN 'true' THEN 1
            WHEN '1' THEN 1
            WHEN 'false' THEN 0
            WHEN '0' THEN 0
            ELSE NULL
        END
        WHERE Programa = @Programa AND Clave = @Clave;
    ELSE IF @Tipo = 'I'
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_Int = TRY_CONVERT(INT, @Valor)
        WHERE Programa = @Programa AND Clave = @Clave;
    ELSE IF @Tipo = 'N'
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_Decimal = TRY_CONVERT(DECIMAL(24, 6), @Valor)
        WHERE Programa = @Programa AND Clave = @Clave;
    ELSE IF @Tipo = 'D'
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_DateTime = TRY_CONVERT(DATETIME, @Valor)
        WHERE Programa = @Programa AND Clave = @Clave;
    ELSE IF @Tipo = 'T'
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_Text = @Valor
        WHERE Programa = @Programa AND Clave = @Clave;
    ELSE
        UPDATE dbo.PQ_PARAMETROS_GRAL
        SET Valor_String = @Valor
        WHERE Programa = @Programa AND Clave = @Clave;

    SELECT
        CAST('OK' AS NVARCHAR(64)) AS resultCode,
        Programa AS programa,
        Clave AS clave,
        tipo_valor AS tipoValor,
        CASE tipo_valor
            WHEN 'B' THEN CONVERT(NVARCHAR(10), Valor_Bool)
            WHEN 'I' THEN CONVERT(NVARCHAR(64), Valor_Int)
            WHEN 'N' THEN CONVERT(NVARCHAR(64), Valor_Decimal)
            WHEN 'D' THEN CONVERT(NVARCHAR(33), Valor_DateTime, 126)
            WHEN 'T' THEN Valor_Text
            ELSE Valor_String
        END AS valor
    FROM dbo.PQ_PARAMETROS_GRAL
    WHERE Programa = @Programa
      AND Clave = @Clave;
END;
