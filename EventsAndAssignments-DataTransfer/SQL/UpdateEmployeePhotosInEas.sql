-- Обновить фото и уменьшенное фото (при наличии) трудозанятого в EventsAndAssignments
UPDATE PuplicEmployeeViews SET
    Photo = @photo,
    PhotoS = @smallPhoto
WHERE
    EmployeeId = @employeeId