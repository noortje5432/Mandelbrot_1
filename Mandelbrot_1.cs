using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Cryptography;

Form scherm = new();
double MidX = 0, MidY = 0, Schaal = 0, MaxA = 0;

//Eerst wordt het volledige scherm en de bitmap aangemaakt.
scherm.Text = "Mandelbrot";
scherm.BackColor = Color.LightBlue;
scherm.ClientSize = new Size(420, 700);
scherm.StartPosition = FormStartPosition.CenterScreen;
Bitmap bm = new(400, 400);


//Aanmaken van de button Go!
Button Go = new(); scherm.Controls.Add(Go);
Go.Location = new Point(280, 170); Go.Text = "Go!";

//tekstvakken maken en locatie geven.
Label labMidX = new(); scherm.Controls.Add(labMidX);
Label labMidY = new(); scherm.Controls.Add(labMidY);
Label labSchaal = new(); scherm.Controls.Add(labSchaal);
Label labMaxA = new(); scherm.Controls.Add(labMaxA);
Label kiesKleur = new(); scherm.Controls.Add(kiesKleur);
Label uitleg = new(); scherm.Controls.Add(uitleg);

labMidX.Location = new Point(10, 10); labMidX.Size = new Size(100, 30); labMidX.Text = "Midden x:";
labMidY.Location = new Point(10, 50); labMidY.Size = new Size(100, 30); labMidY.Text = "Midden y:";
labSchaal.Location = new Point(10, 90); labSchaal.Size = new Size(100, 30); labSchaal.Text = "Schaal:";
labMaxA.Location = new Point(10, 130); labMaxA.Size = new Size(100, 30); labMaxA.Text = "Maximaal aantal:";
kiesKleur.Location = new Point(10, 170); kiesKleur.Size = new Size(100, 30); kiesKleur.Text = "Kies kleur:";
uitleg.Location = new Point(10, 650); uitleg.Size = new Size(190, 30); uitleg.Text = "Linker muisknop is inzoomen " +
    "en rechter muisknop is uitzoomen";


//Invulvlakken maken
TextBox boxMidX = new(); scherm.Controls.Add((TextBox)boxMidX);
TextBox boxMidY = new(); scherm.Controls.Add((TextBox)boxMidY);
TextBox boxSchaal = new(); scherm.Controls.Add((TextBox)boxSchaal);
TextBox boxMaxA = new(); scherm.Controls.Add((TextBox)boxMaxA);

boxMidX.Location = new Point(150, 10); boxMidX.Size = new Size(100, 30); boxMidX.Text = 0.ToString();
boxMidY.Location = new Point(150, 50); boxMidY.Size = new Size(100, 30); boxMidY.Text = 0.ToString();
boxSchaal.Location = new Point(150, 90); boxSchaal.Size = new Size(100, 30); boxSchaal.Text = 0.01.ToString();
boxMaxA.Location = new Point(150, 130); boxMaxA.Size = new Size(100, 30); boxMaxA.Text = 100.ToString();

//Achtergrond Mandelbrot maken
Label afbeelding = new(); scherm.Controls.Add(afbeelding);
afbeelding.Location = new Point(10, 210);
afbeelding.Size = new Size(400, 400);
afbeelding.Image = bm;

// ComboBox maken voor kleurkeuzes
ComboBox ComboBoxKleuren = new(); scherm.Controls.Add(ComboBoxKleuren);
ComboBoxKleuren.Location = new Point(150, 170);
ComboBoxKleuren.Size = new Size(100, 30);
ComboBoxKleuren.Items.Add("ZwartWit");
ComboBoxKleuren.Items.Add("Oase");
ComboBoxKleuren.Items.Add("Blauw");

//Door TekenBitmap uit te voeren, worden de mandelgetallen en kleuren bepaald. 
void TekenBitmap(object o, PaintEventArgs pea)
{
    for (int x = 0; x < 400; x++)
    {
        for (int y = 0; y < 400; y++)
        { 
            int Mandelgetal = mandelgetal(x,y);
            GeefKleur(Mandelgetal, x, y);
        }
        afbeelding.Invalidate();
    }
}

