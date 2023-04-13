// Importera nödvändiga bibliotek och namespaces (kommer direkt vid skapandet av ett projekt)
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Skapa en ny namespace som heter "Ritningsprogram"
namespace Ritningsprogram
{
    // Denna funktion kommer direkt vid skapande av ett projekt och är nödvändig
    public partial class MainWindow : Window
    {
        // Deklarera några privata variabler 
        private bool color = true;           // flagga för att välja om man ritar med en penna eller suddar med ett suddgummi
        private bool isDrawing = false;      // flagga som indikerar om användaren håller musknappen nere för att rita
        private Point startPoint;           // punkten där användaren började rita
        private Line currentLine;           // den aktuella linjen som användaren ritade

        // Denna funktion kommer direkt vid skapande av ett projekt och är nödvändig
        public MainWindow()
        {
            InitializeComponent(); // Initialisera komponenter i fönstret
        }

        // Funktion som körs när användaren trycker ner musknappen på canvasen
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true; // Sätt flaggan för att rita på canvasen till sant
            startPoint = e.GetPosition(Canvas); // Sätt startpositionen till muspekarens position
            if (color) // Om pennan är aktiv
            {
                currentLine = new Line // Skapa en ny linje
                {
                    Stroke = Brushes.Black, // Använd svart färg
                    StrokeThickness = ThicknessSlider.Value / 2, // Använd tjockleken från tjockleksspaken
                    X1 = startPoint.X, // Sätt startpositionen för linjen
                    Y1 = startPoint.Y,
                    X2 = startPoint.X, // Sätt slutpositionen för linjen till startpositionen (för att undvika att det skapas en linje vid klick)
                    Y2 = startPoint.Y
                };
                Canvas.Children.Add(currentLine); // Lägg till linjen till canvasen
            }
            else // Om suddgummit är aktiv
            {
                currentLine = new Line // Skapa en ny linje
                {
                    Stroke = Brushes.White, // Använd vit färg
                    StrokeThickness = ThicknessSlider.Value * 5, // Använd en storlek för suddgummit baserad på tjockleken från tjockleksspaken (gånger 5 för ökad effekt)
                    X1 = startPoint.X, // Sätt startpositionen för linjen
                    Y1 = startPoint.Y,
                    X2 = startPoint.X, // Sätt slutpositionen för linjen till startpositionen (för att undvika att det skapas en linje vid klick)
                    Y2 = startPoint.Y
                };
                Canvas.Children.Add(currentLine); // Lägg till linjen till canvasen
            }

        }

        // Funktion som körs när användaren rör på muspekaren på canvasen
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing) // Om användaren håller ner musknappen och ritar
            {
                Point currentPoint = e.GetPosition(Canvas);
                if (color)
                {
                    Line lineSegment = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = ThicknessSlider.Value / 2,
                        X1 = startPoint.X, 
                        Y1 = startPoint.Y, 
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y
                    };
                    Canvas.Children.Add(lineSegment);
                }
                else
                {
                    Line lineSegment = new()
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = ThicknessSlider.Value * 5,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y
                    };
                    Canvas.Children.Add(lineSegment);
                }
                startPoint = currentPoint; // Uppdatera startpunkten för nästa linje
            }
        }

        // Funktion som kallas när muspekaren släpps
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false; // Sätt isDrawing till false så att användaren slutar rita
        }

        // Funtkion som kallas när användaren klickar på spara-knappen
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(1250, 682, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(Canvas);
            BitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(ms);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
            }
        }

        // Funktionen som körs när penn-knappen klickas på
        private void PenButton_Click(object sender, RoutedEventArgs e)
        {
            color = true; // Sätter färgen för pennan till svart genom att sätta color till true
        }

        // Funktionen som körs när sudd-knappen klickas på
        private void EreaserButton_Click(object sender, RoutedEventArgs e)
        {
            color = false; // Sätter färgen för pennan till vit genom att sätta color till false
        }
        // Funktionen som körs när ångra-knappen klickas på
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            int lineCount = Canvas.Children.Count; // Räknar hur många linjer det finns på canvasen
            if (lineCount > 0) // Tar bort den senast ritade linjen genom att ta bort det sista elementet i Canvas.Children
            {
                Canvas.Children.RemoveAt(lineCount - 1);
            }
        }

        // Funktionen som körs när rensa-knappen klickas på
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear(); // Tar bort alla element i Canvas.Children, vilket rensar canvasen
        }
    }
}
