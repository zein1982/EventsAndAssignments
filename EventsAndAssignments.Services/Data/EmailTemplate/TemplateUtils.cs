using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public static class TemplateUtils
    {
        //public static string GetAssignmentLink(Assignment assignment) =>
        //    @$"/assignment?idFolder={assignment.Protocol!.FolderId}
        //            &nameFolder={assignment.Protocol.Folder.Name}
        //            &assignmentId={assignment.Protocol.Id}
        //            &assignmentName={assignment.Protocol.Name}
        //            &assignmentById={assignment.Id}";

        public static string GetAssignmentLink(Assignment assignment) =>
            $"/allAssignmentsPage?assignmentById={assignment.Id}";

        public static string GetNotificationSubject(Assignment assignment, string textPrefix = "")
        {
            string subject = $"{textPrefix} Поручение №{assignment.Name}_{assignment.Protocol!.Name}";

            return subject;
        }

        public static string GetNotificationBody(Assignment assignment, string responsibleRole, DateTime? executionDate, string frontUrl, string textPrefix = "")
        {
            string text = executionDate is not null
                ? $@"Добрый день! {textPrefix} Вы назначены на роль «{responsibleRole}» по поручению №{assignment.Name} «{assignment.Description}»: 
                    <a href='{frontUrl}{GetAssignmentLink(assignment)}'>ссылка</a>  согласно 
                    «{assignment.Protocol!.Name}». Срок исполнения: не позднее «{executionDate:dd.MM.yyyy}»."
                : $@"Добрый день! {textPrefix} Вы назначены на роль «{responsibleRole}» по поручению №{assignment.Name} «{assignment.Description}»: 
                    <a href='{frontUrl}{GetAssignmentLink(assignment)}'>ссылка</a>  согласно 
                    «{assignment.Protocol!.Name}».";

            return text;
        }

        public static string GetHtmlFormattedNotificationBody2(Assignment assignment, string responsibleRole, DateTime? executionDate, string frontUrl, string textPrefix = "") =>
            $@"<p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Times New Roman"", Times, serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style=""font-size:19px;color:black;"">Добрый день!{textPrefix} Вы назначены на роль &laquo;</span></em><strong><em><span style=""font-size:19px;color:#4472C4;"">{responsibleRole}</span></em></strong><em><span style=""font-size:19px;color:black;"">&raquo; по поручению &laquo;</span></em><strong><em><span style=""font-size:21px;color:black;"">{assignment.Description}</span></em></strong><em><span style=""font-size:19px;color:black;"">&raquo;.</span></em></p>
                   <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Times New Roman"", Times, serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style=""font-size:19px;color:black;"">&nbsp;Поручение №{assignment.Name} согласно &laquo;{assignment.Protocol!.Name}&raquo;&nbsp;</span></em><strong><em><u><span style=""font-size:19px;color:#4472C4;""><a href='{frontUrl}{GetAssignmentLink(assignment)}'>Ссылка</a></span></u></em></strong></p>
                   <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Times New Roman"", Times, serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style=""font-size:19px;color:black;"">&nbsp;Срок исполнения: не позднее &laquo;{executionDate:dd.MM.yyyy}&raquo;.</span></em></p>";

        public static string GetHtmlFormattedNotificationBody(Assignment assignment, string responsibleRole, DateTime? executionDate, string frontUrl, string textPrefix = "") =>
            $@"<p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>Добрый день!&nbsp;</span></em></p>
                <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>{textPrefix} Вы назначены на роль &laquo;</span></em><span style=""font-family: 'Times New Roman', Times, serif;""><strong><em><span style=""font-size:19px;color:#4472C4;"">{responsibleRole}</span></em></strong><em><span style=""font-size:19px;color:black;"">&raquo; по поручению &laquo;</span></em><strong><em><span style=""font-size:21px;color:black;"">{assignment.Description}</span></em></strong><em><span style=""font-size:19px;color:black;"">&raquo;.</span></em></span></p>
                <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><span style=""font-family: 'Times New Roman', Times, serif;""><em><span style=""font-size:19px;color:black;"">Поручение №{assignment.Name} согласно &laquo;{assignment.Protocol!.Name}&raquo;&nbsp;</span></em><strong><em><u><span style=""font-size:19px;color:#4472C4;""><a href='{frontUrl}{GetAssignmentLink(assignment)}'>Ссылка</a></span></u></em></strong></span></p>
                <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'><em><span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>Срок исполнения: не позднее &laquo;{executionDate:dd.MM.yyyy}&raquo;.</span></em></p>";

        public static string GetStatusNotificationBody(Assignment assignment, string frontUrl)
        {
            string text =
                $@"Добрый день! Статус работ по поручению №{assignment.Name} «{assignment.Description}»: 
                сменился на «{assignment.Status!.Name}» <a href='{frontUrl}{GetAssignmentLink(assignment)}'>ссылка</a> 
                согласно «{assignment.Protocol!.Name}».";

            return text;
        }

        public static string GetHtmlFormattedStatusNotificationBody2(Assignment assignment, string frontUrl) =>
            $@"<p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'>
                <em>
                    <span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>
                        Добрый день! Статус исполнения поручения &laquo;
                    </span>
                </em>
              </p>
                <span style=""font-family: 'Times New Roman', Times, serif;"">
                    <strong>
                        <em>
                            <span style=""font-size:21px;color:black;"">
                                {assignment.Description}
                            </span>
                        </em>
                    </strong>
                    <em>
                        <span style=""font-size:19px;color:black;"">
                            &raquo; сменился на &laquo;
                        </span>
                    </em>
                    <strong>
                        <em>
                            <span style=""font-size:19px;color:#4472C4;"">
                                {assignment.Status!.Name}
                            </span>
                        </em>
                    </strong>
                    <em>
                        <span style=""font-size:19px;color:black;"">
                            &raquo;
                        </span>
                    </em>
                </span>
            </p>
            <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'>
                <span style=""font-family: 'Times New Roman', Times, serif;"">
                    <em>
                        <span style=""font-size:19px;color:black;"">
                            Поручение №{assignment.Name} согласно &laquo;{assignment.Protocol!.Name}&raquo;&nbsp;
                        </span>
                    </em>
                </span>
                <strong>
                    <em>
                        <u>
                            <span style='font-size: 19px; color: rgb(68, 114, 196); font-family: ""Times New Roman"", Times, serif;'>
                                <a href='{frontUrl}{GetAssignmentLink(assignment)}'>Ссылка</a>
                            </span>
                        </u>
                    </em>
                </strong>
            </p>";

        public static string GetHtmlFormattedStatusNotificationBody(Assignment assignment, string frontUrl) =>
            $@"<p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'>
                    <em>
                        <span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>
                            Добрый день!&nbsp;
                        </span>
                    </em>
                </p>
                <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'>
                    <em>
                        <span style='font-size: 19px; color: black; font-family: ""Times New Roman"", Times, serif;'>
                            Статус исполнения поручения &laquo;
                        </span>
                    </em>
                        <span style=""font-family: 'Times New Roman', Times, serif;"">
                            <strong>
                                <em>
                                    <span style=""font-size:21px;color:black;"">
                                        {assignment.Description}
                                    </span>
                                </em>
                            </strong>
                            <em>
                                <span style=""font-size:19px;color:black;"">
                                    &raquo; сменился на &laquo;
                                </span>
                            </em>
                            <strong>
                                <em>
                                    <span style=""font-size:19px;color:#4472C4;"">
                                        {assignment.Status!.Name}
                                    </span>
                                </em>
                            </strong>
                            <em>
                                <span style=""font-size:19px;color:black;"">
                                    &raquo;
                                </span>
                            </em>
                        </span>
                </p>
                <p style='margin-right:0cm;margin-left:0cm;font-size:15px;font-family:""Calibri"",sans-serif;margin:0cm;margin-top:0cm;margin-bottom:8.0pt;background:white;'>
                    <span style=""font-family: 'Times New Roman', Times, serif;""><em><span style=""font-size:19px;color:black;"">
                        Поручение №{assignment.Name} согласно &laquo;{assignment.Protocol!.Name}&raquo;&nbsp;
                    </span>
                </em>
                </span>
                <strong>
                    <em>
                        <u>
                            <span style='font-size: 19px; color: rgb(68, 114, 196); font-family: ""Times New Roman"", Times, serif;'>
                                <a href='{frontUrl}{GetAssignmentLink(assignment)}'>Ссылка</a>
                            </span></u></em></strong></p>";

        public static string GetHtmlFormattedExpiredNotificationBody(Assignment assignment, string responsibleRole,
            DateTime? executionDate, string frontUrl) =>
            $@"<p style='margin:0cm;font-size:15px;font-family:""Calibri"",sans-serif;'><em>
                <span style='font-size:19px;font-family:""Times New Roman"",serif;'>Добрый день!&nbsp;</span></em></p>
              <p style='margin:0cm;font-size:15px;font-family:""Calibri"",sans-serif;'><em>
                <span style='font-size:19px;font-family:""Times New Roman"",serif;'>У Вас просрочено поручение &laquo;</span></em><strong><em>
                <span style='font-size:21px;font-family:""Times New Roman"",serif;'>{assignment.Description}</span></em></strong><em>
                <span style='font-size:19px;font-family:""Times New Roman"",serif;'>&raquo;, по которому Вы назначены на роль &laquo;<strong>
                <span style=""color:#4472C4;"">{responsibleRole}</span></strong>&raquo;.</span></em></p>
              <p style='margin:0cm;font-size:15px;font-family:""Calibri"",sans-serif;'><em>
                <span style='font-size:19px;font-family:""Times New Roman"",serif;'>Поручение №{assignment.Name} согласно &laquo;{assignment.Protocol?.Name}&raquo; <strong><u>
                <span style=""color:#4472C4;""><a href='{frontUrl}{GetAssignmentLink(assignment)}'>Ссылка</a></span></u></strong></span></em></p>
              <p><em>
                <span style='font-size:19px;font-family:""Times New Roman"",serif;'>Срок исполнения: не позднее &laquo;{executionDate:dd.MM.yyyy}&raquo;.</span></em></p>";
    }
}