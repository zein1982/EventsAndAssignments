-- Получить фото и уменьшенное фото (при наличии) из MIM по идентификатору трудозанятого
SELECT EmployeeId, Photo, PhotoS FROM publicViewSchema.PuplicEmployeeView
WHERE EmployeeId IN @valuesToGet