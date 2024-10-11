namespace property_price_common;

public static class FileNameUtils
{
    public static string GenerateTransactionCsvFileName()
    {
        return $"transactions-export-{DateTime.Now.ToFileTime()}.csv";
    }
}
