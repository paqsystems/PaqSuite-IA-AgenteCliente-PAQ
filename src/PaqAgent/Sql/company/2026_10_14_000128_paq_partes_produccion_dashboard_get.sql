CREATE OR ALTER PROCEDURE dbo.PAQ_PartesProduccion_DashboardGet
    @Database   NVARCHAR(MAX) = NULL,
    @FechaDesde NVARCHAR(MAX) = NULL,
    @FechaHasta NVARCHAR(MAX) = NULL,
    @IdTurno    NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @fd DATE = TRY_CONVERT(DATE, NULLIF(LTRIM(RTRIM(@FechaDesde)), N''));
    DECLARE @fh DATE = TRY_CONVERT(DATE, NULLIF(LTRIM(RTRIM(@FechaHasta)), N''));
    DECLARE @turno INT = TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(@IdTurno)), N''));

    IF @fd IS NULL
        SET @fd = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    IF @fh IS NULL
        SET @fh = CONVERT(DATE, GETDATE());

    DECLARE @partesAbiertos INT = 0,
            @partesEnviados INT = 0,
            @partesAprobados INT = 0,
            @partesCerrados INT = 0,
            @horasProd FLOAT = 0,
            @horasImprod FLOAT = 0;

    IF OBJECT_ID(N'dbo.PQ_PRD_PARTES_OPERARIO', N'U') IS NULL
    BEGIN
        SELECT
            @partesAbiertos AS partes_abiertos,
            @partesEnviados AS partes_enviados,
            @partesAprobados AS partes_aprobados,
            @partesCerrados AS partes_cerrados,
            @horasProd AS horas_productivas,
            @horasImprod AS horas_improductivas;

        SELECT
            CAST(NULL AS INT) AS id_operario,
            CAST(NULL AS NVARCHAR(200)) AS nombre,
            CAST(NULL AS FLOAT) AS horas_productivas
        WHERE 1 = 0;

        SELECT
            CAST(NULL AS INT) AS id_maquina,
            CAST(NULL AS NVARCHAR(100)) AS codigo,
            CAST(NULL AS FLOAT) AS horas_productivas
        WHERE 1 = 0;

        RETURN;
    END

    SELECT
        @partesAbiertos = SUM(CASE WHEN ESTADO = 0 THEN 1 ELSE 0 END),
        @partesEnviados = SUM(CASE WHEN ESTADO = 1 THEN 1 ELSE 0 END),
        @partesAprobados = SUM(CASE WHEN ESTADO = 2 THEN 1 ELSE 0 END),
        @partesCerrados = SUM(CASE WHEN ESTADO = 3 THEN 1 ELSE 0 END)
    FROM dbo.PQ_PRD_PARTES_OPERARIO
    WHERE FECHA_PARTE >= @fd
      AND FECHA_PARTE <= @fh
      AND (@turno IS NULL OR ID_TURNO = @turno);

    SET @partesAbiertos = ISNULL(@partesAbiertos, 0);
    SET @partesEnviados = ISNULL(@partesEnviados, 0);
    SET @partesAprobados = ISNULL(@partesAprobados, 0);
    SET @partesCerrados = ISNULL(@partesCerrados, 0);

    IF OBJECT_ID(N'dbo.PQ_PRD_PARTES_ENTRADAS', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.PQ_PRD_CONCEPTOS_TIEMPO', N'U') IS NOT NULL
    BEGIN
        SELECT
            @horasProd = ROUND(SUM(CASE WHEN c.ES_PRODUCTIVO = 1 THEN COALESCE(e.MINUTOS, 0) ELSE 0 END) / 60.0, 2),
            @horasImprod = ROUND(SUM(CASE WHEN c.ES_PRODUCTIVO = 0 THEN COALESCE(e.MINUTOS, 0) ELSE 0 END) / 60.0, 2)
        FROM dbo.PQ_PRD_PARTES_ENTRADAS AS e
        INNER JOIN dbo.PQ_PRD_PARTES_OPERARIO AS p ON e.ID_PARTE_OPERARIO = p.ID_PARTE_OPERARIO
        INNER JOIN dbo.PQ_PRD_CONCEPTOS_TIEMPO AS c ON e.ID_CONCEPTO_TIEMPO = c.ID_CONCEPTO_TIEMPO
        WHERE p.FECHA_PARTE >= @fd
          AND p.FECHA_PARTE <= @fh
          AND (@turno IS NULL OR p.ID_TURNO = @turno);

        SET @horasProd = ISNULL(@horasProd, 0);
        SET @horasImprod = ISNULL(@horasImprod, 0);
    END

    SELECT
        @partesAbiertos AS partes_abiertos,
        @partesEnviados AS partes_enviados,
        @partesAprobados AS partes_aprobados,
        @partesCerrados AS partes_cerrados,
        @horasProd AS horas_productivas,
        @horasImprod AS horas_improductivas;

    IF OBJECT_ID(N'dbo.PQ_PRD_PARTES_ENTRADAS', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.PQ_PRD_CONCEPTOS_TIEMPO', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.PQ_SUELD_LEGAJOS', N'U') IS NOT NULL
    BEGIN
        SELECT TOP (5)
            p.ID_OPERARIO AS id_operario,
            LTRIM(RTRIM(CONCAT(COALESCE(leg.APELLIDO, N''), N' ', COALESCE(leg.NOMBRE, N'')))) AS nombre,
            ROUND(SUM(COALESCE(e.MINUTOS, 0)) / 60.0, 2) AS horas_productivas
        FROM dbo.PQ_PRD_PARTES_ENTRADAS AS e
        INNER JOIN dbo.PQ_PRD_PARTES_OPERARIO AS p ON e.ID_PARTE_OPERARIO = p.ID_PARTE_OPERARIO
        INNER JOIN dbo.PQ_PRD_CONCEPTOS_TIEMPO AS c ON e.ID_CONCEPTO_TIEMPO = c.ID_CONCEPTO_TIEMPO
        LEFT JOIN dbo.PQ_SUELD_LEGAJOS AS leg ON p.ID_OPERARIO = leg.ID
        WHERE p.FECHA_PARTE >= @fd
          AND p.FECHA_PARTE <= @fh
          AND c.ES_PRODUCTIVO = 1
          AND (@turno IS NULL OR p.ID_TURNO = @turno)
        GROUP BY p.ID_OPERARIO, leg.APELLIDO, leg.NOMBRE
        ORDER BY SUM(COALESCE(e.MINUTOS, 0)) DESC;
    END
    ELSE
    BEGIN
        SELECT
            CAST(NULL AS INT) AS id_operario,
            CAST(NULL AS NVARCHAR(200)) AS nombre,
            CAST(NULL AS FLOAT) AS horas_productivas
        WHERE 1 = 0;
    END

    IF OBJECT_ID(N'dbo.PQ_PRD_PARTES_ENTRADAS', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.PQ_PRD_CONCEPTOS_TIEMPO', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.PQ_PRD_MAQUINAS', N'U') IS NOT NULL
    BEGIN
        SELECT TOP (5)
            e.ID_MAQUINA AS id_maquina,
            COALESCE(m.CODIGO_MAQUINA, m.NOMBRE, N'Sin máquina') AS codigo,
            ROUND(SUM(COALESCE(e.MINUTOS, 0)) / 60.0, 2) AS horas_productivas
        FROM dbo.PQ_PRD_PARTES_ENTRADAS AS e
        INNER JOIN dbo.PQ_PRD_PARTES_OPERARIO AS p ON e.ID_PARTE_OPERARIO = p.ID_PARTE_OPERARIO
        INNER JOIN dbo.PQ_PRD_CONCEPTOS_TIEMPO AS c ON e.ID_CONCEPTO_TIEMPO = c.ID_CONCEPTO_TIEMPO
        LEFT JOIN dbo.PQ_PRD_MAQUINAS AS m ON e.ID_MAQUINA = m.ID_MAQUINA
        WHERE p.FECHA_PARTE >= @fd
          AND p.FECHA_PARTE <= @fh
          AND c.ES_PRODUCTIVO = 1
          AND e.ID_MAQUINA IS NOT NULL
          AND (@turno IS NULL OR p.ID_TURNO = @turno)
        GROUP BY e.ID_MAQUINA, m.CODIGO_MAQUINA, m.NOMBRE
        ORDER BY SUM(COALESCE(e.MINUTOS, 0)) DESC;
    END
    ELSE
    BEGIN
        SELECT
            CAST(NULL AS INT) AS id_maquina,
            CAST(NULL AS NVARCHAR(100)) AS codigo,
            CAST(NULL AS FLOAT) AS horas_productivas
        WHERE 1 = 0;
    END
END
GO
