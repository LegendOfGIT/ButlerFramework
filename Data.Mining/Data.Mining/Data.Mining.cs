namespace Data.Mining
{
    public static class MiningConstants
    {
        public const string Breadcrumbs = "Breadcrumbs";
        public const string Navigation = "Navigation";
        public const string Nextlink = "Nextlink";
        public const string Previouslink = "Previouslink";
        public const string Results = "Results";
    }

    enum Analysismode {
        None,
        Datalist,
        Metadata,
        List,
        Link,
        Table
    }
}
