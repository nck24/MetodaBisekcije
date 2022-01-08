using System;

namespace MetodaBisekcije{
    class Program
    {
        static void Main(string[] args){
            double[] koeficijenti = KoeficientiPolinoma();
            Console.Write("Na koliko decimalnih mest hoces imeti nicle : ");
            int natancnostNicel = Convert.ToInt32(Console.ReadLine());
            List<double[]> koefOdvodov = new List<double[]>();

            //Dodamo v LIST arr koef vseh odvodov do kvadratne funk (stopnje pol padajo po LISTu)
            koefOdvodov.Add(koeficijenti); //Vlist tudi damo polinom za katerega iscemo nicle
            for (int i = 0; i < (MaxStopnja(koeficijenti) - 2); i++){
                double[] odvod = KoeficientiOdvoda(koefOdvodov[koefOdvodov.Count - 1]);
                koefOdvodov.Add(odvod);
            }

            List<double> intervaliNicel = new List<double>(); //Vrednost za intervale kjer so potencialne nicle

            //Dodamo vse intervale v intervaliNicel
            if (NicleKvadratneFunk(koefOdvodov.Last())[0] != null){
                intervaliNicel.Add(-1000000);
                foreach(double d in NicleKvadratneFunk(koefOdvodov.Last())){
                    intervaliNicel.Add(d);
                }
                intervaliNicel.Add(1000000);
            }else{
                intervaliNicel.Add(-1000000); 
                intervaliNicel.Add(1000000);
            }
            

            //Za vsak odvod najdemo nove nicle in spremenimo intervaliNicel
            for (int i = koefOdvodov.Count - 2; i >= 0; i--){
                intervaliNicel.Sort();
                List<double> nicleTegaPolinoma = new List<double>(); //Nicle polinoma za katerega iscemo nicle
                nicleTegaPolinoma.Add(-1000000);
                //Izracunamo niclo vsakega intervala ce obstaja
                for (int j = 0; j <= intervaliNicel.Count - 2; j++){

                    if (Math.Abs(VrednostFunkVX(koefOdvodov[i], intervaliNicel[j])) <= 0.001){
                        nicleTegaPolinoma.Add(intervaliNicel[j]);
                    }else{
                        if (ImataRazlicenPredznak(koefOdvodov[i], intervaliNicel[j], intervaliNicel[j + 1])){
                            double poz;
                            double neg;
                            if (VrednostFunkVX(koefOdvodov[i], intervaliNicel[j]) < 0){
                                poz = intervaliNicel[j + 1];
                                neg = intervaliNicel[j];
                            }else{
                                poz = intervaliNicel[j];
                                neg = intervaliNicel[j + 1];
                            }

                            nicleTegaPolinoma.Add(IzracunNicleNaIntervalu(koefOdvodov[i], poz, neg, natancnostNicel + 3));
                        }
                    }
                }
                nicleTegaPolinoma.Add(1000000);

                intervaliNicel.Clear();
                foreach(double d in nicleTegaPolinoma){
                    intervaliNicel.Add(d);
                }
                nicleTegaPolinoma.Clear();
            }

            intervaliNicel.RemoveAt(0);intervaliNicel.Remove(intervaliNicel.Last());
            Console.WriteLine("\nNasel sem te realne nicle :\n");
            for (int i = 0; i <= intervaliNicel.Count - 1; i++){
                Console.WriteLine("{0}. nicla je : {1}", i + 1, Math.Round(intervaliNicel[i], natancnostNicel));
            }
           
            Console.ReadLine();
        }

