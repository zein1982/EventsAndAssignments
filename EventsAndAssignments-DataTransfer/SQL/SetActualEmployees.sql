-- Пометить указанны записи труозанятых, как актуальные
UPDATE PuplicEmployeeViews SET IsActive = 1 WHERE PositionId IN @actualEmployees