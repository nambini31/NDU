using System;

namespace EDMS2025.Models.Utility
{
    public class PageUtility
    {
        public static string MakeAjaxPagination(int pageSize, int resultCount, int currentPage, string onclick)
        {
            var nombreDePage = Convert.ToInt32(resultCount / pageSize);
            if (resultCount == 0 || resultCount <= pageSize)
            {
                return "";
            }

            const string? tabIndex = "tabindex='-1'";
            var numeroPaginationDom = "";
            var prevDom = "";
            var nextDom = "";
            const string? classLi = "page-item";
            const string? classA = "page-link";
            const string? active = " active";
            const string? disabled = " disabled";
            var classPrev = classLi;
            var hrefPrev = $"{onclick}({currentPage - 1})";
            var attribute = "";
            var href = $"onclick = '{hrefPrev}'";
            if (currentPage <= 1) { classPrev = $"{classPrev}{disabled}"; attribute = tabIndex; href = ""; }
            prevDom =
                $"<li class='{classPrev}'><a class='{classA}' {href} {attribute} aria-label='Previous'><span aria-hidden='true'>&laquo;</span><span class='sr-only'>Previous</span></a></li>";
            if (resultCount <= pageSize)
            {
                var li = $"<li class='{classLi}{active}'><a class='{classA}' onclick='{onclick}(1)'>1</a></li>";
                numeroPaginationDom = li;
            }
            if (resultCount > pageSize)
            {
                numeroPaginationDom = "";
                if (resultCount % pageSize != 0) nombreDePage++;
                switch (nombreDePage)
                {
                    case <= 5:
                        {
                            for (var i = 1; i <= nombreDePage; i++)
                            {
                                var classListe = classLi;
                                if (i == currentPage) classListe = $"{classListe}{active}";
                                var li = $"<li class='{classListe}'><a class='{classA}' onclick='{onclick}({i})'>{i}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                    case > 5 when IsLastTwoOrFirstTwo(currentPage, nombreDePage):
                        {
                            string[] numpage = { "1", "2", "...", $"{nombreDePage - 1}", $"{nombreDePage}" };
                            string[] classNum = { classLi, classLi, $"{classLi}{disabled}", classLi, classLi };
                            for (var i = 0; i < numpage.Length; i++)
                            {
                                var classe = classNum[i];
                                attribute = "";
                                href = $"onclick='{onclick}({numpage[i]})'";
                                if (classNum[i].Contains("disabled")) { attribute = tabIndex; href = ""; }
                                if (numpage[i].Equals(currentPage.ToString())) classe = $"{classe}{active}";
                                var li =
                                    $"<li class='{classe}'><a class='{classA}' {attribute} {href}>{numpage[i]}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                    case > 5:
                        {
                            string[] numpage = { "1", "...", $"{currentPage}", "...", $"{nombreDePage}" };
                            string[] classNum = { classLi, $"{classLi}{disabled}", $"{classLi}{active}",
                            $"{classLi}{disabled}", classLi };
                            for (var i = 0; i < numpage.Length; i++)
                            {
                                var classe = classNum[i];
                                href = $"onclick='{onclick}({numpage[i]})'";
                                if (classNum[i].Contains("disabled")) { href = ""; }
                                var li = $"<li class='{classe}'><a class='{classA}' {href}>{numpage[i]}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                }
            }
            var classNext = "page-item";
            var hrefNext = $"{onclick}({currentPage + 1})";
            attribute = "";
            href = $"onclick = '{hrefNext}'";
            if (currentPage >= nombreDePage) { classNext = "page-item disabled"; attribute = tabIndex; href = ""; }
            nextDom =
                $"<li class='{classNext}'><a class='{classA}' {attribute} {href}><span aria-hidden='true'>&raquo;</span><span class='sr-only'>Next</span></a></li>";
            return $"{prevDom}{numeroPaginationDom}{nextDom}";
        }

        public static string MakePagination(int pageSize, int resultCount, int currentPage, string url)
        {
            url = FormatUrlForPagination(url);
            var nombreDePage = Convert.ToInt32(resultCount / pageSize);
            if (resultCount == 0 || resultCount <= pageSize)
            {
                return "";
            }

            const string? tabIndex = "tabindex='-1'";
            var numeroPaginationDom = "";
            var prevDom = "";
            var nextDom = "";
            const string? classLi = "page-item";
            const string? classA = "page-link";
            const string? active = " active";
            const string? disabled = " disabled";
            var classPrev = classLi;
            var hrefPrev = $"{url}{currentPage - 1}";
            var attribute = "";
            var href = $"href = '{hrefPrev}'";
            if (currentPage <= 1) { classPrev = $"{classPrev}{disabled}"; attribute = tabIndex; href = ""; }
            prevDom =
                $"<li class='{classPrev}'><a class='{classA}' {href} {attribute} aria-label='Previous'><span aria-hidden='true'>&laquo;</span><span class='sr-only'>Previous</span></a></li>";
            if (resultCount <= pageSize)
            {
                var li = $"<li class='{classLi}{active}'><a class='{classA}' href='{url}1'>1</a></li>";
                numeroPaginationDom = li;
            }
            if (resultCount > pageSize)
            {
                numeroPaginationDom = "";
                if (resultCount % pageSize != 0) nombreDePage++;
                switch (nombreDePage)
                {
                    case <= 5:
                        {
                            for (var i = 1; i <= nombreDePage; i++)
                            {
                                var classListe = classLi;
                                if (i == currentPage) classListe = $"{classListe}{active}";
                                var li = $"<li class='{classListe}'><a class='{classA}' href='{url}{i}'>{i}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                    case > 5 when IsLastTwoOrFirstTwo(currentPage, nombreDePage):
                        {
                            string[] numpage = { "1", "2", "...", $"{nombreDePage - 1}", $"{nombreDePage}" };
                            string[] classNum = { classLi, classLi, $"{classLi}{disabled}", classLi, classLi };
                            for (var i = 0; i < numpage.Length; i++)
                            {
                                var classe = classNum[i];
                                attribute = "";
                                href = $"href='{url}{numpage[i]}'";
                                if (classNum[i].Contains("disabled")) { attribute = tabIndex; href = ""; }
                                if (numpage[i].Equals(currentPage.ToString())) classe = $"{classe}{active}";
                                var li =
                                    $"<li class='{classe}'><a class='{classA}' {attribute} {href}>{numpage[i]}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                    case > 5:
                        {
                            string[] numpage = { "1", "...", $"{currentPage}", "...", $"{nombreDePage}" };
                            string[] classNum = { classLi, $"{classLi}{disabled}", $"{classLi}{active}",
                            $"{classLi}{disabled}", classLi };
                            for (var i = 0; i < numpage.Length; i++)
                            {
                                var classe = classNum[i];
                                href = $"href='{url}{numpage[i]}'";
                                if (classNum[i].Contains("disabled")) { href = ""; }
                                var li = $"<li class='{classe}'><a class='{classA}' {href}>{numpage[i]}</a></li>";
                                numeroPaginationDom = $"{numeroPaginationDom}{li}";
                            }

                            break;
                        }
                }
            }
            var classNext = "page-item";
            var hrefNext = $"{url}{currentPage + 1}";
            attribute = "";
            href = $"href = '{hrefNext}'";
            if (currentPage >= nombreDePage) { classNext = "page-item disabled"; attribute = tabIndex; href = ""; }
            nextDom =
                $"<li class='{classNext}'><a class='{classA}' {attribute} {href}><span aria-hidden='true'>&raquo;</span><span class='sr-only'>Next</span></a></li>";
            return $"{prevDom}{numeroPaginationDom}{nextDom}";
        }
        private static bool IsLastTwoOrFirstTwo(int currentPage, int numberOfPage)
        {
            return currentPage <= 2 || Math.Abs(numberOfPage - currentPage) <= 1;
        }

        private static string FormatUrlForPagination(string url)
        {
            var newUrl = string.Empty;
            const string? page = "page=";
            if (url.Contains("page="))
            {
                var paramAfterPage = url.Substring(url.IndexOf(page) + page.Length);
                if (paramAfterPage.Contains("&"))
                {
                    var index = paramAfterPage.IndexOf("&");
                    var stringToReplace = url.Substring(url.IndexOf(page), page.Length + index - 1);
                    newUrl = $"{url.Replace(stringToReplace, string.Empty)}&{page}";
                }
                else
                {
                    newUrl = url.Substring(0, url.IndexOf(page) + page.Length);
                }
            }
            else
            {
                newUrl = url.Contains("?") ? $"{url}&{page}" : $"{url}?{page}";
            }
            return newUrl;
        }
    }
}