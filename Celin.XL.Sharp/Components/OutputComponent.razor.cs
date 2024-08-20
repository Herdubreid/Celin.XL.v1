using Celin.XL.Sharp.Service;
using Microsoft.AspNetCore.Components;

namespace Celin.XL.Sharp.Components;

public partial class OutputComponent
{
    [Inject]
    public WriterService Writer { get; set; } = null!;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Writer.OnChange += StateHasChanged;
    }
}
