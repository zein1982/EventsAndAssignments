using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Interfaces;

namespace EventsAndAssignments.Infrastructure
{
    public class ReportService : IReportService
    {
        private readonly IAssignmentsGateway _assignmentGateway;
        private readonly IProtocolGateway _protocolGateway;
        private readonly List<string> _byProtocolReportHeader;
        private readonly int _columnWidth;

        private readonly List<string> _columnNames = new()
        {
            "№",
            "Задача",
            "Ответственный",
            "Срок",
        };

        private readonly IList<string> _protocolExcelHeader = new List<string>
        {
            "№",
            "Задача",
            "Ответственный",
            "Срок",
            "Статус",
            "Комментарий"
        };

        public ReportService(IAssignmentsGateway assignmentGateway, IProtocolGateway protocolGateway, IEmployeeService employeeService)
        {
            _assignmentGateway = assignmentGateway;
            _byProtocolReportHeader = new List<string>
            {
                "Направление совещания",
                "Наименование протокола",
                "№ Поручения",
                "Содержание поручения",
                "Дата создания поручения (в системе)",
                "Дата исполнения",
                "Статус",
                "Просрочка да/нет",
                "Осталось дней",
                "Отв. руководитель (ФИО)",
                "Отв. руководитель (должность)",
                "Отв. исполнитель (ФИО)",
                "Отв. исполнитель (должность)",
                "Контролер (ФИО)",
                "Контролер (должность)",
                "Автор (ФИО)",
                "Автор (должность)",
                "Администратор (ФИО)",
                "Администратор (должность)",
            };
            _protocolGateway = protocolGateway;
            _columnWidth = 60;
        }

        public IReadOnlyCollection<Assignment> GetDataForExcelProtocolReport(long id)
            => _assignmentGateway.GetAssignmentForExcelReport(id);

        public MemoryStream MakeShortReportByAssignments(List<long> ids, int timeDifference)
        {
            List<Assignment> dataForReport = _assignmentGateway.GetAssignmentsForShortReport(ids).ToList();
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Assignments");
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //worksheet.RowHeight = _rowHeight;
            worksheet.ColumnWidth = _columnWidth;
            worksheet.Columns().AdjustToContents();
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Column("A").Width = 10;
            worksheet.Column("B").Width = 90;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 15;
            worksheet.Range("A1:D2").Merge();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 15;
            worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Cell(1, 1).Value = dataForReport[0].Protocol!.Name;

            worksheet.Range($"A3:D{dataForReport.Count + 3}").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A3:D{dataForReport.Count + 3}").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A3:D{dataForReport.Count + 3}").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A3:D{dataForReport.Count + 3}").Style.Border.RightBorder = XLBorderStyleValues.Thin;

            worksheet.Range($"A3:D{dataForReport.Count + 3}").Style.Border.OutsideBorder =
                XLBorderStyleValues.Double;
            for (int indexer = 0; indexer < _columnNames.Count; indexer++)
            {
                worksheet.Cell(3, indexer + 1).Value = _columnNames[indexer];
                worksheet.Cell(3, indexer + 1).Style.Fill.BackgroundColor =
                    XLColor.FromArgb(39, 34, 81);
                worksheet.Cell(3, indexer + 1).Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < dataForReport.Count; i++)
            {
                int rowNumber = i + 4;
                worksheet.Cell(rowNumber, 1).Value = dataForReport[i].Name;
                worksheet.Cell(rowNumber, 2).Value = dataForReport[i].Description;
                worksheet.Cell(rowNumber, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                //Если руководитель не установлен то в поле ставим прочерк, а в поле дата комментарий
                if (dataForReport[i].ResponsibleLeader is null)
                {
                    worksheet.Cell(rowNumber, 3).Value = "-";
                    worksheet.Cell(rowNumber, 4).Value =
                        dataForReport[i].Comments is not null && dataForReport[i].Comments!.Count > 0
                            ? dataForReport[i]
                                .Comments!
                                .Last(e => e.CreatedBy == dataForReport[i].CreatedBy)
                                .Content
                            : "-";
                }
                else
                {
                    worksheet.Cell(rowNumber, 3).Value = dataForReport[i].ResponsibleLeader!.GetInitials();
                    worksheet.Cell(rowNumber, 4).Value = dataForReport[i].ExecutionDate
                        .AddHours(-timeDifference)
                        .ToString("dd.MM.yyyy");
                }
            }

            using MemoryStream stream = new();
            workbook.SaveAs(stream);
            return stream;
        }

