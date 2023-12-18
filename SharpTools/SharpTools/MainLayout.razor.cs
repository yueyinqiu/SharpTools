using MudBlazor;

namespace SharpTools;

public partial class MainLayout
{
    private readonly MudTheme theme = new MudTheme()
    {
        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = ["Noto Serif"]
            },
            H1 = new H1()
            {
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H2 = new H2()
            {
                FontSize = "2rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            }
        }
    };
}
