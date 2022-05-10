using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WarsztatV2.Tables;


namespace WarsztatV2.Faktury
{
    class GenerowanieFaktur
    {

        public static async void GenerujFakture(Faktura fav)
        {
            Faktura FakturaFav = new Faktura();
            Warsztat WarsztatFav = new Warsztat();
            Adres AdresWarsztatFav = new Adres();
            Naprawa NaprawaFav = new Naprawa();
            Klient KlientFav = new Klient();
            Adres AdresKlientFav = new Adres();
            Uzyte_czesci Uzyte_CzesciFav = new Uzyte_czesci();
            List<Uzyte_czesci> czesci = new List<Uzyte_czesci>();

            using (databaseConnection newConnection = new databaseConnection())
            {
                FakturaFav = await Task.Run(() => { return newConnection.Faktury.Single<Faktura>(a => a.ID_Naprawa == fav.ID_Naprawa); });
                WarsztatFav = await Task.Run(() => { return newConnection.Warsztaty.Single<Warsztat>(a => a.ID_Warsztat== fav.ID_Warsztat); });
                AdresWarsztatFav = await Task.Run(() => { return newConnection.Adresy.Single<Adres>(a => a.ID_Adres == WarsztatFav.ID_Adres); });
                NaprawaFav = await Task.Run(() => { return newConnection.Naprawy.Single<Naprawa>(a => a.ID_Naprawa == fav.ID_Naprawa); });
                KlientFav = await Task.Run(() => { return newConnection.Klienci.Single<Klient>(a => a.ID_Klient == fav.ID_Klient); });
                AdresKlientFav = await Task.Run(() => { return newConnection.Adresy.Single<Adres>(a => a.ID_Adres == KlientFav.ID_Adres); });
                czesci = newConnection.Uzyte_czesci.Where<Uzyte_czesci>(u => u.ID_Naprawa == fav.ID_Naprawa).ToList();
            }
            int ilosczurzytychczesci = czesci.Count();
            int dlugoscTabeli = 230 + ilosczurzytychczesci * 20;

            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Times New Roman", 10);
            XFont fontBold = new XFont("Times New Roman", 10, XFontStyle.Bold);
            XFont font2 = new XFont("Times New Roman", 20, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);

            XRect rect, rect2, rect3, rect4, rect5, rect6, rect7, rect8, rect9, rect10, rect11, rect12, rect13, rect20, rect21, rect22, rect23, rect24, rect25, rect26, rect27, rect31, rect32;

            rect = new XRect(40, 120, 250, 220);
            rect2 = new XRect(40, 135, 250, 220);
            rect3 = new XRect(200, 20, 250, 220);
            rect4 = new XRect(400, 75, 250, 220);
            rect5 = new XRect(400, 90, 250, 220);

            rect6 = new XRect(400, 120, 250, 220);
            rect7 = new XRect(400, 135, 250, 220);

            tf.DrawString("Faktura nr  " + FakturaFav.ID_Faktura.ToString() + " / " + DateTime.Now.ToString("MM/yyyy"), font2, XBrushes.Black, rect3, XStringFormats.TopLeft);

            tf.DrawString("Data Wystawienia:       " + DateTime.Now.ToString("dd/MM/yyyy"), font, XBrushes.Black, rect4, XStringFormats.TopLeft);
            tf.DrawString("Miejsce Wystawienia:   " + AdresWarsztatFav.Miejscowosc, font, XBrushes.Black, rect5, XStringFormats.TopLeft);


            XPen pen = new XPen(XColors.Black, 1);
            XPoint p1 = new XPoint(5, 110);
            XPoint p2 = new XPoint(590, 110);
            gfx.DrawLine(pen, p1, p2);

            string danewarsztat = WarsztatFav.Nazwa + " \n" +
                                  AdresWarsztatFav.Miejscowosc + " " +
                                  AdresWarsztatFav.Ulica + " " +
                                  AdresWarsztatFav.Numer + "\n" +
                                  AdresWarsztatFav.Kod_pocztowy + " " +
                                  AdresWarsztatFav.Miejscowosc + "\n" +
                                  "NIP: " + WarsztatFav.NIP;

            tf.DrawString("Sprzedawca:\n", fontBold, XBrushes.Black, rect, XStringFormats.TopLeft);
            tf.DrawString(danewarsztat, font, XBrushes.Black, rect2, XStringFormats.TopLeft);

            string daneklient = KlientFav.Imie + " " +
                                KlientFav.Nazwisko + " \n" +
                                AdresKlientFav.Miejscowosc + " " +
                                AdresKlientFav.Ulica + " " +
                                AdresKlientFav.Numer + "\n" +
                                AdresKlientFav.Kod_pocztowy + " " +
                                AdresKlientFav.Miejscowosc + "\n" +
                                KlientFav.Telefon.ToString();

            tf.DrawString("Nabywca:\n", fontBold, XBrushes.Black, rect6, XStringFormats.TopLeft);
            tf.DrawString(daneklient, font, XBrushes.Black, rect7, XStringFormats.TopLeft);

            XPen pen2 = new XPen(XColors.Black, 0.3);
            XPoint t1 = new XPoint(30, 210);
            XPoint t2 = new XPoint(555, 210);
            gfx.DrawLine(pen2, t1, t2);

            XPoint t3 = new XPoint(30, dlugoscTabeli);
            gfx.DrawLine(pen2, t1, t3);

            XPoint t4 = new XPoint(555, dlugoscTabeli);
            gfx.DrawLine(pen2, t2, t4);


            XPoint t30 = new XPoint(30, dlugoscTabeli);
            XPoint t40 = new XPoint(555, dlugoscTabeli);
            gfx.DrawLine(pen2, t30, t40);

            XPoint t5 = new XPoint(55, 210);
            XPoint t6 = new XPoint(55, dlugoscTabeli);

            gfx.DrawLine(pen2, t5, t6);
            rect8 = new XRect(35, 215, 65, 210);
            tf.DrawString("Lp", fontBold, XBrushes.Black, rect8, XStringFormats.TopLeft);



            XPoint t7 = new XPoint(250, 210);
            XPoint t8 = new XPoint(250, dlugoscTabeli);
            gfx.DrawLine(pen2, t7, t8);
            rect9 = new XRect(90, 215, 250, 210);
            tf.DrawString("Nazwa towaru lub usługi", fontBold, XBrushes.Black, rect9, XStringFormats.TopLeft);

            XPoint t9 = new XPoint(300, 210);
            XPoint t10 = new XPoint(300, dlugoscTabeli);
            gfx.DrawLine(pen2, t9, t10);
            rect10 = new XRect(265, 215, 300, 210);
            tf.DrawString("Ilosc", fontBold, XBrushes.Black, rect10, XStringFormats.TopLeft);


            XPoint t11 = new XPoint(380, 210);
            XPoint t12 = new XPoint(380, dlugoscTabeli);
            gfx.DrawLine(pen2, t11, t12);
            rect11 = new XRect(310, 215, 370, 210);
            tf.DrawString("Wartość netto", fontBold, XBrushes.Black, rect11, XStringFormats.TopLeft);

            XPoint t13 = new XPoint(455, 210);
            XPoint t14 = new XPoint(455, dlugoscTabeli);
            gfx.DrawLine(pen2, t13, t14);
            rect12 = new XRect(390, 215, 470, 210);
            tf.DrawString("Stawka VAT", fontBold, XBrushes.Black, rect12, XStringFormats.TopLeft);


            rect13 = new XRect(470, 215, 560, 210);
            tf.DrawString("Wartość Brutto", fontBold, XBrushes.Black, rect13, XStringFormats.TopLeft);

            int dl = 20;
            List<XRect> rectt = new List<XRect>();
            List<XRect> rectt2 = new List<XRect>();
            List<XRect> rectt3 = new List<XRect>();
            List<XRect> rectt4 = new List<XRect>();
            List<XRect> rectt5 = new List<XRect>();
            List<XRect> rectt6 = new List<XRect>();

            double suma = 0;
            for (int i = 0; i < ilosczurzytychczesci; i++)
            {

                int dll = 210;
                XPoint tpr = new XPoint(30, dll += dl);
                XPoint tpl = new XPoint(555, dll);
                gfx.DrawLine(pen2, tpr, tpl);
                rectt.Add(new XRect(35, 215 + dl, 65, 210));
                rectt2.Add(new XRect(90, 215 + dl, 250, 210));
                rectt3.Add(new XRect(265, 215 + dl, 300, 210));
                rectt4.Add(new XRect(310, 215 + dl, 370, 210));
                rectt5.Add(new XRect(390, 215 + dl, 470, 210));
                rectt6.Add(new XRect(470, 215 + dl, 560, 210)); dl += 20;

                Czesc UzytaczescFav = new Czesc();

                using (databaseConnection newConnection = new databaseConnection())
                {
                    int l = czesci[i].ID_Czesci;
                    UzytaczescFav = await Task.Run(() => { return newConnection.Czesci.Single<Czesc>(a => a.ID_Czesci == l); });
                }

                tf.DrawString((i + 1).ToString(), font, XBrushes.Black, rectt[i], XStringFormats.TopLeft);
                tf.DrawString(UzytaczescFav.Nazwa, font, XBrushes.Black, rectt2[i], XStringFormats.TopLeft);
                tf.DrawString(czesci[i].Ilosc.ToString(), font, XBrushes.Black, rectt3[i], XStringFormats.TopLeft);
                tf.DrawString((UzytaczescFav.Cena * czesci[i].Ilosc).ToString(), font, XBrushes.Black, rectt4[i], XStringFormats.TopLeft);
                tf.DrawString("23%", font, XBrushes.Black, rectt5[i], XStringFormats.TopLeft);
                tf.DrawString((Math.Round(UzytaczescFav.Cena * 1.23 * czesci[i].Ilosc, 2)).ToString(), font, XBrushes.Black, rectt6[i], XStringFormats.TopLeft);
                suma += UzytaczescFav.Cena * 1.23 * czesci[i].Ilosc;

            }

            XPoint p10 = new XPoint(30, dlugoscTabeli + 15);
            XPoint p11 = new XPoint(250, dlugoscTabeli + 15);
            gfx.DrawLine(pen2, p10, p11);

            rect20 = new XRect(30, dlugoscTabeli + 20, 250, 220);
            rect21 = new XRect(120, dlugoscTabeli + 20, 250, 220);
            tf.DrawString("Sposób płatności: ", fontBold, XBrushes.Black, rect20, XStringFormats.TopLeft);
            tf.DrawString("przelew w terminie 14 dni", font, XBrushes.Black, rect21, XStringFormats.TopLeft);

            XPoint p12 = new XPoint(30, dlugoscTabeli + 35);
            XPoint p13 = new XPoint(250, dlugoscTabeli + 35);
            gfx.DrawLine(pen2, p12, p13);

            rect22 = new XRect(30, dlugoscTabeli + 40, 250, 220);
            rect23 = new XRect(120, dlugoscTabeli + 40, 250, 220);
            tf.DrawString("Termin płatność: ", fontBold, XBrushes.Black, rect22, XStringFormats.TopLeft);
            tf.DrawString(DateTime.Now.AddDays(14).ToString("dd/MM/yyyy"), font, XBrushes.Black, rect23, XStringFormats.TopLeft);

            XPoint p14 = new XPoint(30, dlugoscTabeli + 55);
            XPoint p15 = new XPoint(250, dlugoscTabeli + 55);
            gfx.DrawLine(pen2, p14, p15);

            rect24 = new XRect(30, dlugoscTabeli + 60, 250, 220);
            rect25 = new XRect(120, dlugoscTabeli + 60, 250, 220);
            tf.DrawString("Numer Konta: ", fontBold, XBrushes.Black, rect24, XStringFormats.TopLeft);
            tf.DrawString(WarsztatFav.Numer_konta_bankowego, font, XBrushes.Black, rect25, XStringFormats.TopLeft);

            // XPoint p16 = new XPoint(30, dlugoscTabeli + 75);
            //  XPoint p17 = new XPoint(250, dlugoscTabeli + 75);
            //  gfx.DrawLine(pen2, p16, p17);



            XPoint p22 = new XPoint(350, dlugoscTabeli + 15);
            XPoint p23 = new XPoint(470, dlugoscTabeli + 15);
            gfx.DrawLine(pen2, p22, p23);

            rect31 = new XRect(350, dlugoscTabeli + 20, 250, 220);
            rect32 = new XRect(400, dlugoscTabeli + 20, 250, 220);
            tf.DrawString("Do zapłaty: ", fontBold, XBrushes.Black, rect31, XStringFormats.TopLeft);
            suma = Math.Round(suma, 2);
            tf.DrawString(suma + " zł", font, XBrushes.Black, rect32, XStringFormats.TopLeft);

            // XPoint p12 = new XPoint(30, dlugoscTabeli + 35);
            // XPoint p13 = new XPoint(250, dlugoscTabeli + 35);
            // gfx.DrawLine(pen2, p12, p13);


            XPoint p18 = new XPoint(50, dlugoscTabeli + 150);
            XPoint p19 = new XPoint(250, dlugoscTabeli + 150);
            gfx.DrawLine(pen2, p18, p19);

            rect26 = new XRect(60, dlugoscTabeli + 150, 350, 220);
            tf.DrawString("Podpis osoby upoważnionej do wystawienia", font, XBrushes.Black, rect26, XStringFormats.TopLeft);

            XPoint p20 = new XPoint(350, dlugoscTabeli + 150);
            XPoint p21 = new XPoint(550, dlugoscTabeli + 150);
            gfx.DrawLine(pen2, p20, p21);

            rect27 = new XRect(360, dlugoscTabeli + 150, 500, 220);
            tf.DrawString("Podpis osoby upoważnionej do odbioru ", font, XBrushes.Black, rect27, XStringFormats.TopLeft);

            string filename = FakturaFav.ID_Faktura.ToString() + ".pdf";

            document.Save(filename);

            Process.Start(filename);

        }



    }
}