//Berekenen Mandelgetal.
int mandelgetal(double x, double y)
{
    int Mandelgetal = 0;
    double a = 0;
    double b = 0;
    while (AfstandTotMidden(a, b) <= 4 && Mandelgetal < MaxA)       //Van afstandTotMidden wordt niet wortel genomen, daarom <= 4. 
    {
        double xschaal = (x - 200.0) * Schaal + MidX;               //xschaal en yschaal zijn de middelpunten van het scherm. 
        double yschaal = (-y + 200.0) * Schaal + MidY;
        double oudea = a;
        double oudeb = b;
        a = Nieuwea(oudea, oudeb, xschaal);         //Hier wordt f(a,b) berekend.  
        b = Nieuweb(oudea, oudeb, yschaal);         //Merk hierbij op dat we oudea en oudeb gebruikt worden, omdat ander f(b) niet goed berekend wordt.
        Mandelgetal++;                              //Er wordt ??n bij Mandelgetal opgeteld. 
    }
    return Mandelgetal;                             //Mandelgetal wordt teruggegeven. 
}


// Dit is de methode om de kleuren uit de Combobox 
void GeefKleur(int Mandelgetal, int x, int y)
{
    Color kleur;
    switch (ComboBoxKleuren.SelectedItem)
    {
        case "ZwartWit":
            kleur = ZwartWit(Mandelgetal);
            break;
        case "Oase":
            kleur = Oase(Mandelgetal);
            break;
        case "Blauw":
            kleur = Blauw(Mandelgetal);
            break;
        default:
            kleur = ZwartWit(Mandelgetal);
            break;
    }
    bm.SetPixel(x, y, kleur);                                       //Pixel (x,y) krijgt een kleur zoals hierboven bepaald.
    afbeelding.Invalidate();
}

/*In BoxVeranderd wordt gekeken of er een getal in de tekstvakken is veranderd. 
Vervolgens wordt er eventueel een komma in de doubles vervangen door een punt.
Deze waarde wordt als double uitgelezen, op de manier hoe de desbetreffende computer hiermee kan rekenen.
Als er 'verkeerde waarden'(geen doubles in midden x, midden y en schaal en geen integer in maximaal aantal) in de tekstvakken
worden ingevuld, zal het tekstvak door de catch rood kleuren.
 */
void BoxVeranderd(object sender, EventArgs e)
{
    try
    {
        string GoedeX = boxMidX.Text.Replace(',', '.');
        MidX = double.Parse(GoedeX, CultureInfo.InvariantCulture); boxMidX.BackColor = Color.White;
        string GoedeY = boxMidY.Text.Replace(',', '.');
        MidY = double.Parse(GoedeY, CultureInfo.InvariantCulture); boxMidY.BackColor = Color.White;
        string GoedeSchaal = boxSchaal.Text.Replace(',', '.');
        Schaal = double.Parse(GoedeSchaal, CultureInfo.InvariantCulture); boxSchaal.BackColor = Color.White;
        MaxA = int.Parse(boxMaxA.Text); boxMaxA.BackColor = Color.White;
        afbeelding.Invalidate();
    }

    catch (Exception)
    {
        ((TextBox)sender).BackColor = Color.Red;
    }
}

//Als de Go-knop wordt gebruikt, wordt het scherm herladen. 
void KlikGo(object sender, EventArgs e)
{
    if (sender == Go)
    {
        scherm.Invalidate();
    }
}

