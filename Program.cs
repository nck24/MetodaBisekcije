using System;

namespace MetodaBisekcije{
    class Program
    {
        static void Main(string[] args){
            double[] polinom = KoeficientiPolinoma();
            Console.Write("Na koliko decimalk natancno (max 14) : ");
            int natancnost = int.Parse(Console.ReadLine());

            Console.WriteLine("\n Realne nicle :");
            foreach (double nicla in NiclePolinoma(polinom, natancnost)){
                Console.WriteLine("  {0}", nicla);
            }
            
            Console.ReadLine();
        }

        //Vrne array kjer je velikost arr - 1 = st(polinoma). Koef. so razvrsceni po padanju stopnjie 
        static double[] KoeficientiPolinoma(){
            Console.Write("Napisi polinom : ");
            string polinom = Console.ReadLine().Replace(" ", "");


            double[] koeficienti = new double[StopnjaPoli(polinom) + 1];

            int pozZnaka = polinom.IndexOf('^');
            while (pozZnaka != -1){
                if (polinom[0] == 'x'){
                    koeficienti[StopnjaPoli(polinom)] = 1D;
                }else if (polinom[0] == '-' && polinom[1] == 'x'){
                    koeficienti[StopnjaPoli(polinom)] = -1D;
                }
                else if(polinom[0] == '+' && polinom[1] == 'x'){
                    koeficienti[StopnjaPoli(polinom)] = 1D;
                }
                else{
                    koeficienti[StopnjaPoli(polinom)] = Convert.ToDouble(polinom.Substring(0, polinom.IndexOf('x')));
                }

                int dolzinaStopnje = StopnjaPoli(polinom).ToString().Length;
                polinom = polinom.Substring(pozZnaka + 1 + dolzinaStopnje);

                pozZnaka = polinom.IndexOf('^');
            }

            if (polinom.IndexOf('x') != -1){

                if (polinom[0] == 'x' || (polinom[0] == '+' && polinom[1] == 'x')){
                    koeficienti[1] = 1D;
                }else if (polinom[0] == '-' && polinom[1] == 'x'){
                    koeficienti[1] = -1D;
                }else{
                    koeficienti[1] = Convert.ToDouble(polinom.Substring(0, polinom.IndexOf('x')));
                }
                polinom = polinom.Substring(polinom.IndexOf('x') + 1);
            }

            if (polinom.Length != 0){
                koeficienti[0] = Convert.ToDouble(polinom);
            }

            Array.Reverse(koeficienti);
            return koeficienti;
        }

        //Vrne stopnjo polinoma
        //Potrebno samo za FUNK Koeficienti polinoma
        static int StopnjaPoli(string polinom){
            if (polinom.IndexOf('^') != -1){
                string a = polinom.Substring(polinom.IndexOf('^') + 1);
                
                if (a.IndexOf('-') == -1 && a.IndexOf('+') == -1){
                    return Convert.ToInt32(a);
                }else if (a.IndexOf('-') == -1){
                    return Convert.ToInt32(a.Substring(0, a.IndexOf('+')));
                }else if (a.IndexOf('+') == -1){
                    return Convert.ToInt32(a.Substring(0, a.IndexOf('-')));
                }
                else{
                    return Convert.ToInt32(a.Substring(0, Math.Min(a.IndexOf('+'), a.IndexOf('-'))));
                }
            }else{
                return 1;
            }
        }
    
        static double[] NiclePolinoma(double[] polinom, int natancnost){
            if (polinom.Length == 2){
                double nicla;
                nicla = (double)polinom[1] * (-1D);
                return new double[] {Math.Round((nicla / (double)polinom[0]), natancnost)};
            }else if (polinom.Length == 3){
                double diskriminanta = ((double)polinom[1]*(double)polinom[1]) - (4*(double)polinom[0]*(double)polinom[2]);
                if (diskriminanta > 0){
                    double[] nicli = new double[2];
                    nicli[0]= Math.Round((((double)polinom[1] * (-1)) - SquereRoot(diskriminanta)) / ((double)polinom[0]*2), natancnost);
                    nicli[1]= Math.Round((((double)polinom[1] * (-1)) + SquereRoot(diskriminanta)) / ((double)polinom[0]*2), natancnost);

                    return nicli;
                }else if(diskriminanta == 0){
                    return new double[] {(double)polinom[1] * (-1) / ((double)polinom[0]*2D)};
                }
                else{
                    return new double[] {};
                }
            }
            else{
                List<double> nicle = new List<double>();

                List<double> intervali = new List<double>(NiclePolinoma(OdvodPolinoma(polinom), 14));
                if (intervali.Count != 0){
                    intervali.Sort();
                    intervali.Add(intervali.Last() + 1000D);
                    intervali.Add(intervali[0] - 1000D);
                    intervali.Sort();
                }else{
                    intervali.Add(-1000000);
                    intervali.Add(1000000);
                }

                for (int i = 0; i < intervali.Count - 1; i++){
                    if (Math.Abs(VrednostVX(polinom, intervali[i])) <= 0.0000000000001D){
                        nicle.Add(intervali[i]);
                    }
                    else if (VrednostVX(polinom, intervali[i]) * VrednostVX(polinom, intervali[i + 1]) < 0){
                        nicle.Add(Bisekcija(polinom, intervali[i], intervali[i + 1], natancnost));
                    }
                }

                return nicle.ToArray();
            }
        }

        //Izracuna koren stevila
        //vrne in uporabi decimal
        static double SquereRoot(double square)
        {
            if(square==0)return 0;
            double root = square / 3;
            int i;
            for (i = 0; i < 32; i++)
                root = (root + square / root) / 2;
            return root;
        }
    
        //Potenciramo decimal
        static double Potenciranje(double osnova, int eksponent){
            double resitev = 1D;
            for (int i = 1; i <= eksponent; i++){
                resitev = resitev * osnova;
            }
            return resitev;
        }
        
        //Izracuna vrednost polinoma v dani točki
        static double VrednostVX(double[] polinom, double x){
            int stopnja = polinom.Length - 1;
            double resitev = 0;

            foreach (double koef in polinom)
            {
                resitev += koef * Potenciranje(x, stopnja);
                stopnja--;
            }
            return resitev;
        }
        
        //Vrne odvod polinoma
        static double[] OdvodPolinoma(double[] polinom){
            int stopnjaOdvoda = polinom.Length - 2;
            double[] odvod = new double[stopnjaOdvoda + 1];

            int stopnjaPolinoma = polinom.Length - 1;
            for(int i = 0; i <= stopnjaOdvoda; i++){
                odvod[i] = polinom[i] * stopnjaPolinoma;
                stopnjaPolinoma -= 1;
            }

            return odvod;
        }
    
        //Iscemo niclo na intervalu
        static double Bisekcija(double[] polinom, double x1, double x2, int natancnost){
            double[] interval = new double[2];
            if (VrednostVX(polinom, x1) < VrednostVX(polinom, x2)){
                interval[0] = x1;
                interval[1] = x2;
            }else{
                interval[1] = x1;
                interval[0] = x2;
            }

            while (Math.Round(interval[0], natancnost) != Math.Round(interval[1], natancnost)){
                double noviX = (interval[0] + interval[1]) / 2D;
                if (VrednostVX(polinom, noviX) < 0){
                    interval[0] = noviX;
                }else{
                    interval[1] = noviX;
                }
            }

            return Math.Round(interval[0], natancnost);
        }
    }
}