        //Vrne array kjer je velikost arr - 1 = st(polinoma). Koef. so razvrsceni po padanju stopnjie 
        static double[] KoeficientiPolinoma(){
            Console.Write("Napisi polinom z vsemi koef. : ");
            string polinom = Console.ReadLine().Replace(" ", "");

            string[] strKoef = polinom.Split('^');
            double[] koef = new double[strKoef.Length - 1];
            koef[0] = Convert.ToDouble(strKoef[0].Substring(0, strKoef[0].Length - 1));

            for (int i = 1; i < strKoef.Length - 1; i++){
                int pozPredznaka = strKoef[i].IndexOf('+') + strKoef[i].IndexOf('-') + 1;
                strKoef[i] = strKoef[i].Substring(pozPredznaka);
                strKoef[i] = strKoef[i].Substring(0, strKoef[i].Length - 1);
                koef[i] = Convert.ToDouble(strKoef[i]);
            }

            return koef;
        }
    
        //Vrne stopnjo polinoma
        static int MaxStopnja(double[] koef){
            return koef.Length - 1;
        }

        //Vrne vrednost polinoma za doloceno vrednost   
        static double VrednostFunkVX(double[] koef, double x){
            int stopnja = MaxStopnja(koef);
            double rezultat = 0;
            for (int i = 0; i <= MaxStopnja(koef); i++){
                rezultat += koef[i] * Math.Pow(x, stopnja);
                stopnja--;
            }
            return rezultat;
        }

        //Vrne arr koef. tako kot KoeficientPolinoma, v FUNKCIJO vstavimo koef. polinoma in vrne njegov odvod  
        static double[] KoeficientiOdvoda(double[] koefFunk){
            double[] koefOdvoda = new double[koefFunk.Length - 1];
            int stopnjaFunk = MaxStopnja(koefFunk);
            for (int i = 0; i < koefOdvoda.Length; i++){
                koefOdvoda[i] = koefFunk[i] * stopnjaFunk;
                stopnjaFunk--;
            }
            return koefOdvoda;
        }
    
        //Vrne arr nicel kvadratne funk.  Ce nicel ni vrne null   Ce so dvojne nicle vrne dve isti nicli
        static double?[] NicleKvadratneFunk(double[] koefKvadratneFunk){
            double?[] nicli = new double?[2];
            double determinanta = Math.Sqrt((Math.Pow(koefKvadratneFunk[1], 2) - 4 * koefKvadratneFunk[0] * koefKvadratneFunk[2]));
            if (determinanta < 0){
                nicli[0] = null;
                nicli[1] = null;
                return nicli;
            }else{
                nicli[0] = (-koefKvadratneFunk[1] + determinanta) / (2 * koefKvadratneFunk[0]);
                nicli[1] = (-koefKvadratneFunk[1] - determinanta) / (2 * koefKvadratneFunk[0]);
                return nicli;
            }
        }
    
        //Vrne niclo ki se nahaja na nekem intervalu  POZOR!!!! : ce na tem intervalu ni nicle se ta FUNKCIJA ne bo nikoli koncala
        static double IzracunNicleNaIntervalu(double[] koefFunk, double pozVrednostIntervala, double negVrednostIntervala, int mestaNatančnosti){
            double[] interval = new double[] {pozVrednostIntervala, negVrednostIntervala};

            while (Math.Round(interval[0], mestaNatančnosti) != Math.Round(interval[1], mestaNatančnosti)){
                double povp = Povp(interval);
                if (VrednostFunkVX(koefFunk, povp) > 0){
                    interval[0] = povp;
                }else{
                    interval[1] = povp;
                }
            }

            return Math.Round(interval[0], mestaNatančnosti);
        }

        //Vrne povprecije stevil v arr
        static double Povp(double[] stevila){
            double vsota = 0;
            foreach (double d in stevila){
                vsota += d;
            }
            return (vsota / stevila.Length);
        }

        //Vrne true ce imata razlicen predznak in false ce imata istega
        static bool ImataRazlicenPredznak(double[] koef, double interval1, double interval2){
            if ( ( VrednostFunkVX(koef, interval1) < 0 ) && (VrednostFunkVX(koef, interval2) < 0 ) ){
               return false;
            }else if( ( VrednostFunkVX(koef, interval1) > 0 ) && (VrednostFunkVX(koef, interval2) > 0) ){
                return false;
            }
            else{
                return true;
            }
        }
    }
}