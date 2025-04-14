-- Выбрать сведения о записи трудозанятого (предполагается использование для проверки актуальности записей)
SELECT
    EmployeeId,
    PositionId,
    TabelNumber,
    Domain,
    [Login],
    Email,
    LastName,
    FirstName,
    MiddleName,
    OrganizationCode,
    OrganizationName,
    PositionCode,
    PositionName,
    DepartmentCode,
    DepartmentName,
    Occupation
FROM publicViewSchema.PuplicEmployeeView