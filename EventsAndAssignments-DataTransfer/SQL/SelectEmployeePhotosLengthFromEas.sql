-- Выбрать идентификаторы трудозанятых с размером их фотографий EventsAndAssignments
SELECT DISTINCT EmployeeId, DATALENGTH(Photo) PhotoLength
FROM PuplicEmployeeViews