// SkyWatcher core
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SkyWatcher {
    public class SkyObject {
        public string Name;
        public SkyObject() {
            
        }
        public SkyObject(string name) {
            Name = name;
        }
        public override string ToString() {
            return Name;
        }
    }
    [ComVisible(true)]
    public class Star : SkyObject {
        public static int readystars;
        public static int totalstars = 261;
        public double RA;
        public double RA2;
        public double Dec;
        public StarProperties Properties;
        public bool IsNamed {
            get {
                return !(Name.Contains(" ")) || !(Name.Contains(GetConstellation().Genitive));
            }
        }
        public Star(string name, double ra, double dec) {
            OnAddedStar(name);
            Name = name;
            RA = ra / 60;
            RA2 = RA - 24;
            Dec = dec;
            CustomColor = Color.FromArgb(70, 255, 255);
        }
        internal static void OnAddedStar(string name) {
            readystars++;
            Console.WriteLine("Initializing {0} ({1}/{2})", name, readystars, totalstars);
        }
        public override string ToString() {
            string nl = "\r\n";
            double ra = Math.Round(RA, 3, MidpointRounding.AwayFromZero);
            return Name + nl + "Coordinates: (" + ra + "h, " + Dec + "\u00b0" + ")" + nl;
        }
        public Point GetLocation(double relative_ra, double relative_dec) {
            double ra = relative_ra - (RA * 60);
            double dec = relative_dec - Dec;
            return new Point((int)(ra * 5), (int)(dec * 20));
        }
        public Point GetLocation2(double relative_ra, double relative_dec) {
            double ra = relative_ra - (RA2 * 60);
            double dec = relative_dec - Dec;
            return new Point((int)(ra * 5), (int)(dec * 20));
        }
        public Constellation GetConstellation() {
            SkyObjectLibrary.Search(Name);
            int index = SkyObjectLibrary.last_index;
            for (int i = index; i >= 0; i--) {
                SkyObject current = SkyObjectLibrary.GetItem(i);
                if (current is Constellation) return (Constellation)(current);
            }
            throw new Exception("Invalid library.");
        }
        static readonly string[] months = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
        public string GetBestMonth() {
            double temp = RA;
            temp -= 6;
            if (temp < 0) temp += 24;
            temp /= 2;
            int result = (int)(Math.Truncate(temp));
            return months[result];
        }
        public Color CustomColor;
        public Star(string name, double ra, double dec, Color displayColor) : this(name, ra, dec) {
            CustomColor = displayColor;
        }
        public override bool Equals(object obj)
        {
            return Name == ((Star)(obj)).Name;
        }
        public Star(string name, double ra, double dec, StarProperties properties) : this(name, ra, dec) {
            Properties = properties;
        }
        public Star(string name, double ra, double dec, Color displayColor, StarProperties properties) : this(name, ra, dec, displayColor) {
            Properties = properties;
        }
    }
    [Flags]
    public enum StarProperties {
        Normal = 0,
        Double = 1,
        VariableMagnitude = 2
    }
    public sealed class Group : SkyObject {
        public int[] Stars;
        public Group(string name, int[] stars) {
            Star.OnAddedStar(name);
            Name = name;
            Stars = stars;
        }
        public Star[] GetStars() {
            Star[] return_array = new Star[Stars.Length];
            for (int i = 0; i < Stars.Length; i++) {
                return_array[i] = (Star)(SkyObjectLibrary.GetItem(Stars[i]));
            }
            return return_array;
        }
        public Constellation GetConstellation() {
            return ((Star)(SkyObjectLibrary.GetItem(Stars[0]))).GetConstellation();
        }
    }
    public sealed class Constellation : SkyObject {
        public int Start;
        public int End;
        public string Genitive;
        public Constellation(string name, int start, int end, string genitive) {
            Star.OnAddedStar(name);
            Name = name;
            Start = start;
            End = end;
            Genitive = genitive;
        }
        public SkyObject[] GetStars() {
            SkyObject[] return_array = new SkyObject[(End + 1) - Start];
            for (int i = 0; i < (End + 1) - Start; i++) {
                return_array[i] = SkyObjectLibrary.GetItem(i + Start);
            }
            return return_array;
        }
    }
    public class Nebula : Star {
        public Nebula(string name, int ra, int dec) : base(name, ra, dec) {
            CustomColor = Color.FromArgb(243, 255, 128, 255);
            if (name.StartsWith("M", StringComparison.CurrentCulture) && !IsNamed) {
                Name = "M" + name.Substring(2);
            }
            if (name.StartsWith("NGC", StringComparison.CurrentCulture) && !IsNamed) {
                Name = "NGC" + name.Substring(4);
            }
        }
    }
    public class Galaxy : Star {
        public Galaxy(string name, int ra, int dec) : base(name, ra, dec) {
            CustomColor = Color.FromArgb(236, Color.Teal);
        }
    }
    public static class SkyObjectLibrary {
        public static SkyObject[] Value;
        public static SkyObject GetItem(int id) {
            return Value[id];
        }
        public static SkyObject Search(string searchText) {
            int result;
            if (int.TryParse(searchText, out result)) {
                if (result < Star.totalstars) {
                    return Value[result];
                }
            }
            for (int i = 0; i < Star.totalstars; i++) {
                if (Value[i] == null) continue;
                if (Value[i].Name == searchText) {
                    last_index = i;
                    return Value[i];
                }
            }
            last_index = -1;
            MessageBox.Show(searchText + " was not found in the program database of " + Star.totalstars + " stars, groups, constellations and more!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return new SkyObject("Invalid search");
        }
        public static int last_index;
        public static Star[] specialStars;
        public static void InitializeLibrary() {
            // Create the array
            SkyObject[] value = new SkyObject[2000];

            // Initialize stars of Andromeda
            value[1] = new Star("Alpheratz", 11, 29, StarProperties.Double);
            value[2] = new Star("Beta Andromedae", 71, 35, StarProperties.Double);
            value[3] = new Star("Gamma Andromedae", 126, 42, StarProperties.Double);
            value[4] = new Star("Delta Andromedae", 43, 31, StarProperties.Double);
            value[5] = new Star("Epsilon Andromedae", 42, 29);
            value[6] = new Star("Zeta Andromedae", 49, 23, StarProperties.VariableMagnitude);
            value[7] = new Star("Eta Andromedae", 57, 22, StarProperties.Double);
            value[8] = new Star("Theta Andromedae", 14, 39);
            value[9] = new Star("Iota Andromedae", 1423, 43, StarProperties.Double);
            value[10] = new Star("Kappa Andromedae", 1425, 44, StarProperties.Double);
            value[11] = new Star("Lambda Andromedae", 1422, 46, StarProperties.VariableMagnitude);
            value[12] = new Star("Mu Andromedae", 58, 39, StarProperties.Double);
            value[13] = new Star("Nu Andromedae", 52, 41);
            value[14] = new Star("Csi Andromedae", 80, 46);
            value[15] = new Star("Omicron Andromedae", 1385, 42, StarProperties.VariableMagnitude);
            value[16] = new Star("Pi Andromedae", 40, 33, StarProperties.Double);
            value[17] = new Star("Rho Andromedae", 22, 38);
            value[18] = new Star("R Andromedae", 24, 39, StarProperties.VariableMagnitude);
            value[19] = new Star("Sigma Andromedae", 23, 37);
            value[20] = new Star("Tau Andromedae", 101, 41, StarProperties.Double);
            value[21] = new Star("Phi Andromedae", 70, 47, StarProperties.Double);

            // Initialize stars of Antlia
            value[23] = new Star("Alpha Antliae", 631, -31);
            value[24] = new Star("Delta Antliae", 633, -30, StarProperties.Double);
            value[25] = new Star("Epsilon Antliae", 575, -37);
            value[26] = new Star("Zeta 1 Antliae", 577, -32, StarProperties.Double);
            value[27] = new Star("Zeta 2 Antliae", 581, -32);
            value[28] = new Star("Eta Antliae", 598, -36);
            value[29] = new Star("Theta Antliae", 583, -27, StarProperties.Double);
            value[30] = new Star("Iota Antliae", 655, -37);
            value[31] = new Star("S Antliae", 576, -28, StarProperties.VariableMagnitude);
            value[32] = new Star("U Antliae", 634, -39, StarProperties.VariableMagnitude);
            
            // Initialize stars of Apus
            value[34] = new Star("Alpha Apodis", 886, -79);
            value[35] = new Star("Beta Apodis", 1009, -77);
            value[36] = new Star("Gamma Apodis", 1015, -78);
            value[37] = new Star("Delta 1 Apodis", 977, -79);
            value[38] = new Star("Delta 2 Apodis", 978, -79);
            value[39] = new Star("Epsilon Apodis", 873, -80);
            value[40] = new Star("Zeta Apodis", 1052, -68);
            value[41] = new Star("Theta Apodis", 851, -76, StarProperties.VariableMagnitude);
            value[42] = new Star("Iota Apodis", 1063, -70);
            value[43] = new Star("Kappa 1 Apodis", 932, -73, StarProperties.Double);
            value[44] = new Star("Kappa 2 Apodis", 936, -73, StarProperties.Double);
            
            // Initialize stars of Aquarius
            value[46] = new Star("Alpha Aquarii", 1329, -1);
            value[47] = new Star("Beta Aquarii", 1363, -6, StarProperties.Double);
            value[48] = new Star("Gamma Aquarii", 1343, -2);
            value[49] = new Star("Delta Aquarii", 1374, -16);
            value[50] = new Star("Epsilon Aquarii", 1242, -9);
            value[51] = new Star("Zeta Aquarii", 1352, 0, StarProperties.Double);
            value[52] = new Star("Eta Aquarii", 1355, 0);
            value[53] = new Star("Theta Aquarii", 1337, -8);
            value[54] = new Star("Iota Aquarii", 1323, -14);
            value[55] = new Star("Kappa Aquarii", 1357, -5);
            value[56] = new Star("Lambda Aquarii", 1371, -8);
            value[57] = new Star("Mu Aquarii", 1250, -9);
            value[58] = new Star("Nu Aquarii", 1267, -11);
            value[59] = new Star("Csi Aquarii", 1296, -8);
            value[60] = new Star("Omicron Aquarii", 1322, -2);
            value[61] = new Star("Pi Aquarii", 1345, 1);
            value[62] = new Star("Rho Aquarii", 1341, -8);
            value[63] = new Star("Tau 1 Aquarii", 1361, -4, StarProperties.Double);
            value[64] = new Star("Tau 2 Aquarii", 1364, -3);
            value[65] = new Star("Phi Aquarii", 1397, -6);
            value[66] = new Star("Psi 1 Aquarii", 1396, -9);
            value[67] = new Star("Psi 2 Aquarii", 1398, -9);
            value[68] = new Star("Psi 3 Aquarii", 1399, -9, StarProperties.Double);
            value[69] = new Star("Omega 1 Aquarii", 1421, -4);
            value[70] = new Star("Omega 2 Aquarii", 1424, -5, StarProperties.Double);
            
            // Initialize stars of Aquila
            value[72] = new Star("Altair", 1189, 9);
            value[73] = new Star("Beta Aquilae", 1194, 7, StarProperties.Double);
            value[74] = new Star("Gamma Aquilae", 1182, 11);
            value[75] = new Star("Delta Aquilae", 1164, 3, StarProperties.Double);
            value[76] = new Star("Epsilon Aquilae", 1140, 15);
            value[77] = new Star("Zeta Aquilae", 1145, 3, StarProperties.Double);
            value[78] = new Star("Eta Aquilae", 1186, 1, StarProperties.VariableMagnitude);
            value[79] = new Star("Theta Aquilae", 1213, -1);
            value[80] = new Star("Iota Aquilae", 1178, -1, StarProperties.Double);
            value[81] = new Star("Kappa Aquilae", 1177, -7);
            value[82] = new Star("Lambda Aquilae", 1144, -5);
            value[83] = new Star("Mu Aquilae", 1171, 7, StarProperties.Double);
            value[84] = new Star("Nu Aquilae", 1163, 0);
            value[85] = new Star("Omicron Aquilae", 1187, 11);
            value[86] = new Star("Pi Aquilae", 1188, 12);
            value[87] = new Star("QS Aquilae", 1181, 14, StarProperties.Double | StarProperties.VariableMagnitude);
            value[88] = new Star("Rho Aquilae", 1212, 15);
            value[89] = new Star("R Aquilae", 1147, 8, StarProperties.VariableMagnitude);
            value[90] = new Star("Sigma Aquilae", 1175, 5);
            value[91] = new Star("Tau Aquilae", 1206, 8);
            value[92] = new Star("U Aquilae", 1168, -7, StarProperties.VariableMagnitude);
            value[93] = new Star("Chi Aquilae", 1179, 12);
            value[94] = new Star("Phi Aquilae", 1193, 12);
            value[95] = new Star("Psi Aquilae", 1176, 13);
            
            // Initialize stars of Ara
            value[97] = new Star("Alpha Arae", 1053, -50);
            value[98] = new Star("Beta Arae", 1051, -55);
            value[99] = new Star("Gamma Arae", 1050, -56, StarProperties.Double);
            value[100] = new Star("Delta Arae", 1055, -61);
            value[101] = new Star("Epsilon 1 Arae", 1019, -53);
            value[102] = new Star("Epsilon 2 Arae", 1021, -53);
            value[103] = new Star("Zeta Arae", 1019, -57);
            value[104] = new Star("Eta Arae", 1008, -59);
            value[105] = new Star("Theta Arae", 1087, -50);
            value[106] = new Star("Iota Arae", 1047, -47);
            value[107] = new Star("Kappa Arae", 1044, -51);
            value[108] = new Star("Lambda Arae", 1075, -49);
            value[109] = new Star("Mu Arae", 1063, -52);
            value[110] = new Star("Pi Arae", 1064, -55);
            value[111] = new Star("R Arae", 1001, -57);
            value[112] = new Star("Sigma Arae", 1053, -47);
            value[113] = new Star("U Arae", 1071, -51);
            
            // Initialize stars of Aries
            value[115] = new Star("Alpha Arietis", 128, 23);
            value[116] = new Star("Beta Arietis", 113, 21);
            value[117] = new Star("Gamma Arietis", 111, 19, StarProperties.Double);
            value[118] = new Star("Delta Arietis", 196, 20);
            value[119] = new Star("Epsilon Arietis", 179, 21);
            value[120] = new Star("Zeta Arietis", 197, 21);
            value[121] = new Star("Eta Arietis", 131, 21);
            value[122] = new Star("Theta Arietis", 134, 20);
            value[123] = new Star("Iota Arietis", 119, 18);
            value[124] = new Star("Kappa Arietis", 128, 22);
            value[125] = new Star("Lambda Arietis", 119, 23, StarProperties.Double);
            value[126] = new Star("Mu Arietis", 161, 20);
            value[127] = new Star("Nu Arietis", 157, 22);
            value[128] = new Star("Csi Arietis", 142, 10);
            value[129] = new Star("Omicron Arietis", 165, 15);
            value[130] = new Star("Pi Arietis", 168, 17, StarProperties.Double);
            value[131] = new Star("Rho Arietis", 173, 18);
            value[132] = new Star("Sigma Arietis", 171, 15);
            value[133] = new Star("Tau Arietis", 199, 21);
            value[134] = new Star("U Arietis", 194, 15);
            
            // Initialize stars of Auriga
            value[136] = new Star("Capella", 319, 45);
            value[137] = new Star("Beta Aurigae", 360, 45, StarProperties.VariableMagnitude);
            value[138] = new Star("Delta Aurigae", 360, 54);
            value[139] = new Star("Epsilon Aurigae", 302, 43, StarProperties.VariableMagnitude);
            value[140] = new Star("Zeta Aurigae", 302, 41, StarProperties.VariableMagnitude);
            value[141] = new Star("Eta Aurigae", 304, 41);
            value[142] = new Star("Theta Aurigae", 360, 38, StarProperties.Double);
            value[143] = new Star("Kappa Aurigae", 370, 29);
            value[144] = new Star("Lambda Aurigae", 320, 40, StarProperties.Double);
            value[145] = new Star("Mu Aurigae", 310, 39);
            value[146] = new Star("Nu Aurigae", 354, 39);
            value[147] = new Star("Csi Aurigae", 355, 55);
            value[148] = new Star("Omicron Aurigae", 341, 50);
            value[149] = new Star("Pi Aurigae", 360, 46);
            value[150] = new Star("PU Aurigae", 321, 43, StarProperties.VariableMagnitude);
            value[151] = new Star("Rho Aurigae", 324, 41);
            value[152] = new Star("R Aurigae", 319, 53, StarProperties.VariableMagnitude);
            value[153] = new Star("RT Aurigae", 385, 30, StarProperties.Double | StarProperties.VariableMagnitude);
            value[154] = new Star("Sigma Aurigae", 325, 37);
            value[155] = new Star("Tau Aurigae", 352, 39, StarProperties.Double);
            value[156] = new Star("UU Aurigae", 394, 38, StarProperties.Double | StarProperties.VariableMagnitude);
            
            // Initialize stars of Bootes
            value[158] = new Star("Arcturus", 854, 19, ArcturusColor);
            value[159] = new Star("A Bootis", 852, 35);
            value[160] = new Star("Beta Bootis", 901, 40);
            value[161] = new Star("Gamma Bootis", 869, 38, StarProperties.Double);
            value[162] = new Star("Delta Bootis", 911, 33, StarProperties.Double);
            value[163] = new Star("Epsilon Bootis", 881, 27, StarProperties.Double);
            value[164] = new Star("Zeta Bootis", 883, 13);
            value[165] = new Star("Eta Bootis", 837, 19);
            value[166] = new Star("Theta Bootis", 859, 52);
            value[167] = new Star("Iota Bootis", 851, 51, StarProperties.Double);
            value[168] = new Star("Kappa Bootis", 849, 52, StarProperties.Double);
            value[169] = new Star("Lambda Bootis", 853, 46);
            value[170] = new Star("Mu Bootis", 921, 37, StarProperties.Double);
            value[171] = new Star("Nu 1 Bootis", 932, 41);
            value[172] = new Star("Nu 2 Bootis", 933, 41, StarProperties.Double);
            value[173] = new Star("Csi Bootis", 887, 19, StarProperties.Double);
            value[174] = new Star("Omicron Bootis", 883, 17);
            value[175] = new Star("Pi Bootis", 881, 16, StarProperties.Double);
            value[176] = new Star("Rho Bootis", 870, 30);
            value[177] = new Star("Sigma Bootis", 874, 30);
            value[178] = new Star("Tau Bootis", 826, 17, StarProperties.Double);
            value[179] = new Star("Chi Bootis", 910, 29);
            value[180] = new Star("Phi Bootis", 937, 40);
            value[181] = new Star("Psi Bootis", 904, 27);
            value[182] = new Star("Omega Bootis", 901, 25);
            
            // Initialize stars of Caelum
            value[184] = new Star("Alpha Caeli", 282, -42);
            value[185] = new Star("Beta Caeli", 283, -37);
            value[186] = new Star("Gamma Caeli", 304, -35);
            value[187] = new Star("Delta Caeli", 270, -45);
            value[188] = new Star("Zeta Caeli", 286, -30);
            value[189] = new Star("R Caeli", 282, -38);
            
            // Initialize stars of Camelopardalis
            value[191] = new Star("Alpha Camelopardalis", 293, 66);
            value[192] = new Star("Beta Camelopardalis", 300, 60);
            value[193] = new Star("Gamma Camelopardalis", 231, 71);
            value[194] = new Star("VZ Camelopardalis", 447, 83, StarProperties.Double | StarProperties.VariableMagnitude);
            value[195] = new Star("Z Camelopardalis", 511, 74, StarProperties.VariableMagnitude);
            
            // Initialize stars of Cancer
            value[197] = new Star("Alpha Cancri", 539, 12);
            value[198] = new Star("Beta Cancri", 499, 9);
            value[199] = new Star("Gamma Cancri", 521, 22);
            value[200] = new Star("Delta Cancri", 523, 18, StarProperties.Double);
            value[201] = new Star("Epsilon Cancri", 517, 19);
            value[202] = new Star("Zeta Cancri", 494, 17, StarProperties.Double);
            value[203] = new Star("Eta Cancri", 511, 20);
            value[204] = new Star("Theta Cancri", 509, 18);
            value[205] = new Star("Iota Cancri", 529, 29, StarProperties.Double);
            value[206] = new Star("Kappa Cancri", 549, 11);
            value[207] = new Star("Lambda Cancri", 503, 24);
            value[208] = new Star("Mu Cancri", 488, 22);
            value[209] = new Star("Nu Cancri", 542, 24);
            value[210] = new Star("Csi Cancri", 490, 22);
            value[211] = new Star("Omicron 1 Cancri", 537, 15);
            value[212] = new Star("Omicron 2 Cancri", 538, 16);
            value[213] = new Star("Pi Cancri", 562, 15);
            value[214] = new Star("Rho Cancri", 532, 28, StarProperties.Double);
            value[215] = new Star("R Cancri", 499, 12, StarProperties.VariableMagnitude);
            value[216] = new Star("RS Cancri", 490, 31, StarProperties.Double | StarProperties.VariableMagnitude);
            value[217] = new Star("Sigma Cancri", 536, 33);
            value[218] = new Star("Tau Cancri", 489, 30);
            value[219] = new Star("Chi Cancri", 503, 27);
            value[220] = new Star("Phi 1 Cancri", 508, 28, StarProperties.Double);
            value[221] = new Star("Phi 2 Cancri", 508, 27, StarProperties.Double);
            value[222] = new Star("Psi Cancri", 495, 26);
            value[223] = new Star("Omega Cancri", 480, 26);
            
            // Initialize stars of Canes Venatici
            value[225] = new Star("Cor Caroli", 778, 38, StarProperties.Double);
            value[226] = new Star("Beta Canum Venaticorum", 751, 42);
            value[227] = new Star("R Canum Venaticorum", 821, 40, StarProperties.VariableMagnitude);
            value[228] = new Star("TU Canum Venaticorum", 714, 47, StarProperties.VariableMagnitude);
            value[229] = new Star("Y Canum Venaticorum", 697, 46, StarProperties.VariableMagnitude);
            
            // Initialize stars of Canis Major
            value[231] = new Star("Sirius", 398, -17, StarProperties.Double);
            value[232] = new Star("Beta Canis Majoris", 383, -18);
            value[233] = new Star("Gamma Canis Majoris", 427, -16);
            value[234] = new Star("Wezen", 429, -26);
            value[235] = new Star("Adhara", 419, -29);
            value[236] = new Star("Zeta Canis Majoris", 386, -30);
            value[237] = new Star("Eta Canis Majoris", 447, -29);
            value[238] = new Star("Theta Canis Majoris", 407, -12);
            value[239] = new Star("Iota Canis Majoris", 413, 13);
            value[240] = new Star("Kappa Canis Majoris", 405, -33);
            value[241] = new Star("Lambda Canis Majoris", 387, -33);
            value[242] = new Star("Mu Canis Majoris", 413, -14, StarProperties.Double);
            value[243] = new Star("Nu Canis Majoris", 411, -19, StarProperties.Double);
            value[244] = new Star("Csi Canis Majoris", 409, -24, StarProperties.Double);
            value[245] = new Star("Omicron 1 Canis Majoris", 413, -24);
            value[246] = new Star("Omicron 2 Canis Majoris", 423, -24);
            value[247] = new Star("Pi Canis Majoris", 413, -20, StarProperties.Double);
            value[248] = new Star("R Canis Majoris", 380, -16, StarProperties.VariableMagnitude);
            value[249] = new Star("Sigma Canis Majoris", 362, -28, StarProperties.Double);
            value[250] = new Star("Tau Canis Majoris", 441, -25, StarProperties.Double);
            value[251] = new Star("Omega Canis Majoris", 399, -27);
            
            // Initialize stars of Canis Minor
            value[253] = new Star("Procyon", 456, 5, ProcyonColor, StarProperties.Double);
            value[254] = new Star("Beta Canis Minoris", 445, 8);
            value[255] = new Star("Gamma Canis Minoris", 446, 9, StarProperties.Double);
            value[256] = new Star("Delta 1 Canis Minoris", 447, 2);
            value[257] = new Star("Delta 2 Canis Minoris", 450, 4);
            value[258] = new Star("Delta 3 Canis Minoris", 451, 4);
            value[259] = new Star("Epsilon Canis Minoris", 444, 9);
            value[260] = new Star("Zeta Canis Minoris", 467, 2);
            
            // Initialize stars of Capricornus
            value[262] = new Star("Alpha 1 Capricorni", 1217, -13, StarProperties.Double);
            value[263] = new Star("Alpha 2 Capricorni", 1219, -13, StarProperties.Double);
            value[264] = new Star("Beta Capricorni", 1224, -15);
            value[265] = new Star("Gamma Capricorni", 1301, -16);
            value[266] = new Star("Delta Capricorni", 1306, -14);
            value[267] = new Star("Epsilon Capricorni", 1299, -20, StarProperties.Double | StarProperties.VariableMagnitude);
            value[268] = new Star("Zeta Capricorni", 1289, -23);
            value[269] = new Star("Eta Capricorni", 1267, -20);
            value[270] = new Star("Theta Capricorni", 1269, -17);
            value[271] = new Star("Iota Capricorni", 1284, -17);
            value[272] = new Star("Kappa Capricorni", 1302, -19);
            value[273] = new Star("Lambda Capricorni", 1301, -18);
            value[274] = new Star("Mu Capricorni", 1312, -14);
            value[275] = new Star("Nu Capricorni", 1220, -13, StarProperties.Double);
            value[276] = new Star("Csi Capricorni", 1212, -13, StarProperties.Double);
            value[277] = new Star("Omicron Capricorni", -18, StarProperties.Double);
            value[278] = new Star("Pi Capricorni", 1245, -18, StarProperties.Double);
            value[279] = new Star("Rho Capricorni", 1246, -18, StarProperties.Double);
            value[280] = new Star("RT Capricorni", 1219, -22, StarProperties.VariableMagnitude);
            value[281] = new Star("Sigma Capricorni", 1219, -18, StarProperties.Double);
            value[282] = new Star("Tau Capricorni", 1235, -15, StarProperties.Double);
            value[283] = new Star("Chi Capricorni", 1272, -21, StarProperties.Double);
            value[284] = new Star("Phi Capricorni", 1276, -20);
            value[285] = new Star("Psi Capricorni", 1241, -27);
            value[286] = new Star("Omega Capricorni", 1245, -28);
            
            // Initialize constellations
            value[0] = new Constellation("Andromeda", 1, 21, "Andromedae");
            value[22] = new Constellation("Antlia", 23, 32, "Antliae");
            value[33] = new Constellation("Apus", 34, 44, "Apodis");
            value[45] = new Constellation("Aquarius", 46, 70, "Aquarii");
            value[71] = new Constellation("Aquila", 72, 95, "Aquilae");
            value[96] = new Constellation("Ara", 97, 113, "Arae");
            value[114] = new Constellation("Aries", 115, 134, "Arietis");
            value[135] = new Constellation("Auriga", 136, 156, "Aurigae");
            value[157] = new Constellation("Bootes", 158, 182, "Bootis");
            value[183] = new Constellation("Caelum", 184, 189, "Caeli");
            value[190] = new Constellation("Camelopardalis", 191, 195, "Camelopardalis");
            value[197] = new Constellation("Cancer", 196, 223, "Cancri");
            value[224] = new Constellation("Canes Venatici", 225, 229, "Canum Venaticorum");
            value[230] = new Constellation("Canis Major", 231, 251, "Canis Majoris");
            value[252] = new Constellation("Canis Minor", 253, 260, "Canis Minoris");
            value[261] = new Constellation("Capricornus", 262, 286, "Capricorni");
            
            // If you have new SkyObjects, insert them in InitializeLibrary6 or in the following methods.
            // Examples:
            // M 13: insert it between the comment 'Hercules' and the next empty line
            // I already don't know the value of the ra and dec variables in the code below.
            // value[2013] = new GroupAsStar("M 13", ra, dec);
            // Psi 1 to 9 Aurigae: I've missed these stars.
            // Insert in InitializeLibrary7 (example and with ra and dec values unknown):
            // value[2120] = new Star("Psi 1 Aurigae", ra, dec);
            // This project is available on SourceForge at:
            // http://sourceforge.net/projects/skywatcher
            // I have an issue (#3). Do a pull request in https://github.com/luismark/skywatcher
            // before the 12nd of October.
            
            // Calls to the following methods
            InitializeLibrary2(value);
            InitializeLibrary3(value);
            InitializeLibrary4(value);
            InitializeLibrary5(value);
            InitializeLibrary6(value);
            
            Value = value;
        }
        public static void InitializeLibrary2(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitializeLibrary3(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitializeLibrary4(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitializeLibrary5(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitializeLibrary6(SkyObject[] value) {
            // This method is for initializing Messier objects.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static Constellation Andromeda {
            get {
                return (Constellation)(Value[AndromedaLocation]);
            }
        }
        public static int AndromedaLocation = 0;
        public static Constellation Antlia {
            get {
                return (Constellation)(Value[AntliaLocation]);
            }
        }
        public static int AntliaLocation = 22;
        public static Constellation Apus {
            get {
                return (Constellation)(Value[ApusLocation]);
            }
        }
        public static int ApusLocation = 33;
        public static Constellation Aquarius {
            get {
                return (Constellation)(Value[AquariusLocation]);
            }
        }
        public static int AquariusLocation = 45;
        public static Constellation Aquila {
            get {
                return (Constellation)(Value[AquilaLocation]);
            }
        }
        public static int AquilaLocation = 71;
        public static Constellation Ara {
            get {
                return (Constellation)(Value[AraLocation]);
            }
        }
        public static int AraLocation = 96;
        public static Constellation Aries {
            get {
                return (Constellation)(Value[AriesLocation]);
            }
        }
        public static int AriesLocation = 114;
        public static Constellation Auriga {
            get {
                return (Constellation)(Value[AurigaLocation]);
            }
        }
        public static int AurigaLocation = 135;
        public static Constellation Bootes {
            get {
                return (Constellation)(Value[BootesLocation]);
            }
        }
        public static int BootesLocation = 157;
        public static Constellation Caelum {
            get {
                return (Constellation)(Value[CaelumLocation]);
            }
        }
        public static int CaelumLocation = 183;
        public static Constellation Camelopardalis {
            get {
                return (Constellation)(Value[CamelopardalisLocation]);
            }
        }
        public static int CamelopardalisLocation = 190;
        public static Constellation Cancer {
            get {
                return (Constellation)(Value[CancerLocation]);
            }
        }
        public static int CancerLocation = 196;
        public static Constellation CanesVenatici {
            get {
                return (Constellation)(Value[CanesVenaticiLocation]);
            }
        }
        public static int CanesVenaticiLocation = 224;
        public static Constellation CanisMajor {
            get {
                return (Constellation)(Value[CanisMajorLocation]);
            }
        }
        public static int CanisMajorLocation = 230;
        public static Constellation CanisMinor {
            get {
                return (Constellation)(Value[CanisMinorLocation]);
            }
        }
        public static int CanisMinorLocation = 252;
        public static Constellation Capricornus {
            get {
                return (Constellation)(Value[CapricornusLocation]);
            }
        }
        public static int CapricornusLocation = 261;
        public static Color ArcturusColor = Color.FromArgb(255, 226, 0);
        public static Color ProcyonColor = Color.FromArgb(255, 226, 0);
    }
}
