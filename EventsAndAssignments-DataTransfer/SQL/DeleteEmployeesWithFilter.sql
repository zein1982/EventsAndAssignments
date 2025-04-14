-- Удалить записи о трудозанятых за исключением определенных в EventsAndAssignments
DELETE FROM PuplicEmployeeViews
WHERE EmployeeId NOT IN @employeesNotToRemove