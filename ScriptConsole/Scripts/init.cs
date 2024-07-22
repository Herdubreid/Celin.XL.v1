record F0101 : Celin.AIS.FormResponse
{
    public record Row(int f0101_an8, string f0101_alph);
    // Instead of defining row members, we use the DynamicJsonElment
    //public DynamicFormResult fs_DATABROWSE_F0101 { get; set; } = null!;
    public RecordFormResult<Row> fs_DATABROWSE_F0101 { get; set; } = null!;
    public IEnumerable<Row> Rows => fs_DATABROWSE_F0101.data.gridData.rowset;
}

Console.WriteLine("Init...");
