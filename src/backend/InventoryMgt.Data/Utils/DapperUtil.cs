namespace InventoryMgt.Data.Utils;

public static class DapperUtil
{
    public static void ConfigureDapper()
    {
        // match snake_case to PascalCase
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}