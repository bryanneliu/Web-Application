using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkToAPI
{
    static public class Constants
    {
        static public string HttpRequest_OLS = @"https://localhost/shredderservice/api/search/search?tenant=microsoft-my.sharepoint-df.com&lcid=1033&queryText=&rowlimit=10";
        static public string HttpRequest_ThreeS = @"https://localhost/shredderservice/api/search/search?tenant=microsoft-my.sharepoint-df.com&lcid=1033&queryText=&rowlimit=10";
        static public string HttpRequest_QF = @"https://localhost/shredderservice/api/search/search?tenant=microsoft-my.sharepoint-df.com&lcid=1033&queryText=rank&rowlimit=10";
        static public string HttpRequest_SPO = @"https://localhost/shredderservice/api/search/search?tenant=microsoft-my.sharepoint-df.com&lcid=1033&queryText=rank&rowlimit=10";
        static public string HttpRequest_TermSearch_Prefix = @"https://localhost/shredderservice/api/search/search?tenant=microsoft.sharepoint-df.com&lcid=1033&queryText=";
        static public string HttpRequest_TermSearch_Suffix = @"&rowlimit=25";

        static public string HttpRequest_TermSearch = @"https://localhost/shredderservice/api/search/search?tenant=microsoft-my.sharepoint-df.com&lcid=1033&queryText=junk%20detection&rowlimit=25";

        static public string Flight_OLS = @"OlsSearch";
        static public string Flight_ThreeS = @"ZeroTermThroughThreeS";
        static public string Flight_QF = @"QFSearch";
        static public string Flight_SPO = @"SPSearch";

        static public string LocalPath_HttpResponse_OLS = @"D:\Test\recallAnalysis\OLS\httpResponse.txt";
        static public string LocalPath_HttpResponse_ThreeS = @"D:\Test\recallAnalysis\ThreeS\httpResponse.txt";
        static public string LocalPath_HttpResponse_QF = @"D:\Test\recallAnalysis\QF\httpResponse.txt";
        static public string LocalPath_HttpResponse_SPO = @"D:\Test\recallAnalysis\SPO\httpResponse.txt";
        static public string LocalPath_HttpResponse_TermSearch = @"D:\Test\recallAnalysis\TapTermSearch\httpResponse.txt";

        static public string LocalPath_ParsedFields_OLS = @"D:\Test\recallAnalysis\OLS\parsedFields.txt";
        static public string LocalPath_ParsedFields_ThreeS = @"D:\Test\recallAnalysis\ThreeS\parsedFields.txt";
        static public string LocalPath_ParsedFields_QF = @"D:\Test\recallAnalysis\QF\parsedFields.txt";
        static public string LocalPath_ParsedFields_SPO = @"D:\Test\recallAnalysis\SPO\parsedFields.txt";
        static public string LocalPath_ParsedFields_TermSearch = @"D:\Test\recallAnalysis\TapTermSearch\parsedFields.txt";
        static public string LocalPath_ParsedFields_TermSearch_Test = @"D:\Test\recallAnalysis\TapTermSearch\parsedFields_Test.txt";

        static public string LocalPath_HttpResponse_MSW = @"D:\Test\recallAnalysis\MSW\httpResponse.txt";
        static public string LocalPath_ParsedFields_MSW_Personal = @"D:\Test\recallAnalysis\MSW\parsedFields_Personal.txt";
        static public string LocalPath_ParsedFields_MSW_Test = @"D:\Test\recallAnalysis\MSW\parsedFields_Test.txt";

        static public string BearerToken_MSW = @"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6InU0T2ZORlBId0VCb3NIanRyYXVPYlY4NExuWSIsImtpZCI6InU0T2ZORlBId0VCb3NIanRyYXVPYlY4NExuWSJ9.eyJhdWQiOiJodHRwczovL291dGxvb2sub2ZmaWNlMzY1LmNvbS9hdXRvc3VnZ2VzdCIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0Ny8iLCJpYXQiOjE1NjM1NzM0OTcsIm5iZiI6MTU2MzU3MzQ5NywiZXhwIjoxNTYzNTc3Mzk3LCJhY3IiOiIxIiwiYWlvIjoiQVZRQXEvOE1BQUFBQXNudkRBMmNqZEFpQWx6eGtkVUZRdmZRcXpZWlhvVEpweHFGQ1c0VVJUeENBT2xhNHo5Syt2WnNCMGJBTmQ1NXlEYVNHRFc1NUVxNU9FdEJiWUZDNHpxMDRmaHlyL1g5d3ZzaGRhM0MrQnM9IiwiYW1yIjpbIndpYSIsIm1mYSJdLCJhcHBpZCI6ImQzNTkwZWQ2LTUyYjMtNDEwMi1hZWZmLWFhZDIyOTJhYjAxYyIsImFwcGlkYWNyIjoiMCIsImRldmljZWlkIjoiZjQyNDdkMmItYzgyNC00ZDM4LTg5MDktNzMxZmUyYjIzYTZjIiwiZmFtaWx5X25hbWUiOiJMaXUgKE9GRklDRSkiLCJnaXZlbl9uYW1lIjoiSmlhIiwiaXBhZGRyIjoiMTMxLjEwNy4xNDcuMjA4IiwibmFtZSI6IkppYSBMaXUgKE9GRklDRSkiLCJvaWQiOiI5OGIxMzVlYS1iZTVlLTRhMjUtOGIzNi00MjllZTM4NjdmOWQiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjEyNzUyMTE4NC0xNjA0MDEyOTIwLTE4ODc5Mjc1MjctMTc1NzU4NjIiLCJwdWlkIjoiMTAwMzAwMDA4MUIwMEQwNiIsInJoIjoiSSIsInNjcCI6IlN1YnN0cmF0ZVNlYXJjaC1JbnRlcm5hbC5SZWFkV3JpdGUiLCJzdWIiOiJxNF81ai1YUzltSVlFSmpOU2NyNk1HSnNTMU5qbXQ5dmFjQXctVmg2d3VVIiwidGlkIjoiNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3IiwidW5pcXVlX25hbWUiOiJsaXVqaUBtaWNyb3NvZnQuY29tIiwidXBuIjoibGl1amlAbWljcm9zb2Z0LmNvbSIsInV0aSI6ImMtdVRGT3BKVUVHNEc1c04xMGNCQUEiLCJ2ZXIiOiIxLjAifQ.Wck4K5IqGK4g-XbgLiFRrJEeqIc6LXW_hPbX14ymvWV_lZN-NcUzOfVdBnusm0ZVA0cnTxymQ6vYZSnca9lXWZi1kYkGSq7CMF5rXY0Eo80amHH51ropH7c1Zl7kmta3Q6XHvHV4JE7OgGzHai01ukTL24SFiU816phT6NgbO2lBcpTO8a4EOX_T5SqWoYuDGM5NsnhbqDPHpllkwdIwZ-ev-fOvVIl6O34ikA7kYG-UXYZ9yeTHyun6TMkf41KSuKTLV98Xalqkvrh04VqePfB6L-9ZwW5OthPatjnDEagKLM1hihyD8VxNKr0_Abl8FJGHP6SOwmUnbWaiqVW6Mw";

        static public string LocalPath_Query_Personal = @"D:\Test\recallAnalysis\Query\personal.txt";
        static public string LocalPath_Query_Test = @"D:\Test\recallAnalysis\Query\microsoft.random.20.dev.tsv";

        static public string LocalPath_Results_MSW_Personal_Personal = @"D:\Test\recallAnalysis\MSW\Personal\parsedFields_Personal.txt";
        static public string LocalPath_Results_Tap_Personal_Personal = @"D:\Test\recallAnalysis\TapTermSearch\Personal\parsedFields_Personal.txt";
        static public string LocalPath_Html_MSW_Personal_Personal = @"D:\Test\recallAnalysis\MSW\Personal\html_Personal.txt";
        static public string LocalPath_Html_Tap_Personal_Personal = @"D:\Test\recallAnalysis\TapTermSearch\Personal\html_Personal.txt";

        static public string LocalPath_SBS_Personal = @"D:\Test\recallAnalysis\TapTermSearch\Personal\SBS.html";

        static public string Debug = @"D:\Test\recallAnalysis\debug1.txt";
    }
}
