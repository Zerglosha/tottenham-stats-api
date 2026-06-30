using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Common;

public class PaginationParameters
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPage = 10000;
    public const int MaxPageSize = 100;

    [Range(1, MaxPage)]
    public int? Page { get; set; }

    [Range(1, MaxPageSize)]
    public int? PageSize { get; set; }

    public int CurrentPage => Page ?? DefaultPage;

    public int CurrentPageSize => PageSize ?? DefaultPageSize;
}