        public MemoryStream MakeReportByProtocol(List<long> ids)
        {
            List<Assignment> dataForReport = _protocolGateway.GetDataForByProtocolReport(ids).ToList();
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add(DateTime.UtcNow.ToString("dd.MM.yyyy"));
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Column("D").Width = 70;
            worksheet.Column("B").Width = 30;

            for (int indexer = 0; indexer < 19; indexer++)
            {
                worksheet.Cell(1, indexer + 1).Value = _byProtocolReportHeader[indexer];
                worksheet.Cell(1, indexer + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(142, 169, 219);
                worksheet.Cell(1, indexer + 1).Style.Font.Bold = true;
                worksheet.Cell(1, indexer + 1).Style.Font.FontSize = 14;
            }

            bool colorSwitcher = true;

            for (int i = 0; i < dataForReport.Count; i++)
            {
                int rowNumber = i+2;

                if(colorSwitcher == true)
                {
                    worksheet.Range("A:S").Row(i + 2).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 225, 242);
                    worksheet.Row(i + 2).Height = 60;
                    colorSwitcher = false;
                }
                else
                {
                    worksheet.Row(i + 2).Height = 60;
                    colorSwitcher = true;
                }

                worksheet.Cell(rowNumber, 1).Value = dataForReport[i].Protocol?.Folder?.Name;
                worksheet.Cell(rowNumber, 1).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 2).Value = dataForReport[i].Protocol?.Name;
                worksheet.Cell(rowNumber, 3).Value = dataForReport[i].Name;
                worksheet.Cell(rowNumber, 4).Value = dataForReport[i].Description;
                worksheet.Cell(rowNumber, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell(rowNumber, 5).SetValue (dataForReport[i].Created.Date);
                worksheet.Cell(rowNumber, 6).SetValue(dataForReport[i].ExecutionDate.Date);
                worksheet.Cell(rowNumber, 7).Value = dataForReport[i].Status?.Name;

                if(((DateTime.UtcNow > dataForReport[i].ExecutionDate)
                  && (dataForReport[i].Status?.StatusCode != 7))
                  || (dataForReport[i].CompletionDate > dataForReport[i].ExecutionDate))
                {
                    worksheet.Cell(rowNumber, 8).Value = "Да";
                    worksheet.Cell(rowNumber, 8).Style.Font.SetFontColor(XLColor.Red);
                }
                else
                {
                    worksheet.Cell(rowNumber, 8).Value = "Нет";
                }

                if (Math.Round((dataForReport[i].ExecutionDate - DateTime.UtcNow).TotalDays) > 0)
                {
                    worksheet.Cell(rowNumber, 9).Value = Math.Round((dataForReport[i].ExecutionDate - DateTime.UtcNow).TotalDays);
                }
                else if (dataForReport[i].Status?.StatusCode != 7)
                {
                    worksheet.Cell(rowNumber, 9).Value = Math.Round((dataForReport[i].ExecutionDate - DateTime.UtcNow).TotalDays);
                    worksheet.Cell(rowNumber, 9).Style.Font.SetFontColor(XLColor.Red);
                }
                else if ((int?)(dataForReport[i].ExecutionDate - dataForReport[i].CompletionDate)?.TotalDays < 0)
                {
                    worksheet.Cell(rowNumber, 9).Value = (int?) (dataForReport[i].ExecutionDate - dataForReport[i].CompletionDate)?.TotalDays;
                    worksheet.Cell(rowNumber, 9).Style.Font.SetFontColor(XLColor.Red);
                }
                else
                {
                    worksheet.Cell(rowNumber, 9).Value = (int?)(dataForReport[i].ExecutionDate - dataForReport[i].CompletionDate)?.TotalDays;
                }

                worksheet.Cell(rowNumber, 10).Value = dataForReport[i].ResponsibleLeader is not null
                    ? dataForReport[i].ResponsibleLeader!.GetFullName()
                    : " не установлено ";
                worksheet.Cell(rowNumber, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 11).Value = dataForReport[i].ResponsibleLeader is not null
                    ? dataForReport[i].ResponsibleLeader!.PositionName
                    : " не установлено ";
                worksheet.Cell(rowNumber, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 12).Value = dataForReport[i].ResponsibleExecutor is not null
                    ? dataForReport[i].ResponsibleExecutor!.GetFullName()
                    : " не установлено ";
                worksheet.Cell(rowNumber, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 13).Value = dataForReport[i].ResponsibleExecutor is not null
                    ? dataForReport[i].ResponsibleExecutor!.PositionName
                    : " не установлено ";
                worksheet.Cell(rowNumber, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 14).Value = dataForReport[i].ResponsibleInspector is not null
                    ? dataForReport[i].ResponsibleInspector!.GetFullName()
                    : " не установлено ";
                worksheet.Cell(rowNumber, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 15).Value = dataForReport[i].ResponsibleInspector is not null
                    ? dataForReport[i].ResponsibleInspector!.PositionName
                    : " не установлено ";
                worksheet.Cell(rowNumber, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 16).Value = dataForReport[i].Author is not null
                  ? dataForReport[i].Author?.GetFullName()
                  : " не установлено ";
                worksheet.Cell(rowNumber, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 17).Value = dataForReport[i].Author is not null
                    ? dataForReport[i].Author?.PositionName
                    : " не установлено ";
                worksheet.Cell(rowNumber, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 18).Value = dataForReport[i].Protocol?.Folder?.CreatedByNavigation is not null
                  ? dataForReport[i].Protocol?.Folder?.CreatedByNavigation?.GetFullName()
                  : " не установлено ";
                worksheet.Cell(rowNumber, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                worksheet.Cell(rowNumber, 19).Value = dataForReport[i].Protocol?.Folder?.CreatedByNavigation is not null
                    ? dataForReport[i].Protocol?.Folder?.CreatedByNavigation?.PositionName
                    : " не установлено ";
                worksheet.Cell(rowNumber, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            }

            worksheet.ColumnWidth = 20;
            worksheet.Cells().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cells().Style.Border.OutsideBorderColor = XLColor.Black;
            
            using MemoryStream stream = new();
            workbook.SaveAs(stream);
            return stream;
        }

        public MemoryStream MakeReportByAssignments(List<Assignment> dataForReport, int timeDifference)
        {
            //Строка с которой начинается таблица
            const int tableStartRow = 2;

            //Создание документа
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Sheet1");
            worksheet.Style.Alignment.WrapText = true;
            worksheet.ColumnWidth = _columnWidth;
            worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Columns().AdjustToContents();
            worksheet.Column("A").Width = 10;
            worksheet.Column("E").Width = 10;
            worksheet.Column("D").Width = 15;
            worksheet.Column("C").Width = 20;
            worksheet.Column("B").Width = 90;
            worksheet.Range($"A{tableStartRow}:F{dataForReport.Count + 3}").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A{tableStartRow}:F{dataForReport.Count + 3}").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A{tableStartRow}:F{dataForReport.Count + 3}").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A{tableStartRow}:F{dataForReport.Count + 3}").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A{tableStartRow}:F{dataForReport.Count + 3}").Style.Border.OutsideBorder =
                XLBorderStyleValues.Double;

            //Формирование общего названия документа
            worksheet.Row(1).Height = 25;
            worksheet.Range("A1:B1").Row(1).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.Navy;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 20;
            worksheet.Cell(1, 1).Value = "Статус выполнения протокола";

            //Формирование заголовков столбцов
            for (int i = 0; i < _protocolExcelHeader.Count; i++)
            {
                int cellNumber = i+1;
                worksheet.Cell(tableStartRow, cellNumber).Style.Font.Bold = true;
                worksheet.Cell(tableStartRow, cellNumber).Value = _protocolExcelHeader[i];
                worksheet.Cell(tableStartRow, cellNumber).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 224, 188);
                worksheet.Cell(tableStartRow + 1, cellNumber).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Cell(tableStartRow, cellNumber).Style.Font.FontColor = XLColor.Black;
                worksheet.Cell(tableStartRow, cellNumber).Style.Font.Bold = true;
            }

