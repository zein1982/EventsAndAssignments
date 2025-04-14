-- Выбрать идентификаторы трудозанятых с размером их фотографий из MIM 
SELECT DISTINCT EmployeeId, DATALENGTH(Photo) PhotoLength
FROM publicViewSchema.PuplicEmployeeView