/*
Bij muisklik wordt in- of uitgezoomd (respectievelijk linkermuisknop en rechtermuisknop). 
Ook wordt de pixel waar de muis op klikt als nieuw middelpunt van de mandelbrot. 
*/
void MuisKlik(object sender, MouseEventArgs e)
{
    double PixelX = e.X;                                                //Hier wordt het pixelco?rdinaat van de muisklik opgeslagen. 
    double PixelY = e.Y;
    double OudeSchaal = Schaal;
    double CoordX = (PixelX - 200) * OudeSchaal + MidX;                 //Het midden wordt nu als verschaald co?rdinaat gegeven. 
    double CoordY = (-PixelY + 200) * OudeSchaal + MidY;
    boxMidX.Text = CoordX.ToString();                                   //Het nieuwe co?rdinaat wordt in het tekstvak gezet.
    boxMidY.Text = CoordY.ToString();
    if (e.Button == System.Windows.Forms.MouseButtons.Left)             //Bij linkermuisknop wordt het plaatje ingezoomd. 
    {
        Schaal *= 0.5;
        double NieuweSchaal = Schaal;
        boxSchaal.Text = NieuweSchaal.ToString();
    }
    else if (e.Button == System.Windows.Forms.MouseButtons.Right)       //Bij rechtermuisknop wordt het plaatje uitgezoomd. 
    {
        Schaal *= 2;
        double NieuweSchaal = Schaal;
        boxSchaal.Text = NieuweSchaal.ToString();
    }
    for (int x = 0; x < 400; x++)                      //Nu wordt weer de kleur van iedere pixel bepaald aan de hand van Mandelgetal. 
    {
        for (int y = 0; y < 400; y++)
        {
            int Mandelgetal = mandelgetal(x, y);
            GeefKleur(Mandelgetal, x, y);
        }
        afbeelding.Invalidate();
    }
}

/*Hier wordt de afstand van een pixel tot het midde berekend.
Merk op dat de wortel weggelaten is, hierdoor is in TekenBitmap <=4 gebruikt. */
double AfstandTotMidden(double a, double b)
{
    double d = a * a + b * b;
    return d;
}

//f(a) wordt berekend. 
double Nieuwea(double a, double b, double xschaal)
{
    double nieuwea = a * a - b * b + xschaal;
    return nieuwea;
}

//f(b) wordt berekend. 
double Nieuweb(double a, double b, double yschaal)
{
    double nieuweb = 2 * a * b + yschaal;
    return nieuweb;
}

//Als het mandelgetal even is en kleiner is dat het maximale aantal, wordt de pixel zwart, anders wit. 
Color ZwartWit(int Mandelgetal)
{
    if (Mandelgetal % 2 == 0 && Mandelgetal <= MaxA)
        return Color.Black;
    else
        return Color.White;
}

/* hier wordt het colorpalet van oase aangeduid.
   Verschillende mandelgetallen hebben verschillende kleuren */
Color Oase(int Mandelgetal)
{
        int R = 0;
        int G = 0;
        int B = 0;
        while (Mandelgetal > 767)
            Mandelgetal -= 256;
        if (Mandelgetal >= 512)
        {
            R = Mandelgetal - 512;
            G = 255 - R;
            return Color.FromArgb(R, G, B);
        }
        else if (Mandelgetal >= 256)
        {
            G = Mandelgetal - 256;
            B = 255 - G;
            return Color.FromArgb(R, G, B);
        }
        else
        {
            B = Mandelgetal;
            return Color.FromArgb(R, G, B);
        }       
}

/* Hier wordt de kleur blauw aangeduid.
Blauw is hier met "schaduwen" vanwege de procent/rest delingen*/
Color Blauw(int Mandelgetal)
{
    if (Mandelgetal % 2 == 0)
        return Color.Black;
    else
        return Color.FromArgb(0, 0, Mandelgetal % 16 * 15);
}

//Hier wordt functies herladen als er iets is veranderd/gebeurd.
Go.Click += KlikGo;
boxMidX.TextChanged += BoxVeranderd;
boxMidY.TextChanged += BoxVeranderd;
boxSchaal.TextChanged += BoxVeranderd;
boxMaxA.TextChanged += BoxVeranderd;
BoxVeranderd(null, null);
afbeelding.MouseClick += MuisKlik;
scherm.Paint += TekenBitmap;
Application.Run(scherm);                           //Als laatste wordt het scherm herladen om de nieuwe waarden/afbeelding te laden.