            //Формирование серого поля
            worksheet.Cell(3, 2).Value = dataForReport[0].Protocol!.Name;
            worksheet.Cell(3, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(3, 2).Style.Font.Bold = true;

            //Заполнение ячеек данными
            for (int i = 0; i < dataForReport.Count; i++)
            {
                int rowNumber  = i+4;

                worksheet.Cell(rowNumber, 1).Value = dataForReport[i].Name; //Наименование поручения
                worksheet.Cell(rowNumber, 2).Value = dataForReport[i].Description; //Описание задачи в поручении
                worksheet.Cell(rowNumber, 2).Style.Alignment
                    .SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell(rowNumber, 3).Value = dataForReport[i].ResponsibleLeader is null //Ответственный руководитель
                    ? "-"
                    : dataForReport[i].ResponsibleLeader!.GetInitials();
                worksheet.Cell(rowNumber, 4).Value = dataForReport[i].LeaderExecutionDate is null//Дата исполнения отв. руководителя
                    ? string.Empty
                    : dataForReport[i].LeaderExecutionDate?.AddHours(-timeDifference)//(TODO)пока не знаю что с этим делать
                        .ToString("dd.MM.yyyy");

                //Цвета и значения ячейки статус

                //Если срок исполнения поручения еще не наступил, то цвет ячейки белый
                if (dataForReport[i].LeaderExecutionDate is null
                    || dataForReport[i].LeaderExecutionDate >= DateTime.UtcNow.Date)
                {
                    //если поручение в статусе готово,и срок исполнения еще не наступил,
                    // то красим в зеленый цвет
                    //в комментарий выводим коммент указаный при изменении статуса  
                    if (dataForReport[i].StatusId is 7)
                    {
                        worksheet.Cell(rowNumber, 5).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(rowNumber, 6).Value = dataForReport[i].Comments?.Any() != true
                            ? string.Empty
                            : dataForReport[i].Comments?.First().Content;
                    }
                    else
                    {
                        worksheet.Cell(rowNumber, 5).Style.Fill.BackgroundColor = XLColor.White;
                        //тут логика следующая
                        //если поручение в статусе 1 или 2 то комменты введенные администратором в отчет не идут
                        //не делаем проверку на то является ли автором комментария администратор, так как в статусе 1 и 2 
                        //коммент пишет администратор
                        worksheet.Cell(rowNumber, 6).Value =
                            dataForReport[i].StatusId is not 1
                                and not 2
                            ? worksheet.Cell(rowNumber, 6).Value = "В работе"
                            : string.Empty;
                    }

                    //если поручение в статусе новое 
                    //и не был выбран отв рук, то цвет ячейки всегда белый
                    if (dataForReport[i].StatusId == (long)Status.Created
                        && dataForReport[i].ResponsibleLeader is null)
                    {
                        worksheet.Cell(rowNumber, 5).Style.Fill.BackgroundColor = XLColor.White;
                    }
                }
                //Если срок исполнения уже наступил
                else
                {
                    //Если поручение в статусе "готово"
                    if (dataForReport[i].StatusId == 7)
                    {
                        //Если поручение перешло в статус готово до истечения срока выполнения то цвет ячейки "зеленый" иначе "красный"
                        worksheet.Cell(rowNumber, 5).Style.Fill.BackgroundColor =
                            XLColor.Green; //(всегда зеленый - по просьбе заказчика)

                        worksheet.Cell(rowNumber, 6).Value = dataForReport[i].Comments?.Any() != true
                            ? string.Empty
                            : dataForReport[i].Comments?.First().Content;
                    }
                    //Срок исполнения наступил а поручение не в статусе "Готово"
                    else
                    {
                        worksheet.Cell(rowNumber, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(251, 129, 129);
                        worksheet.Cell(rowNumber, 6).Value = dataForReport[i].Comments?.Any() != true
                            ? string.Empty
                            : dataForReport[i].Comments?.First().Content;
                    }
                }

                //если ответственный исполнитель при создании поручения не был назначен
                // и если администратором при создании поручения были созданы комменты 
                // то они вносятся в ячейку комментарий
                if ((dataForReport[i].ResponsibleLeader is null)
                    && (dataForReport[i].StatusId == 1))
                {
                    worksheet.Cell(rowNumber, 6).Value = dataForReport[i].Comments?.Any() != true
                        ? "Принято решение"
                        : dataForReport[i].Comments?.First().Content;
                }
            }

            MemoryStream stream = new();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}